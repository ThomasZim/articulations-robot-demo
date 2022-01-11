using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class movement : MonoBehaviour
{
    // x : 0.2 -> 0.8
    // z : -0.4 -> 0.4
    // Start is called before the first frame update

    public GameObject cube;

    public bool isMoving = false;

    public float xOffset;
    public float zOffset;

    void Start()
    {
        
        xOffset = 0.005f;
        zOffset = 0.005f;
    }

    // Update is called once per frame
    void Update()
    {
        if(isMoving)
        {
            float x = cube.transform.position.x;
            float y = cube.transform.position.y;
            float z = cube.transform.position.z;

            if(x > 0.8 || x<0.2)
            {
                xOffset = -xOffset;
                x+=2*xOffset;
            }
            if(z>0.4||z<-0.4)
            {
                zOffset = -zOffset;
                z+=2*zOffset;
            }
            Vector3 newPos = new Vector3(x+xOffset,y,z+zOffset);
            cube.transform.position = newPos;
        }
    }
}
