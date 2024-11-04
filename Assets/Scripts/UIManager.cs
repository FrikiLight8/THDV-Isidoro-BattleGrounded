using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private GameObject pauseUI;
    [SerializeField]
    private PlayerMovement player;
    [HideInInspector]
    public bool isPaused;
    public void ActivatePause()
    {
        pauseUI.SetActive(true);
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        player.EnableMouse(true);
        isPaused = true;
    }

    public void DesactivatePause()
    {
        pauseUI.SetActive(false);
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        player.EnableMouse(false);
        isPaused = false;
    }
}
