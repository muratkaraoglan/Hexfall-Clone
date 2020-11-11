using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CanvasScript : MonoBehaviour
{
    Manage m;


    public void gameOverButton() 
    {
        SceneManager.LoadScene(0);
    }
}
