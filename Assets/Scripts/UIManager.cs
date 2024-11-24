using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private GameObject pauseUI;
    [SerializeField]
    private PlayerMovement player;
    [SerializeField]
    private TextMeshProUGUI velocidad;
    [HideInInspector]
    public bool isPaused;

    private void Update()
    {
        UpdateHUD();
    }

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

    public void UpdateHUD()
    {
        int mag = player.PlayerSpeed();
        velocidad.text = "Velocidad: " + mag;
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("tu vieja pta");
    }
}
