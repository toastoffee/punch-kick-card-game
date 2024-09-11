using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

class Unit
{
    public int hp;
    public float def, pwr;
    
    public Unit(int hp, float def, float pwr)
    {
        this.hp  = hp;
        this.def = def;
        this.pwr = pwr;
    }
}

class Buff
{
    public string bufName;
    public bool isEverLast = false;
    public int lastTurns = 0;

    public Buff(string bufName, bool isEverLast, int lastTurns)
    {
        this.bufName = bufName;
        this.isEverLast = isEverLast;
        this.lastTurns = lastTurns;
    }

    public bool IsOn()
    {
        return isEverLast || (lastTurns > 0);
    }

    public void SetLastRounds(int round)
    {
        lastTurns = round * 2;
    }
}

public enum CardType
{
    LightPunch,
    HeavyPunch,
    Parry,
    MindPower,
    Overload,
}

public class DuelManager : MonoBehaviour
{
    [SerializeField]
    private int mPlayerHp = 50, mEnemyHp = 40, mPlayerApPerTurn = 3, mPlayerCardsPerTurn = 4;

    [SerializeField]
    private float mPlayerDef = 2.0f, mPlayerPwr = 6.0f, 
                  mEnemyDef = 0.0f, mEnemyPwr = 5.0f;

    [SerializeField]
    private Text mPlayerStatusText, mEnemyStatusText, mDuelLogText, mHandCardText;

    [SerializeField]
    private List<CardType> mCardsLibrary;

    private List<CardType> _mHandCards, _mUsedCards;

    [SerializeField]
    private int mMaxLogs = 15;
    private List<string> _mLogs;

    private bool _mIsPlayerTurn = true;
    private int _mPlayerAp, _mPlayerDrawCards;

    private Unit _mPlayer, _mEnemy;

    private int _mEnemyActIdx = -1;

    private int _mSelectedCardIdx = 0;

    private List<CardType> _mSeq;
    
    // player buffs
    private Buff _mParryBuff, _mReverseBuff, _mPostureBuff, _mOverloadBuff;
    
    // enemy buffs
    private Buff _mStumbleBuff, _mShatteredBuff;

    private List<Buff> _mBuffs;
    
    private void Start()
    {
        _mPlayer = new Unit(mPlayerHp, mPlayerDef, mPlayerPwr);
        _mEnemy = new Unit(mEnemyHp, mEnemyDef, mEnemyPwr);

        Shuffle(mCardsLibrary);
        _mHandCards = new List<CardType>();
        _mUsedCards = new List<CardType>();
        _mLogs = new();
        _mSeq = new();

        _mPlayerAp = mPlayerApPerTurn;
        _mPlayerDrawCards = mPlayerCardsPerTurn;

        _mParryBuff = new Buff("招架", false, 0);
        _mReverseBuff = new Buff("逆势", false, 0);
        _mPostureBuff = new Buff("气势", false, 0);
        _mOverloadBuff = new Buff("过载", false, 0);
        _mStumbleBuff = new Buff("踉跄", false, 0);
        _mShatteredBuff = new Buff("气海破碎", false, 0);

        _mBuffs = new List<Buff>();
        _mBuffs.Add(_mParryBuff);
        _mBuffs.Add(_mReverseBuff);
        _mBuffs.Add(_mPostureBuff);
        _mBuffs.Add(_mOverloadBuff);
        _mBuffs.Add(_mStumbleBuff);
        _mBuffs.Add(_mShatteredBuff);
        
        UpdateStatusUI();
        UpdateLogs();
        
        DrawCards();
        UpdateHandCards();
    }

