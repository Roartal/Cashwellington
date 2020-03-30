using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate_On_Y_Axis : MonoBehaviour
{
    public Transform target;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 newRotation = new Vector3(transform.eulerAngles.x, target.transform.eulerAngles.y,transform.eulerAngles.z);
        this.transform.eulerAngles = newRotation;
    }
}
