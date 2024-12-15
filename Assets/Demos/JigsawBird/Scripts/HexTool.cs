using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JigsawBird {
    public static class HexTool {
        
        private static readonly float InnerToOuter = 1.154700538379252f;
        private static readonly float OuterToInner = 0.8660254037844386f;
        
        public static Vector3 ConvertToWorldPos(Vector3 hexPos, float ob) {
            return ConvertToWorldPos(hexPos.x, hexPos.y, hexPos.z, ob);
        }

        public static Vector3 ConvertToWorldPos(float q, float r, float s, float ob) {
            float x = (q - s) * ob;
            float z = ob * InnerToOuter * 1.5f * r;

            return new Vector3(x, 0f, z);
        }
    }   
}