using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReturnToZero : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.localEulerAngles = Vector3.Slerp(transform.localEulerAngles, Vector3.zero, 15f*Time.deltaTime);
    }
}
