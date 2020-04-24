using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IK_Footsies : MonoBehaviour
{
    [SerializeField]
    private Vector2[] FootOffset;

    [SerializeField]
    private GameObject[] Tootsies;

    [SerializeField]
    private float Updateintervall = 0.25f;

    [SerializeField]
    private float FootSpeed = 50;

    [SerializeField]
    private float PushForce = 10;

    [SerializeField]
    private float VelocityCompensation = 0.2f;

    [SerializeField]
    private bool PlayerControllable;

    [SerializeField]
    private float PlayerWalkSpeed = 10;

    [SerializeField]
    private float PlayerTurnSpeed = 10;

    [SerializeField]
    private float EnemySize = 1;

    private int ActiveTootsie;
    private float TimeToNextUpdate;
    private Vector3 CGPosition;
    private Rigidbody rb;
    public float checkGroundRay = 1.5f;

    private bool isGrounded;
    private bool fallingTimerTriggered;
    private float fallingTimerTime;


    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        //DOT_front = Mathf.Abs(Vector3.Dot(transform.right, Vector3.down));
        //DOT_right = Mathf.Abs(Vector3.Dot(transform.forward, Vector3.down));
        //Handle Update Intervalls
       // if (fallingTimerTriggered)
         //   isGrounded = false;
        if (TimeToNextUpdate <= 0)
        {
            UpdateTootsies();
            TimeToNextUpdate = Updateintervall;
        }
        else TimeToNextUpdate -= Time.deltaTime;
    }

    void UpdateTootsies()
    {
        //Cylce through foots
        ActiveTootsie++;
        if (ActiveTootsie == Tootsies.Length) ActiveTootsie = 0;

        //Make a new layerMask with every layer except #31 (Enemy Layer)
        int layerMask = (1 << 9) | (1 << 10);
        layerMask = ~layerMask;

        //Check for Ground and set CGPosition

        if (fallingTimerTriggered)
        {
            if (fallingTimerTime >= 0)
            {
                fallingTimerTime -= Time.deltaTime;
                return;
            }
            else
            {
                fallingTimerTriggered = false;
                fallingTimerTime = 0f;
            }
        }

        if ((Vector3.Dot(-transform.up, Vector3.down) > 0
            ) 
            && (Physics.Raycast(gameObject.transform.position, Vector3.down, out RaycastHit hit, checkGroundRay, layerMask))
            && !fallingTimerTriggered)
        {
            isGrounded = true;
            CGPosition = hit.point;
            Debug.DrawLine(gameObject.transform.position, CGPosition, Color.green, Updateintervall);
        }
        else
        {
            // isGrounded = false;
            // fallingTimerTriggered = true;
            // if(fallingTimerTime == 0f)
            //    fallingTimerTime = 0.1f;
        }
    }

    private void FixedUpdate()
    {
        if (!isGrounded)
        {
            return;
        }

        if (PlayerControllable) PlayerMove();

        //Position Tootsies with Calculate-function
        if (Vector3.Distance(Tootsies[ActiveTootsie].gameObject.transform.position, CalculateTootsies(ActiveTootsie)) >= 0.1f)
            Tootsies[ActiveTootsie].transform.position = ((CalculateTootsies(ActiveTootsie) - Tootsies[ActiveTootsie].transform.position) * FootSpeed);
        else
        {
            //Freeze foot when it's at it's position to avoid overshoot
          //  Tootsies[ActiveTootsie].velocity = Vector3.zero;
        }

        Debug.DrawLine(CalculateTootsies(ActiveTootsie), Tootsies[ActiveTootsie].transform.position, Color.blue, Time.deltaTime);

        //Push Body Up (Stabilizing) - The lower it is, the harder the upwards Force
        rb.AddForce((EnemySize * 4.5f - Vector3.Distance(gameObject.transform.position, CGPosition)) * (Vector3.up * PushForce));
    }

    Vector3 CalculateTootsies(int TootsieIndex)
    {
        /*Position calculation: 
        1) Take the previously calculated CGPosition. 
        2) Add the Offset per Foot. 
        3) Add an offset for the velocity to not just have them toes dragging behind the body. */
        Vector3 FootsiePosition = (CGPosition + transform.TransformDirection(FootOffset[TootsieIndex].y, 0, FootOffset[TootsieIndex].x)) + new Vector3(rb.velocity.x, 0, rb.velocity.z) * VelocityCompensation;
        return FootsiePosition;
    }

    void PlayerMove()
    {
        //Push the body around if the Player is able to control it
        rb.AddRelativeForce(-Input.GetAxis("Vertical") * PlayerWalkSpeed, 0, Input.GetAxis("Horizontal") * PlayerWalkSpeed);
        rb.AddRelativeTorque(new Vector3(0, Input.GetAxis("Mouse X") * PlayerTurnSpeed, 0));
    }
}
