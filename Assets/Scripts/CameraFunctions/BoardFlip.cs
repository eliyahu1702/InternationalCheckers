using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardFlip : MonoBehaviour
{
    // Start is called before the first frame update
    public Vector3 rotation = new Vector3(0,0,1);
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
    }
    IEnumerator Waiter()
    {
        for (int i = 1; i <= 180; i++)
        {
            transform.Rotate(rotation);
            yield return new WaitForSeconds(0.005f);
        }
        
      
    }
}
