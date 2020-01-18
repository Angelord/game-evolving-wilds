using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrowOnStart : MonoBehaviour {

    public float GrowDuration = 1.0f;
    
    private void Start() {
        StartCoroutine(Grow());
    }

    private IEnumerator Grow() {

        float totalTime = 0.0f;
        
        transform.localScale = Vector3.zero;

        while (totalTime < GrowDuration) {
            yield return 0;
            transform.localScale = Vector3.one * totalTime / GrowDuration;
            totalTime += Time.deltaTime;
        }

        transform.localScale = Vector3.one;
    }
}
