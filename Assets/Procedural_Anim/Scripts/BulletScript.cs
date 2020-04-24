using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletScript : MonoBehaviour
{
    public string damageMethodName = "Damage";
    public float shooterPosY;

    public AnimationCurve damageDropCurve;

    public float speed = 250;
    public LayerMask layerMask;
    public float maxLifeTime = 3;

    public virtual float Damage()
    {
        return AdvancedCalculations() * damage;
    }

    private float AdvancedCalculations()
    {
        return damageDropCurve.Evaluate(curDistance / 200);
    }

    //Bullet Stuff
    public float damage = 16;

    //use for shotguns
    public float bulletForce = 100;

    //Hit Effects
    public GameObject hitEffect;
    public float hitEffectDestroyTime = 1;
    public string hitEffectTag = "HitBox";
    public GameObject missEffect;
    public float missEffectDestroyTime = 1;
    public float timeToDestroyAfterHitting = 0.1f;
    private RaycastHit hit;
    private Transform myTransform;
    public ParticleSystem impact;

    //Rotation
    private Quaternion targetRotation;

    private Transform target = null;
    public float homingTrackingSpeed = 3;

    public float minDistToDetonate = 0.5f;
    private float minDistToDetonateSqr;

    public string friendlyTag;
    bool skipFrame = false;
    public byte maxPenetrationCount = 1;
    byte penetrationCount = 0;

    private float curDistance;
    private Vector3 velocity = Vector3.zero;


    void Awake()
    {
        myTransform = transform;
        velocity = transform.forward * speed;
        Move();
        StartCoroutine(SetTimeToDestroy());
    }

    //Automatically destroy the bullet after a certain amount of time
    //Especially useful for missiles, which may end up flying endless circles around their target,
    //long after the appropriate sound effects have ended.
    IEnumerator SetTimeToDestroy()
    {
        yield return new WaitForSeconds(maxLifeTime);

        Destroy(gameObject);
    }

    void ApplyDamage(bool d = true)
    {
        penetrationCount++;
        Instantiate(hitEffect, myTransform.position, myTransform.rotation);
        //Reduce the enemy's health
        //Does NOT travel up the heirarchy.  
        if (hit.transform.tag != friendlyTag)
        {
            //if (hit.collider.GetComponent<Hitable>())
             //   hit.collider.GetComponent<Hitable>().Hit(Damage(), shooterPosY, false, hit.point);
           // else
             //   hit.collider.SendMessage(damageMethodName, Damage(), SendMessageOptions.DontRequireReceiver);
        }

        //PlayerEffects.sfx.BulletMarks(hit, false);

        //Wait a fram to apply forces as we need to make sure the thing is dead
        if (hit.rigidbody)
            hit.rigidbody.AddForceAtPosition(myTransform.forward * bulletForce, hit.point, ForceMode.Impulse);

        //			print ("HIT" + hit.collider);
        if (d)
        {
            Destroy(gameObject, timeToDestroyAfterHitting);
            this.enabled = false;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Move();
    }

    //Move is a seperate method as the bullet must move IMMEDIATELY.
    //If it does not, the shooter may literally outrun the bullet-momentarily.
    //If the shooter passes the bullet, the bullet will then start moving and hit the shooter in the back.
    //That would not be good.
    void Move()
    {
        //Check to see if we're going to hit anything.  If so, move right to it and deal damage
        if (Physics.Raycast(myTransform.position, myTransform.forward, out hit, speed * Time.deltaTime, layerMask.value))
        {
            if ((penetrationCount < maxPenetrationCount) && (hit.transform.tag == "Penetrateable"))
            {
                myTransform.position = hit.point + (transform.forward * 0.5f);
                myTransform.eulerAngles = myTransform.eulerAngles + (myTransform.eulerAngles * Random.Range(-0.05f, 0.05f));
                ApplyDamage(false);
            }
            else
            {
                myTransform.position = hit.point;
                ApplyDamage();
            }
           // PlayerEffects.sfx.ImpactMarker(hit);
        }
        else
        {
            //Move the bullet forwards
            // transform.Translate(Vector3.forward * Time.deltaTime * speed);
            Vector3 position = transform.position;
            curDistance += speed * Time.deltaTime;
            velocity += Physics.gravity * Time.deltaTime;
            Vector3 delta = velocity * Time.deltaTime;
            transform.position += delta;
            transform.rotation = Quaternion.LookRotation(delta);
        }

        //Home in on the target
        if (target != null)
        {
            //Firgure out the rotation required to move directly towards the target
            targetRotation = Quaternion.LookRotation(target.position - transform.position);
            //Smoothly rotate to face the target over several frames.  The slower the rotation, the easier it is to dodg.
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, homingTrackingSpeed * Time.deltaTime);

            Debug.DrawRay(transform.position, (target.position - transform.position).normalized * minDistToDetonate, Color.red);
            //Projectile will "detonate" upon getting close enough to the target..
            if (Vector3.SqrMagnitude(target.position - transform.position) < minDistToDetonateSqr)
            {
                //The hitEffect should be your explosion.
                Instantiate(hitEffect, myTransform.position, myTransform.rotation);
                GameObject.Destroy(gameObject);
            }
        }
    }

    //To avoid costly SqrRoot in Vector3.Distance
    public void SetDistToDetonate(float x)
    {
        minDistToDetonateSqr = x * x;
    }

    //Call this method upon instantating the bullet in order to make it home in on a target.
    public void SetAsHoming(Transform t)
    {
        target = t;
        SetDistToDetonate(minDistToDetonate);
    }
}


