using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
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
            EndTurn();
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
            _mIsPlayerTurn = true;
            EnemyAct();
            
            // end enemy turn, set player turn(recover mana, draw cards)
            _mPlayerAp = mPlayerApPerTurn;
            _mPlayerDrawCards = mPlayerCardsPerTurn;
            DrawCards();
            UpdateHandCards();
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
    
    public void PlayerEndTurn()
    {
        _mIsPlayerTurn = false;
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

    public void EndTurn()
    {
        DiscardCards();
        _mIsPlayerTurn = false;
    }

    private void UpdateStatusUI()
    {
        string playerText = $"玩家\n<color=green>生命\t:{_mPlayer.hp}</color>\n" 
                            + $"<color=orange>力量\t:{_mPlayer.pwr}</color>\n" 
                            + $"<color=brown>防御\t:{_mPlayer.def}</color>\n"
                            + $"<color=yellow>行动点\t:{_mPlayerAp}</color>\n"
                            + $"<color=silver>下回合抽牌数\t:{_mPlayerDrawCards}</color>";
        string enemyText = $"敌人\n<color=green>生命\t:{_mEnemy.hp}</color>\n" 
                            + $"<color=orange>力量\t:{_mEnemy.pwr}</color>\n" 
                            + $"<color=brown>防御\t:{_mEnemy.def}</color>";

        mPlayerStatusText.text = playerText;
        mEnemyStatusText.text = enemyText;
    }

    private int CauseDamage(float damage, Unit damagedUnit)
    {
        // floor the damage to int 
        int iDmg = Mathf.FloorToInt(damage);
        int iDef = Mathf.FloorToInt(damagedUnit.def);

        if (iDmg != 0)
        {
            // damage equation
            iDmg = Math.Clamp(iDmg - iDef, 1, 2 * iDmg);
        }
        
        damagedUnit.hp -= iDmg;
        return iDmg;
    }

    private void EnemyStealDef(float amount)
    {
        _mPlayer.def -= amount;
        _mEnemy.def += amount;
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
                break;
            case CardType.HeavyPunch:
                break;
            case CardType.Parry:
                break;
            case CardType.MindPower:
                break;
            case CardType.Overload:
                break;
        }
        
    }
}
