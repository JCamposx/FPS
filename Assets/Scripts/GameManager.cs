using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { private set; get; }

    public GameObject crossHair;
    public GameObject gameOverCanvas;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        gameOverCanvas.SetActive(false);
    }

    public void HideCrossHair()
    {
        crossHair.SetActive(false);
    }

    public void ShowCrossHair()
    {
        crossHair.SetActive(true);
    }

    public void ManageRestartMatch()
    {
        gameOverCanvas.SetActive(true);

        if (Input.GetKeyDown(KeyCode.Return)) RestartScene();
    }

    private void RestartScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
