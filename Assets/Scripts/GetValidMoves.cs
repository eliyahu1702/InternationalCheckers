using System.Collections.Generic;
using UnityEngine;
public class GetValidMoves : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    public static List<BoardTile> Get_Valid_Moves(Board gameBoard, int index_x, int index_z)// returns valid moves for a picked checker
    {
        gameBoard.untagAll();
        List<BoardTile> valid_moves = new List<BoardTile>();
        Checker pickedChecker = gameBoard.GetBoardTiles()[index_x, index_z].getChecker();
        BoardTile[,] possitions = gameBoard.GetBoardTiles();
        if (pickedChecker.GetValue() == gameValues.whiteChecker())
        {
            valid_moves = CanTake(gameBoard, pickedChecker, index_x, index_z);
            if (valid_moves.Count > 0)
                return valid_moves;
            else
            {
                valid_moves = getCheckerMoves(gameBoard, index_x, index_z);
                return valid_moves;
            }

        }
        else if (pickedChecker.GetValue() == gameValues.blackChecker())
        {
            valid_moves = CanTake(gameBoard, pickedChecker, index_x, index_z);
            if (valid_moves.Count > 0)
                return valid_moves;
            else
            {
                valid_moves = getCheckerMoves(gameBoard, index_x, index_z);
                return valid_moves;
            }

        }
        else if (pickedChecker.GetValue() == gameValues.WhiteQueen() || pickedChecker.GetValue() == gameValues.BlackQueen())
        {
            valid_moves = CanTake(gameBoard, pickedChecker, index_x, index_z);
            if (valid_moves.Count > 0)
                return valid_moves;
            valid_moves = getQueenMoves(gameBoard, index_x, index_z);
            return valid_moves;
        }
        return valid_moves;
    }//returns a list of avalible moves for all checkers 
    public static bool InRange(int x, int z)
    {
        if (x > 9 || x < 0 || z < 0 || z > 9)
            return false;
        return true;
    }//retuns true if x and z are in the range of the board matrix.
    public static List<BoardTile> getCheckerMoves(Board gameBoard, int index_x, int index_z)// returns moves for a checker
    {
        Checker pickedChecker = gameBoard.GetBoardTiles()[index_x, index_z].getChecker();
        List<BoardTile> valid_moves = new List<BoardTile>();
        BoardTile[,] possitions = gameBoard.GetBoardTiles();
        if (pickedChecker.GetValue() == gameValues.blackChecker())
        {
            if (InRange(index_x - 1, index_z + 1) && possitions[index_x - 1, index_z + 1].getChecker() == null)
                valid_moves.Add(possitions[index_x - 1, index_z + 1]);
            if (InRange(index_x - 1, index_z - 1) && possitions[index_x - 1, index_z - 1].getChecker() == null)
                valid_moves.Add(possitions[index_x - 1, index_z - 1]);
        }
        else
        {
            if (InRange(index_x + 1, index_z + 1) && possitions[index_x + 1, index_z + 1].getChecker() == null)
                valid_moves.Add(possitions[index_x + 1, index_z + 1]);
            if (InRange(index_x + 1, index_z - 1) && possitions[index_x + 1, index_z - 1].getChecker() == null)
                valid_moves.Add(possitions[index_x + 1, index_z - 1]);
        }
        return valid_moves;

    }
    public static List<BoardTile> getQueenMoves(Board gameBoard, int index_x, int index_z)
    {
        int temp_x = index_x, temp_z = index_z;
        List<BoardTile> valid_moves = new List<BoardTile>();
        while (InRange(++temp_x, ++temp_z) && gameBoard.GetBoardTiles()[temp_x, temp_z].getChecker() == null)
            valid_moves.Add(gameBoard.GetBoardTiles()[temp_x, temp_z]);
        temp_x = index_x;
        temp_z = index_z;
        while (InRange(--temp_x, ++temp_z) && gameBoard.GetBoardTiles()[temp_x, temp_z].getChecker() == null)
            valid_moves.Add(gameBoard.GetBoardTiles()[temp_x, temp_z]);
        temp_x = index_x;
        temp_z = index_z;
        while (InRange(++temp_x, --temp_z) && gameBoard.GetBoardTiles()[temp_x, temp_z].getChecker() == null)
            valid_moves.Add(gameBoard.GetBoardTiles()[temp_x, temp_z]);
        temp_x = index_x;
        temp_z = index_z;
        while (InRange(--temp_x, --temp_z) && gameBoard.GetBoardTiles()[temp_x, temp_z].getChecker() == null)
            valid_moves.Add(gameBoard.GetBoardTiles()[temp_x, temp_z]);
        return valid_moves;
    }
    public static List<Checker> MovableCheckers(Board gameBoard, int turn)
    {
        gameBoard.untagAll();
        List<Checker> TakingCheckers = new List<Checker>();
        List<Checker> movingCheckers = new List<Checker>();
        int[,] direction_arr = new int[2, 2];
        List<BoardTile> exzists = null;
        BoardTile tile = null;
        for (int i = 0; i < 10; i++)
            for (int j = 0; j < 10; j++)
            {
                tile = gameBoard.GetBoardTiles()[i, j];
                if (tile.getChecker() != null)
                {
                    if (tile.getChecker().GetColor() == turn)
                    {
                        exzists = CanTake(gameBoard, tile.getChecker(), i, j);
                        
                        if (exzists.Count > 0)
                        {
                            gameBoard.untagAll();
                            TakingCheckers.Add(tile.getChecker());                 
                        }
                        if (Get_Valid_Moves(gameBoard, i, j).Count > 0)
                        {
                            movingCheckers.Add(tile.getChecker());
                            gameBoard.untagAll();
                        }
                            
                    }
                    
                }
            }
        if (TakingCheckers.Count > 0)
            return TakingCheckers;

        return movingCheckers;

    }//returns a list of movable checkers and queens
    public static List<BoardTile> CanTake(Board gameBoard, Checker checker, int index_x, int index_z)
    {
        if (checker.GetType() == typeof(Queen))
            return CanTakeQueen(gameBoard, (Queen)checker, index_x, index_z);
        List<BoardTile> moves = new List<BoardTile>();
       
        if (InRange(index_x + 1, index_z + 1) && !gameBoard.GetBoardTiles()[index_x + 1, index_z + 1].isEmpty() && gameBoard.GetBoardTiles()[index_x + 1, index_z + 1].isEnemyChecker(checker) && !gameBoard.GetBoardTiles()[index_x + 1, index_z + 1].IsTagged())
            if (InRange(index_x + 2, index_z + 2) && gameBoard.GetBoardTiles()[index_x + 2, index_z + 2].isEmpty())
            {
                moves.Add(gameBoard.GetBoardTiles()[index_x + 2, index_z + 2]);
                gameBoard.GetBoardTiles()[index_x + 1, index_z + 1].SetTag(true);
                List<BoardTile> more_moves = CanTake(gameBoard, checker, index_x + 2, index_z + 2);
                if (more_moves.Count > 0)
                    return more_moves;
            } 
        if (InRange(index_x - 1, index_z + 1) && !gameBoard.GetBoardTiles()[index_x - 1, index_z + 1].isEmpty() && gameBoard.GetBoardTiles()[index_x - 1, index_z + 1].isEnemyChecker(checker) && !gameBoard.GetBoardTiles()[index_x - 1, index_z + 1].IsTagged())
            if (InRange(index_x - 2, index_z + 2) && gameBoard.GetBoardTiles()[index_x - 2, index_z + 2].isEmpty())
            {
                moves.Add(gameBoard.GetBoardTiles()[index_x - 2, index_z + 2]);
                gameBoard.GetBoardTiles()[index_x - 1, index_z + 1].SetTag(true);
                List<BoardTile> more_moves = CanTake(gameBoard, checker, index_x - 2, index_z + 2);
                if (more_moves.Count > 0)
                    return more_moves;
            }

        if (InRange(index_x - 1, index_z - 1) && !gameBoard.GetBoardTiles()[index_x - 1, index_z - 1].isEmpty() && gameBoard.GetBoardTiles()[index_x - 1, index_z - 1].isEnemyChecker(checker) && !gameBoard.GetBoardTiles()[index_x - 1, index_z - 1].IsTagged())
            if (InRange(index_x - 2, index_z - 2) && gameBoard.GetBoardTiles()[index_x - 2, index_z - 2].isEmpty())
            {
                moves.Add(gameBoard.GetBoardTiles()[index_x - 2, index_z - 2]);
                gameBoard.GetBoardTiles()[index_x - 1, index_z - 1].SetTag(true);
                List<BoardTile> more_moves = CanTake(gameBoard, checker, index_x - 2, index_z - 2);
                if (more_moves.Count > 0)
                    return more_moves;
            }

        if (InRange(index_x + 1, index_z - 1) && !gameBoard.GetBoardTiles()[index_x + 1, index_z - 1].isEmpty() && gameBoard.GetBoardTiles()[index_x + 1, index_z - 1].isEnemyChecker(checker) && !gameBoard.GetBoardTiles()[index_x + 1, index_z - 1].IsTagged())
            if (InRange(index_x + 2, index_z - 2) && gameBoard.GetBoardTiles()[index_x + 2, index_z - 2].isEmpty())
            {
                moves.Add(gameBoard.GetBoardTiles()[index_x + 2, index_z - 2]);
                gameBoard.GetBoardTiles()[index_x + 1, index_z - 1].SetTag(true);
                List<BoardTile> more_moves = CanTake(gameBoard, checker, index_x + 2, index_z - 2);
                if (more_moves.Count > 0)
                    return more_moves;
            }
        return moves;
    }// returns the allowed squares for a checker to go after it takes
    public static bool onPremotionsquare(Board gameBoard, Checker checker)
    {
        if (gameValues.isChecker(checker))
        {
            if (gameValues.isWhitePease(checker))
            {
                for (int i = 0; i < 10; i++)
                {
                    try
                    {
                        if (gameBoard.GetBoardTiles()[9, i].getChecker() == checker)
                            return true;
                    }
                    catch 
                    {
                        Debug.Log(9 + " " + i + " throws exception");
                        continue;
                    }
                   
                }
            }
            else
            {
                for (int i = 0; i < 10; i++)
                {
                    try
                    {
                        if (gameBoard.GetBoardTiles()[0, i].getChecker() == checker)
                            return true;
                    }
                    catch
                    {
                        Debug.Log(0+ " " + i + " throws exception");
                        continue;
                    }
                }
            }
        }
        return false;
    }//checks if a piece is on a promotion squeare to a queen.
    public static List<BoardTile> CanTakeQueen(Board gameBoard, Queen queen, int index_x, int index_z)//searches if can take in diagonals and shows the avalible places after taking a piece
    {
        List<BoardTile> moves = new List<BoardTile>();
        int temp_x = index_x, temp_z = index_z;
        while (InRange(temp_x, temp_z))
        {
            if (InRange(temp_x + 1, temp_z + 1) && !gameBoard.GetBoardTiles()[temp_x + 1, temp_z + 1].isEmpty())
                if (gameBoard.GetBoardTiles()[temp_x + 1, temp_z + 1].isEnemyChecker(queen) && !gameBoard.GetBoardTiles()[temp_x + 1, temp_z + 1].IsTagged())
                {
                    if (InRange(temp_x + 2, temp_z + 2) && gameBoard.GetBoardTiles()[temp_x + 2, temp_z + 2].isEmpty())
                    {
                        moves.Add(gameBoard.GetBoardTiles()[temp_x + 2, temp_z + 2]);
                        gameBoard.GetBoardTiles()[temp_x + 1, temp_z + 1].SetTag(true);
                        temp_x += 3;
                        temp_z += 3;
                        while (InRange(temp_x, temp_z) && gameBoard.GetBoardTiles()[temp_x, temp_z].isEmpty())
                        {
                            moves.Add(gameBoard.GetBoardTiles()[temp_x, temp_z]);
                            temp_x++;
                            temp_z++;
                        }
                        break;
                    }
                    else break;
                }
                else
                    break;
            temp_x++;
            temp_z++;
        }
        temp_x = index_x;
        temp_z = index_z;
        while (InRange(temp_x, temp_z))
        {
            if (InRange(temp_x - 1, temp_z + 1) && !gameBoard.GetBoardTiles()[temp_x - 1, temp_z + 1].isEmpty())
                if (gameBoard.GetBoardTiles()[temp_x - 1, temp_z + 1].isEnemyChecker(queen) && !gameBoard.GetBoardTiles()[temp_x - 1, temp_z + 1].IsTagged())
                {
                    if (InRange(temp_x - 2, temp_z + 2) && gameBoard.GetBoardTiles()[temp_x - 2, temp_z + 2].isEmpty())
                    {
                        moves.Add(gameBoard.GetBoardTiles()[temp_x - 2, temp_z + 2]);
                        gameBoard.GetBoardTiles()[temp_x - 1, temp_z + 1].SetTag(true);
                        temp_x -= 3;
                        temp_z += 3;
                        while (InRange(temp_x, temp_z) && gameBoard.GetBoardTiles()[temp_x, temp_z].isEmpty())
                        {
                            moves.Add(gameBoard.GetBoardTiles()[temp_x, temp_z]);
                            temp_x--;
                            temp_z++;
                        }
                        break;
                    }
                    else break;
                }
                else
                    break;

            temp_x--;
            temp_z++;
        }
        temp_x = index_x;
        temp_z = index_z;
        while (InRange(temp_x, temp_z))
        {
            if (InRange(temp_x - 1, temp_z - 1) && !gameBoard.GetBoardTiles()[temp_x - 1, temp_z - 1].isEmpty())
                if (gameBoard.GetBoardTiles()[temp_x - 1, temp_z - 1].isEnemyChecker(queen) && !gameBoard.GetBoardTiles()[temp_x - 1, temp_z - 1].IsTagged())
                {
                    if (InRange(temp_x - 2, temp_z - 2) && gameBoard.GetBoardTiles()[temp_x - 2, temp_z - 2].isEmpty())
                    {
                        moves.Add(gameBoard.GetBoardTiles()[temp_x - 2, temp_z - 2]);
                        gameBoard.GetBoardTiles()[temp_x - 1, temp_z - 1].SetTag(true);
                        temp_x -= 3;
                        temp_z -= 3;
                        while (InRange(temp_x, temp_z) && gameBoard.GetBoardTiles()[temp_x, temp_z].isEmpty())
                        {
                            moves.Add(gameBoard.GetBoardTiles()[temp_x, temp_z]);
                            temp_x--;
                            temp_z--;
                        }
                        break;
                    }
                    else break;
                }
                else
                    break;
            temp_x--;
            temp_z--;
        }
        temp_x = index_x;
        temp_z = index_z;
        while (InRange(temp_x, temp_z))
        {
            if (InRange(temp_x + 1, temp_z - 1) && !gameBoard.GetBoardTiles()[temp_x + 1, temp_z - 1].isEmpty())
                if (gameBoard.GetBoardTiles()[temp_x + 1, temp_z - 1].isEnemyChecker(queen) && !gameBoard.GetBoardTiles()[temp_x + 1, temp_z - 1].IsTagged())
                {
                    if (InRange(temp_x + 2, temp_z - 2) && gameBoard.GetBoardTiles()[temp_x + 2, temp_z - 2].isEmpty())
                    {
                        moves.Add(gameBoard.GetBoardTiles()[temp_x + 2, temp_z - 2]);
                        gameBoard.GetBoardTiles()[temp_x + 1, temp_z - 1].SetTag(true);
                        temp_x += 3;
                        temp_z -= 3;
                        while (InRange(temp_x, temp_z) && gameBoard.GetBoardTiles()[temp_x, temp_z].isEmpty())
                        {
                            moves.Add(gameBoard.GetBoardTiles()[temp_x, temp_z]);
                            temp_x++;
                            temp_z--;
                        }
                        break;
                    }
                    else break;
                }
                else
                    break;
            temp_x++;
            temp_z--;
        }
        return moves;
       
    }

    /*public Vector3[] Can_take_White(int index_x,int index_z,int counter)
    {
        if(InRange(index_x-1,inde))
    }
    public Vector3[] Can_take_Black(int index_x, int index_z, int counter)
    {
        
    }
   */
}