    private void Update()
    {
        // input 
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // end turn 
            PlayerEndTurn();
            UpdateHandCards();
        }
        if (Input.GetKeyDown(KeyCode.J))
        {
            // use card
            TryPlayCard();
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            _mSelectedCardIdx = Mathf.Max(_mSelectedCardIdx - 1, 0);
            UpdateHandCards();
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            _mSelectedCardIdx = Mathf.Min(_mSelectedCardIdx + 1, _mHandCards.Count - 1);
            UpdateHandCards();
        }
        
        
        if (_mIsPlayerTurn)
        {
            // wait for the end button
            
        }
        else
        {
            EnemyAct();

            EnemyEndTurn();
        }
    }
    
    public void PlayerEndTurn()
    {
        DiscardCards();
        _mIsPlayerTurn = false;

        UpdateBuffs();
        
        UpdateStatusUI();
    }

    public void EnemyEndTurn()
    {
        // end enemy turn, set player turn(recover mana, draw cards)
        _mIsPlayerTurn = true;
        _mPlayerAp = mPlayerApPerTurn;
        _mPlayerDrawCards = mPlayerCardsPerTurn;
        DrawCards();
        UpdateBuffs();
        
        UpdateHandCards();
        UpdateStatusUI();
    }

    private void UpdateBuffs()
    {
        foreach (var buff in _mBuffs)
        {
            buff.lastTurns--;
        }
    }

    private void AddLog(string s)
    {
        _mLogs.Add(s + '\n');
        UpdateLogs();
    }
    
    private void UpdateLogs()
    {
        string logs = "";

        if (_mLogs.Count <= mMaxLogs)
        {
            foreach (var s in _mLogs)
            {
                logs += s;
            }
        }
        else
        {
            for (int i = _mLogs.Count - mMaxLogs; i < _mLogs.Count; i++)
            {
                logs += _mLogs[i];
            }
        }

        mDuelLogText.text = logs;
    }

    private float GetPlayerDef()
    {
        if (_mParryBuff.IsOn())
        {
            return _mPlayer.def + 3.0f;
        }
        else
        {
            return _mPlayer.def;
        }
    }

    private float GetEnemyDef()
    {
        var def = _mEnemy.def;
        
        if (_mShatteredBuff.IsOn())
        {
            def -= 2.0f;
        }

        if (_mStumbleBuff.IsOn())
        {
            def -= 1.0f;
        }

        return def;
    }
    
    /// <summary>
    /// cards
    /// </summary>

    private void Shuffle<T>(List<T> list)
    {
        int size = list.Count;
        while (size > 1)
        {
            size--;
            int randIdx = Random.Range(0, size + 1);
            (list[randIdx], list[size]) = (list[size], list[randIdx]);
        }
    }

    private void DrawCard()
    {
        if (mCardsLibrary.Count > 0)    
        {
            var card = mCardsLibrary[0];
            mCardsLibrary.RemoveAt(0);
            _mHandCards.Add(card);
        }
        else    // cards empty
        {
            mCardsLibrary = _mUsedCards;
            Shuffle(mCardsLibrary);

            _mUsedCards = new List<CardType>();
        }
    }
    
    private void DrawCards()
    {
        for (int i = 0; i < mPlayerCardsPerTurn; i++)
        {
            DrawCard();
        }
    }

    private void DiscardCards()
    {
        while (_mHandCards.Count != 0)
        {
            var card = _mHandCards[0];
            _mHandCards.RemoveAt(0);
            _mUsedCards.Add(card);
        }
    }

    private void UpdateHandCards()
    {
        string s = "";
        for (int i = 0; i < _mHandCards.Count; i++)
        {
            if (i == _mSelectedCardIdx)
            {
                s += "[";
            }

            switch (_mHandCards[i])
            {
                case CardType.LightPunch:
                    s += "轻击";
                    break;
                case CardType.HeavyPunch:
                    s += "重击";
                    break;
                case CardType.Parry:
                    s += "招架";
                    break;
                case CardType.MindPower:
                    s += "感受念力";
                    break;
                case CardType.Overload:
                    s += "过载";
                    break;
            }
            
            if (i == _mSelectedCardIdx)
            {
                s += "]";
            }

            s += "\t\t";
        }
        mHandCardText.text = s;
    }
    
    
    private void EnemyAct()
    {
        _mEnemyActIdx = (_mEnemyActIdx + 1) % 3;
        if (_mEnemyActIdx == 0)
        {
            // normal attack (1 * pwr)
            float fDmg = 1.0f * _mEnemy.pwr;
            var d =CauseDamage(fDmg, _mPlayer);
            EnemyStealDef(1.0f);
            AddLog($"[敌人]使用攻击对[玩家]造成{d}点伤害");
            AddLog($"[敌人]从[玩家]处偷取了1点防御");
        }
        else if (_mEnemyActIdx == 1)
        {
            // double attack (2 * 0.25 * pwr)
            float fDmg = 0.25f * _mEnemy.pwr; 
            var d = CauseDamage(fDmg, _mPlayer);
            EnemyStealDef(1.0f);
            AddLog($"[敌人]使用双重攻击第一击对[玩家]造成{d}点伤害");
            AddLog($"[敌人]从[玩家]处偷取了1点防御");
            
            d = CauseDamage(fDmg, _mPlayer);
            EnemyStealDef(1.0f);
            AddLog($"[敌人]使用双重攻击第二击对[玩家]造成{d}点伤害");
            AddLog($"[敌人]从[玩家]处偷取了1点防御");
        }
        else
        {
            // defense improved attack
            float fDmg = 1.0f * _mEnemy.pwr + 1.0f * _mEnemy.def;
            var d =CauseDamage(fDmg, _mPlayer);
            EnemyStealDef(1.0f);
            
            AddLog($"[敌人]使用防御加成攻击对[玩家]造成{d}点伤害");
            AddLog($"[敌人]从[玩家]处偷取了1点防御");
        }
        
        UpdateStatusUI();
    }

    private void UpdateStatusUI()
    {
        string playerText = $"玩家\n<color=green>生命\t:{_mPlayer.hp}</color>\n" 
                            + $"<color=orange>力量\t:{_mPlayer.pwr}</color>\n" 
                            + $"<color=brown>防御\t:{GetPlayerDef()}</color>\n"
                            + $"<color=yellow>行动点\t:{_mPlayerAp}</color>\n"
                            + $"<color=silver>下回合抽牌数\t:{_mPlayerDrawCards}</color>\n";
        
        string enemyText = $"敌人\n<color=green>生命\t:{_mEnemy.hp}</color>\n" 
                            + $"<color=orange>力量\t:{_mEnemy.pwr}</color>\n" 
                            + $"<color=brown>防御\t:{GetEnemyDef()}</color>\n";

        string playerBuffs = (_mParryBuff.IsOn() ? _mParryBuff.bufName + $" 持续{Mathf.CeilToInt(_mParryBuff.lastTurns / 2f)}回合\n" : "")
                            + (_mReverseBuff.IsOn() ? _mReverseBuff.bufName + $" 持续{Mathf.CeilToInt(_mReverseBuff.lastTurns / 2f)}回合\n" : "")
                            + (_mPostureBuff.IsOn() ? _mPostureBuff.bufName + $" 持续{Mathf.CeilToInt(_mPostureBuff.lastTurns / 2f)}回合\n" : "")
                            + (_mOverloadBuff.IsOn() ? _mOverloadBuff.bufName + $" 持续{Mathf.CeilToInt(_mOverloadBuff.lastTurns / 2f)}回合\n" : "");
        
        string enemyBuffs  = (_mStumbleBuff.IsOn() ? _mStumbleBuff.bufName + $" 持续{Mathf.CeilToInt(_mStumbleBuff.lastTurns / 2f)}回合\n" : "")
                             + (_mShatteredBuff.IsOn() ? _mShatteredBuff.bufName + $" 持续到结束\n" : "");

        string playerCardsSeq = "\n";
        int seqShownSize = 0;
        for (int i = _mSeq.Count - 1; i >= 0; i--)
        {
            seqShownSize++;
            if (seqShownSize > 3)
            {
                break;
            }

            string s = "";
            switch (_mSeq[i])
            {
                case CardType.LightPunch:
                    s = "轻击";
                    break;
                case CardType.HeavyPunch:
                    s = "重击";
                    break;
                case CardType.Parry:
                    s = "招架";
                    break;
                case CardType.MindPower:
                    s = "感受念力";
                    break;
                case CardType.Overload:
                    s = "过载";
                    break;
            }

            playerCardsSeq = s + " -> " + playerCardsSeq;
        }
        
        mPlayerStatusText.text = playerText + playerBuffs + playerCardsSeq;
        mEnemyStatusText.text = enemyText + enemyBuffs;
    }

    private int CauseDamage(float damage, Unit damagedUnit)
    {
        // floor the damage to int 
        int iDmg = Mathf.FloorToInt(damage);
        
        int iDef;
        if (damagedUnit == _mPlayer)
        {
            iDef = Mathf.FloorToInt(GetPlayerDef());
        }
        else
        {
            iDef = Mathf.FloorToInt(GetEnemyDef());
        }
        
        if (iDmg != 0)
        {
            // damage equation
            iDmg = Math.Clamp(iDmg - iDef, 1, 2 * iDmg);
        }
        
        damagedUnit.hp -= iDmg;

        if (damagedUnit == _mPlayer && _mParryBuff.IsOn())
        {
            // player get reverse and lose parry
            _mParryBuff.SetLastRounds(0);
            _mReverseBuff.SetLastRounds(2);
            AddLog($"[玩家]受到攻击失去“招架”,获得了“逆势”(持续两回合)");
        }
        
        return iDmg;
    }

    private void EnemyStealDef(float amount)
    {
        _mPlayer.def -= amount;
        _mEnemy.def += amount;
    }

    public bool CheckSeqPattern(params CardType[] pattern)
    {
        int patternSize = pattern.Length;

        if (_mSeq.Count < patternSize)
        {
            return false;
        }

        for (int i = 0; i < patternSize; i++)
        {
            if (_mSeq[_mSeq.Count - patternSize + i] != pattern[i])
            {
                return false;
            }
        }
        return true;
    }
    
    public void TryPlayCard()
    {
        var type = _mHandCards[_mSelectedCardIdx];
        int apCost = 0;
        switch (type)
        {
            case CardType.LightPunch:
            case CardType.Parry:
                apCost = 1;
                break;
            case CardType.HeavyPunch:
                apCost = 2;
                break;
            default:
                apCost = 0;
                break;
        }

        if (apCost > _mPlayerAp)
        {
            return;
        }
        else
        {
            _mPlayerAp -= apCost;
        }

        switch (type)
        {
            case CardType.LightPunch:
                PlayLightPunch();
                break;
            case CardType.HeavyPunch:
                PlayHeavyPunch();
                break;
            case CardType.Parry:
                PlayParry();
                break;
            case CardType.MindPower:
                PlayMindPower();
                break;
            case CardType.Overload:
                PlayOverload();
                break;
        }
        _mSeq.Add(type);
        
        _mHandCards.RemoveAt(_mSelectedCardIdx);
        _mSelectedCardIdx = Math.Clamp(_mSelectedCardIdx - 1, 0, _mSelectedCardIdx);
        UpdateHandCards();
        UpdateStatusUI();
    }

    private void PlayLightPunch()
    {
        // in reversePosture, light punch and consume reversePosture
        if (_mReverseBuff.IsOn())
        {
            float fDmg = 0.5f * _mPlayer.pwr; 
            var d = CauseDamage(fDmg, _mEnemy);
            AddLog($"[玩家]使用逆势轻击对[敌人]造成{d}点伤害");
            _mStumbleBuff.SetLastRounds(1);
            AddLog($"[玩家]使用逆势轻击对[敌人]施加了踉跄效果(防御-1，持续一回合)");
            return;
        }

        // light-light-light => double light
        if (CheckSeqPattern(CardType.LightPunch, CardType.LightPunch))
        {
            float fDmg = 0.5f * _mPlayer.pwr; 
            var d = CauseDamage(fDmg, _mEnemy);
            AddLog($"[玩家]使用双重打击，第一击对[敌人]造成{d}点伤害");
            d = CauseDamage(fDmg, _mEnemy);
            AddLog($"[玩家]使用双重打击，第二击对[敌人]造成{d}点伤害");
            return;
        }
        
        // normal light
        {
            float fDmg = 0.5f * _mPlayer.pwr; 
            var d = CauseDamage(fDmg, _mEnemy);
            AddLog($"[玩家]使用轻击对[敌人]造成{d}点伤害");
        }
        
    }

    private void PlayHeavyPunch()
    {
        // in posture && mind-heavy && ONLY-ONCE!!! => chi attack B
        if (_mPostureBuff.IsOn() && CheckSeqPattern(CardType.MindPower) && !_mShatteredBuff.IsOn())
        {
            // consume posture
            _mPostureBuff.SetLastRounds(0);
            
            // double ratio attack
            float fDmg = 2.4f * _mPlayer.pwr; 
            var d = CauseDamage(fDmg, _mEnemy);
            AddLog($"[玩家]使用运气攻击B对[敌人]造成{d}点伤害");
            
            // apply enemy shattered
            _mShatteredBuff.isEverLast = true;
            AddLog($"[玩家]使用运气攻击B对[敌人]施加了气海破碎效果(防御-2，持续到结束)");
        }
        
        // in posture && light-heavy => chi attack A
        if (_mPostureBuff.IsOn() && CheckSeqPattern(CardType.LightPunch))
        {
            // consume posture
            _mPostureBuff.SetLastRounds(0);
            
            // double ratio attack
            float fDmg = 2.4f * _mPlayer.pwr; 
            var d = CauseDamage(fDmg, _mEnemy);
            AddLog($"[玩家]使用运气攻击A对[敌人]造成{d}点伤害");
            
            // acquire posture
            _mPostureBuff.SetLastRounds(2);
            AddLog($"[玩家]触发运气攻击A,获得了“气势”(持续2回合)");
            
            return;
        }
        
        // normal heavy
        {
            float fDmg = 1.2f * _mPlayer.pwr; 
            var d = CauseDamage(fDmg, _mEnemy);
            AddLog($"[玩家]使用重击对[敌人]造成{d}点伤害");
        }
    }

    private void PlayParry()
    {
        // acquire parry buff
        _mParryBuff.SetLastRounds(1);
        AddLog($"[玩家]使用招架，获得“招架”(防御+3，持续1回合，被攻击时获得“逆势”)");
    }

    private void PlayMindPower()
    {
        // light-mindpower raise chi
        if (CheckSeqPattern(CardType.LightPunch))
        {
            DrawCard();
            AddLog($"[玩家]使用感受念力，抽一张牌");
            
            _mPostureBuff.SetLastRounds(2);
            AddLog($"[玩家]触发攒劲,获得了“气势”(持续2回合)");
            return;
        }
        
        DrawCard();
        AddLog($"[玩家]使用感受念力，抽一张牌");
    }

    private void PlayOverload()
    {
        _mPlayerAp += 2;
        _mOverloadBuff.SetLastRounds(1);
        AddLog($"[玩家]使用过载，行动点+2，下回合行动点-2");
    }
}
