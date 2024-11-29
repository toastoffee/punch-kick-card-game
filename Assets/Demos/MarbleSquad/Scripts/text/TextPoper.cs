using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;


namespace MarbleSquad {
    public class TextPoper : MonoSingleton<TextPoper> {

    public enum PresetColor
    {
        RedToBlue,
        GreenToYellow,
        BlueToGreen,
        RedToRed,
        RedToWhite
    }
    
    [SerializeField] 
    private TMP_Text textPrefab;

    [SerializeField] 
    private float popUpwardDist,
                    popSwellScale,
                    popSwellDuration,
                    popShrinkScale,
                    popShrinkDuration;

    public void GeneratePopUpText(Vector3 pos, string content, Color startColor, Color endColor)
    {
        
        TMP_Text text = Instantiate(textPrefab, pos, Quaternion.identity);
        
        text.text = content;
        text.color = startColor;

        Tween tween = text.transform.DOMove(pos.ToVec2() + popUpwardDist * Vector2.up, popSwellDuration + popShrinkDuration+(float)0.5);
        
        var seq = DOTween.Sequence();
        seq.Append(text.transform.DOScale(popSwellScale, popSwellDuration));
        seq.Append(text.transform.DOScale(popShrinkScale, popShrinkDuration));
        
        tween = text.DOColor(new Color(endColor.r, endColor.g, endColor.b, 0f), popSwellDuration + popShrinkDuration+(float)0.5);

        StartCoroutine(DestroyAfterwards(text.gameObject, popSwellDuration + popShrinkDuration));

    }
    
    public void GeneratePopUpText(Vector3 pos, string content, PresetColor colorType)
    {
        Color startColor = Color.white, endColor = Color.white;
        switch (colorType)
        {
            case PresetColor.RedToBlue:
                startColor = Color.red;
                endColor = Color.blue;
                break;
            case PresetColor.BlueToGreen:
                startColor = Color.blue;
                endColor = Color.green;
                break;
            case PresetColor.GreenToYellow:
                startColor = Color.green;
                endColor = Color.yellow;
                break;
            case PresetColor.RedToRed:
                startColor = Color.red;
                endColor = Color.red;
                break;
            case PresetColor.RedToWhite:
                startColor = Color.red;
                endColor = Color.white;
                break;
        }
        
        GeneratePopUpText(pos, content, startColor, endColor);

    }
    
    

    private IEnumerator DestroyAfterwards(GameObject obj, float interval)
    {
        yield return new WaitForSeconds(interval);
        
        Destroy(obj);
    }
        
    }
}
