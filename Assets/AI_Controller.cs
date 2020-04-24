using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class AI_Controller : MonoBehaviour
{

    public Transform target;
    public float Updateintervall = 0.1f;
    float TimeToNextUpdate;
    public Renderer mesh;

    public AcR_Body movement;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(transform.position, target.position) < 3)
        {
        }
        if (TimeToNextUpdate <= 0)
        {
            BrainMethods();
            TimeToNextUpdate = Updateintervall;
        }
        else TimeToNextUpdate -= Time.deltaTime;
    }

    void BrainMethods()
    {
        movement.AIInstructions(target);

    }
}
