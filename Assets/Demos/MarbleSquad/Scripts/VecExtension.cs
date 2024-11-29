using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace MarbleSquad {
    public static class VecExtension
    {
        public static Vector3 ToVec3(this Vector2 vec2) {
            return new Vector3(vec2.x, vec2.y, 0f);
        }

        public static Vector2 ToCoordination(this Vector2 vec2, Vector2 orig_x_axis, Vector2 orig_y_axis, Vector2 new_x_axis, Vector2 new_y_axis) {
            
            // switch to (1,0) as x-axis, (0,1) as y-axis
            Vector2 formalized = vec2.x * orig_x_axis + vec2.y * orig_y_axis;

            float new_x_axis_mag = new_x_axis.magnitude;
            float new_y_axis_mag = new_y_axis.magnitude;
            
            // projection
            float x_amount = formalized.ProjectLength(new_x_axis) / new_x_axis_mag;
            float y_amount = formalized.ProjectLength(new_y_axis) / new_y_axis_mag;

            return new Vector2(x_amount, y_amount);
        }
    }   
}
