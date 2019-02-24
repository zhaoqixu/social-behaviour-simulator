using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TravellerBehaviourScript : MonoBehaviour
{

    // Use this for initialization
    // Target positions
    private Vector3 topTargetPos;
    private Vector3 bottomTargetPos;
    public Vector3 targetPos;


    public Vector3 currentVelocity;
    public int maxVelocity;
    private float targetTime;
    public const float TARGET_TIME = 30.0f;

    GameManagerScript GMS;

    private void Awake()
    {
        // initialize target postion
        topTargetPos = new Vector3(25, 5, 38);
        bottomTargetPos = new Vector3(-25, 5, 38);
        if (Random.Range(0, 2) == 0)
        {
            targetPos = topTargetPos;
        } else
        {
            targetPos = bottomTargetPos;
        }
        targetTime = TARGET_TIME;
    }

    void Start()
    {
        GMS = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
        maxVelocity = Random.Range(6, 10);
        currentVelocity = new Vector3(0, 0, maxVelocity);

    }

    private void Update()
    {
        UpdateTimer();
    }

    void UpdateTimer()
    {
        targetTime -= Time.deltaTime;
        if (targetTime < 0)
        {
            if (targetPos == topTargetPos)
            {
                targetTime = TARGET_TIME;
                targetPos = bottomTargetPos;
                //Debug.Log("Change Target to bottom doorway");
            }
            else if (targetPos == bottomTargetPos)
            {
                targetTime = TARGET_TIME;
                targetPos = topTargetPos;
                //Debug.Log("Change Target to top doorway");
            }
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        ApplyVelocity();
    }

    private Vector3 Seek()
    {
        // SET force to zero
        Vector3 steering = Vector3.zero;
        Vector3 desiredVelocity;

        // SET desiredForce to direction from target to owner's position
        desiredVelocity = targetPos - this.transform.position;

        // SET desiredForce y to zero 
        desiredVelocity.y = 0f;

        desiredVelocity = desiredVelocity.normalized * maxVelocity;
        steering = desiredVelocity - currentVelocity;

        Debug.DrawRay(transform.position, currentVelocity.normalized * 5, Color.green);
        Debug.DrawRay(transform.position, desiredVelocity.normalized * 5, Color.magenta);

        return steering;
    }

    private Vector3 AvoidanceForce (GameObject obj)
    {
        Vector3 avoidance_force = new Vector3();
        Vector3 ahead = new Vector3();
        ahead = transform.position + currentVelocity.normalized * maxVelocity;
        avoidance_force = ahead - obj.transform.position;
        avoidance_force.y = 0;
        return avoidance_force.normalized * maxVelocity;
    }

    void ApplyVelocity ()
    {
        currentVelocity = currentVelocity + Seek() * Time.deltaTime * 2;

        if (currentVelocity.magnitude > maxVelocity)
        {
            // SET velocity to velocity normalized x maxVelocity
            currentVelocity = currentVelocity.normalized * maxVelocity;
        }

        if (currentVelocity != Vector3.zero)
        {
            // SET position to position + velocity x delta time
            transform.position = transform.position + currentVelocity * Time.deltaTime;
            // SET rotation to Quarternion.LookRotation velocity
            transform.rotation = Quaternion.LookRotation(currentVelocity, Vector3.up);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // encounter obstacle
        if (collision.gameObject.tag == "obstacle")
        {
            currentVelocity = currentVelocity + AvoidanceForce(collision.gameObject) * Time.deltaTime*2;
           
        } else if (collision.gameObject.tag == "trigger")
        {
            if (GMS.travellers.Contains(gameObject))
            {
                GMS.travellers.Remove(gameObject);
            } else
            {
                Debug.Log("Traveller does not exist!");
            }
            GMS.createTraveller();
            Destroy(gameObject);
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        // encounter obstacle
        if (collision.gameObject.tag == "obstacle")
        {
            currentVelocity = currentVelocity + AvoidanceForce(collision.gameObject) * Time.deltaTime*2;
        }
    }
}
