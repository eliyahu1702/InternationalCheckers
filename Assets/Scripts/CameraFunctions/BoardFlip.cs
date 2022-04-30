using System.Collections;
using UnityEngine;

public class BoardFlip : MonoBehaviour
{
    // Start is called before the first frame update
    public Vector3 rotation = new Vector3(0, 0, 1);
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.F))
        {
            StartCoroutine(Waiter());

        }
        if (Input.GetKeyUp(KeyCode.R))
        {

            GameObject Board = GameObject.Find("Board");
            Board.GetComponent<CheckerGeneration>().enabled = false;
            Board.GetComponent<CheckerGeneration>().enabled = true;
        }
    }
    IEnumerator Waiter()
    {
        for (int i = 1; i <= 180; i++)
        {
            transform.Rotate(rotation);
            yield return new WaitForSeconds(0.005f);
        }


    }
    public void SpinTheBoard()
    {
        transform.Rotate(new Vector3(0, 0, 180));
    }
    public void ReturnToRegularPosition()
    {
        transform.rotation = Quaternion.Euler(90, 0, -180);
    }
}
