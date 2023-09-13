using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSession : MonoBehaviour
{
    [SerializeField] int playerLives = 3;

    void Awake()
    {
        int numGameSessions = FindObjectsOfType<GameSession>().Length;
        if (numGameSessions > 1)
        {
            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
        }
    }
    
    public void ProcessPlayerDeath()
    {
        if (playerLives > 1)
        {
            TakeLife();
            int currenySceneIndex = SceneManager.GetActiveScene().buildIndex;
            SceneManager.LoadScene(currenySceneIndex);
        }
        else
        {
            ResetGameSession();
        }
    }

    void TakeLife()
    {
        playerLives -= 1;
    }

    void ResetGameSession()
    {
        SceneManager.LoadScene(0);
        Destroy(gameObject);
    }
}
