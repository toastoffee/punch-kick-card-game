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
        private static readonly int LineLengthId = Shader.PropertyToID("_lineLength");


        [SerializeField]
        public float maxI, chargingRate;

        private float chargingSign = + 1.0f;
        private float chargingI = 0.0f;
        
        
        private void Start() {
            turnText.text = $"回合 {turn}";
        }

        private void Update() {
            if (isAllAllyDied()) {
                turnText.text = $"友方全灭!";
                return;
            }

            // 结束玩家的回合
            if (isYourTurn && isAllChessStopped() && isAllAllyMoved()) {
                isYourTurn = false;
                turnText.text = $"对方回合 {turn}";
                foreach (var chess in allChess) {
                    if (chess.isMain) {
                        chess.hasMoved = false;
                    }
                }
            }

            // 进行并结束敌人的回合
            if (!isYourTurn && !isAllAllyDied() && isAllChessStopped()) {
                isYourTurn = true;
                turn++;
                turnText.text = $"回合 {turn}";

                if (turn % 3 == 1) {
                    // all solid
                    foreach (var chess in allChess) {
                        if (!chess.isMain && chess.type == EnemyType.Solid) {
                            // eject
                            chess.EjectToNearestPlayer();
                        }
                    }
                }
                else if (turn % 3 == 2) {
                    // all stripe
                    foreach (var chess in allChess) {
                        if (!chess.isMain && chess.type == EnemyType.Stripe) {
                            // eject
                            chess.EjectToNearestPlayer();
                        }
                    }
                }
                else if (turn % 3 == 0) {
                    // black
                    foreach (var chess in allChess) {
                        if (!chess.isMain && chess.type == EnemyType.Black) {
                            // eject
                            chess.EjectToNearestPlayer();
                        }
                    }
                }
                
            }
            
            
        }

        private bool isAllAllyDied() {
            foreach (var chess in allChess) {
                if (chess.isMain) {
                    return false;
                }
            }
            return true;
        }
        
        private bool isAllChessStopped() {
            foreach (var chess in allChess) {
                if (chess._isMoving) {
                    return false;
                }
            }
            return true;
        }

        private bool isAllAllyMoved() {
            foreach (var chess in allChess) {
                if (chess.isMain && !chess.hasMoved) {
                    return false;
                }
            }
            return true;
        }
    }   
}
