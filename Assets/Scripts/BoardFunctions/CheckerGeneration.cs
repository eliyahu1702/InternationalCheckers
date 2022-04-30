using UnityEngine;
public static class gameValues // class for the values of the game. all numbers are here 
{

    public static int searchDeapth()
    { return 6; }
    public static int whiteChecker() { return 1; }
    public static int blackChecker() { return 2; }
    public static int WhiteQueen() { return 3; }
    public static int BlackQueen() { return 4; }
    public static int ValueOfPiece(Checker c)
    {
        if (isChecker(c))
            return ValueOfChecker();
        return ValueOfQueen();
    }
    public static int ValueOfChecker() { return 1; }
    public static int ValueOfQueen() { return 5; }
    public static int InnitialMaterial() { return 20; }
    public static int InnitialEvaluation() { return 0; }
    public static bool isWhitePease(Checker checker) { return checker.GetValue() % 2 == 1; }
    public static bool isChecker(Checker checker)
    { return checker != null && checker.GetValue() <= gameValues.blackChecker(); }
}
public class Checker
{
    protected GameObject gameObject;
    protected int value;
    public Checker(GameObject gameObject, int value)
    {
        this.gameObject = gameObject;
        this.value = value;
    }

    public BoardFlip BoardFlip
    {
        get => default;
        set
        {
        }
    }

    public BoardTile BoardTile
    {
        get => default;
        set
        {
        }
    }

    public int GetValue()
    { return this.value; }
    public void SetValue(int value)
    {
        this.value = value;
    }
    public GameObject GetCheckerObject()
    { return this.gameObject; }
    public void SetCheckerObject(GameObject gameObject)
    {
        this.gameObject = gameObject;
    }
    public void destroy()
    {
        if (gameObject != null)
            Object.Destroy(gameObject);
    }
    public int GetColor()
    {
        if (value % 2 == 0)
            return 2;
        return 1;
    }
    public virtual Checker Clone()
    { return new Checker(null, this.value); }
}
public class Queen : Checker
{
    public Queen(GameObject gameObject, int value) : base(gameObject, value)
    {

    }

    public override Checker Clone()
    { return new Queen(null, this.value); }
}
public class BoardTile
{
    int X_Index;
    int Z_Index;
    Vector3 BoardPossition;
    Checker checker;
    bool Tagged;
    public BoardTile(Vector3 boardPossition, Checker checker)
    {
        BoardPossition = boardPossition;
        this.checker = checker;
        Tagged = false;
    }
    public BoardTile(Vector3 boardPossition, Checker checker, int x, int z)
    {
        BoardPossition = boardPossition;
        this.checker = checker;
        X_Index = x;
        Z_Index = z;
    }

    public Board Board
    {
        get => default;
        set
        {
        }
    }

    public int getX()
    { return X_Index; }
    public void setX(int x)
    { X_Index = x; }
    public int getZ()
    { return Z_Index; }
    public void setZ(int z)
    { Z_Index = z; }
    public void setXandZ(int x, int z)
    {
        X_Index = x;
        Z_Index = z;
    }
    public Vector3 getBoardPossition()
    { return BoardPossition; }
    public void setBoardPossition(Vector3 possition)
    { BoardPossition = possition; }
    public Checker getChecker()
    { return checker; }
    public void setChecker(Checker checker)
    { this.checker = checker; }
    public bool isEmpty()
    {
        return checker == null;
    }
    public bool isEnemyChecker(Checker comparitor)
    {
        return comparitor != null && this.checker.GetValue() % 2 != comparitor.GetValue() % 2 && comparitor.GetValue() != 0;
    }
    public void DestroyPiece()
    {
        if (checker != null)
        {
            checker.destroy();
        }
        checker = null;
    }


