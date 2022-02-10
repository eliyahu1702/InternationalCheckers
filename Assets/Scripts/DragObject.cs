
using System.Collections.Generic;

using UnityEngine;



public class DragObject : MonoBehaviour
{
    public AudioSource TakingSound;
    public Board gameBoard;
    private Vector3 mOffset;
    public GameObject TakenChecker;
    public Transform spawningpos;
    private float mZCoord;
    int index_x, index_z, index_highlight_x, index_highlight_z, index_taken_x, index_taken_z;
    public Vector3 location, temp_location;
    List<BoardTile> AdditionalMoves = null;
    List<Checker> movable_checkers;
    Stack<BoardTile> tileStack;
    Ray ray;

    private AudioSource source;
        [SerializeField] private List<AudioClip> audioClips;
    void Start()
    {
        GameObject board = GameObject.Find("Board");
        gameBoard = board.GetComponent<CheckerGeneration>().gameBoard;
        tileStack = new Stack<BoardTile>();
    }
    void OnMouseDown()
    {
        gameBoard.untagAll();
        mZCoord = Camera.main.WorldToScreenPoint(
         gameObject.transform.position).z;
        // Store offset = gameobject world pos - mouse world pos
        mOffset = gameObject.transform.position - GetMouseAsWorldPoint();
        location = transform.position;
        temp_location = location;
        List<BoardTile> moves;
        movable_checkers = GetValidMoves.MovableCheckers(gameBoard, gameBoard.getTurn());
        //Debug.Log(gameBoard.TaggedCount());
        Checker SelectedPiece = null;
        SelectedPiece = PieceFromTransform(location);

        if (SelectedPiece == null || !movable_checkers.Contains(SelectedPiece) || gameBoard.getTurn() != SelectedPiece.GetColor()) return;

        gameBoard.untagAll();
        moves = GetValidMoves.Get_Valid_Moves(gameBoard, index_x, index_z);

        /*if (moves.Count == 0)
        {
            gameBoard.ChangeTurn();
            Debug.Log(gameValues.nameByTurn(gameBoard.getTurn()) + " Won");
        }*/
        GameObject Board = GameObject.Find("Board");
        GameObject Highlight = Board.GetComponent<ObjectPooling>().GetPooledObjects();
        if (Highlight != null)
            foreach (var item in moves)
            {
                Highlight.transform.position = item.getBoardPossition();
                Highlight.transform.rotation = Quaternion.identity;
                Highlight.SetActive(true);
                Highlight = Board.GetComponent<ObjectPooling>().GetPooledObjects();
            }
    }
    

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Highlight")
        {
            bool found = false;
            for (index_highlight_x = 0; index_highlight_x < 10; index_highlight_x++)
            {
                for (index_highlight_z = 0; index_highlight_z < 10; index_highlight_z++)
                {
                    if (other.gameObject.transform.position == gameBoard.GetBoardTiles()[index_highlight_x, index_highlight_z].getBoardPossition())
                    { found = true; break; }
                }
                if (index_highlight_z < 10)
                    break;
            }
            if (found)
                temp_location = other.gameObject.transform.position;
            else
                temp_location = location;
        }
        else if (other.gameObject.tag == "Black_Checker" || other.gameObject.tag == "White_Checker" || other.gameObject.tag == "Black_King" || other.gameObject.tag == "White_King")
        {
            bool found = false;
            for (index_taken_x = 0; index_taken_x < 10; index_taken_x++)
            {
                for (index_taken_z = 0; index_taken_z < 10; index_taken_z++)
                {
                    if (other.gameObject.transform.position == gameBoard.GetBoardTiles()[index_taken_x, index_taken_z].getBoardPossition())
                    { found = true; break; }
                }
                if (index_taken_z < 10)
                    break;
            }
            if (found)
                TakenChecker = other.gameObject;
            else
                TakenChecker = null;
        }

    }
    private void OnTriggerExit(Collider other)
    {
        temp_location = location;
    }
    public Checker PieceFromTransform(Vector3 location)
    {
        Checker SelectedPiece = gameBoard.GetBoardTiles()[0,0].getChecker();
        for (index_x = 0; index_x < 10; index_x++)
        {
            for (index_z = 0; index_z < 10; index_z++)
            {
                if (location == gameBoard.GetBoardTiles()[index_x, index_z].getBoardPossition())
                {
                    SelectedPiece = gameBoard.GetBoardTiles()[index_x, index_z].getChecker();
                    break;
                }

            }
            if (index_z < 10)
                break;
        }
        return SelectedPiece;
    }
    private Vector3 GetMouseAsWorldPoint()
    {

        // Pixel coordinates of mouse (x,y)

        Vector3 mousePoint = Input.mousePosition + new Vector3(0, 2);



        // z coordinate of game object on screen

        mousePoint.z = mZCoord;



        // Convert it to world points
        ray = Camera.main.ScreenPointToRay(mousePoint);
        return Camera.main.ScreenToWorldPoint(mousePoint);

    }



    void OnMouseDrag()
    {
        transform.position = GetMouseAsWorldPoint() + mOffset;
        SnapIntoPossition.snapChecker(transform);

    }
    private void OnMouseUp()
    {
        GameObject[] Highlights = GameObject.FindGameObjectsWithTag("Highlight");
        foreach (GameObject h in Highlights)
            h.SetActive(false);
        if (location != temp_location)
        {
            Checker movedChecker;
            if (gameBoard.TaggedCount()>0)
            {
                bool took_pieace;
                transform.position = temp_location;
                ChangeBoard.changePossition(gameBoard, index_x, index_z, index_highlight_x, index_highlight_z);
                movedChecker = gameBoard.GetBoardTiles()[index_highlight_x, index_highlight_z].getChecker();
                if (gameValues.isChecker(movedChecker))
                {
                    BackTrack(index_x, index_z, index_highlight_x, index_highlight_z);
                    unpackTaggedStack();
                    took_pieace = gameBoard.DestoyTagged();
                }
                else
                    took_pieace = gameBoard.destroyPiecesBetween(index_x, index_z, index_highlight_x, index_highlight_z);     
                if (took_pieace)
                    playTakingNoice();
                else
                    playMovingNoice(movedChecker);
                AdditionalMoves = GetValidMoves.CanTake(gameBoard, movedChecker, index_highlight_x, index_highlight_z);
                if (!took_pieace || AdditionalMoves.Count == 0)
                {
                    gameBoard.ChangeTurn();
                }
                    
                //Debug.Log("White Material count = " + gameBoard.getWhiteCheckerCount() + " Black material count = " + gameBoard.getBlackCheckerCount());
            }
            else
            {
                transform.position = temp_location;
                ChangeBoard.changePossition(gameBoard, index_x, index_z, index_highlight_x, index_highlight_z);
                movedChecker = gameBoard.GetBoardTiles()[index_highlight_x, index_highlight_z].getChecker();
                gameBoard.ChangeTurn();
                playMovingNoice(movedChecker);

            }
            gameBoard.untagAll();
            if (GetValidMoves.onPremotionsquare(gameBoard, movedChecker) && AdditionalMoves.Count == 0)
            {
                CheckerGeneration.QueenPiece(gameBoard, index_highlight_x, index_highlight_z);
                playPromotionNoice();
            }
            
            
        }
        else
        {
            transform.position = location;
            gameBoard.untagAll();
        }
        movable_checkers = GetValidMoves.MovableCheckers(gameBoard, gameBoard.getTurn());
        if (movable_checkers.Count == 0)
        {
            GameObject manager = GameObject.Find("Game_Manager");
            manager.GetComponent<GameManager>().EndGame(gameBoard.getTurn());
        }
        //Debug.Log(gameBoard.getTurn());
        
    }
    private void OnMouseUpAsButton()
    {

    }
    private void playMovingNoice(Checker c)
    {
        GameObject Board = GameObject.Find("Board");
         
        if (c.GetType() == typeof(Checker))
        {
            Board.GetComponent<playGameSounds>().playCheckerMove(c);
        }
        else
        {
            Board.GetComponent<playGameSounds>().playQueenMove();
        }
            
    }
    private void playTakingNoice()
    {
        GameObject Board = GameObject.Find("Board");
        Board.GetComponent<playGameSounds>().playTaking();
    }
    public void playPromotionNoice()
    {
        GameObject Board = GameObject.Find("Board");
        Board.GetComponent<playGameSounds>().playPromotion();
    }
    public bool BackTrack(int src_x, int src_z, int dest_x, int dest_z)
    {
        bool took = false;
        if (src_x == dest_x && src_z == dest_z)
        {
            return true;
        }
        if (GetValidMoves.InRange(src_x + 1, src_z + 1) && gameBoard.GetBoardTiles()[src_x + 1, src_z + 1].IsTagged())
        {
            gameBoard.GetBoardTiles()[src_x + 1, src_z + 1].SetTag(false);
            if (BackTrack(src_x + 2, src_z + 2, dest_x, dest_z))
            {
                tileStack.Push(gameBoard.GetBoardTiles()[src_x + 1, src_z + 1]);
                took = true;
            }       
        }
        if (GetValidMoves.InRange(src_x - 1, src_z + 1) && gameBoard.GetBoardTiles()[src_x - 1, src_z + 1].IsTagged())
        {
            gameBoard.GetBoardTiles()[src_x - 1, src_z + 1].SetTag(false);
            if (BackTrack(src_x - 2, src_z + 2, dest_x, dest_z))
            {
                tileStack.Push(gameBoard.GetBoardTiles()[src_x - 1, src_z + 1]);
                took = true;
            }
        }
        if (GetValidMoves.InRange(src_x - 1, src_z - 1) && gameBoard.GetBoardTiles()[src_x - 1, src_z - 1].IsTagged())
        {
            gameBoard.GetBoardTiles()[src_x - 1, src_z - 1].SetTag(false);
            if (BackTrack(src_x - 2, src_z - 2, dest_x, dest_z))
            {
                tileStack.Push(gameBoard.GetBoardTiles()[src_x - 1, src_z - 1]);
                took = true;
            }
        }
        if (GetValidMoves.InRange(src_x + 1, src_z - 1) && gameBoard.GetBoardTiles()[src_x + 1, src_z - 1].IsTagged())
        {
            gameBoard.GetBoardTiles()[src_x + 1, src_z - 1].SetTag(false);
            if (BackTrack(src_x + 2, src_z - 2, dest_x, dest_z))
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
            //BoardTile tile = tileStack.Pop();
            //Debug.Log(tile.getX() +" "+ tile.getZ());
            tileStack.Pop().SetTag(true);
        }
    }
}