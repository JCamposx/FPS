using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { private set; get; }

    public GameObject crossHair;

    private void Awake()
    {
        Instance = this;
    }

    public void HideCrossHair()
    {
        crossHair.SetActive(false);
    }

    public void ShowCrossHair()
    {
        crossHair.SetActive(true);
    }
}
