using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace MarbleSquad {
 
    public class PhysicsConsts : MonoSingleton<PhysicsConsts> {
    
        [SerializeField]
        public float d_mu, s_mu;
    
        public float g = 9.81f;

        public float bounce_decay = 0.9f;
        
        public float upBound, downBound, leftBound, rightBound;
    }

    
}