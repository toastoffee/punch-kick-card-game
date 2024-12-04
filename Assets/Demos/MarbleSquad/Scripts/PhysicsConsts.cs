using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace MarbleSquad {
 
    public class PhysicsConsts : MonoSingleton<PhysicsConsts> {
    
        [SerializeField]
        public float d_mu, s_mu;
    
        public float g = 9.81f;

        public float bounce_decay = 0.9f;
        
        public float dmg2Amount, dmg3Amount;

        public float timeScale = 1.0f;

        public TMP_Text timeScaleText;

        private List<float> timeScalePresets = new List<float>{ 1.0f , 0.2f, 0.0f };

        private int activateIdx = 0;

        private void Start() {
            timeScale = timeScalePresets[activateIdx];
        }

        private void Update() {
            if (Input.GetKeyDown(KeyCode.Space)) {
                timeScale = timeScalePresets[activateIdx++ % timeScalePresets.Count];
            }

            timeScaleText.text = $"Time Scale = {timeScale}";
        }
    }
    
}