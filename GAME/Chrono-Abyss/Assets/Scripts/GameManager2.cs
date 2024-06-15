using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager2 : MonoBehaviour
{
    public static GameManager2 instance;
    public bool platformsActivated = false;  // Flag to check if platforms should be active

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ActivatePlatforms()
    {
        platformsActivated = true;
    }
}
