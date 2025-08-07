using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class terrainTreeCountFinder : MonoBehaviour
{
    //terrain de kaç tane tree var onu görmek icin
    public Terrain terrain;

    void Start()
    {
        if (terrain == null)
            terrain = Terrain.activeTerrain;

        int treeCount = terrain.terrainData.treeInstanceCount;
        Debug.Log("Toplam aðaç sayýsý: " + treeCount);
    }
}
