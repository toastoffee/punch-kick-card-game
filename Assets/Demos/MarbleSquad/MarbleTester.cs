using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace MarbleSquad {
 
    public class MarbleTester : MonoBehaviour {
        
        private Rigidbody _rigidbody;
        
        // Start is called before the first frame update
        void Start() {
            _rigidbody = GetComponent<Rigidbody>();
            
            _rigidbody.AddForce(new Vector3(500.0f, 0.0f, 0.0f));
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.J)) {
                _rigidbody.AddForce(new Vector3(1000.0f, 0.0f, 0.0f));
            }
        }
    }
    
}
