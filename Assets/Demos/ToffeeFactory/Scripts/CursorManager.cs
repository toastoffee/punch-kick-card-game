using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ToffeeFactory {
  public class CursorManager : MonoSingleton<CursorManager> {
    public Texture2D noneCursor;
    public Texture2D buttonDownCursor;
    public Texture2D dragCursor;
    
    public Vector2 hotspot = Vector2.zero;
    
    private void Start() {
      Cursor.SetCursor(noneCursor, hotspot, CursorMode.Auto);  
    }

    private void Update() {
      if (Input.GetMouseButton(2)) {
        Cursor.SetCursor(dragCursor, hotspot, CursorMode.Auto);
      }
      else if (Input.GetMouseButton(0) || Input.GetMouseButton(1)) {
        Cursor.SetCursor(buttonDownCursor, hotspot, CursorMode.Auto);
      } 
      else {
        Cursor.SetCursor(noneCursor, hotspot, CursorMode.Auto);  
      }
    }
  }  
}

