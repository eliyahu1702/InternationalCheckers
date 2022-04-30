using System.Collections.Generic;
using System.Diagnostics;
using Unity.Mathematics;
using UnityEngine;


[PreferBinarySerialization]
public class ComputerPlayer : MonoBehaviour
{
    public Stack<BoardTile> tileStack;
    GameObject GameManager;
    public Dictionary<string, double> TranspositionTable;
    public int possitions_looked;
    // Start is called before the first frame update
    void Start()
    {
        possitions_looked = 0;
        UnityEngine.Debug.Log(Application.dataPath);
        tileStack = new Stack<BoardTile>();
        GameManager = GameObject.Find("Game_Manager");
        TranspositionTable = GameManager.GetComponent<GameManager>().LoadFile();
        if (TranspositionTable == null)
            TranspositionTable = new Dictionary<string, double>();
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
        char[,] logicBoard = TranslateBoard(gameBoard);
        GameManager.GetComponent<GameManager>().Moves_Played.Push(logicBoard);
        char[,] bestMove = Pick_Logic_Move(logicBoard, gameBoard.getTurn());
        Logic_To_Display(gameBoard, bestMove);
        GameManager.GetComponent<GameManager>().Moves_Played.Push(bestMove);
        gameBoard.ChangeTurn();
        //UnityEngine.Debug.Log(TranspositionTable.Count);
        List<char[,]> PossibleResponses = validMoves(bestMove, gameBoard.getTurn());
        if (PossibleResponses == null || PossibleResponses.Count == 0)
        {
            GameManager.GetComponent<GameManager>().EndGame(gameBoard.getTurn());
        }
    }// function called after the human player completes a move, plays the best move according to the computer
    public void Logic_To_Display(Board gameBoard, char[,] logic_board) // translates logics board to display board
    {
        for (int i = 0; i < 10; i++)
        {
            for (int j = i % 2; j < 10; j += 2)
            {
                if (gameBoard.GetBoardTiles()[i, j].getChecker() != null)
                {
                    gameBoard.GetBoardTiles()[i, j].DestroyPiece();
                }
                GameObject.Find("Board").GetComponent<CheckerGeneration>().SetPiece(gameBoard, i, j, logic_board[i, j]);
            }
        }
    }
    public char[,] TranslateBoard(Board gameBoard)
    {
        char[,] result = new char[10, 10];
        for (int i = 0; i < 10; i++)
        {
            for (int j = 0; j < 10; j++)
            {
                try
                {
                    result[i, j] = (char)gameBoard.GetBoardTiles()[i, j].getChecker().GetValue();
                }
                catch
                {
                    result[i, j] = (char)0;
                }

            }
        }
        return result;
    } // parses the grafical board for the computer player
    public List<char[,]> validMoves(char[,] logic_board, int turn)
    {
        List<char[,]> taking_moves = new List<char[,]>();
        List<char[,]> walking_moves = new List<char[,]>();
        bool has_to_take = false;
        for (int i = 0; i < 10; i++)
        {
            for (int j = i % 2; j < 10; j += 2)
            {
                if (logic_board[i, j] != 0 && logic_board[i, j] % 2 == turn % 2)
                {
                    List<char[,]> tempMoves = canTakeLogic(logic_board, i, j);
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
    public List<char[,]> canTakeLogic(char[,] logic_board, int index_x, int index_z)
    {
        if (logic_board[index_x, index_z] > 2)
            return CanTakeQueenLogic(logic_board, index_x, index_z);
        char pieceValue = logic_board[index_x, index_z];
        List<char[,]> moves = new List<char[,]>();
        int[,] directions = new int[4, 2] { { 1, 1 }, { 1, -1 }, { -1, -1 }, { -1, 1 } };
        for (int rep = 0; rep < 4; rep++)
        {
            char[,] temp_move = (char[,])logic_board.Clone();
            try
            {
                if (temp_move[index_x + directions[rep, 0], index_z + directions[rep, 1]] > 0 && temp_move[index_x + directions[rep, 0], index_z + directions[rep, 1]] % 2 != temp_move[index_x, index_z] % 2)
                {
                    if (temp_move[index_x + directions[rep, 0] * 2, index_z + directions[rep, 1] * 2] <= 0)
                    {
                        temp_move[index_x, index_z] = temp_move[index_x + directions[rep, 0], index_z + directions[rep, 1]] = (char)0; // tagging the checker for recursion
                        temp_move[index_x + directions[rep, 0] * 2, index_z + directions[rep, 1] * 2] = (char)pieceValue;
                        List<char[,]> AditionalMoves = canTakeLogic(temp_move, index_x + directions[rep, 0] * 2, index_z + directions[rep, 1] * 2);
                        if (AditionalMoves.Count == 0)
                        {
                            if ((pieceValue == gameValues.whiteChecker() && (index_x + directions[rep, 0] * 2 == 9)) || (pieceValue == gameValues.blackChecker() && (index_x + directions[rep, 0] * 2 == 0)))
                                temp_move[index_x + directions[rep, 0] * 2, index_z + directions[rep, 1] * 2] = (char)(pieceValue + 2); // qeening the piece
                            temp_move[9, 0] = (char)1; // tagging for taking move
                            moves.Add(temp_move);
                        }
                        else
                            moves.AddRange(AditionalMoves);
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
    public List<char[,]> MovementLogic(char[,] logic_board, int index_x, int index_z)
    {
        int i;
        int pieceValue = logic_board[index_x, index_z];
        if (pieceValue > 2)
            return MovementLogicQueen(logic_board, index_x, index_z);
        List<char[,]> moves = new List<char[,]>();
        int[,] directions = new int[4, 2] { { 1, 1 }, { 1, -1 }, { -1, -1 }, { -1, 1 } };
        if (pieceValue == gameValues.whiteChecker())
            i = 2;
        else
            i = 4;
        for (int j = i - 2; j < i; j++)
        {
            char[,] newBoard = (char[,])logic_board.Clone();
            int new_X_Index = index_x + directions[j, 0];
            int new_Z_Index = index_z + directions[j, 1];
            try
            {
                if (newBoard[new_X_Index, new_Z_Index] <= 0)
                {
                    newBoard[new_X_Index, new_Z_Index] = (char)pieceValue;
                    newBoard[index_x, index_z] = (char)0;
                    if ((pieceValue == gameValues.whiteChecker() && (new_X_Index == 9)) || (pieceValue == gameValues.blackChecker() && (new_X_Index == 0)))
                        newBoard[new_X_Index, new_Z_Index] = (char)(pieceValue + 2); // qeening the piece
                    newBoard[9, 0] = (char)0;
                    moves.Add(newBoard);
                }
            }
            catch { }//no directions 
        }
        return moves;
    }// returns the possible boards for a piece moving
    public List<char[,]> MovementLogicQueen(char[,] logic_board, int index_x, int index_z)
    {
        List<char[,]> moves = new List<char[,]>();
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
                    char[,] newBoard = (char[,])logic_board.Clone();
                    newBoard[9, 0] = (char)0;
                    newBoard[temp_x, temp_z] = (char)peiceValue;
                    newBoard[index_x, index_z] = (char)0;
                    moves.Add(newBoard);
                    temp_x += directions[i, 0];
                    temp_z += directions[i, 1];
                }
            }
            catch { };
        }
        return moves;
    }// movement logic for the queen
    public List<char[,]> CanTakeQueenLogic(char[,] logic_board, int index_x, int index_z)
    {
        int peiceValue = logic_board[index_x, index_z];
        List<char[,]> moves = new List<char[,]>();
        int[,] directions = new int[4, 2] { { 1, 1 }, { 1, -1 }, { -1, -1 }, { -1, 1 } };
        for (int i = 0; i < 4; i++)
        {
            int temp_x = index_x + directions[i, 0];
            int temp_z = index_z + directions[i, 1];
            try
            {
                while (logic_board[temp_x, temp_z] == 0)
                {
                    temp_x += directions[i, 0];
                    temp_z += directions[i, 1];
                }

                if (logic_board[temp_x, temp_z] % 2 != peiceValue % 2 && logic_board[temp_x + directions[i, 0], temp_z + directions[i, 1]] == 0)
                {

                    char[,] tempMove = (char[,])logic_board.Clone();
                    tempMove[9, 0] = (char)1;
                    tempMove[index_x, index_z] = (char)0;
                    tempMove[temp_x, temp_z] = (char)0; // tagging the checker for recursion
                    tempMove[temp_x + directions[i, 0], temp_z + directions[i, 1]] = (char)peiceValue;
                    List<char[,]> AditionalMoves = CanTakeQueenLogic(tempMove, temp_x + directions[i, 0], temp_z + directions[i, 1]);
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
                        char[,] cont = (char[,])tempMove.Clone();
                        cont[extra_move_x, extra_move_z] = (char)peiceValue;
                        cont[temp_x, temp_z] = (char)0;
                        moves.Add(cont);
                        extra_move_x += directions[i, 0];
                        extra_move_z += directions[i, 1];
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
    public double Evaluate_Logic(char[,] logic_board, int side)
    {
        double TotalEval, QueenCount, CheckerCount, CentralizedCheckers;
        TotalEval = QueenCount = CheckerCount = CentralizedCheckers = 0;
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
                    CentralizedCheckers += CentralizedChecker(logic_board, i, j);
                }

            }
        }
        TotalEval = CheckerCount + QueenCount * 8 + CentralizedCheckers;
        if (TotalEval == 0)
            return double.NegativeInfinity;
        return TotalEval;
    }// function  that evaluates the board for one side
    public double CentralizedChecker(char[,] logic_board, int index_x, int index_z)
    {
        double totalEval = 0;
        if (logic_board[index_x, index_z] % 2 == gameValues.whiteChecker() % 2)
        {
            if (index_x == 4)
                totalEval += 0.1;
            if (index_x == 5)
                totalEval += 0.07;
            if (index_z >= 3 && index_z <= 5)
                totalEval += 0.2;
        }
        else
        {
            if (index_x == 5)
                totalEval += 0.1;
            if (index_x == 4)
                totalEval += 0.07;
            if (index_z >= 3 && index_z <= 5)
                totalEval += 0.2;
        }
        return totalEval;
    }
    public double Logic_Board_Evalution(char[,] logic_Board)
    {
        double white_eval = Evaluate_Logic(logic_Board, 1);
        double black_eval = Evaluate_Logic(logic_Board, 2);
        return white_eval - black_eval;
    }// returns the evaluation of one side subtracted by another, positive for white winning, negetive for black, 0 for a draw
    public char[,] Pick_Logic_Move(char[,] logic_Board, int side)
    {
        char[,] bestMove = null;
        if (!GameManager.GetComponent<GameManager>().GetPlayingAi())
            return null;
        List<char[,]> allMoves = validMoves(logic_Board, side);
        if (allMoves.Count <= 0)
        {
            GameManager.GetComponent<GameManager>().EndGame(side);
            return logic_Board;
        }
        Stopwatch st = new Stopwatch();
        st.Start();
        if (side == 1)
        {
            double MaxEval = double.NegativeInfinity;
            foreach (char[,] move in allMoves)
            {
                double MoveEval;
                string flattenedMove = ToFemString(move);
                if (TranspositionTable.TryGetValue(flattenedMove, out MoveEval))
                {
                    if (MaxEval <= MoveEval)
                    {
                        bestMove = move;
                        MaxEval = MoveEval;
                    }
                }
                else
                {
                    MoveEval = Minimax(move, gameValues.searchDeapth(), double.NegativeInfinity, double.PositiveInfinity, false);
                    if (MaxEval <= MoveEval)
                    {
                        bestMove = move;
                        MaxEval = MoveEval;
                    }
                    TranspositionTable.TryAdd(flattenedMove, MoveEval);
                }

            }
            st.Stop();
            UnityEngine.Debug.Log("ammount of possitions evaluated = " + possitions_looked + ". time elapsed " + st.ElapsedMilliseconds + " ms");
            possitions_looked = 0;
            return bestMove;
        }
        else
        {
            double MinEval = double.PositiveInfinity;
            foreach (char[,] move in allMoves)
            {
                double MoveEval;
                string flattenedMove = ToFemString(move);
                if (TranspositionTable.TryGetValue(flattenedMove, out MoveEval))
                {
                    if (MinEval >= MoveEval)
                    {
                        bestMove = move;
                        MinEval = MoveEval;
                    }
                }
                else
                {
                    MoveEval = Minimax(move, gameValues.searchDeapth(), double.NegativeInfinity, double.PositiveInfinity, true);
                    if (MinEval >= MoveEval)
                    {
                        bestMove = move;
                        MinEval = MoveEval;
                    }
                    TranspositionTable.TryAdd(flattenedMove, MoveEval);
                }

            }
            st.Stop();
            UnityEngine.Debug.Log("ammount of possitions looked = " + possitions_looked + ". time elapsed " + st.ElapsedMilliseconds + " ms");
            possitions_looked = 0;
            return bestMove;
        }

    }// takes in a board and finds the best move
    public double Minimax(char[,] logic_Board, int deapth, double alpha, double beta, bool maximizing_player)
    {
        if (deapth == 0)
        {
            possitions_looked++;
            return Logic_Board_Evalution(logic_Board);
        }

        double eval = 0;
        if (maximizing_player)
        {
            List<char[,]> newPossitions = validMoves(logic_Board, 1);
            double maxEval = double.NegativeInfinity;
            string femstringOfBestMove = null;
            foreach (char[,] possition in newPossitions)
            {
                string femString = ToFemString(possition);
                if (TranspositionTable.TryGetValue(femString, out eval)) { }
                else
                {
                    eval = Minimax(possition, deapth - 1, alpha, beta, false);

                }
                // eval = Minimax(possition, deapth - 1, alpha, beta, false);

                maxEval = math.max(maxEval, eval);
                femstringOfBestMove = maxEval == eval ? femString : femstringOfBestMove;
                alpha = math.max(alpha, eval);
                if (beta <= alpha)
                    break;
            }
            if (femstringOfBestMove != null && gameValues.searchDeapth() - deapth <= 2)
                TranspositionTable.TryAdd(femstringOfBestMove, eval);
            return maxEval;
        }
        else
        {
            List<char[,]> newPossitions = validMoves(logic_Board, 2);
            double minEval = double.PositiveInfinity;
            string femstringOfBestMove = null;
            foreach (char[,] possition in newPossitions)
            {
                string femString = ToFemString(possition);
                if (TranspositionTable.TryGetValue(femString, out eval)) { }
                else
                {
                    eval = Minimax(possition, deapth - 1, alpha, beta, true);
                }
                //eval = Minimax(possition, deapth - 1, alpha, beta, true);
                minEval = math.min(minEval, eval);
                femstringOfBestMove = minEval == eval ? femString : femstringOfBestMove;
                beta = math.min(beta, eval);
                if (beta <= alpha)
                    break;
            }
            if (femstringOfBestMove != null && gameValues.searchDeapth() - deapth <=2)
                TranspositionTable.TryAdd(femstringOfBestMove, eval);
            return minEval;
        }

    }// finds board evaluation using the Minimax algorithm with alpha beta pruning
    static string ToFemString(char[,] logic_board)// converts the logical board to Forsyth-Edwards Notation
    {
        int count = 0;
        string FemString = "";
        string[] pieceNames = new string[] { "w", "b", "W", "B" };
        for (int i = 0; i < 10; i++)
        {
            for (int j = i % 2; j < 10; j += 2)
            {
                if (logic_board[i, j] == 0)
                    count++;
                else
                {
                    if (count != 0)
                    {
                        FemString += count + pieceNames[logic_board[i, j] - 1];
                        count = 0;
                    }
                    else
                    {
                        FemString += pieceNames[logic_board[i, j] - 1];
                    }
                }
            }
            FemString += "/";
            count = 0;
        }
        return FemString;
    }
    public Dictionary<string, double> getTranspositionTable()
    {
        return TranspositionTable;
    }
}
