using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


namespace MarbleSquad {
    public class TurnManager : MonoSingleton<TurnManager> {
        
        private int turn = 1;
        private bool isYourTurn = true;

        [HideInInspector]
        public List<ChessPhysics> allChess = new List<ChessPhysics>();
        [HideInInspector]
        public ChessPhysics player = null;
        
        public TMP_Text turnText;
        public LineRenderer dashLine;
        private static readonly int LineLengthId = Shader.PropertyToID("_lineLength");

        private void Start() {
            turnText.text = $"回合 {turn}";
            
            dashLine.gameObject.SetActive(true);
        }

        private void Update() {

            if (isYourTurn) {
                // Draw Line
                Vector3 cursorPos = Input.mousePosition;
                Vector3 cursorWorldPos = Camera.main.ScreenToWorldPoint(cursorPos);

                Vector2 cursorWorldVec2 = cursorWorldPos.ToVec2();
                
                dashLine.SetPositions(new [] {
                    player.transform.position + Vector3.forward * 0.1f, 
                    cursorWorldVec2.ToVec3() + Vector3.forward * 0.1f, 
                });
                
                dashLine.material.SetFloat(LineLengthId, (cursorWorldVec2 - player.transform.position.ToVec2()).magnitude);
            }
            
            if (Input.GetMouseButtonDown(0) && isYourTurn) {
                Vector3 cursorPos = Input.mousePosition;
                Vector3 cursorWorldPos = Camera.main.ScreenToWorldPoint(cursorPos);

                Vector2 cursorWorldVec2 = cursorWorldPos.ToVec2();

                Vector2 force = cursorWorldVec2 - player.transform.position.ToVec2();
                
                player.AddForce(force * 2.0f);

                // end turn
                isYourTurn = false;
                dashLine.gameObject.SetActive(false);
            }

            if (!isYourTurn && isAllChessStopped()) {
                isYourTurn = true;
                turn++;
                turnText.text = $"回合 {turn}";
                
                dashLine.gameObject.SetActive(true);
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
