
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;



public class DragObject : MonoBehaviour
{
    public Checker Tempchecker;
    public AudioSource TakingSound;
    public Board gameBoard;
    private Vector3 mOffset;
    public GameObject TakenChecker;
    public Transform spawningpos;
    private float mZCoord;
    int index_x, index_z, index_highlight_x, index_highlight_z, index_taken_x, index_taken_z;
    public Vector3 location, temp_location;
    List<BoardTile> AdditionalMoves = null;
    public GameObject manager;
    List<Checker> movable_checkers;
    Stack<BoardTile> tileStack;
    Ray ray;
    private AudioSource source;
        [SerializeField] private List<AudioClip> audioClips;
    void Start()
    {
        Tempchecker = new Checker(null, 1);
        GameObject board = GameObject.Find("Board");
        gameBoard = board.GetComponent<CheckerGeneration>().gameBoard;
        tileStack = new Stack<BoardTile>();
        manager = GameObject.Find("Game_Manager");
    }
    void OnMouseDown()
    {
        mZCoord = Camera.main.WorldToScreenPoint(
         gameObject.transform.position).z;
        mOffset = gameObject.transform.position - GetMouseAsWorldPoint();
        location = transform.position;
        temp_location = location;
        List<BoardTile> moves;
        movable_checkers = GetValidMoves.MovableCheckers(gameBoard, gameBoard.getTurn());
        Checker SelectedPiece = null;
        SelectedPiece = PieceFromTransform(location);

        if (SelectedPiece == null || !movable_checkers.Contains(SelectedPiece) || gameBoard.getTurn() != SelectedPiece.GetColor()) return;
        moves = GetValidMoves.Get_Valid_Moves(gameBoard, index_x, index_z);
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
    } // checkes if mouse is held, shows avelable moves for the checker
    public void Update()
    {
        if (manager.GetComponent<GameManager>().Playing_Ai && manager.GetComponent<GameManager>().moveMade == true && Time.frameCount % 30 == 0)
        {
            manager.GetComponent<GameManager>().SetCalculatingText(true);
            manager.GetComponent<GameManager>().doingMove = true;  
            

        }
        if (manager.GetComponent<GameManager>().doingMove && Time.frameCount % 60 == 0 )
        {
            ComputerMove();
            manager.GetComponent<GameManager>().SetCalculatingText(false);
            manager.GetComponent<GameManager>().doingMove = false;
        }
    } // works every frame (60fps) checkes if a player made a move and calls the computer move function for it

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

    } // checks collision with highlight which shows avalible moves
    private void OnTriggerExit(Collider other)
    {
        temp_location = location;
    } // checkes if checker stops colliding with the and returns the checker to its oreginal possition 
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
    }// finds selected checker on the board and returns its location
    private Vector3 GetMouseAsWorldPoint()
    {

        // Pixel coordinates of mouse (x,y)

        Vector3 mousePoint = Input.mousePosition + new Vector3(0, 2);



        // z coordinate of game object on screen

        mousePoint.z = mZCoord;



        // Convert it to world points
        ray = Camera.main.ScreenPointToRay(mousePoint);
        return Camera.main.ScreenToWorldPoint(mousePoint);

    }// returns mouse possition scaled to the possition of the camera
    void OnMouseDrag()
    {
        transform.position = GetMouseAsWorldPoint() + mOffset;
        SnapIntoPossition.snapChecker(transform);

    } // moves the object to the mouse possition while mouse is dragged
    private void OnMouseUp()// places the piece on the destination square, changes the turn if a move was made
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
                    BackTrack(gameBoard,index_x, index_z, index_highlight_x, index_highlight_z);
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
                    if (movable_checkers == null || movable_checkers.Count == 0)
                    {
                        manager.GetComponent<GameManager>().EndGame(gameBoard.getTurn());
                    }
                    else
                        manager.GetComponent<GameManager>().moveMade = true;
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
                manager.GetComponent<GameManager>().moveMade = true;
            }
            gameBoard.untagAll();
            
            if (GetValidMoves.onPremotionsquare(gameBoard, movedChecker) && (AdditionalMoves == null  || AdditionalMoves.Count == 0))
            {
                CheckerGeneration.QueenPiece(gameBoard, index_highlight_x, index_highlight_z);
                playPromotionNoice();
            }
        }
        else
        {
            transform.position = location;
        }
        movable_checkers = GetValidMoves.MovableCheckers(gameBoard, gameBoard.getTurn());
        if (movable_checkers == null || movable_checkers.Count == 0)
        {
            manager.GetComponent<GameManager>().EndGame(gameBoard.getTurn());
        }
    }
    private void OnMouseUpAsButton()
    {

    }
    private void playMovingNoice(Checker c)
    {
        GameObject Board = GameObject.Find("Board");
        try
        {
            if (c.GetType() == typeof(Checker))
            {
                Board.GetComponent<playGameSounds>().playCheckerMove(c);
            }
            else
            {
                Board.GetComponent<playGameSounds>().playQueenMove();
            }
        }
        catch
        {
            Board.GetComponent<playGameSounds>().playCheckerMove(Tempchecker);
        }

            
    }// plays a sound if a piece is moved/taken/promoted
    private void playTakingNoice()
    {
        GameObject Board = GameObject.Find("Board");
        Board.GetComponent<playGameSounds>().playTaking();
    }//plays moving sound
    public void playPromotionNoice()
    {
        GameObject Board = GameObject.Find("Board");
        Board.GetComponent<playGameSounds>().playPromotion();
    }//plays promotion sound
    public bool BackTrack(Board gameBoard,int src_x, int src_z, int dest_x, int dest_z)
    {
        bool took = false;
        if (src_x == dest_x && src_z == dest_z)
        {
            return true;
        }
        if (GetValidMoves.InRange(src_x + 1, src_z + 1) && gameBoard.GetBoardTiles()[src_x + 1, src_z + 1].IsTagged())
        {
            gameBoard.GetBoardTiles()[src_x + 1, src_z + 1].SetTag(false);
            if (BackTrack(gameBoard,src_x + 2, src_z + 2, dest_x, dest_z))
            {
                try
                {
                    tileStack.Push(gameBoard.GetBoardTiles()[src_x + 1, src_z + 1]);
                    took = true;
                }
                catch
                {
                    return true;
                }
                
            }       
        }
        if (GetValidMoves.InRange(src_x - 1, src_z + 1) && gameBoard.GetBoardTiles()[src_x - 1, src_z + 1].IsTagged())
        {
            gameBoard.GetBoardTiles()[src_x - 1, src_z + 1].SetTag(false);
            if (BackTrack(gameBoard, src_x - 2, src_z + 2, dest_x, dest_z))
            {
                try
                {
                    tileStack.Push(gameBoard.GetBoardTiles()[src_x - 1, src_z + 1]);
                    took = true;
                }
                catch
                {
                    return true;
                }

            }
        }
        if (GetValidMoves.InRange(src_x - 1, src_z - 1) && gameBoard.GetBoardTiles()[src_x - 1, src_z - 1].IsTagged())
        {
            gameBoard.GetBoardTiles()[src_x - 1, src_z - 1].SetTag(false);
            if (BackTrack(gameBoard, src_x - 2, src_z - 2, dest_x, dest_z))
            {
                try
                {
                    tileStack.Push(gameBoard.GetBoardTiles()[src_x - 1, src_z - 1]);
                    took = true;
                }
                catch
                {
                    return true;
                }
            }
        }
        if (GetValidMoves.InRange(src_x + 1, src_z - 1) && gameBoard.GetBoardTiles()[src_x + 1, src_z - 1].IsTagged())
        {
            gameBoard.GetBoardTiles()[src_x + 1, src_z - 1].SetTag(false);
            if (BackTrack(gameBoard, src_x + 2, src_z - 2, dest_x, dest_z))
            {
                try
                {
                    tileStack.Push(gameBoard.GetBoardTiles()[src_x + 1, src_z - 1]);
                    took = true;
                }
                catch
                {
                    return true;
                }
            }
        }
        return took;
    }//function made for chain taking pieces
    public void unpackTaggedStack()
    {
        while (tileStack.Count > 0)
        {
            //BoardTile tile = tileStack.Pop();
            //Debug.Log(tile.getX() +" "+ tile.getZ());
            tileStack.Pop().SetTag(true);
        }
    }// helper function for backtrack
    public async void ComputerMove()
    {
        manager.GetComponent<ComputerPlayer>().PlayComputerMove(GameObject.Find("Board").GetComponent<CheckerGeneration>().gameBoard);
        playMovingNoice(Tempchecker);
        manager.GetComponent<GameManager>().moveMade = false;
        await Task.Yield();

    }// activates playComputerMove() from ComputerPlayer.cs
}