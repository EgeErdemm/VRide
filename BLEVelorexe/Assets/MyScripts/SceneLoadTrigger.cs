using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneLoadTrigger : MonoBehaviour
{
    private SceneAdditiveManager _sceneAdditiveManager;

    private void Start()
    {
        _sceneAdditiveManager = SceneAdditiveManager.Instance;
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.GetComponent<Bicycle>())
        {
            string sceneName = "scene" + (_sceneAdditiveManager.sceneIndex);
            _sceneAdditiveManager.LoadSceneAdditive(sceneName);
            StartCoroutine(UnLoadTrigger());
            Debug.Log("bicycle detected");
        }
    }

    IEnumerator UnLoadTrigger()
    {
        string sceneName ="scene" + (_sceneAdditiveManager.sceneIndex-2);
        yield return new WaitForSeconds(5);
        _sceneAdditiveManager.UnloadScene(sceneName);

    }
}
