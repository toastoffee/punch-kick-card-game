using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;


namespace MarbleSquad {
    public class TurnManager : MonoSingleton<TurnManager> {
        
        private int turn = 1;
        private bool isYourTurn = true;
        private bool isCharging = false;
        
        
        [HideInInspector]
        public List<ChessPhysics> allChess = new List<ChessPhysics>();
        [HideInInspector]
        public ChessPhysics player = null;
        
        public TMP_Text turnText;
        public LineRenderer dashLine;
        private static readonly int LineLengthId = Shader.PropertyToID("_lineLength");


        [SerializeField]
        private float maxI, chargingRate;

        private float chargingSign = + 1.0f;
        private float chargingI = 0.0f;
        
        
        private void Start() {
            turnText.text = $"回合 {turn}";
            
            dashLine.gameObject.SetActive(true);
        }

        private void Update() {

            if (!player) {
                return;
            }

            if (isYourTurn && player) {
                // Draw Line
                Vector3 cursorPos = Input.mousePosition;
                Vector3 cursorWorldPos = Camera.main.ScreenToWorldPoint(cursorPos);

                Vector2 cursorWorldVec2 = cursorWorldPos.ToVec2();

                Vector2 playerToCursorDir = (cursorWorldVec2 - player.transform.position.ToVec2()).normalized;

                Vector2 aimEnd = cursorWorldVec2 - playerToCursorDir * 0.5f;
                
                dashLine.SetPositions(new [] {
                    player.transform.position + Vector3.forward * 0.1f, 
                    aimEnd.ToVec3() + Vector3.forward * 0.1f, 
                });
                
                dashLine.material.SetFloat(LineLengthId, (cursorWorldVec2 - player.transform.position.ToVec2()).magnitude);
            }
            
            if (Input.GetMouseButtonDown(0) && isYourTurn && !isCharging) {
                isCharging = true;
                

            }

            if (Input.GetMouseButton(0) && isYourTurn && isCharging) {
                
                chargingI += chargingSign * chargingRate * Time.deltaTime;
                float clampedI = Math.Clamp(chargingI, 0.0f, maxI);

                if (Math.Abs(chargingI - clampedI) > 0.0001f) {
                    chargingSign = -chargingSign;
                }

                chargingI = clampedI;
                
                player.speedRenderer.material.SetFloat("_Percent", chargingI / maxI);
            }

            if (Input.GetMouseButtonUp(0) && isYourTurn && isCharging) {
                Vector3 cursorPos = Input.mousePosition;
                Vector3 cursorWorldPos = Camera.main.ScreenToWorldPoint(cursorPos);

                Vector2 cursorWorldVec2 = cursorWorldPos.ToVec2();

                Vector2 dir = (cursorWorldVec2 - player.transform.position.ToVec2()).normalized;
                
                player.AddForce(dir * chargingI);
                
                // hide charging bar
                player.speedRenderer.transform.DOScale(Vector3.zero, 0.2f);

                // end turn
                isYourTurn = false;
                isCharging = false;
                dashLine.gameObject.SetActive(false);
            }

            if (!isYourTurn && isAllChessStopped()) {
                isYourTurn = true;
                turn++;
                turnText.text = $"回合 {turn}";
                
                dashLine.gameObject.SetActive(true);
                
                // display charging bar
                if (player) {
                    player.speedRenderer.transform.DOScale(Vector3.one, 0.2f);
                    chargingI = 0.0f;
                    chargingSign = +1.0f;
                    player.speedRenderer.material.SetFloat("_Percent", chargingI / maxI);

                }
            }
        }


        private bool isAllChessStopped() {
            foreach (var chess in allChess) {
                if (chess._isMoving) {
                    return false;
                }
            }
            return true;
        }
    }   
}
