using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class Bicycle : MonoBehaviour
{
    [SerializeField] private GameObject Wheele1, Wheele2, Pedal;
    [SerializeField] private float rotationSpeed;
    private Rigidbody rb;

    private float speed = 0f;
    private IEventBus eventBus;
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        eventBus = EventBus.Instance;
        DontDestroyOnLoad(gameObject);
    }
    private void OnEnable()
    {
        eventBus.Subscribe<SpeedEvent>(OnSpeedChange);
    }

    private void OnSpeedChange(SpeedEvent @event)
    {
        speed = @event.speed;
    }

    void Update()
    {
        Vector3 forward = transform.forward;
        rb.velocity = forward * speed;
        if(rb.velocity.sqrMagnitude <0.2f) { rb.velocity = Vector3.zero; return; }
        RotateLocally(Pedal, 0f);
        RotateLocally(Wheele1, 1f);
        RotateLocally(Wheele2, 1f);
    }

    private void RotateLocally(GameObject obj, float pedalSpeed)
    {
        obj.transform.localRotation *= Quaternion.Euler(pedalSpeed * rb.velocity.sqrMagnitude *rotationSpeed * Time.deltaTime , 0f, 0f);
        if(pedalSpeed ==0f)
        {
            obj.transform.localRotation *= Quaternion.Euler(rb.velocity.sqrMagnitude *rotationSpeed*0.33f * Time.deltaTime , 0f, 0f);

        }
    }
}
