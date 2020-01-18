using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shaker : MonoBehaviour {

    [SerializeField] private float shakeAmount = 1.0f;
    [SerializeField] private float shakeSpeed = 1.0f;

    private Vector3 initialPos;

    public float ShakeAmount { get { return shakeAmount; } set { shakeAmount = value; } }
    public float ShakeSpeed { get { return shakeSpeed; } set { shakeSpeed = value; } }

    private void OnEnable() {
        initialPos = transform.localPosition;
    }

    private void OnDisable() {
        transform.localPosition = initialPos;
    }

    private void Update() {
        
        Vector3 pos = transform.localPosition;
        pos.x = initialPos.x + Mathf.Sin(Time.time * shakeSpeed) * shakeAmount;
        transform.localPosition = pos;
    }
}