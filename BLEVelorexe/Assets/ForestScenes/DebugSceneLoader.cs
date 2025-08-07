using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class DebugSceneLoader : MonoBehaviour
{
    private SceneAdditiveManager _sceneAdditiveManager;
    public int index = 1;
    private void Start()
    {
        _sceneAdditiveManager = SceneAdditiveManager.Instance;
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            index++;
            string scene ="scene"+index.ToString();
            _sceneAdditiveManager.LoadSceneAdditive(scene);
            Debug.Log("Loading scene: " + scene);

        }
    }
}
