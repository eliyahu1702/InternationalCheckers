using System.Collections.Generic;
using Unity.Collections;
using Unity.Mathematics;
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
    public void PlayComputerMove(Board gameBoard)
    {
        if (!GameManager.GetComponent<GameManager>().GetPlayingAi())
            return;
        gameBoard.setTurn(GameManager.GetComponent<GameManager>().Playing_White ? 2 : 1);
        int[,] bestMove = Pick_Logic_Move(TranslateBoard(gameBoard),gameBoard.getTurn());
        Logic_To_Display(gameBoard, bestMove);
        gameBoard.ChangeTurn();
        List<int[,]> PossibleResponses = validMoves(bestMove, gameBoard.getTurn());
        if (PossibleResponses == null || PossibleResponses.Count == 0)
        {
            GameManager.GetComponent<GameManager>().EndGame(gameBoard.getTurn());
        }
    }// function called after the human player completes a move, plays the best move according to the computer
    public void Logic_To_Display(Board gameBoard, int[,] logic_board) // translates logics board to display board
    {
        for (int i = 0; i < 10; i++)
        {
            for (int j = i%2; j < 10; j+=2)
            {
                if (gameBoard.GetBoardTiles()[i, j].getChecker() != null)
                {
                    gameBoard.GetBoardTiles()[i, j].DestroyPiece();
                }
                GameObject.Find("Board").GetComponent<CheckerGeneration>().SetPiece(gameBoard,i,j,logic_board[i,j]);
            }
        }
    }
    public int[,] TranslateBoard(Board gameBoard)
    {
        int[,] result = new int[10,10];
        for (int i = 0; i < 10; i++)
        {
            for (int j = 0; j < 10; j++)
            {
                try
                {
                    result[i, j] = gameBoard.GetBoardTiles()[i, j].getChecker().GetValue();
                }
                catch
                {
                    result[i, j] = 0;
                }

            }
        }
        return result;
    } // parses the grafical board for the computer player
    public List<int[,]> validMoves(int[,] logic_board, int turn)
    {
        List<int[,]> taking_moves = new List<int[,]>();
        List<int[,]> walking_moves = new List<int[,]>();
        bool has_to_take = false;
        for (int i = 0; i < 10; i++)
        {
            for (int j = i % 2; j < 10; j += 2)
            {
                if (logic_board[i, j] != 0 && logic_board[i, j] % 2 == turn % 2)
                {
                    List<int[,]> tempMoves = canTakeLogic(logic_board, i, j);
                    if (tempMoves.Count > 0)
                    {
                        has_to_take = true;
                        taking_moves.AddRange(tempMoves);
                    }
                    if (!has_to_take)
                    {
                        tempMoves = MovementLogic(logic_board, i, j);
                        walking_moves.AddRange(tempMoves);
                    }

                }

            }
        }
        return has_to_take ? taking_moves : walking_moves;
    } // find all the valid moves for one side, returns a list of boards
    public List<int[,]> canTakeLogic(int[,] logic_board, int index_x, int index_z)
    {
        if (logic_board[index_x, index_z] > 2)
            return CanTakeQueenLogic(logic_board, index_x, index_z);
        int pieceValue = logic_board[index_x, index_z];
        List<int[,]> moves = new List<int[,]>();
        int[,] directions = new int[4, 2] { { 1, 1 }, { 1, -1 }, { -1, -1 }, { -1, 1 } };
        for (int rep = 0; rep < 4; rep++)
        {
            int[,] temp_move = (int[,])logic_board.Clone();
            try
            {
                if (temp_move[index_x + directions[rep, 0], index_z + directions[rep, 1]] > 0 && temp_move[index_x + directions[rep, 0], index_z + directions[rep, 1]] %2 != temp_move[index_x, index_z] %2)
                {
                    if (temp_move[index_x + directions[rep, 0] * 2, index_z + directions[rep, 1] * 2] <= 0)
                    {
                        temp_move[index_x,index_z] = temp_move[index_x + directions[rep, 0], index_z + directions[rep, 1]] = 0; // tagging the checker for recursion
                        temp_move[index_x + directions[rep, 0] * 2, index_z + directions[rep, 1] * 2] = pieceValue;
                        List<int[,]> AditionalMoves = canTakeLogic(temp_move, index_x + directions[rep, 0] * 2, index_z + directions[rep, 1] * 2);
                        if (AditionalMoves.Count == 0)
                        {
                            if ((pieceValue == gameValues.whiteChecker() && (index_x + directions[rep, 0] * 2 == 9)) || (pieceValue == gameValues.blackChecker() && (index_x + directions[rep, 0] * 2 == 0)))
                                temp_move[index_x + directions[rep, 0] * 2, index_z + directions[rep, 1] * 2] = pieceValue + 2; // qeening the piece
                            moves.Add(temp_move);
                        }
                        else
                            moves = AditionalMoves;
                    }
                }
            }
            catch
            {
                //piece on edge of the screen
            }
        }
        return moves;
    }// returns the possible boards for a piece taking another
    public List<int[,]> MovementLogic(int[,] logic_board, int index_x, int index_z)
    {
        int i;
        int pieceValue = logic_board[index_x, index_z];
        if (pieceValue > 2)
            return MovementLogicQueen(logic_board, index_x, index_z);
        List<int[,]> moves = new List<int[,]>();
        int[,] directions = new int[4, 2] { { 1, 1 }, { 1, -1 }, { -1, -1 }, { -1, 1 } };
        if (pieceValue == gameValues.whiteChecker())
            i = 2;
        else
            i = 4;
        for (int j = i - 2; j < i; j++)
        {
            int[,] newBoard = (int[,])logic_board.Clone();
            int new_X_Index = index_x + directions[j, 0];
            int new_Z_Index = index_z + directions[j, 1];
            try
            {
                if (newBoard[new_X_Index, new_Z_Index] <= 0)
                {
                    newBoard[new_X_Index, new_Z_Index] = pieceValue;
                    newBoard[index_x, index_z] = 0;
                    if ((pieceValue == gameValues.whiteChecker() && (new_X_Index == 9)) || (pieceValue == gameValues.blackChecker() && (new_X_Index == 0)))
                        newBoard[new_X_Index, new_Z_Index] = pieceValue + 2; // qeening the piece
                    moves.Add(newBoard);
                }
            }
            catch { }//no directions 
        }
        return moves;
    }// returns the possible boards for a piece moving
    public List<int[,]> MovementLogicQueen(int[,] logic_board, int index_x, int index_z)
    {
        List<int[,]> moves = new List<int[,]>();
        int[,] directions = new int[4, 2] { { 1, 1 }, { 1, -1 }, { -1, -1 }, { -1, 1 } };
        int peiceValue = logic_board[index_x, index_z];
        for (int i = 0; i < 4; i++)
        {
            
            int temp_x = index_x + directions[i, 0];
            int temp_z = index_z + directions[i, 1];
            try
            {
                while (logic_board[temp_x, temp_z] == 0)
                {
                    int[,] newBoard = (int[,])logic_board.Clone();
                    newBoard[temp_x, temp_z] = peiceValue;
                    newBoard[index_x, index_z] = 0;
                    moves.Add(newBoard);
                    temp_x += directions[i, 0];
                    temp_z += directions[i, 1];
                }
            }
            catch { };
        }
        return moves;
    }// movement logic for the queen
    public List<int[,]> CanTakeQueenLogic(int[,] logic_board, int index_x, int index_z)
    {
        int peiceValue = logic_board[index_x, index_z];
        List<int[,]> moves = new List<int[,]>();
        int[,] directions = new int[4, 2] { { 1, 1 }, { 1, -1 }, { -1, -1 }, { -1, 1 } };
        for (int i = 0; i < 4; i++)
        {
            int temp_x = index_x + directions[i, 0];
            int temp_z = index_z + directions[i, 1];
            try
            {
                while (logic_board[temp_x, temp_z] == 0 )
                {
                    temp_x += directions[i, 0];
                    temp_z += directions[i, 1];
                }

                if (logic_board[temp_x, temp_z] % 2 != peiceValue % 2 && logic_board[temp_x + directions[i, 0], temp_z + directions[i, 1]] == 0)
                {
                       
                    int[,] tempMove = (int[,])logic_board.Clone();
                    tempMove[index_x, index_z] = 0;
                    tempMove[temp_x, temp_z] = 0; // tagging the checker for recursion
                    tempMove[temp_x + directions[i, 0], temp_z + directions[i, 1]] = peiceValue;
                    List<int[,]> AditionalMoves = CanTakeQueenLogic(tempMove, temp_x + directions[i, 0], temp_z + directions[i, 1]);
                    if (AditionalMoves.Count == 0)
                    {
                        moves.Add(tempMove);
                    }
                    else
                        moves.AddRange(AditionalMoves);
                    temp_x += directions[i, 0];
                    temp_z += directions[i, 1];
                    int extra_move_x = temp_x + directions[i, 0];
                    int extra_move_z = temp_z + directions[i, 1];
                    while (tempMove[extra_move_x, extra_move_z] == 0)
                    {
                        int[,] cont = (int[,])tempMove.Clone();
                        cont[temp_x, temp_z] = peiceValue;
                        cont[temp_x + directions[i, 0], temp_z + directions[i, 1]] = 0;
                        moves.Add(cont);
                        extra_move_x+=directions[i, 0];
                        extra_move_z+=directions[i, 1]; 
                    }
                }

            
            }
            catch
            {
                //piece on edge of the screen
            }
        }
        return moves;
    }// capturing logic for the queen
    public double Evaluate_Logic(int[,] logic_board, int side)
    {
        double TotalEval, QueenCount, CheckerCount, CentralizedCheckers, AttackedCheckers;
        TotalEval = QueenCount = CheckerCount = CentralizedCheckers = AttackedCheckers = 0;
        for (int i = 0; i < 10; i++)
        {
            for (int j = i % 2; j < 10; j++)
            {
                if (logic_board[i, j] != 0 && logic_board[i, j] % 2 == side % 2)
                {
                    if (logic_board[i, j] < 3)
                        CheckerCount++;
                    else
                        QueenCount++;
                }

            }
        }
        TotalEval = CheckerCount + QueenCount * 6;
        return TotalEval;
    }// function  that evaluates the board for one side
    public double Logic_Board_Evalution(int[,] logic_Board)
    {
        return Evaluate_Logic(logic_Board, 1) - Evaluate_Logic(logic_Board, 2);
    }// returns the evaluation of one side subtracted by another, positive for white winning, negetive for black, 0 for a draw
    public int[,] Pick_Logic_Move(int[,] logic_Board, int side)
    {
        int[,] bestMove = null;
        if (!GameManager.GetComponent<GameManager>().GetPlayingAi())
            return null;
        List<int[,]> allMoves = validMoves(logic_Board, side);
        if (allMoves.Count <= 0)
        {
            GameManager.GetComponent<GameManager>().EndGame(side);
            return null;
        }
        if (side == 1)
        {
            double MaxEval = double.NegativeInfinity;
            foreach (int[,] move in allMoves)
            {
                double MoveEval = Minimax(move, gameValues.searchDeapth(), double.NegativeInfinity, double.PositiveInfinity, false);
                if (MaxEval < MoveEval)
                {
                    bestMove = move;
                    MaxEval = MoveEval;
                }
            }
            return bestMove;
        }
        else
        {
            double MinEval = double.PositiveInfinity;
            foreach (int[,] move in allMoves)
            {
                double MoveEval = Minimax(move, gameValues.searchDeapth(), double.NegativeInfinity, double.PositiveInfinity, true);
                if (MinEval > MoveEval)
                {
                    bestMove = move;
                    MinEval = MoveEval;
                }
            }
            return bestMove;
        }

    }// takes in a board and finds the best move
    public double Minimax(int[,] logic_Board, int deapth, double alpha, double beta, bool maximizing_player)
    {
        if (deapth == 0)
            return Logic_Board_Evalution(logic_Board);
        double eval = 0;
        if (maximizing_player)
        {
            List<int[,]> newPossitions = validMoves(logic_Board, 1);
            double maxEval = double.NegativeInfinity;
            foreach (int[,] possition in newPossitions)
            {   
                eval = Minimax(possition, deapth - 1, alpha, beta, false);
                maxEval = math.max(maxEval, eval);
                alpha = math.max(alpha, eval);
                if (beta <= alpha)
                    break;
            }
            return maxEval;
        }
        else
        {
            List<int[,]> newPossitions = validMoves(logic_Board, 2);
            double minEval = double.PositiveInfinity;
            foreach (int[,] possition in newPossitions)
            {
                eval = Minimax(possition, deapth - 1, alpha, beta, true);
                minEval = math.min(minEval, eval);
                beta = math.min(beta, eval);
                if (beta <= alpha)
                    break;
            }
            return minEval;
        }
        
    }// finds board evaluation using the Minimax algorithm with alpha beta pruning
}
