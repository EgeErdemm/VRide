using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cellMov : MonoBehaviour
{
    public float speed = 2f; // Birim/sn

    void Start()
    {

        float randomY = Random.Range(0f, 360f);

        // Yeni rotasyon oluþtur
        Quaternion randomRotation = Quaternion.Euler(0f, 0f, randomY);
        transform.rotation = randomRotation;
        float speedMultiplier = Random.Range(2f, 5f);
        speed *= speedMultiplier;
    }
    void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime, Space.Self);
    }
}
