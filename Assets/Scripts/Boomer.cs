using UnityEngine;
using System.Collections;

// Applies an explosion force to all nearby rigidbodies
public class Boomer : MonoBehaviour
{
    public float radius = 5.0F;
    public float power = 10.0F;

    public float timing;
    float nextBoom;

    void Update()
    {
        if (nextBoom < Time.fixedTime)
        { 
            Vector3 explosionPos = transform.position;
            Collider[] colliders = Physics.OverlapSphere(explosionPos, radius);
            foreach (Collider hit in colliders)
            {
                Rigidbody rb = hit.GetComponent<Rigidbody>();
                Mover mover = hit.GetComponent<Mover>();
                if (rb != null)
                {
                    if (mover != null)
                        mover.AddVelocity(power, explosionPos, radius, 3.0F);
                    else
                        rb.AddExplosionForce(power, explosionPos, radius, 3.0F);
                }
            }
            nextBoom = Time.fixedTime + timing;
        }
    }
}