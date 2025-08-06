using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForcedStop : MonoBehaviour
{
    public bool end=false;
    [SerializeField] private GameObject endTEXT;
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Bicycle>())
        {
            Bicycle bicycle = other.GetComponent<Bicycle>();
            if(!end)
            {
                StartCoroutine(ForceStopCoroutine(bicycle));
            }
            else
            {
                if(endTEXT != null) {
                    endTEXT.SetActive(true);
                    bicycle.bicycleStoper = true;
                }
            }

        }
    }




    private IEnumerator ForceStopCoroutine(Bicycle bicycle)
    {
        float duration = 6f;
        float timer = 0f;

        Transform parentTransform = transform.parent;
        float totalDistance = 15f;
        Vector3 startPosition = parentTransform.position;
        Vector3 targetPosition = startPosition + new Vector3(totalDistance, 0f, 0f);

        while (timer < duration)
        {
            bicycle.speed = 0f;
            float t = timer / duration;
            parentTransform.position = Vector3.Lerp(startPosition, targetPosition, t);
            timer += Time.deltaTime;
            yield return null; 
        }
        parentTransform.position = targetPosition;
    }

}
