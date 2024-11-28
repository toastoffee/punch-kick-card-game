using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace MarbleSquad {
 
    public class PhysicsConsts : MonoSingleton<PhysicsConsts> {
    
        [SerializeField]
        public float d_mu, s_mu;
    
        public float g = 9.81f;
    }

    
}