using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class terrainTreeCountFinder : MonoBehaviour
{
    //terrain de ka� tane tree var onu g�rmek icin
    public Terrain terrain;

    void Start()
    {
        if (terrain == null)
            terrain = Terrain.activeTerrain;

        int treeCount = terrain.terrainData.treeInstanceCount;
        Debug.Log("Toplam a�a� say�s�: " + treeCount);
    }
}
