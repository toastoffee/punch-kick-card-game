using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace MarbleSquad {
    public class TurnManager : MonoSingleton<TurnManager> {
        
        private int turn = 1;
        private bool isYourTurn = true;


        public List<ChessPhysics> allChess = new List<ChessPhysics>();

        
    }   
}
