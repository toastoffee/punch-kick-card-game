using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using Sequence = DG.Tweening.Sequence;

namespace MarbleSquad {
 
    public class ChessPhysics : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler {
    
        private CircleCollider2D _circleCollider;

        [SerializeField]
        private float _mass = 1.0f;
    
        private Vector2 _velocity = Vector2.zero;

        public bool _isMoving => _velocity.magnitude != .0f;

        public bool isMain = false;

        public Transform visualPart;

        public CircleValueBar healthBar;

        public int maxHealth;

        private int health;

        [SerializeField]
        public MeshRenderer speedRenderer;

        [SerializeField]
        private float maxI;

        [SerializeField]
        private float maxChargingLen;
        
        private Material speedMat;

        [SerializeField]
        private Transform selectedFrame;

        private bool isSelected = false;
        private bool isDragging = false;
        
        private static readonly int LineLengthId = Shader.PropertyToID("_lineLength");

        public LineRenderer dashLine1;
        public LineRenderer dashLine2;

        public bool hasMoved = false;
        
        // Start is called before the first frame update
        void Start() {
            _circleCollider = GetComponent<CircleCollider2D>();
            
            // register 
            TurnManager.Instance.allChess.Add(this);
            if (isMain) {
                TurnManager.Instance.player = this;
            }

            health = maxHealth;
            healthBar.SetMaxVal(maxHealth);
            healthBar.SetVal(health);

            speedMat = speedRenderer.material;
        }

