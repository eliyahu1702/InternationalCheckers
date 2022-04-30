using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using System.Runtime.Serialization.Formatters.Binary;
using Newtonsoft.Json;

public class GameManager : MonoBehaviour
{
    // Start is called before the first frame update
    public Canvas GameEndCanvas;
    public Text outcomeText;
    public GameObject CalculatingText;
    public bool Playing_White = true;
    public bool Playing_Ai = true;
    public bool Computer_Playoff = false;
    public bool moveMade = false;
    public bool doingMove = false;
    public Stack<char[,]> Moves_Played = new Stack<char[,]>();
    public Stack<char[,]> moves_back = new Stack<char[,]>();

    public CheckerGeneration CheckerGeneration
    {
        get => default;
        set
        {
        }
    }

    // Update is called once per frame
    public void ExitTheGame()
    {
        SaveFile();
        Application.Quit();
    }
    public void EnableCanvas(Canvas c)
        { c.enabled = true; }
    public void DisableCanvas(Canvas c)
    { c.enabled = false; }
    public void EndGame(int LosingTurn)
    {
        GameObject Board = GameObject.Find("Board");
        GameEndCanvas.enabled = true;
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
    public void setPlayingAi(bool value)
    { Playing_Ai = value; }

    public void setComputerPlayoff(bool value)
    {
        Computer_Playoff = value;
    }
    public bool GetPlayingAi()
    { return Playing_Ai; }
    public bool GetPlayingWhite()
    { return Playing_White; }

    public void SetCalculatingText(bool Enabled)
    {
        CalculatingText.SetActive(Enabled);
    }
    public void previus_move()
    {
        try
        {
            if (moves_back.Count > 0)
            {
                moves_back.Push(Moves_Played.Pop());
            } 
        }
        catch
        {
            //no more moves back
        }
    }
    public void next_move()
    {
        try
        {
            if (moves_back.Count > 0)
                Moves_Played.Push(moves_back.Pop());
        }
        catch
        {
            //no more moves back
        }
    }
    public void SaveFile()
    { 
        string possitionDestination = Application.dataPath + "/positions.dat";
        Dictionary<string, double> data = GameObject.Find("Game_Manager").GetComponent<ComputerPlayer>().getTranspositionTable();
        string json = JsonConvert.SerializeObject(data,Formatting.Indented);
        File.WriteAllText(possitionDestination, json);
    }
    public Dictionary<string, double> LoadFile()
    {
        string json;
        string possitionDestination = Application.dataPath + "/positions.dat";
        Dictionary<string, double> data;
        try
        {
            json = File.ReadAllText(possitionDestination);
            data = JsonConvert.DeserializeObject<Dictionary<string, double>>(json);

        }
        catch
        {
            data = null;
        }
        return data;
    }
}

