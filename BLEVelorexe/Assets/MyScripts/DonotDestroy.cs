using UnityEngine;

public class DonotDestroy : MonoBehaviour
{
    void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

}
