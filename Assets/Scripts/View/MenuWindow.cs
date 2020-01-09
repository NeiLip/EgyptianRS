using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuWindow : MonoBehaviour
{
 

    // Update is called once per frame
    public void BackToMenu() {
        SceneManager.LoadScene("Menu");
    }

    public void ToHelp() {
        SceneManager.LoadScene("HowTo");
    }

    public void StartGame() {
        SceneManager.LoadScene("GameScene");
    }
}
