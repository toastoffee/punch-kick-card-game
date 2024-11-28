using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace MarbleSquad {
    public static class VecExtension
    {
        public static Vector3 ToVec3(this Vector2 vec2) {
            return new Vector3(vec2.x, vec2.y, 0f);
        }
    }   
}
