using UnityEngine;
using System.Collections.Generic;

using DG.Tweening;

/// <summary>
/// usage:
/// 1. define a transition from [TState]A to [TState]B with a tween provider
/// 2. call GoToState(TState state) from outter (transition tween will be picked internally) (fastMode is provided)
/// 3. this class will keep only 1 tween active at the same time 
///    (uncompleted last tween WILL be completed before next one start)
/// </summary>
/// <typeparam name="TState"></typeparam>
public class UIStateTransitionTween<TState> where TState : struct, System.Enum {
  public delegate Tween TweenProvider();

  private Dictionary<TState, Dictionary<TState, TweenProvider>> m_tweenDict;
  private Tween m_cachedTween;
  private TState m_currentState;

  public bool isPlaying {
    get {
      return m_cachedTween != null && m_cachedTween.IsPlaying();
    }
  }

  public UIStateTransitionTween(TState defaultState) {
    m_tweenDict = new Dictionary<TState, Dictionary<TState, TweenProvider>>();
    _SetCurrentStateAndUpdateKey(defaultState);
  }

  public void SetTweenProvider(TState fromState, TState toState, TweenProvider tweenProvider) {
    var dict = _GetStateTweenDict(fromState);
    dict[toState] = tweenProvider;
  }

  public void GoToState(TState toState, bool isFastMode = false) {
    // a goddamn boxing
    if (toState.Equals(m_currentState)) {
      return;
    }

    TweenProvider tweenProvider;
    if (!_TryGetTransition(m_currentState, toState, out tweenProvider)) {
      Debug.LogError($"[UIStateTransitionTween] transition not defined: from [{m_currentState}] to [{m_currentState}]");
      return;
    }
    if (m_cachedTween != null) {
      m_cachedTween.Kill(complete: true);
    }

    _SetCurrentStateAndUpdateKey(toState);
    m_cachedTween = tweenProvider.Invoke();
    if (!isFastMode) {
      m_cachedTween.Goto(0, andPlay: true);
    } else {
      m_cachedTween.Goto(m_cachedTween.Duration());
    }
  }

  private void _SetCurrentStateAndUpdateKey(TState state) {
    m_currentState = state;
  }

  private Dictionary<TState, TweenProvider> _GetStateTweenDict(TState state) {
    Dictionary<TState, TweenProvider> res = null;
    if (!m_tweenDict.TryGetValue(state, out res)) {
      m_tweenDict[state] = new Dictionary<TState, TweenProvider>();
      res = m_tweenDict[state];
    }
    return res;
  }

  private bool _TryGetTransition(TState fromState, TState toState, out TweenProvider tweenProvider) {
    tweenProvider = null;
    var dict = _GetStateTweenDict(fromState);
    if (dict != null && dict.TryGetValue(toState, out tweenProvider)) {
      return true;
    }
    return false;
  }
}
