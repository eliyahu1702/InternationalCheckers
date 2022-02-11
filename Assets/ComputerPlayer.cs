using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComputerPlayer : MonoBehaviour
{
    GameObject GameManager;
    // Start is called before the first frame update
    void Start()
    {
        GameManager = GameObject.Find("Game_Manager");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void PlayRandomMove(Board gameBoard,List<Checker> checkers)
    {
        if (!GameManager.GetComponent<GameManager>().GetPlayingAi())
            return;
        if (checkers.Count <= 0)
        {
            GameManager.GetComponent<GameManager>().EndGame(gameBoard.getTurn());
            return;
        }
        System.Threading.Thread.Sleep(200);
        int index = Random.Range(0, checkers.Count);
        Checker pickedChecker = checkers[index];
        BoardTile tileofChecker = findTile(gameBoard,pickedChecker);
        List<BoardTile> PossibleCheckerMoves = GetValidMoves.Get_Valid_Moves(gameBoard, tileofChecker.getX(), tileofChecker.getZ());
        index = Random.Range(0, PossibleCheckerMoves.Count);
        GameManager.GetComponent<DragObject>().MoveMade(gameBoard,tileofChecker.getX(), tileofChecker.getZ(), PossibleCheckerMoves[index].getX(), PossibleCheckerMoves[index].getZ());
        //Debug.Log("x = "+PossibleCheckerMoves[index].getX() + " z = " + PossibleCheckerMoves[index].getZ());
    }
    public BoardTile findTile(Board gameBoard, Checker checker)
    {
        for (int i = 0; i < 10; i++)
        {
            for (int j = 0; j < 10 ; j++)
            {
                if (gameBoard.GetBoardTiles()[i, j].getChecker() == checker)
                    return gameBoard.GetBoardTiles()[i, j];
            }
        }
        return null;
    }
   /* public BoardTile DestenationTile(Board gameBoard,BoardTile srcTile)
    {
        
    }*/

    
}
