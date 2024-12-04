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
        
        private void Update() {
            if (Input.GetKey(KeyCode.Space)) {
                timeScale = 0.2f;
            } else {
                timeScale = 1.0f;
            }

            timeScaleText.text = $"Time Scale = {timeScale}";
        }
    }
    
}