using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManagment : MonoBehaviour
{
    // Start is called before the first frame update
    public Canvas GameEndCanvas;
    public Text outcomeText;

    // Update is called once per frame
    public void ExitTheGame()
    { Application.Quit(); }
    public void EnableCanvas(Canvas c)
        { c.enabled = true; }
    public void DisableCanvas(Canvas c)
    { c.enabled = false; }
    public void EndGame(int LosingTurn)
    {
        GameEndCanvas.gameObject.SetActive(true);
        if (LosingTurn == gameValues.whiteChecker())
            outcomeText.text = "Black Won";
        else
            outcomeText.text = "White Won"; 
    }
}
