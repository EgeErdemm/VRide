using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraLock : MonoBehaviour
{
    private Vector3 initialLocalPosition;

    void Awake()
    {
        initialLocalPosition = transform.localPosition;
    }

    void LateUpdate()
    {
        if (transform.localPosition != initialLocalPosition)
        {
            transform.localPosition = initialLocalPosition;
        }
    }
}

