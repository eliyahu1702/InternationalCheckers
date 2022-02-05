using UnityEngine;

public class ChangeBoard : MonoBehaviour
{
    GameObject Queen;
    Transform QueenTransform;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    public static void changePossition(Board gameBoard, int index_src_x, int index_src_z, int index_dest_x, int index_dest_z)
    {
        gameBoard.GetBoardTiles()[index_dest_x, index_dest_z].setChecker(gameBoard.GetBoardTiles()[index_src_x, index_src_z].getChecker());
        gameBoard.GetBoardTiles()[index_src_x, index_src_z].setChecker(null);
    }

    /*public static void changeRegualrTakingPossition(int index_x, int index_z, int index_location_x, int index_location_z)
    {
        GameObject board = GameObject.Find("Board");
        int[,] possition = board.GetComponent<CheckerGeneration>().possition;
        if (index_x < index_location_x)
        {
            if (index_z < index_location_z)
                possition[index_x + 1, index_z + 1] = 0;
            else
                possition[index_x + 1, index_z -1 ] = 0;
        }
        else
        {
            if (index_z < index_location_z)
                possition[index_x - 1, index_z + 1] = 0;
            else
                possition[index_x - 1, index_z - 1] = 0;
        }
        possition[index_location_x, index_location_z] = possition[index_x, index_z];
        possition[index_x, index_z] = 0;
    }*/
}
