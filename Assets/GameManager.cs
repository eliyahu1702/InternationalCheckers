using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    // Start is called before the first frame update
    public Canvas GameEndCanvas;
    public Text outcomeText;
    public bool Playing_White = true;

    // Update is called once per frame
    public void ExitTheGame()
    { Application.Quit(); }
    public void EnableCanvas(Canvas c)
        { c.enabled = true; }
    public void DisableCanvas(Canvas c)
    { c.enabled = false; }
    public void EndGame(int LosingTurn)
    {
        GameObject Board = GameObject.Find("Board");
        GameEndCanvas.gameObject.SetActive(true);
        if (LosingTurn == gameValues.whiteChecker())
        {
            outcomeText.text = "Black Won";
            if (Playing_White)
                Board.GetComponent<playGameSounds>().playLosing();
            else
                Board.GetComponent<playGameSounds>().playWinning();
        }
        else
        {
            outcomeText.text = "White Won";
            if (Playing_White)
                Board.GetComponent<playGameSounds>().playWinning();
            else
                Board.GetComponent<playGameSounds>().playLosing();
        }
             
    }
    public void setPlayingWithWhite(bool setting)
    {
        Playing_White = setting;
    }
}