    public bool IsTagged()
    { return Tagged; }
    public void SetTag(bool value)
    { Tagged = value; }
}
public class Board
{
    private BoardTile[,] BoardTiles;
    private double evaluation;
    private int WhiteMaterial;
    private int WhiteCheckerCount = 20;
    private int BlackMaterial;
    private int BlackCheckerCount = 20;
    private int turn;
    private GameObject queen;
    private Transform QueenPosition;
    private bool takingTurn;
    public Board(BoardTile[,] BoardTiles)
    {
        this.BoardTiles = BoardTiles;
        evaluation = gameValues.InnitialEvaluation();
        WhiteMaterial = gameValues.InnitialMaterial();
        BlackMaterial = gameValues.InnitialMaterial();
        turn = gameValues.whiteChecker();
        takingTurn = false;
    }

    public CheckerGeneration CheckerGeneration
    {
        get => default;
        set
        {
        }
    }

    public BoardTile[,] GetBoardTiles()
    { return BoardTiles; }
    public int getTurn()
    { return turn; }
    public void setTurn(int turn)
    { this.turn = turn; }
    public void ChangeTurn()
    { if (!takingTurn) turn ^= 3; }
    public void destroyPiece(int x_index, int z_index)
    {
        RemovePieceCount(BoardTiles[x_index, z_index].getChecker());
        BoardTiles[x_index, z_index].DestroyPiece();
    }
    public bool destroyPiecesBetween(int src_x, int src_z, int dest_x, int dest_z)
    {
        bool taken = false;
        int x_progression = src_x > dest_x ? -1 : 1;
        int z_progression = src_z > dest_z ? -1 : 1;
        for (src_x += x_progression, src_z += z_progression; src_x != dest_x; src_x += x_progression, src_z += z_progression)
        {
            if (BoardTiles[src_x, src_z].getChecker() != null)
            {
                RemovePieceCount(BoardTiles[src_x, src_z].getChecker());
                BoardTiles[src_x, src_z].DestroyPiece();
                taken = true;
            }
        }
        return taken;
    }
    public void untagAll()
    {
        for (int i = 0; i < 10; i++)
        {
            for (int j = 0; j < 10; j++)
            {
                if (BoardTiles[i, j].IsTagged())
                    BoardTiles[i, j].SetTag(false);
            }
        }
    }
    public int TaggedCount()
    {
        int count = 0;
        for (int i = 0; i < 10; i++)
        {
            for (int j = 0; j < 10; j++)
            {
                if (BoardTiles[i, j].IsTagged())
                {
                    count++;
                }

            }
        }
        return count;
    }
    public bool DestoyTagged()
    {
        bool destroyed = false;
        for (int i = 0; i < 10; i++)
        {
            for (int j = 0; j < 10; j++)
            {
                if (BoardTiles[i, j].IsTagged())
                {
                    BoardTiles[i, j].DestroyPiece();
                    destroyed = true;
                }

            }
        }
        return destroyed;
    }
    public void RemovePieceCount(Checker c)
    {
        if (c == null) return;
        if (gameValues.isWhitePease(c))
        {
            WhiteCheckerCount--;
            WhiteMaterial -= gameValues.ValueOfPiece(c);
        }
        else
        {
            BlackCheckerCount--;
            BlackMaterial -= gameValues.ValueOfPiece(c);
        }

    }
    public void setQueen(GameObject queen)
    { this.queen = queen; }
    public GameObject getQueen()
    { return this.queen; }
    public Transform getQueenTransform()
    { return QueenPosition; }
    public void setQueenTransform(Transform transform)
    { QueenPosition = transform; }

}

public class CheckerGeneration : MonoBehaviour
{

