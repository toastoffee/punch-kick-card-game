using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

struct Unit
{
    public int hp;
    public float def, pwr;
    
    public Unit(int hp, float def, float pwr)
    {
        this.hp = hp;
        this.def = def;
        this.pwr = pwr;
    }
}

public class DuelManager : MonoBehaviour
{
    [SerializeField]
    private int mPlayerHp = 50, mEnemyHp = 40;

    [SerializeField]
    private float mPlayerDef = 2.0f, mPlayerPwr = 6.0f, 
                  mEnemyDef = 0.0f, mEnemyPwr = 5.0f;

    [SerializeField]
    private Text mPlayerStatusText, mEnemyStatusText;
    
    private bool _mIsPlayerTurn = true;

    private Unit _mPlayer, _mEnemy;

    private int _mEnemyActIdx = -1;
    
    private void Start()
    {
        _mPlayer = new Unit(mPlayerHp, mPlayerDef, mPlayerPwr);
        _mEnemy = new Unit(mEnemyHp, mEnemyDef, mEnemyPwr);
        
        UpdateStatusUI();
    }

    private void Update()
    {
        if (_mIsPlayerTurn)
        {
            // wait for the end button
            
        }
        else
        {
            _mIsPlayerTurn = true;
            EnemyAct();
        }
    }

    public void PlayerEndTurn()
    {
        _mIsPlayerTurn = false;
    }
    
    private void EnemyAct()
    {
        _mEnemyActIdx = _mEnemyActIdx++ % 3;
        if (_mEnemyActIdx == 0)
        {
            // normal attack (1 * pwr)
            float fDmg = 1.0f * _mEnemy.pwr;
            CauseDamage(fDmg, _mPlayer);
            EnemyStealDef(1.0f);
        }
        else if (_mEnemyActIdx == 1)
        {
            // double attack (2 * 0.25 * pwr)
            float fDmg = 0.25f * _mEnemy.pwr;
            CauseDamage(fDmg, _mPlayer);
            EnemyStealDef(1.0f);
            CauseDamage(fDmg, _mPlayer);
            EnemyStealDef(1.0f);
        }
        else
        {
            // defense improved attack
            float fDmg = 1.0f * _mEnemy.pwr + 1.0f * _mEnemy.def;
            CauseDamage(fDmg, _mPlayer);
            EnemyStealDef(1.0f);
        }
        
        UpdateStatusUI();
    }

    private void EndTurn()
    {
        
    }

    private void UpdateStatusUI()
    {
        string playerText = $"玩家\n<color=green>生命\t:{_mPlayer.hp}</color>\n" 
                            + $"<color=orange>力量\t:{_mPlayer.pwr}</color>\n" 
                            + $"<color=brown>防御\t:{_mPlayer.def}</color>";
        string enemyText = $"敌人\n<color=green>生命\t:{_mEnemy.hp}</color>\n" 
                            + $"<color=orange>力量\t:{_mEnemy.pwr}</color>\n" 
                            + $"<color=brown>防御\t:{_mEnemy.def}</color>";

        mPlayerStatusText.text = playerText;
        mEnemyStatusText.text = enemyText;
    }

    private void CauseDamage(float damage, Unit damagedUnit)
    {
        // floor the damage to int 
        int iDmg = Mathf.FloorToInt(damage);
        int iDef = Mathf.FloorToInt(damagedUnit.def);
        
        // damage equation
        iDmg = Math.Min(Math.Max(iDmg - iDef, 1), iDmg);

        damagedUnit.hp -= iDmg;
    }

    private void EnemyStealDef(float amount)
    {
        _mPlayer.def -= amount;
        _mEnemy.def += amount;
    }
}
