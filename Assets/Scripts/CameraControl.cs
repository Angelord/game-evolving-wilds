using System;
using System.Collections;
using System.Collections.Generic;
using EvolvingWilds;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraControl : MonoBehaviour {
    public float MinSize = 3.0f;
    public float MaxSize = 7.0f;
    public float ZoomSpeed = 1.0f;
    public float PanSpeed = 12.0f;
    public World World;
    [Range(0.01f, 0.3f)]public float PanMargin;
    private Camera _camera;


    private void Start() {
        _camera = GetComponent<Camera>();
    }

    private void Update() {

        HandleZoom();
        
        HandlePan();
        
        ClampPos();
    }

    private void HandleZoom() {
        float wheel = Input.GetAxis("Mouse ScrollWheel");

        float newSize = _camera.orthographicSize - wheel * ZoomSpeed * Time.deltaTime;
        _camera.orthographicSize = Mathf.Clamp(newSize, MinSize, MaxSize);
    }

    private void HandlePan() {
        Vector2 mousePos = Input.mousePosition;

        float xScreenPos = mousePos.x / Screen.width;
        float yScreenPos = mousePos.y / Screen.height;

        Vector2 pan = Vector2.zero;
        if (xScreenPos < PanMargin) {
            pan.x = -1.0f;
        }
        else if (xScreenPos > 1.0f - PanMargin) {
            pan.x = +1.0f;
        }

        if (yScreenPos < PanMargin) {
            pan.y = -1.0f;
        }
        else if (yScreenPos > 1.0f - PanMargin) {
            pan.y = 1.0f;
        }
        
        pan.Normalize();

        transform.position += PanSpeed * Time.deltaTime * (Vector3) pan;
    }

    private void ClampPos() {
        Vector2 cameraBottomLeft = _camera.ScreenToWorldPoint(new Vector3(0.0f, 0.0f, 0.0f));
        Vector2 cameraTopRight = _camera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0.0f));

        Vector3 offset = Vector3.zero;
        
        if (cameraBottomLeft.x < World.Left) {
            offset.x += World.Left - cameraBottomLeft.x;
        }
        else if (cameraTopRight.x > World.Right) {
            offset.x -= cameraTopRight.x - World.Right;
        }
        
        if (cameraBottomLeft.y < World.Bottom) {
            offset.y += World.Bottom - cameraBottomLeft.y;
        }
        else if (cameraTopRight.y > World.Top) {
            offset.y -= cameraTopRight.y - World.Top;
        }
        
        transform.position += offset;
    }
}
