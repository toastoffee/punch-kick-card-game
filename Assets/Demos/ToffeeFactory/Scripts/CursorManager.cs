using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ToffeeFactory {
  public class CursorManager : MonoSingleton<CursorManager> {
    public Texture2D noneCursor;
    public Texture2D buttonDownCursor;
    public Texture2D dragCursor;
    
    private void Start() {
      Cursor.SetCursor(noneCursor, Vector2.zero, CursorMode.Auto);  
    }

    private void Update() {
      if (Input.GetMouseButton(2)) {
        Cursor.SetCursor(dragCursor, Vector2.zero, CursorMode.Auto);
      }
      else if (Input.GetMouseButton(0) || Input.GetMouseButton(1)) {
        Cursor.SetCursor(buttonDownCursor, Vector2.zero, CursorMode.Auto);
      } 
      else {
        Cursor.SetCursor(noneCursor, Vector2.zero, CursorMode.Auto);  
      }
    }
  }  
}

