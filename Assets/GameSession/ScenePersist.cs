using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScenePersist : MonoBehaviour
{
    void Awake()
    {
        int numbScenePersist = FindObjectsOfType<ScenePersist>().Length;
        if (numbScenePersist > 1)
        {
            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
        }
    }
    public void ResetScenePersist()
    {
        Destroy(gameObject);
    }
}
