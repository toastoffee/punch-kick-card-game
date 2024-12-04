using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace MarbleSquad {
 
    public class CircleValueBar : MonoBehaviour {
        private Material _material;
        private static readonly int MaxHp = Shader.PropertyToID("_MaxHp");
        private static readonly int Hp = Shader.PropertyToID("_Hp");

        private void Awake() {
            _material = GetComponent<MeshRenderer>().material;
        }

        public void SetMaxVal(int val) {
            _material?.SetFloat(MaxHp, val);
        }

        public void SetVal(int val) {
            _material?.SetFloat(Hp, val);
        }
        
    }
}
