using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BloodCellPoling : MonoBehaviour
{
    [SerializeField] private Transform cycle;
    public GameObject cellPrefab;
    public int poolSize = 50;

    private Queue<GameObject> pool = new Queue<GameObject>();

    private bool maxCell= false;

    private void Start()
    {
        Debug.Log("start");
        StartCoroutine(SpawnCells());
    }



    private Vector3 GetRandomPositionAroundCycle()
    {
        float randomX = Random.Range(-3f, 3f);
        float randomY = Random.Range(-1f, 6f);

        Vector3 offset = new Vector3(randomX, randomY, 0f);
        return cycle.position + offset;
    }


    IEnumerator SpawnCells()
    {
        while (pool.Count <= poolSize && !maxCell)
        {
            yield return new WaitForSeconds(0.025f);
            Vector3 cylcepos = GetRandomPositionAroundCycle();
            GameObject cell = Instantiate(cellPrefab,cylcepos, Quaternion.identity);
            pool.Enqueue(cell);
            if(pool.Count >= poolSize) { maxCell = true; StartCoroutine(TeleportCell());
            }
        }

    }

    IEnumerator TeleportCell()
    {
        while(maxCell)
        {
            yield return new WaitForSeconds(0.025f);
            GameObject cell = pool.Dequeue();
            pool.Enqueue(cell);
            Vector3 vector3 = cell.transform.position;
            vector3.z = cycle.position.z;
            cell.transform.position = vector3;
        }
    }

}
