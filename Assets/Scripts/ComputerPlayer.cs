using System.Collections.Generic;
using UnityEngine;

public class GameMove
{
    public int source_x;
    public int source_z;
    public int target_x;
    public int target_z;
    public GameMove(int source_x, int source_z, int target_x, int target_z)
    {
        this.source_x = source_x;
        this.source_z = source_z;
        this.target_x = target_x; 
        this.target_z = target_z;
    }
    public GameMove()
    {
        source_x = 0;
        source_z = 0;  
        target_x = 0;
        target_z = 0;
    }
    public void UpdateMove(int source_x, int source_z, int target_x, int target_z)
    {
        this.source_x = source_x;
        this.source_z = source_z;
        this.target_x = target_x;
        this.target_z = target_z;
    }
    public int GetSrc_X()
        { return source_x; }
    public void SetSrc_X(int src_x)
    { source_x = src_x; }

    public int GetSrc_Z()
        { return source_z; }
    public void SetSrc_Z(int src_z)
    { source_z = src_z; }
    public int GetTrt_X()
        { return target_x; }
    public void SetTrt_X(int trt_x)
    { target_x = trt_x; }
    public int GetTrt_Z()
        { return target_z; }
    public void SetTrt_Z(int trt_z)
    { target_z = trt_z; }

}
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
    public void PlayRandomMove(Board gameBoard)
    {
        if (!GameManager.GetComponent<GameManager>().GetPlayingAi())
            return;
        List<Checker> checkers = GetValidMoves.MovableCheckers(gameBoard, gameBoard.getTurn());
        if (checkers.Count <= 0)
        {
            GameManager.GetComponent<GameManager>().EndGame(gameBoard.getTurn());
            return;
        }

        GameMove bestMove = ComputerMove(gameBoard);
        GameManager.GetComponent<DragObject>().MoveMade(gameBoard, bestMove.GetSrc_X(),bestMove.GetSrc_Z() ,bestMove.GetTrt_X(), bestMove.GetTrt_Z());
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
    public GameMove ComputerMove(Board gameBoard)
    {
        double maxEval = gameBoard.getTurn() == gameValues.WhiteTurn() ? double.NegativeInfinity : double.PositiveInfinity;
        int count = 0;
        GameMove bestMove = new GameMove();
        if (!GameManager.GetComponent<GameManager>().GetPlayingAi())
            return null;
        List<Checker> checkers = GetValidMoves.MovableCheckers(gameBoard, gameBoard.getTurn());
        if (checkers.Count <= 0)
        {
            GameManager.GetComponent<GameManager>().EndGame(gameBoard.getTurn());
            return null;
        }
        foreach(Checker SelecteChecker in checkers)
        {
            BoardTile tileofChecker = findTile(gameBoard, SelecteChecker);
            List<BoardTile> PossibleCheckerMoves = GetValidMoves.Get_Valid_Moves(gameBoard, tileofChecker.getX(), tileofChecker.getZ());
            foreach (BoardTile tile in PossibleCheckerMoves)
            {
                count++;
                Board newBoard = CloneBoard(gameBoard);
                SimulateMove(newBoard,tileofChecker.getX(),tileofChecker.getZ(),tile.getX(),tile.getZ());
                double boardEval = BoardEvaluation(newBoard);
                if (gameBoard.getTurn() == gameValues.WhiteTurn())
                {
                    if (boardEval > maxEval)
                    {
                        bestMove.UpdateMove(tileofChecker.getX(), tileofChecker.getZ(), tile.getX(), tile.getZ());
                        maxEval = boardEval;
                    }
                }
                else
                {
                    if (boardEval < maxEval)
                    {
                        bestMove.UpdateMove(tileofChecker.getX(), tileofChecker.getZ(), tile.getX(), tile.getZ());
                        maxEval = boardEval;
                    }
                }
               // Debug.Log("move number - "+count+": "+(Evaluate(newBoard, gameValues.WhiteTurn()) - Evaluate(newBoard, gameValues.BlackTurn())));
            }
        }
        GetValidMoves.Get_Valid_Moves(gameBoard, bestMove.GetSrc_X(), bestMove.GetSrc_Z());
        return bestMove;
    }
    public void SimulateMove(Board gameBoard, int src_x, int src_z, int dest_x, int dest_z)
    {
        List<BoardTile> AdditionalMoves = new List<BoardTile>();
        Checker movedChecker = gameBoard.GetBoardTiles()[src_x, src_z].getChecker();
        if (gameBoard.TaggedCount() > 0)
        {
            bool took_pieace;
            ChangeBoard.changePossition(gameBoard, src_x, src_z, dest_x, dest_z);
            movedChecker = gameBoard.GetBoardTiles()[dest_x, dest_z].getChecker();
            if (gameValues.isChecker(movedChecker))
            {
                BackTrack(gameBoard, src_x, src_z, dest_x, dest_z);
                unpackTaggedStack();
                took_pieace = gameBoard.DestoyTagged();
            }
            else
                took_pieace = gameBoard.destroyPiecesBetween(src_x, src_z, dest_x, dest_z);
            AdditionalMoves = GetValidMoves.CanTake(gameBoard, movedChecker, dest_x, dest_z);
            if (!took_pieace || AdditionalMoves.Count == 0)
            {
                gameBoard.ChangeTurn();
            }

            //Debug.Log("White Material count = " + gameBoard.getWhiteCheckerCount() + " Black material count = " + gameBoard.getBlackCheckerCount());
        }
        else
        {
            ChangeBoard.changePossition(gameBoard, src_x, src_z, dest_x, dest_z);
            movedChecker = gameBoard.GetBoardTiles()[dest_x, dest_z].getChecker();
            gameBoard.ChangeTurn();

        }
        gameBoard.untagAll();
        if (GetValidMoves.onPremotionsquare(gameBoard, movedChecker) && AdditionalMoves.Count == 0)
        {
            gameBoard.GetBoardTiles()[dest_x, dest_z].setChecker(new Queen(null, movedChecker.GetValue() + 2));
        }
    }
    public double BoardEvaluation(Board gameBoard)
    {
        return Evaluate(gameBoard, gameValues.WhiteTurn()) - Evaluate(gameBoard, gameValues.BlackTurn());
    }
    public double Evaluate(Board gameBoard, int side) // TODO: Evaluate Possition and Display it on screan
    {
        bool tempo = false;
        BoardTile[,] Tiles = gameBoard.GetBoardTiles();
        if (gameBoard.getTurn() == side)
            tempo = true;
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
                        material_count += gameValues.isChecker(checker) ? 1.3 : 5.6;
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

        undefended_checkers *= tempo ? 0.05 : 0.3;
        return material_count - undefended_checkers + centralized_checkers + undefendedBackRow(gameBoard, side);
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
        double eval = 0;
        if (index_x == 4 || index_x == 5)
        {
            try
            {
                if (gameBoard.GetBoardTiles()[index_x - 1, index_z - 1].getChecker().GetColor() == gameBoard.GetBoardTiles()[index_x, index_z].getChecker().GetColor())
                    eval += 0.2;
                if (gameBoard.GetBoardTiles()[index_x + 1, index_z + 1].getChecker().GetColor() == gameBoard.GetBoardTiles()[index_x, index_z].getChecker().GetColor())
                    eval += 0.2;
                if (gameBoard.GetBoardTiles()[index_x + 1, index_z - 1].getChecker().GetColor() == gameBoard.GetBoardTiles()[index_x, index_z].getChecker().GetColor())
                    eval += 0.2;
                if (gameBoard.GetBoardTiles()[index_x - 1, index_z + 1].getChecker().GetColor() == gameBoard.GetBoardTiles()[index_x, index_z].getChecker().GetColor())
                    eval += 0.2;
            }
            catch
            {
                eval += 0.1;
            }
            if (index_z >= 4 && index_z <= 6)
                eval += 0.1;
            eval += 0.2;
        }
        return 0;
    }
    //private double closeToPromotion(Board gameBoard, int side)
    //{
    //    if(side = )
    //}
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
        newBoard.setTurn(gameBoard.getTurn());
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
