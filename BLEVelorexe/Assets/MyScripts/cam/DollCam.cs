using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Oculus.Interaction.Samples;

public class DollCam : MonoBehaviour
{
    public CinemachineDollyCart cart;
    public float[] stopPoint;
    public float waitTime;
    private int currentStopIndex = 0;
    private bool isWaiting=false;
    private float waitTimer;
    private float initalSpeed;

    public Transform lookAtObject;

    private void Start()
    {
        initalSpeed = cart.m_Speed;
        Debug.Log(initalSpeed);
    }
    private void Update()
    {
        if (isWaiting)
        {

            if (currentStopIndex % 2 == 0)
            {
                // lookTarget'ý saða kaydýr (YEREL koordinatlarda)
                Vector3 targetPosition = new Vector3(10f, lookAtObject.localPosition.y, lookAtObject.localPosition.z);
                lookAtObject.localPosition = Vector3.Lerp(lookAtObject.localPosition, targetPosition, Time.deltaTime * 1f);
            }

            cart.m_Speed = 0;
            waitTimer += Time.deltaTime;
            if (waitTimer > waitTime)
            {
                cart.m_Speed = initalSpeed;
                isWaiting = false;
                waitTimer = 0;

            }
        }
        else
        {
            if(lookAtObject.localPosition.x <= 0.1f) { return; }
            Vector3 targetPosition = new Vector3(0f, lookAtObject.localPosition.y, lookAtObject.localPosition.z);
            lookAtObject.localPosition = Vector3.Lerp(lookAtObject.localPosition, targetPosition, Time.deltaTime * 2f);
        }
        Debug.Log(cart.m_Speed);


    }

    private void FixedUpdate()
    {
        if (cart.m_Position >= stopPoint[currentStopIndex]-5f && cart.m_Position <= stopPoint[currentStopIndex] && !isWaiting)
        {
            currentStopIndex++;
            isWaiting = true;
            Debug.Log(cart.m_Position);
        }
    }

}
