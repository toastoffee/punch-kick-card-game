using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TurnGame {
  public class TeaProp : SeqNumModel {
    public string name;
    public bool isEnemy;
    public bool isFront;
    public int teamIdx;
    public int hp;
    public int maxHp;
    public int atk;
    public int def;
    public float defRatio => 1 - 20f / (20 + def);
    public string debug_name => name.WrapColor(isEnemy ? Color.green : Color.red);

    public List<SkillProp> skillProps = new List<SkillProp>();
    public void LoadPlyaerSkill(PlayerTeaModel model) {
      skillProps.Clear();
      foreach (var skillModel in model.skills) {
        var skillProp = new SkillProp();
        skillProp.Load(skillModel);
        skillProps.Add(skillProp);
      }
    }

    public void ApplyDamage(int damage, out int realTake) {
      realTake = Mathf.RoundToInt(damage * (1 - defRatio));
      hp -= realTake;
      hp = Mathf.Max(hp, 0);
    }
  }

  public class TeamProp : SeqNumModel {
    public int frontIndex;
    public List<TeaProp> teaProps = new List<TeaProp>();

    public void SetFront(int idx) {
      for (int i = 0; i < teaProps.Count; i++) {
        teaProps[i].isFront = idx == i;
      }
      frontIndex = idx;
    }

    public void SyncTeamIdx() {
      for (int i = 0; i < teaProps.Count; i++) {
        teaProps[i].teamIdx = i;
      }
    }

    public override void IncSeqNum() {
      base.IncSeqNum();
      for (int i = 0; i < teaProps.Count; i++) {
        teaProps[i].IncSeqNum();
      }
    }
  }

  public class PlayerProp : SeqNumModel {
    public int ap;
    public int maxAp;
    public int mp;
    public int maxMp;
  }


  public class DuelProp : SeqNumModel {
    public int turnIdx;
    public bool inPlayerTurn;
    public PlayerProp playerProp;
    public TeamProp playerTeamProp;
    public TeamProp enemyTeamProp;
  }

  public abstract class DuelEvent {
    public bool isBlock { get; private set; }

    public IEnumerator RunProcess() {
      isBlock = true;
      yield return Process();
      isBlock = false;
    }

    protected virtual IEnumerator Process() {
      yield break;
    }
  }

  public class SetTeamFront : DuelEvent {
    public int frontIdx;
  }

  public class Signal : DuelEvent {
    public string sig;
    public Signal(string sig) {
      this.sig = sig;
    }
  }

  public class EnemyTurn : DuelEvent {
    protected override IEnumerator Process() {
      yield return new WaitForSeconds(1);
      var sig = new Signal("enemy_end_turn");
      TurnGame.Instance.PushEvent(sig);
    }
  }

  public class DamageEvent : DuelEvent {
    public TeaProp attacker;
    public TeaProp receiver;
    public int damage;
  }

  public class CastSkillEvent : DuelEvent {
    public CastingContext ctx;
  }

  public class CastingContext : System.IDisposable {
    public TeaProp caster;
    public TeaProp receiver;
    public SkillProp skillProp;
    public CentralHintUI.Control centralHintControl;
    public bool isCasting { get; private set; }

    public void OnSelectTarget(TeaProp targetProp) {
      var validator = skillProp.skillModel.targetValidator;
      if (validator(targetProp, skillProp)) {
        receiver = targetProp;
        var castEve = new CastSkillEvent() {
          ctx = this,
        };
        TurnGame.Instance.PushEvent(castEve);
        isCasting = false;
      }
    }

    public void Dispose() {
      isCasting = false;
      centralHintControl?.Close();
    }

    public static IEnumerator StartCast(CastingContext ctx) {
      ctx.isCasting = true;
      ctx.centralHintControl = CentralHintUI.Instance.GetControl();
      var tmplModel = ctx.skillProp.skillModel.tmplModel;
      using (ctx) {
        while (ctx.isCasting) {
          if (Input.GetMouseButtonUp(1)) {
            yield break;
          }
          tmplModel.onCasting?.Invoke(ctx);
          yield return null;
        }
      }
    }
  }
}