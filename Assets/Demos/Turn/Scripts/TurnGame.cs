using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TurnGame {
  public class TurnGame : MonoSingleton<TurnGame> {
    public RectTransform teamContainer;
    public RectTransform enemyContainer;
    public DuelUI duelUi;
    public OptionUI optionUi;
    public AutoLoader autoLoader;
    TeaProp carryTeaProp;
    TeaProp tankTeaProp;
    DuelProp duelProp;

    private CastingContext m_castingCtx;

    private Stack<DuelEvent> eventStack = new Stack<DuelEvent>();

    private void Start() {
      StartCoroutine(SolveEventThread());

      carryTeaProp = new TeaProp() {
        name = "Carry",
        hp = 100,
        maxHp = 100,
        atk = 20,
        def = 0,
        isFront = true,
        swapCost = 1,
      };
      carryTeaProp.LoadPlyaerSkill(PlayerTeaDefine.pt_1_carry);
      carryTeaProp.IncSeqNum();

      tankTeaProp = new TeaProp() {
        name = "Tank",
        hp = 100,
        maxHp = 100,
        atk = 8,
        def = 4,
        swapCost = 1,
      };
      tankTeaProp.LoadPlyaerSkill(PlayerTeaDefine.pt_2_tank);
      tankTeaProp.IncSeqNum();

      var teaViewPrefab = autoLoader.Load<GameObject>("tea").GetComponent<TeaView>();

      var carryTeaView = Instantiate(teaViewPrefab, teamContainer);
      carryTeaView.Bind(carryTeaProp);

      var tankTeaView = Instantiate(teaViewPrefab, teamContainer);
      tankTeaView.Bind(tankTeaProp);

      var enemy1prop = new TeaProp {
        name = "Monster",
        hp = 60,
        maxHp = 60,
        atk = 10,
        def = 1,
        isFront = true,
        isEnemy = true,
      };
      enemy1prop.InitEnemyProp();
      enemy1prop.IncSeqNum();

      _InstTeaView(enemy1prop, enemyContainer);

      var playerProp = new PlayerProp {
        ap = 5,
        maxAp = 5,
      };

      duelProp = new DuelProp {
        enemyTeamProp = new TeamProp(),
        playerTeamProp = new TeamProp(),
        playerProp = playerProp,
        inPlayerTurn = true,
      };
      duelProp.IncSeqNum();
      duelProp.playerTeamProp.IncSeqNum();
      duelProp.enemyTeamProp.IncSeqNum();
      duelUi.Bind(duelProp);
      optionUi.Bind(duelProp.playerTeamProp);

      duelProp.enemyTeamProp.teaProps.Add(enemy1prop);
      duelProp.enemyTeamProp.SyncTeamIdx();

      duelProp.playerTeamProp.teaProps.Add(carryTeaProp);
      duelProp.playerTeamProp.teaProps.Add(tankTeaProp);
      duelProp.playerTeamProp.SyncTeamIdx();
    }

    private void _InstTeaView(TeaProp teaProp, RectTransform container) {
      var teaViewPrefab = autoLoader.Load<GameObject>("tea").GetComponent<TeaView>();
      var teaview = Instantiate(teaViewPrefab, container);
      teaview.Bind(teaProp);
    }

    public void OnTeaClick(TeaProp teaProp) {
      if (m_castingCtx != null && m_castingCtx.isCasting) {
        //����ѡ��ʩ��Ŀ��
        m_castingCtx.OnSelectTarget(teaProp);
        return;
      }

      if (!teaProp.isEnemy) {
        var setFrontEve = new SetTeamFront() {
          frontIdx = teaProp.teamIdx,
        };
        PushEvent(setFrontEve);
      }
    }

    private IEnumerator SolveEventThread() {
      DuelEvent curEve = null;
      while (true) {
        if (eventStack.Count == 0) {
          yield return null;
          continue;
        }
        if (curEve != null && curEve.isBlock) {
          yield return null;
          continue;
        }
        var playerProp = duelProp.playerProp;

        curEve = eventStack.Pop();

        if (curEve is Signal sig) {
          switch (sig.sig) {
            case "end_turn":
              if (!duelProp.inPlayerTurn) {
                break;
              }
              duelProp.inPlayerTurn = false;
              duelProp.IncSeqNum();

              var enemyTurnEve = new EnemyTurn() {
                nextMoveEnemy = duelProp.enemyTeamProp.GetNextMoveEnemy(duelProp),
                hintControl = CentralHintUI.Instance.GetControl(),
              };
              PushEvent(enemyTurnEve);
              break;

            case "enemy_end_turn":
              var delay = sig.param as EnmeyEndTurnDelay;
              delay.lastEnemyTurn.hintControl.Close();
              duelProp.turnIdx++;
              duelProp.inPlayerTurn = true;
              playerProp.ap = playerProp.maxAp;

              duelProp.IncSeqNum();
              break;
          }
        }

        if (curEve is SetTeamFront sfe) {
          if (sfe.frontIdx == duelProp.playerTeamProp.frontIndex) {
            continue;
          }
          if (playerProp.ap <= 0) {
            continue;
          }
          playerProp.ap--;
          duelProp.playerTeamProp.SetFront(sfe.frontIdx);
          duelProp.playerTeamProp.IncSeqNum();
          duelProp.IncSeqNum();
        }

        if (curEve is CastSkillEvent cse) {
          var ctx = cse.ctx;
          var input = new SkillInput {
            caster = ctx.caster,
            receiver = ctx.receiver,
            inputTable = null,
          };
          playerProp.ap -= ctx.skillProp.apCost;
          playerProp.IncSeqNum();
          duelProp.IncSeqNum();
          var skillModel = ctx.skillProp.skillModel;
          ctx.skillProp.skillModel.tmplModel.onCast(input, ctx.skillProp.skillModel.args);
          Debug.Log($"[{ctx.caster.debug_name}] ʩ���� [{skillModel.name.WrapColor(Color.yellow)}]");
        }

        if (curEve is DamageEvent dmg) {
          int realDmg;
          dmg.receiver.ApplyDamage(dmg.damage, out realDmg); ;
          dmg.receiver.IncSeqNum();
          Debug.Log($"[{dmg.attacker.debug_name}] �� [{dmg.receiver.debug_name}] ����� [{realDmg.ToString().WrapColor(Color.white)}] ���˺�");
        }

        if (curEve is EnemyTurn et) {
          HandleEnemyTurn(et);
        }

        StartCoroutine(curEve.RunProcess());
        yield return null;
      }
    }

    public void PushEvent(DuelEvent eve) {
      eventStack.Push(eve);
    }

    public void OnTryCastSkill(CastingContext ctx) {
      var skilProp = ctx.skillProp;
      var playerProp = duelProp.playerProp;
      if (skilProp.apCost > playerProp.ap) {
        Debug.Log("��������");
        return;
      }

      if (m_castingCtx != null && m_castingCtx.isCasting) {
        return;
      }
      m_castingCtx = ctx;
      if (!m_castingCtx.skillProp.skillModel.tmplModel.needReceiver) {
        var castEve = new CastSkillEvent() {
          ctx = ctx,
        };
        PushEvent(castEve);
        return;
      }

      StartCoroutine(CastingContext.StartCast(ctx));
    }

    public void HandleEnemyTurn(EnemyTurn enemyTurn) {
      if (enemyTurn.nextMoveEnemy == null) {
        var endTurnDelay = new EnmeyEndTurnDelay {
          lastEnemyTurn = enemyTurn,
        };
        PushEvent(endTurnDelay);
        return;
      }
      enemyTurn.hintControl.ShowHint(CentralHintUI.Hint.ENEMY_MOVING);
      var tea = enemyTurn.nextMoveEnemy;

      tea.enemyProp.lastMoveTurn = duelProp.turnIdx;

      var nextTurn = new EnemyTurn() {
        nextMoveEnemy = duelProp.enemyTeamProp.GetNextMoveEnemy(duelProp),
        hintControl = enemyTurn.hintControl,
      };
      PushEvent(nextTurn);
    }
  }
}