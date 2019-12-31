using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class WinningWindow : MonoBehaviour
{
    public Text _winningNameText;
    // Start is called before the first frame update
    void Start()
    {
        _winningNameText.text = GameStats.WinningPlayer;
    }

    // Update is called once per frame
    public void GoBackToGame() {
        SceneManager.LoadScene("GameScene");
    }
}
