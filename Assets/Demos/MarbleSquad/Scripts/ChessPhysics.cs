using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;

namespace MarbleSquad {
 
    public class ChessPhysics : MonoBehaviour {
    
        private CircleCollider2D _circleCollider;

        [SerializeField]
        private float _mass = 1.0f;
    
        private Vector2 _velocity = Vector2.zero;

        private bool _isMoving => _velocity.magnitude != .0f;

        public bool isMain = false;
    
        // Start is called before the first frame update
        void Start() {
            _circleCollider = GetComponent<CircleCollider2D>();
        }

        // Update is called once per frame
        void Update() {
            
            // apply friction (if moves)
            if (_isMoving) {
                Vector2 friction_dir = -_velocity.normalized;
                Vector2 friction_f = friction_dir * (PhysicsConsts.Instance.d_mu * _mass * PhysicsConsts.Instance.g);
                AddForce(friction_f * Time.deltaTime);


                if (Vector2.Dot(friction_dir, _velocity) > .0f) {
                    _velocity = Vector2.zero;
                }
            }

            if (Input.GetMouseButtonDown(0) && isMain) {
                Vector3 cursorPos = Input.mousePosition;
                Vector3 cursorWorldPos = Camera.main.ScreenToWorldPoint(cursorPos);

                Vector2 cursorWorldVec2 = cursorWorldPos.ToVec2();

                Vector2 force = cursorWorldVec2 - transform.position.ToVec2();
                
                AddForce(force * 2.0f);
            }

            if (Input.GetKeyDown(KeyCode.J) && isMain) {
                AddForce(Vector2.right * 10.0f);
            }
            
            
            // move as speed (if moves)
            if (_isMoving) {
                transform.position += _velocity.ToVec3() * Time.deltaTime;
            }

            if (_isMoving) {
                // rotate to its forward
                var z_deg = Vector2.Angle(Vector2.right, _velocity);
                var rot = Quaternion.Euler(.0f, .0f, z_deg);

                transform.rotation = rot;
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

        public static void HandleChessCollide(ChessPhysics a, ChessPhysics b) {

            bool is_collision_opposite = Vector2.Dot(a._velocity, b._velocity) < 0.0f;
            Vector2 collidePos = (a.transform.position + b.transform.position) / 2.0f;
            
            Vector2 a_v_orig = a._velocity;
            Vector2 b_v_orig = b._velocity;
            
            // Vector2 F_dir = (a.transform.position - b.transform.position).normalized;
            // Vector2 F_v_dir = new Vector2(- F_dir.y, F_dir.x);
            //
            // Vector2 v1_h = a._velocity.ProjectLength(F_dir) * F_dir;
            // Vector2 v2_h = b._velocity.ProjectLength(F_dir) * F_dir;
            // Vector2 v1_h_new = (2 * b._mass * v2_h + (a._mass - b._mass) * v1_h) / (a._mass + b._mass);
            // Vector2 v2_h_new = (2 * a._mass * v1_h + (b._mass - a._mass) * v2_h) / (a._mass + b._mass);
            //
            // Vector2 v1_v = a._velocity.ProjectLength(F_v_dir) * F_v_dir;
            // Vector2 v2_v = b._velocity.ProjectLength(F_v_dir) * F_v_dir;
            // Vector2 v1_v_new = (2 * b._mass * v2_v + (a._mass - b._mass) * v1_v) / (a._mass + b._mass);
            // Vector2 v2_v_new = (2 * a._mass * v1_v + (b._mass - a._mass) * v2_v) / (a._mass + b._mass);
            //
            // Vector2 v1_new = v1_h_new + v1_v_new;
            // Vector2 v2_new = v2_h_new + v2_v_new;
            
            // ver 2 
            // float v1_x_new = (2 * b._mass * b._velocity.x + (a._mass - b._mass) * a._velocity.x) / (a._mass + b._mass);
            // float v2_x_new = (2 * a._mass * a._velocity.x + (b._mass - a._mass) * b._velocity.x) / (a._mass + b._mass);
            //
            // float v1_y_new = (2 * b._mass * b._velocity.y + (a._mass - b._mass) * a._velocity.y) / (a._mass + b._mass);
            // float v2_y_new = (2 * a._mass * a._velocity.y + (b._mass - a._mass) * b._velocity.y) / (a._mass + b._mass);
            //
            // a._velocity = new Vector2(v1_x_new, v1_y_new);
            // b._velocity = new Vector2(v2_x_new, v2_y_new);
            
            // ver 3
            // Vector2 I_dir = (b.transform.position - a.transform.position).normalized;
            // Vector2 velocity_diff = b._velocity - a._velocity;
            //
            // float alpha = I_dir.y / I_dir.x;
            //
            // float numerator = -2.0f * a._mass * b._mass * (velocity_diff.x + alpha * velocity_diff.y);
            // float denominator = (a._mass + b._mass) * (1.0f + alpha * alpha);
            //
            // float x = numerator / denominator;
            //
            // // I to B
            // Vector2 I = new Vector2(x, alpha * x);
            //
            // b._velocity += I / b._mass;
            // a._velocity -= I / a._mass;
            
            
            // ver 4
            Vector2 I_dir = (b.transform.position - a.transform.position).normalized;
            Vector2 new_x_axis = I_dir;
            Vector2 new_y_axis = new Vector2(-I_dir.y, I_dir.x);
            
            Vector2 new_coord_a_v = a._velocity.ToCoordination(Vector2.right, Vector2.up, new_x_axis, new_y_axis);
            Vector2 new_coord_b_v = b._velocity.ToCoordination(Vector2.right, Vector2.up, new_x_axis, new_y_axis);
            
            Vector2 velocity_diff = new_coord_b_v - new_coord_a_v;
            
            float numerator = -2.0f * a._mass * b._mass * velocity_diff.x;
            float denominator = (a._mass + b._mass) * 1.0f;
            
            float x = numerator / denominator;
            Vector2 I_new = new Vector2(x, 0);
            Vector2 I_orig = I_new.ToCoordination(new_x_axis, new_y_axis, Vector2.right, Vector2.up);
            
            b._velocity += I_orig / b._mass;
            a._velocity -= I_orig / a._mass;
            
            
            // Calculate damage
            if (is_collision_opposite) {
                float dmg_a = Vector2.Dot(a._velocity, b_v_orig * b._mass);
                float dmg_b = Vector2.Dot(b._velocity, a_v_orig * a._mass);
                TextPoper.Instance.GeneratePopUpText(a.transform.position + Vector3.back * 0.2f, "-" + Math.Floor(dmg_a), TextPoper.PresetColor.RedToBlue);
                TextPoper.Instance.GeneratePopUpText(b.transform.position + Vector3.back * 0.2f, "-" + Math.Floor(dmg_b), TextPoper.PresetColor.RedToBlue);
            } else {
                Vector2 aToCollision = collidePos - a.transform.position.ToVec2();
                bool isABackCollided = Vector2.Dot(aToCollision, a_v_orig) <= 0.0f;

                ChessPhysics collided = isABackCollided ? a : b;
                
                ChessPhysics collider = isABackCollided ? b : a;
                Vector2 collider_v_orig = isABackCollided ? b_v_orig : a_v_orig;
                
                float dmg_collided = Vector2.Dot(collided._velocity, collider_v_orig * collider._mass);
                TextPoper.Instance.GeneratePopUpText(collided.transform.position + Vector3.back * 0.2f, "-" + dmg_collided, TextPoper.PresetColor.RedToWhite);
            }
            
            // generate pop event text
            // if (is_collision_opposite) {
            //     TextPoper.Instance.GeneratePopUpText(collidePos.ToVec3() + Vector3.back * 0.2f, "正撞！", TextPoper.PresetColor.RedToBlue);
            // } else {
            //     TextPoper.Instance.GeneratePopUpText(collidePos.ToVec3() + Vector3.back * 0.2f, "背撞！", TextPoper.PresetColor.BlueToGreen);
            // }

            // apply visual effects 
            // b.transform.DOShakeScale(0.1f, Vector2.right * 1.1f);
            // a.transform.DOShakeScale(0.1f, Vector2.right * 1.1f);

        }

        // private void OnTriggerEnter2D(Collider2D other) {
        //     if (other.transform.GetComponent<ChessPhysics>() != null && isMain) {
        //         HandleChessCollide(this, other.transform.GetComponent<ChessPhysics>());
        //     }
        //
        //     if (other.CompareTag("BarrierX")) {
        //         Debug.Log("nmsl");
        //         _velocity = new Vector2(-_velocity.x, _velocity.y) * PhysicsConsts.Instance.bounce_decay;
        //     }
        //     
        //     if (other.CompareTag("BarrierY")) {
        //         _velocity = new Vector2(_velocity.x, -_velocity.y) * PhysicsConsts.Instance.bounce_decay;
        //     }
        // }

        private void OnCollisionEnter2D(Collision2D other) {
            if (other.transform.GetComponent<ChessPhysics>() != null) {

                ChessPhysics a = this;
                ChessPhysics b = other.transform.GetComponent<ChessPhysics>();

                float definer = (a.transform.position.x - b.transform.position.x) * (a._velocity.x - b._velocity.x) 
                                + (a.transform.position.y - b.transform.position.y) * (a._velocity.y - b._velocity.y);

                if (definer < .0f) {
                    HandleChessCollide(this, other.transform.GetComponent<ChessPhysics>());
                }
            }

            if (other.transform.CompareTag("BarrierX")) {
                _velocity = new Vector2(-_velocity.x, _velocity.y) * PhysicsConsts.Instance.bounce_decay;
            }
            
            if (other.transform.CompareTag("BarrierY")) {
                _velocity = new Vector2(_velocity.x, -_velocity.y) * PhysicsConsts.Instance.bounce_decay;
            }
        }
    }

    
}