    public Board gameBoard;
    public GameObject draught;
    public GameObject Queen;
    public Transform QueenSpawn;
    public Transform spawningpos;
    // Start is called before the first frame update
    void Start()
    {

    }
    public void BuildBoard()
    {
        BoardTile[,] BoardTiles = new BoardTile[10, 10];
        Debug.Log("starting now");
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
                if ((i >= 0 && i < 4) && j % 2 == 0)
                {

                    if (i % 2 == 0)
                    {
                        Checker newChecker;
                        GameObject MynewObject = new GameObject();
                        MynewObject = Instantiate(draught, BoardTiles[i, j].getBoardPossition(), spawningpos.rotation) as GameObject;
                        MynewObject.GetComponent<MeshRenderer>().material.color = Color.white;
                        MynewObject.transform.localPosition = BoardTiles[i, j].getBoardPossition();
                        MynewObject.tag = "White_Checker";
                        newChecker = new Checker(MynewObject, gameValues.whiteChecker());
                        BoardTiles[i, j].setChecker(newChecker);
                        BoardTiles[i, j].setXandZ(i, j);
                    }
                    else
                    {
                        Checker newChecker;
                        GameObject MynewObject = new GameObject();
                        MynewObject = Instantiate(draught, BoardTiles[i, j + 1].getBoardPossition(), spawningpos.rotation) as GameObject;
                        MynewObject.GetComponent<MeshRenderer>().material.color = Color.white;
                        MynewObject.transform.localPosition = BoardTiles[i, j + 1].getBoardPossition();
                        MynewObject.tag = "White_Checker";
                        newChecker = new Checker(MynewObject, gameValues.whiteChecker());
                        BoardTiles[i, j + 1].setChecker(newChecker);
                        BoardTiles[i, j + 1].setXandZ(i, j + 1);
                    }


                }

                else if (i > 5 && j % 2 == 0)
                {

                    if (i % 2 == 0)
                    {
                        Checker newChecker;
                        GameObject MynewObject = new GameObject();
                        MynewObject = Instantiate(draught, BoardTiles[i, j].getBoardPossition(), spawningpos.rotation) as GameObject;
                        MynewObject.GetComponent<MeshRenderer>().material.color = Color.red;
                        MynewObject.transform.localPosition = BoardTiles[i, j].getBoardPossition();
                        MynewObject.tag = "Black_Checker";
                        newChecker = new Checker(MynewObject, gameValues.blackChecker());
                        BoardTiles[i, j].setChecker(newChecker);
                        BoardTiles[i, j].setXandZ(i, j);
                    }
                    else
                    {
                        Checker newChecker;
                        GameObject MynewObject = new GameObject();
                        MynewObject = Instantiate(draught, BoardTiles[i, j + 1].getBoardPossition(), spawningpos.rotation) as GameObject;
                        MynewObject.GetComponent<MeshRenderer>().material.color = Color.red;
                        MynewObject.tag = "Black_Checker";
                        newChecker = new Checker(MynewObject, gameValues.blackChecker());
                        BoardTiles[i, j + 1].setChecker(newChecker);
                        BoardTiles[i, j + 1].setXandZ(i, j + 1);
                    }


                }
            }
        }
        gameBoard = new Board(BoardTiles);
        gameBoard.setQueen(Queen);
        gameBoard.setQueenTransform(QueenSpawn);
        GameObject manager = GameObject.Find("Game_Manager");
        if (manager.GetComponent<GameManager>().GetPlayingAi() && !manager.GetComponent<GameManager>().GetPlayingWhite())
        {
            manager.GetComponent<GameManager>().moveMade = true;
            //manager.GetComponent<ComputerPlayer>().PlayRandomMove(gameBoard);
        }
    }
    public void CascadeBoard()
    {
        for (int i = 0; i < 10; i++)
        {
            for (int j = 0; j < 10; j++)
            {
                gameBoard.destroyPiece(i, j);
            }
        }
    }
    public GameObject GetQueenObject()
    { return Queen; }
    public static void QueenPiece(Board gameBoard, int index_x, int index_z)
    {
        Queen queen;
        GameObject MynewObject = new GameObject();
        Checker temp;
        try
        {
            temp = gameBoard.GetBoardTiles()[index_x, index_z].getChecker();
        }
        catch
        {
            temp = new Checker(MynewObject, index_x == 0 ? gameValues.whiteChecker() : gameValues.blackChecker());
        }
        gameBoard.destroyPiece(index_x, index_z);
        if (gameValues.isWhitePease(temp))
        {
            MynewObject = Instantiate(gameBoard.getQueen(), gameBoard.GetBoardTiles()[index_x, index_z].getBoardPossition(), gameBoard.getQueenTransform().rotation);
            MynewObject.GetComponent<MeshRenderer>().material.color = Color.white;
            MynewObject.tag = "White_Queen";
            queen = new Queen(MynewObject, gameValues.WhiteQueen());
            gameBoard.GetBoardTiles()[index_x, index_z].setChecker(queen);
        }
        else
        {

            MynewObject = Instantiate(gameBoard.getQueen(), gameBoard.GetBoardTiles()[index_x, index_z].getBoardPossition(), gameBoard.getQueenTransform().rotation);
            MynewObject.GetComponent<MeshRenderer>().material.color = Color.red;
            MynewObject.tag = "Black_Queen";
            queen = new Queen(MynewObject, gameValues.BlackQueen());
            gameBoard.GetBoardTiles()[index_x, index_z].setChecker(queen);
        }
    }
    public void SetPiece(Board gameBoard,int index_x,int index_z,int Value)
    {
        Checker newChecker;
        GameObject MynewObject = new GameObject();
        switch (Value)
        {
            case 1:
                MynewObject = Instantiate(draught, gameBoard.GetBoardTiles()[index_x, index_z].getBoardPossition(), spawningpos.rotation) as GameObject;
                MynewObject.GetComponent<MeshRenderer>().material.color = Color.white;
                MynewObject.transform.localPosition = gameBoard.GetBoardTiles()[index_x,index_z].getBoardPossition();
                MynewObject.tag = "White_Checker";
                newChecker = new Checker(MynewObject, gameValues.whiteChecker());
                gameBoard.GetBoardTiles()[index_x, index_z].setChecker(newChecker);
                break;

            case 2:
                MynewObject = Instantiate(draught, gameBoard.GetBoardTiles()[index_x, index_z].getBoardPossition(), spawningpos.rotation) as GameObject;
                MynewObject.GetComponent<MeshRenderer>().material.color = Color.red;
                MynewObject.transform.localPosition = gameBoard.GetBoardTiles()[index_x, index_z].getBoardPossition();
                MynewObject.tag = "Black_Checker";
                newChecker = new Checker(MynewObject, gameValues.blackChecker());
                gameBoard.GetBoardTiles()[index_x, index_z].setChecker(newChecker);
                break;
            case 3:
                MynewObject = Instantiate(gameBoard.getQueen(), gameBoard.GetBoardTiles()[index_x, index_z].getBoardPossition(), gameBoard.getQueenTransform().rotation);
                MynewObject.GetComponent<MeshRenderer>().material.color = Color.white;
                MynewObject.tag = "White_Queen";
                newChecker = new Queen(MynewObject, gameValues.WhiteQueen());
                gameBoard.GetBoardTiles()[index_x, index_z].setChecker(newChecker);
                break;
            case 4:
                MynewObject = Instantiate(gameBoard.getQueen(), gameBoard.GetBoardTiles()[index_x, index_z].getBoardPossition(), gameBoard.getQueenTransform().rotation);
                MynewObject.GetComponent<MeshRenderer>().material.color = Color.red;
                MynewObject.tag = "Black_Queen";
                newChecker = new Queen(MynewObject, gameValues.BlackQueen());
                gameBoard.GetBoardTiles()[index_x, index_z].setChecker(newChecker);
                break;
            default:
                gameBoard.GetBoardTiles()[index_x,index_z].DestroyPiece();
                break;

        }

    }
}


// Update is called once per frame


