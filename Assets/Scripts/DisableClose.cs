using UnityEngine;

using System;
using System.Runtime.InteropServices;
using UnityEngine;

public class DisableClose : MonoBehaviour
{
    private bool allowQuit = false;

    void OnApplicationQuit()
    {
        if (!allowQuit)
        {
            Debug.Log("Quit prevented.");
            // Cancel the quit by reloading the main scene or resetting state
            UnityEngine.SceneManagement.SceneManager.LoadScene(
                UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex
            );
        }
    }

    public void QuitNormally()
    {
        allowQuit = true;
        Application.Quit();
    }
}
