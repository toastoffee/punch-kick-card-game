using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MarbleSquad {
 
    public class ChessPhysics : MonoBehaviour {
    
        private CircleCollider2D _circleCollider;

        [SerializeField]
        private float _mass = 1.0f;
    
        private Vector2 _velocity = Vector2.zero;

        private bool _isMoving => _velocity.magnitude != .0f;
    
        // Start is called before the first frame update
        void Start() {
            _circleCollider = GetComponent<CircleCollider2D>();
        }

        // Update is called once per frame
        void Update() {
            
            // apply friction (if moves)
            if (_isMoving) {
                Vector2 friction_dir = -_velocity.normalized;
                Vector2 friction_f = friction_dir * (PhysicsConsts.Instance.s_mu * _mass * PhysicsConsts.Instance.g);
                AddForce(friction_f * Time.deltaTime);


                if (Vector2.Dot(friction_dir, _velocity) > .0f) {
                    _velocity = Vector2.zero;
                }
            }
            
            // move as speed (if moves)
            if (_isMoving) {
                transform.position += _velocity.ToVec3() * Time.deltaTime;
            }


            if (Input.GetKeyDown(KeyCode.J)) {
                AddForce(Vector2.right * 3.0f);
            }
        }

        public void AddForce(Vector2 ft) {
            
            float force_scale = ft.magnitude;
            
            // if force is less than s_mu * m * g;
            float static_friction = PhysicsConsts.Instance.s_mu * _mass * PhysicsConsts.Instance.g;


            if (!_isMoving && force_scale <= static_friction) {
                // dont move
            } else {
                // calculate acceleration F = m * a
                _velocity += ft / _mass;
            }
        }
    }

    
}