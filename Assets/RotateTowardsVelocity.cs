using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateTowardsVelocity : MonoBehaviour
{

    public Rigidbody rb;
    ParticleSystem ps;
    float calculatedRateOverTime;
    public Transform velocityLeaningHandler;
    float lean,curLean;
    float currentVelocity;
    // Start is called before the first frame update
    void Start()
    {
        ps = GetComponent<ParticleSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        // Vector3 b = Vector3.ClampMagnitude(rb.velocity / 15f,10f);
        //  transform.localPosition = Vector3.Slerp(transform.localPosition,new Vector3(b.x,b.y),Time.deltaTime * 0.5f);
        CalculateLean();
        var emission = ps.emission;
        calculatedRateOverTime = Mathf.Clamp(rb.velocity.magnitude, 0.0f, 55.0f);

        if (calculatedRateOverTime < 7.5f)
            calculatedRateOverTime = 0.0f;
        emission.rateOverTime = calculatedRateOverTime;
    }

    void CalculateLean()
    {
        lean = -Vector3.Dot(velocityLeaningHandler.right, rb.velocity.normalized);
        curLean = Mathf.SmoothDamp(curLean, lean * Mathf.Clamp(rb.velocity.magnitude / 5f, 0.0f, 2.0f), ref currentVelocity, Time.deltaTime * 10f);
        velocityLeaningHandler.localEulerAngles = new Vector3(0f, 0f, curLean);
    }

    public void EmitInstantly(int amount)
    {
        ps.Emit(amount);
    }
}
