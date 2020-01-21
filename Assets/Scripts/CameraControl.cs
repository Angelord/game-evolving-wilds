using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraControl : MonoBehaviour {
    public float MinSize = 3.0f;
    public float MaxSize = 7.0f;
    public float ZoomSpeed = 1.0f;
    private Camera _camera;


    private void Start() {
        _camera = GetComponent<Camera>();
    }

    private void Update() {

        float wheel = Input.GetAxis("Mouse ScrollWheel");

        float newSize = _camera.orthographicSize - wheel * ZoomSpeed * Time.deltaTime;
        _camera.orthographicSize = Mathf.Clamp(newSize, MinSize, MaxSize);
    }
}
