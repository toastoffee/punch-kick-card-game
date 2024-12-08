using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace TurnGame {
  public class Table : Dictionary<string, object> {
    public Table(Action<Table> initer = null) {
      initer?.Invoke(this);
    }
    public T Read<T>(string key) {
      T val = default;
      try {
        val = (T)this[key];
      } catch (System.Exception ex) {
        Debug.LogError($"failed to read key [{key}] as [{typeof(T)}]");
        Debug.LogException(ex);
      }
      return val;
    }
  }

  public class SkillInput {
    public TeaProp caster;
    public TeaProp receiver;
    public Table inputTable;
  }

  public class SkillTmplModel {
    public string id;
    public bool needReceiver;
    public OnCast onCast;
    public OnCasting onCasting;

    public delegate void OnCasting(CastingContext ctx);
    public delegate void OnCast(SkillInput input, Table args);
  }

  [AutoModelTable(typeof(SkillTmplModel))]
  public class SkillTmplDefine {
    public static SkillTmplModel st_1_atk {
      get {
        var ret = new SkillTmplModel();
        ret.id = nameof(st_1_atk);
        ret.needReceiver = true;
        ret.onCast = (input, args) => {
          var ratio = args.Read<float>("ratio_0");
          var dmg = Mathf.FloorToInt(ratio * input.caster.atk);
          var dmgEve = new DamageEvent {
            attacker = input.caster,
            receiver = input.receiver,
            damage = dmg,
          };

          TurnGame.Instance.PushEvent(dmgEve);
        };
        ret.onCasting = (ctx) => {
          var skillProp = ctx.skillProp;
          ctx.centralHintControl.ShowHint(CentralHintUI.Hint.SELECTING_TARGET, skillProp.skillModel.name);
        };
        return ret;
      }
    }
  }

  public class SkillModel {
    public string name;
    public int apCost;
    public TeaValidator targetValidator;
    public SkillTmplModel tmplModel;
    public Table args;
  }

  public class SkillProp {
    public SkillModel skillModel;
    public int apCost;

    public void Load(SkillModel skillModel) {
      this.skillModel = skillModel;
      apCost = skillModel.apCost;
    }
  }


  public class PlayerTeaModel {
    public string id;
    public List<SkillModel> skills;
  }

  [AutoModelTable(typeof(PlayerTeaModel))]
  public class PlayerTeaDefine {
    public static PlayerTeaModel pt_1_carry {
      get {
        var ret = new PlayerTeaModel() { id = nameof(pt_1_carry) };
        ret.skills = new List<SkillModel> {
          new SkillModel {
            name = "¹¥»÷",
            apCost = 1,
            targetValidator = TeaValidatorDefine.selectEnemy,
            tmplModel = SkillTmplDefine.st_1_atk,
            args = new Table((tab) => {
              tab["ratio_0"] = 1.0f;
            }),
          }
        };
        return ret;
      }
    }

    public static PlayerTeaModel pt_2_tank {
      get {
        var ret = new PlayerTeaModel() { id = nameof(pt_2_tank) };
        ret.skills = new List<SkillModel> {
          new SkillModel {
            name = "¶ÜÃÍ",
            apCost = 1,
            targetValidator = TeaValidatorDefine.selectEnemy,
            tmplModel = SkillTmplDefine.st_1_atk,
            args = new Table((tab) => {
              tab["ratio_0"] = 1.0f;
            }),
          }
        };
        return ret;
      }
    }
  }

  public delegate bool TeaValidator(TeaProp teaProp, SkillProp skillProp);
  public class TeaValidatorDefine {
    public static TeaValidator selectEnemy => (teaProp, skillProp) => {
      return teaProp.isEnemy;
    };
  }

}