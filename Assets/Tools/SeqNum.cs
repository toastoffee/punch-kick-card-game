using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct SeqNum {
  public int value { get; set; }
  public void Inc() {
    value++;
  }
}

public interface ISeqNumModel {
  SeqNum seqNum { get; }
  void IncSeqNum();
}

public abstract class SeqNumModel : ISeqNumModel {
  private SeqNum m_seqNum;
  public SeqNum seqNum => m_seqNum;

  public virtual void IncSeqNum() {
    m_seqNum.Inc();
  }
}

public abstract class SeqNumModelBinder<T> : MonoBehaviour where T : ISeqNumModel {
  private T m_prop;
  private SeqNumChecker m_checker;
  protected T prop { get { return m_prop; } }
  public void Bind(T prop) {
    m_prop = prop;
  }

  protected virtual void LateUpdate() {
    if (m_prop == null) {
      return;
    }
    if (m_checker.ConsumeUpdate(m_prop.seqNum.value)) {
      OnSeqNumUpdate(m_prop);
    }
  }

  public abstract void OnSeqNumUpdate(T prop);
}

public abstract class SeqProp {
  public int seqNum { get; private set; }
}

public class SeqProp<TModel> : SeqProp {
  public TModel value { get; private set; }
  public SeqProp(TModel initVal = default) {
    value = initVal;
  }
}

public class SeqNumCache {
  private Dictionary<SeqProp, int> m_seqNumCache = new Dictionary<SeqProp, int>();

  public bool ConsumeUpdate(SeqProp prop) {
    if (!m_seqNumCache.ContainsKey(prop)) {
      m_seqNumCache[prop] = 0;
    }
    var flag = m_seqNumCache[prop] != prop.seqNum;
    m_seqNumCache[prop] = prop.seqNum;
    return flag;
  }
}