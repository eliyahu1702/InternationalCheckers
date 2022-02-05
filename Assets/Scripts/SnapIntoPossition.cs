using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SnapIntoPossition : MonoBehaviour
{
    public Board gameBoard;
    public GameObject[] Checkers;
    public float mindist = 1000;

    // Start is called before the first frame update
    void Start()
    {
        GameObject board = GameObject.Find("Board");
        gameBoard = board.GetComponent<CheckerGeneration>().gameBoard;
        Checkers = GameObject.FindGameObjectsWithTag("Regular_checker");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public static void snapChecker(Transform checker)
    {

        
    }
}

