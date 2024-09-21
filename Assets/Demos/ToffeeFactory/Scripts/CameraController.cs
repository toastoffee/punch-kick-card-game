using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ToffeeFactory {
  public class CameraController : MonoBehaviour {

    [SerializeField]
    private float zoomSpeed;

    private Camera _camera;

    private Vector2 _dragOrigScreenPos;
    private Vector3 _camOrigPos;
    
    private void Start() {
      _camera = GetComponent<Camera>();
    }

    void Update() {
      
      // ZOOM
      float mouseScrollVal = Input.mouseScrollDelta.y;
      
      if (mouseScrollVal != 0) {
        mouseScrollVal = mouseScrollVal > 0 ? -1f : 1f;
        _camera.orthographicSize += mouseScrollVal * zoomSpeed * Time.deltaTime;
      }
      
      // DRAG MOVE
      if (Input.GetMouseButtonDown(2)) {
        _dragOrigScreenPos = Input.mousePosition;
        _camOrigPos = _camera.transform.position;
      }
      if (Input.GetMouseButton(2)) {
        Vector2 currentScreenPos = Input.mousePosition;

        Vector3 curPos = _camera.ScreenToWorldPoint(currentScreenPos);
        Vector3 origPos = _camera.ScreenToWorldPoint(_dragOrigScreenPos);
        
        Vector3 dir = origPos - curPos;
        dir.z = 0f;
        _camera.transform.position = _camOrigPos + dir;
      }
      
    }
  } 
}
