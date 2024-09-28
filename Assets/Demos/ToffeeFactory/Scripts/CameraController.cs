using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ToffeeFactory {
  public class CameraController : MonoBehaviour {

    [SerializeField]
    private float zoomSpeed;
    [SerializeField]
    private float moveSpeed;

    private Camera _camera;

    private Vector2 _dragOrigScreenPos;
    private Vector3 _camOrigPos;
    private DampValue m_dampSpeed;
    private Vector2 m_cachedDir;


    private void Start() {
      _camera = GetComponent<Camera>();
      m_dampSpeed = new DampValue(0.25f);
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

      //WASD MOVE
      var rawInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
      var normInput = rawInput.normalized;

      if (rawInput != Vector2.zero) {
        m_dampSpeed.OnActive();
        m_cachedDir = normInput;
      }
      _camera.transform.position += (Vector3)m_cachedDir * m_dampSpeed.ReadValue() * moveSpeed * Time.deltaTime;
    }
  }
}
