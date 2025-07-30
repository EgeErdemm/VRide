using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class SceneAdditiveManager : MonoBehaviour
{
    public static SceneAdditiveManager Instance { get; private set; }
    public HashSet<string> loadedScenes = new HashSet<string>();
    public int sceneIndex {get; private set;}
    [SerializeField] private GameObject scene0Cam;


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(this.gameObject);
        sceneIndex = 1;
    }
    private void Start()
    {
        loadedScenes.Add("scene0");
        LoadSceneAdditive("scene2");
    }

    public void StartCycling()
    {
        scene0Cam.SetActive(false);
        UnloadScene("scene0");
        LoadSceneAdditive("scene1");
        LoadSceneAdditive("scene2");

    }

    public void LoadSceneAdditive(string sceneName)
    {
        if (!loadedScenes.Contains(sceneName))
        {
            StartCoroutine(LoadSceneCoroutine(sceneName));
        }
    }

    public void UnloadScene(string sceneName)
    {
        if (loadedScenes.Contains(sceneName))
        {
            StartCoroutine(UnloadSceneCoroutine(sceneName));
            Debug.Log("scene deleting:" + sceneName);
        }

    }

    IEnumerator LoadSceneCoroutine(string sceneName)
    {
        AsyncOperation op = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        op.allowSceneActivation = true;
        yield return new WaitUntil(() => op.isDone);
        loadedScenes.Add(sceneName);
        sceneIndex++;

    }

    IEnumerator UnloadSceneCoroutine(string sceneName)
    {
        AsyncOperation op = SceneManager.UnloadSceneAsync(sceneName);
        yield return new WaitUntil(()=> op.isDone);
        loadedScenes.Remove(sceneName);
    }

    public bool IsSceneLoaded(string sceneName)
    {
        return loadedScenes.Contains(sceneName);
    }
    

}
