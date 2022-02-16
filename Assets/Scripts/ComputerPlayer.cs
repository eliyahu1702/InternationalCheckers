using System.Collections.Generic;
using UnityEngine;

public class ComputerPlayer : MonoBehaviour
{
    public Stack<BoardTile> tileStack;
    GameObject GameManager;
    // Start is called before the first frame update
    void Start()
    {
        tileStack = new Stack<BoardTile>();
        GameManager = GameObject.Find("Game_Manager");
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void PlayRandomMove(Board gameBoard, List<Checker> checkers)
    {
        if (!GameManager.GetComponent<GameManager>().GetPlayingAi())
            return;
        if (checkers.Count <= 0)
        {
            GameManager.GetComponent<GameManager>().EndGame(gameBoard.getTurn());
            return;
        }
        int index = Random.Range(0, checkers.Count);
        Checker pickedChecker = checkers[index];
        BoardTile tileofChecker = findTile(gameBoard, pickedChecker);
        List<BoardTile> PossibleCheckerMoves = GetValidMoves.Get_Valid_Moves(gameBoard, tileofChecker.getX(), tileofChecker.getZ());
        index = Random.Range(0, PossibleCheckerMoves.Count);

        Debug.Log(Evaluate(gameBoard, gameValues.WhiteTurn()) - Evaluate(gameBoard, gameValues.BlackTurn()));

        GameManager.GetComponent<DragObject>().MoveMade(gameBoard, tileofChecker.getX(), tileofChecker.getZ(), PossibleCheckerMoves[index].getX(), PossibleCheckerMoves[index].getZ());
    }
    public BoardTile findTile(Board gameBoard, Checker checker)
    {
        for (int i = 0; i < 10; i++)
        {
            for (int j = 0; j < 10; j++)
            {
                if (gameBoard.GetBoardTiles()[i, j].getChecker() == checker)
                    return gameBoard.GetBoardTiles()[i, j];
            }
        }
        return null;
    }
    public double BoardEvaluation(Board gameBoard)
    {
        return Evaluate(gameBoard, gameValues.WhiteTurn()) - Evaluate(gameBoard, gameValues.BlackTurn());
    }
    public double Evaluate(Board gameBoard, int side) // TODO: Evaluate Possition and Display it on screan
    {

        BoardTile[,] Tiles = gameBoard.GetBoardTiles();

        double material_count = 0;
        double undefended_checkers = 0;
        double centralized_checkers = 0;
        for (int i = 0; i < 10; i++)
        {
            for (int j = 0; j < 10; j++)
            {
                Checker checker = Tiles[i, j].getChecker();
                try
                {
                    if (checker.GetColor() == side)
                    {
                        material_count += gameValues.isChecker(checker) ? 1 : 4;
                        if (UndefendenAttackedChecker(gameBoard, i, j))
                        {
                            undefended_checkers++;
                        }
                        centralized_checkers += CentralizedChecker(gameBoard, i, j);

                    }

                }
                catch
                {
                    //no checker in the tile
                }
            }
        }
        return material_count - (undefended_checkers * 0.5) + centralized_checkers + undefendedBackRow(gameBoard, side);
    }

    private bool UndefendenAttackedChecker(Board gameBoard, int index_x, int index_z)
    {
        bool underAttack = false;
        Checker currentChecker = gameBoard.GetBoardTiles()[index_x, index_z].getChecker();
        int temp_x, temp_z;
        try
        {
            if (gameBoard.GetBoardTiles()[index_x + 1, index_z + 1].isEmpty())
            {
                if (gameBoard.GetBoardTiles()[index_x - 1, index_z - 1].isEnemyChecker(currentChecker))
                    underAttack = true;
                temp_x = index_x - 1;
                temp_z = index_z - 1;
                try
                {
                    while (gameBoard.GetBoardTiles()[temp_x, temp_z].isEmpty())
                    {
                        temp_x--;
                        temp_z--;
                    }
                    if (gameBoard.GetBoardTiles()[temp_x, temp_z].isEnemyChecker(currentChecker) && (gameBoard.GetBoardTiles()[temp_x, temp_z].getChecker().GetValue() == gameValues.BlackQueen() || gameBoard.GetBoardTiles()[temp_x, temp_z].getChecker().GetValue() == gameValues.WhiteQueen()))
                        underAttack = true;
                }
                catch
                {
                    // no checker found
                }
            }
            if (gameBoard.GetBoardTiles()[index_x - 1, index_z - 1].isEmpty())
            {
                if (gameBoard.GetBoardTiles()[index_x + 1, index_z + 1].isEnemyChecker(currentChecker))
                    underAttack = true;
                else
                {
                    temp_x = index_x + 1;
                    temp_z = index_z + 1;
                    try
                    {
                        while (gameBoard.GetBoardTiles()[temp_x, temp_z].isEmpty())
                        {
                            temp_x++;
                            temp_z++;
                        }
                        if (gameBoard.GetBoardTiles()[temp_x, temp_z].isEnemyChecker(currentChecker) && (gameBoard.GetBoardTiles()[temp_x, temp_z].getChecker().GetValue() == gameValues.BlackQueen() || gameBoard.GetBoardTiles()[temp_x, temp_z].getChecker().GetValue() == gameValues.WhiteQueen()))
                            underAttack = true;
                    }
                    catch
                    {
                        // no checker found
                    }
                }

            }
            if (gameBoard.GetBoardTiles()[index_x + 1, index_z - 1].isEmpty())
            {
                if (gameBoard.GetBoardTiles()[index_x - 1, index_z + 1].isEnemyChecker(currentChecker))
                    underAttack = true;
                else
                {
                    temp_x = index_x - 1;
                    temp_z = index_z + 1;
                    try
                    {
                        while (gameBoard.GetBoardTiles()[temp_x, temp_z].isEmpty())
                        {
                            temp_x--;
                            temp_z++;
                        }
                        if (gameBoard.GetBoardTiles()[temp_x, temp_z].isEnemyChecker(currentChecker) && (gameBoard.GetBoardTiles()[temp_x, temp_z].getChecker().GetValue() == gameValues.BlackQueen() || gameBoard.GetBoardTiles()[temp_x, temp_z].getChecker().GetValue() == gameValues.WhiteQueen()))
                            underAttack = true;
                    }
                    catch
                    {
                        // no checker found
                    }
                }

            }
            if (gameBoard.GetBoardTiles()[index_x - 1, index_z + 1].isEmpty())
            {
                if (gameBoard.GetBoardTiles()[index_x + 1, index_z - 1].isEnemyChecker(currentChecker))
                    underAttack = true;
                else
                {
                    temp_x = index_x + 1;
                    temp_z = index_z - 1;
                    try
                    {
                        while (gameBoard.GetBoardTiles()[temp_x, temp_z].isEmpty())
                        {
                            temp_x++;
                            temp_z--;
                        }
                        if (gameBoard.GetBoardTiles()[temp_x, temp_z].isEnemyChecker(currentChecker) && (gameBoard.GetBoardTiles()[temp_x, temp_z].getChecker().GetValue() == gameValues.BlackQueen() || gameBoard.GetBoardTiles()[temp_x, temp_z].getChecker().GetValue() == gameValues.WhiteQueen()))
                            underAttack = true;
                    }
                    catch
                    {
                        // no checker found
                    }
                }

            }

        }
        catch
        {
            return underAttack;
        }

        return underAttack;
    }
    private double CentralizedChecker(Board gameBoard, int index_x, int index_z)
    {
        if (index_x == 4 || index_x == 5)
        {
            if (index_z >= 4 && index_z <= 6)
                return 0.1;
            return 0.05;
        }
        return 0;
    }
    private double undefendedBackRow(Board gameBoard, int turn)
    {
        double backRowCount = 0;
        if (turn == gameValues.WhiteTurn())
        {
            for (int i = 0; i < 10; i++)
            {
                try
                {
                    gameBoard.GetBoardTiles()[0, i].getChecker().GetValue();
                    backRowCount += 0.1;
                }
                catch
                {
                    backRowCount -= 0.1;
                }
            }
        }
        else
        {
            for (int i = 0; i < 10; i++)
            {
                try
                {
                    gameBoard.GetBoardTiles()[9, i].getChecker().GetValue();
                    backRowCount += 0.1;
                }
                catch
                {
                    backRowCount -= 0.1;
                }
            }
        }
        return backRowCount;
    }
    private Board CloneBoard(Board gameBoard)
    {
        Board newBoard = null;
        BoardTile[,]
        BoardTiles = new BoardTile[10, 10];
        Debug.Log("Constructing new Board");
        int counterrows = 0;
        int countercols = 0;
        for (float i = 4.5f; i >= -4.5; i--)
        {
            countercols = 0;
            for (float j = 4.5f; j >= -4.5f; j--)
            {
                BoardTiles[counterrows, countercols] = new BoardTile(new Vector3(j, 8.5f, i), null, counterrows, countercols);
                countercols++;
            }
            counterrows++;
        }
        for (int i = 0; i < 10; i++)
        {
            for (int j = 0; j < 10; j++)
            {
                try
                {
                    BoardTiles[i, j].setChecker(gameBoard.GetBoardTiles()[i, j].getChecker().Clone());
                }
                catch 
                { 
                    //no checker found;
                }
            }
        }
        newBoard = new Board(BoardTiles);
        return newBoard;
    }
    public bool BackTrack(Board gameBoard, int src_x, int src_z, int dest_x, int dest_z)
    {
        bool took = false;
        if (src_x == dest_x && src_z == dest_z)
        {
            return true;
        }
        if (GetValidMoves.InRange(src_x + 1, src_z + 1) && gameBoard.GetBoardTiles()[src_x + 1, src_z + 1].IsTagged())
        {
            gameBoard.GetBoardTiles()[src_x + 1, src_z + 1].SetTag(false);
            if (BackTrack(gameBoard, src_x + 2, src_z + 2, dest_x, dest_z))
            {
                tileStack.Push(gameBoard.GetBoardTiles()[src_x + 1, src_z + 1]);
                took = true;
            }
        }
        if (GetValidMoves.InRange(src_x - 1, src_z + 1) && gameBoard.GetBoardTiles()[src_x - 1, src_z + 1].IsTagged())
        {
            gameBoard.GetBoardTiles()[src_x - 1, src_z + 1].SetTag(false);
            if (BackTrack(gameBoard, src_x - 2, src_z + 2, dest_x, dest_z))
            {
                tileStack.Push(gameBoard.GetBoardTiles()[src_x - 1, src_z + 1]);
                took = true;
            }
        }
        if (GetValidMoves.InRange(src_x - 1, src_z - 1) && gameBoard.GetBoardTiles()[src_x - 1, src_z - 1].IsTagged())
        {
            gameBoard.GetBoardTiles()[src_x - 1, src_z - 1].SetTag(false);
            if (BackTrack(gameBoard, src_x - 2, src_z - 2, dest_x, dest_z))
            {
                tileStack.Push(gameBoard.GetBoardTiles()[src_x - 1, src_z - 1]);
                took = true;
            }
        }
        if (GetValidMoves.InRange(src_x + 1, src_z - 1) && gameBoard.GetBoardTiles()[src_x + 1, src_z - 1].IsTagged())
        {
            gameBoard.GetBoardTiles()[src_x + 1, src_z - 1].SetTag(false);
            if (BackTrack(gameBoard, src_x + 2, src_z - 2, dest_x, dest_z))
            {
                tileStack.Push(gameBoard.GetBoardTiles()[src_x + 1, src_z - 1]);
                took = true;
            }
        }
        return took;
    }
    public void unpackTaggedStack()
    {
        while (tileStack.Count > 0)
        {
            tileStack.Pop().SetTag(true);
        }
    }
}
