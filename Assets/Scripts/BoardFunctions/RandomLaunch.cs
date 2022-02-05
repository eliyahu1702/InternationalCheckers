using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomLaunch : MonoBehaviour
{
    public Vector3 offset;
    // Start is called before the first frame update
    void Start()
    {
        GenerateVector();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.R))
        {
            GetComponent<Rigidbody>().velocity = offset;
            GetComponent<Rigidbody>().angularVelocity = offset;
            GetComponent<Rigidbody>().transform.Rotate(offset, Random.Range(0,360));
        }
    }
    public void GenerateVector()
    {
        float vectors = Random.Range(5, 10);
        offset = new Vector3 (vectors, vectors*3, -vectors);
    }
}