        // Update is called once per frame
        void Update() {
            
            // apply friction (if moves)
            if (_isMoving) {
                Vector2 friction_dir = -_velocity.normalized;
                Vector2 friction_f = friction_dir * (PhysicsConsts.Instance.d_mu * _mass * PhysicsConsts.Instance.g);
                AddForce(friction_f * (Time.deltaTime * PhysicsConsts.Instance.timeScale));


                if (Vector2.Dot(friction_dir, _velocity) > .0f) {
                    _velocity = Vector2.zero;
                }
            }
            
            // move as speed (if moves)
            if (_isMoving) {
                transform.position += _velocity.ToVec3() * (Time.deltaTime * PhysicsConsts.Instance.timeScale);
            }
            
            // if (_isMoving) {
            //     // rotate to its forward
            //     var z_deg = Vector2.Angle(Vector2.right, _velocity);
            //     var rot = Quaternion.Euler(.0f, .0f, z_deg);
            //
            //     transform.rotation = rot;
            // }

            if (isDragging) {
                dashLine1.gameObject.SetActive(true);
                dashLine2.gameObject.SetActive(true);
                
                // Draw Line
                Vector3 cursorPos = Input.mousePosition;
                Vector3 cursorWorldPos = Camera.main.ScreenToWorldPoint(cursorPos);
                
                Vector2 cursorWorldVec2 = cursorWorldPos.ToVec2();
                
                Vector2 playerToCursorDir = cursorWorldVec2 - transform.position.ToVec2();
                
                Vector2 aimStart = transform.position.ToVec2() - playerToCursorDir;

                Vector2 aimEnd = cursorWorldVec2;
                
                dashLine1.SetPositions(new [] {
                    transform.position.ToVec2().ToVec3() + Vector3.forward * 0.1f, 
                    aimEnd.ToVec3() + Vector3.forward * 0.1f, 
                });
                
                dashLine2.SetPositions(new [] {
                    transform.position.ToVec2().ToVec3() + Vector3.forward * 0.1f, 
                    aimStart.ToVec3() + Vector3.forward * 0.1f, 
                });
                
                dashLine1.material.SetFloat(LineLengthId, (aimStart - aimEnd).magnitude / 2.0f);
                dashLine2.material.SetFloat(LineLengthId, (aimStart - aimEnd).magnitude / 2.0f);
                
                
                // render charging bar
                float chargingRate = (aimStart - aimEnd).magnitude / maxChargingLen;
                chargingRate = Math.Clamp(chargingRate, 0.0f, 1.0f);
                
                speedRenderer.material.SetFloat("_Percent", chargingRate);
            } else {
                dashLine1.gameObject.SetActive(false);
                dashLine2.gameObject.SetActive(false);
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

        public void TakeDamage(int dmg) {
            health = Math.Max(0, health - dmg);
            healthBar.SetVal(health);

            if (health == 0) {
                TurnManager.Instance.allChess.Remove(this);
                Destroy(gameObject);
            }
        }

        public static void HandleChessCollide(ChessPhysics a, ChessPhysics b) {

            bool is_collision_opposite = Vector2.Dot(a._velocity, b._velocity) < 0.0f;
            Vector2 collidePos = (a.transform.position + b.transform.position) / 2.0f;
            
            Vector2 a_v_orig = a._velocity;
            Vector2 b_v_orig = b._velocity;
            
            
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
            
            // b._velocity += I_orig / b._mass;
            b.AddForce(I_orig);
            
            // a._velocity -= I_orig / a._mass;
            a.AddForce(-I_orig);
            
            
            // Calculate damage
            if (is_collision_opposite) {
                float i_dmg_a = Vector2.Dot(a._velocity, b_v_orig * b._mass);
                i_dmg_a = Math.Abs(i_dmg_a);
                
                float i_dmg_b = Vector2.Dot(b._velocity, a_v_orig * a._mass);
                i_dmg_b = Math.Abs(i_dmg_b);

                int dmg_a = 1;
                int dmg_b = 1;

                if (i_dmg_a > PhysicsConsts.Instance.dmg2Amount) {
                    dmg_a = 2;
                }
                if (i_dmg_a > PhysicsConsts.Instance.dmg3Amount) {
                    dmg_a = 3;
                }
                
                if (i_dmg_b > PhysicsConsts.Instance.dmg2Amount) {
                    dmg_b = 2;
                }
                if (i_dmg_b > PhysicsConsts.Instance.dmg3Amount) {
                    dmg_b = 3;
                }
                
                TextPoper.Instance.GeneratePopUpText(a.transform.position + Vector3.back * 0.2f, "-" + dmg_a, TextPoper.PresetColor.RedToBlue);
                TextPoper.Instance.GeneratePopUpText(b.transform.position + Vector3.back * 0.2f, "-" + dmg_b, TextPoper.PresetColor.RedToBlue);
                
                a.TakeDamage(dmg_a);
                b.TakeDamage(dmg_b);
                
            } else {
                Vector2 aToCollision = collidePos - a.transform.position.ToVec2();
                bool isABackCollided = Vector2.Dot(aToCollision, a_v_orig) <= 0.0f;

                ChessPhysics collided = isABackCollided ? a : b;
                
                ChessPhysics collider = isABackCollided ? b : a;
                Vector2 collider_v_orig = isABackCollided ? b_v_orig : a_v_orig;
                
                float i_dmg_collided = Vector2.Dot(collided._velocity, collider_v_orig * collider._mass);
                int dmg_collided = 1;
                if (i_dmg_collided > PhysicsConsts.Instance.dmg2Amount) {
                    dmg_collided = 2;
                }
                if (i_dmg_collided > PhysicsConsts.Instance.dmg3Amount) {
                    dmg_collided = 3;
                }
                
                TextPoper.Instance.GeneratePopUpText(collided.transform.position + Vector3.back * 0.2f, "-" + dmg_collided, TextPoper.PresetColor.RedToWhite);
                collided.TakeDamage(dmg_collided);
                
            }
            
            // generate pop event text
            // if (is_collision_opposite) {
            //     TextPoper.Instance.GeneratePopUpText(collidePos.ToVec3() + Vector3.back * 0.2f, "正撞！", TextPoper.PresetColor.RedToBlue);
            // } else {
            //     TextPoper.Instance.GeneratePopUpText(collidePos.ToVec3() + Vector3.back * 0.2f, "背撞！", TextPoper.PresetColor.BlueToGreen);
            // }

            // apply visual effects 
            float a_angle = Mathf.Atan2(a._velocity.y, a._velocity.x) * Mathf.Rad2Deg;
            float b_angle = Mathf.Atan2(b._velocity.y, b._velocity.x) * Mathf.Rad2Deg;
            a.visualPart.DORotate(new Vector3(0, 0, a_angle), 0.2f);
            b.visualPart.DORotate(new Vector3(0, 0, b_angle), 0.2f);

            Sequence a_seq = DOTween.Sequence();
            Sequence b_seq = DOTween.Sequence();
            
            a_seq.Append(a.visualPart.DOScale(new Vector3(0.9f, 1.1f, 1.0f), 0.07f));
            a_seq.Append(a.visualPart.DOScale(new Vector3(1.1f, 0.9f, 1.0f), 0.07f));
            a_seq.Append(a.visualPart.DOScale(new Vector3(1.0f, 1.0f, 1.0f), 0.07f));

            b_seq.Append(b.visualPart.DOScale(new Vector3(0.9f, 1.1f, 1.0f), 0.07f));
            b_seq.Append(b.visualPart.DOScale(new Vector3(1.1f, 0.9f, 1.0f), 0.07f));
            b_seq.Append(b.visualPart.DOScale(new Vector3(1.0f, 1.0f, 1.0f), 0.07f));
            
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
                
                visualPart.DORotate(new Vector3(0, 0, 90), 0.2f);
                Sequence seq = DOTween.Sequence();
                seq.Append(visualPart.DOScale(new Vector3(0.9f, 1.1f, 1.0f), 0.07f));
                seq.Append(visualPart.DOScale(new Vector3(1.1f, 0.9f, 1.0f), 0.07f));
                seq.Append(visualPart.DOScale(new Vector3(1.0f, 1.0f, 1.0f), 0.07f));
            }
            
            if (other.transform.CompareTag("BarrierY")) {
                _velocity = new Vector2(_velocity.x, -_velocity.y) * PhysicsConsts.Instance.bounce_decay;
                
                visualPart.DORotate(new Vector3(0, 0, 0), 0.2f);
                Sequence seq = DOTween.Sequence();
                seq.Append(visualPart.DOScale(new Vector3(0.9f, 1.1f, 1.0f), 0.07f));
                seq.Append(visualPart.DOScale(new Vector3(1.1f, 0.9f, 1.0f), 0.07f));
                seq.Append(visualPart.DOScale(new Vector3(1.0f, 1.0f, 1.0f), 0.07f));
            }
            
            if (other.transform.CompareTag("Hole")) {
                TextPoper.Instance.GeneratePopUpText(transform.position + Vector3.back * 0.2f, "进洞！", TextPoper.PresetColor.RedToWhite);
                TurnManager.Instance.allChess.Remove(this);
                Destroy(gameObject);
            }
        }
        public void OnPointerEnter(PointerEventData eventData) {
            if (!isMain || hasMoved) {
                return;
            }
            
            selectedFrame.DOScale(Vector3.one * 1.1f, 0.1f);
        }
        public void OnPointerExit(PointerEventData eventData) {
            if (!isMain || hasMoved) {
                return;
            }
            
            selectedFrame.DOScale(Vector3.one, 0.1f);

            if (isSelected) {
                isSelected = false;
                isDragging = true;
                
                speedRenderer.transform.DOScale(Vector3.one, 0.2f);
            }
        }
        public void OnPointerDown(PointerEventData eventData) {
            if (!isMain || hasMoved) {
                return;
            }
            
            isSelected = true;
        }
        public void OnPointerUp(PointerEventData eventData) {
            if (!isMain || hasMoved) {
                return;
            }
            
            isSelected = false;
            isDragging = false;
            hasMoved = true;
            
            // launch
            speedRenderer.transform.DOScale(Vector3.zero, 0.2f);
            
            Vector3 cursorPos = Input.mousePosition;
            Vector3 cursorWorldPos = Camera.main.ScreenToWorldPoint(cursorPos);
                
            Vector2 cursorWorldVec2 = cursorWorldPos.ToVec2();
            Vector2 playerToCursorDir = cursorWorldVec2 - transform.position.ToVec2();
            Vector2 aimStart = transform.position.ToVec2() - playerToCursorDir;
            Vector2 aimEnd = cursorWorldVec2;

            Vector2 dir = (aimStart - aimEnd).normalized;
            
            float chargingRate = (aimStart - aimEnd).magnitude / maxChargingLen;
            chargingRate = Math.Clamp(chargingRate, 0.0f, 1.0f);
            
            AddForce(dir * chargingRate * maxI);
        }
    }

    
}