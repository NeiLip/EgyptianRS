using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class MouseOnBtn : MonoBehaviour
{
    public GameObject button;

    public void OnMouseDown() {
        
        switch (button.name) {
            case "PlayerBtn":
               
                break;
            case "StartBtn":
                GameHandler.StartGame();
                break;
            default:
                break;
        }
    }

    private void OnMouseEnter() {
          Debug.Log("Mouse Over!!!sfddsf");
        //button.GetComponent<SpriteRenderer>().enabled = true;
    }

    //private void OnMouseExit() {
    //    button.GetComponent<SpriteRenderer>().enabled = false;
    //}
}
