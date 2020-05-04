using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateTowardsVelocity : MonoBehaviour
{

    public Rigidbody rb;
    ParticleSystem ps;
    // Start is called before the first frame update
    void Start()
    {
        ps = GetComponent<ParticleSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 b = rb.velocity / 20f;
        transform.localPosition = Vector3.Slerp(transform.localPosition,new Vector3(b.x,b.y),Time.deltaTime * 0.5f);
        var emission = ps.emission;
        emission.rateOverTime = rb.velocity.magnitude;
    }
}
