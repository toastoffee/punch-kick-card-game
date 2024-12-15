using System;
using UnityEngine;

namespace JigsawBird {
    public class TestHexGenerator : MonoSingleton<TestHexGenerator> {
        public float ob = 1.0f;
        public Transform hexPrefab;

        private void Start() {
            // for (int i = -10; i < 10; i++) {
            //     for (int j = -10; j < 10; j++) {
            //         var qsr = new Vector3(i, j, 0-i-j);
            //         Instantiate(hexPrefab, HexTool.ConvertToWorldPos(qsr, oa), Quaternion.identity);
            //     }
            // }
            
            Instantiate(hexPrefab, HexTool.ConvertToWorldPos(new Vector3(0, 0, 0), ob), Quaternion.identity);
            Instantiate(hexPrefab, HexTool.ConvertToWorldPos(new Vector3(1, 0, -1), ob), Quaternion.identity);
            Instantiate(hexPrefab, HexTool.ConvertToWorldPos(new Vector3(1, -1, 0), ob), Quaternion.identity);
        }
    }
}