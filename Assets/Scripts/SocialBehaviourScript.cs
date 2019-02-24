using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SocialBehaviourScript : MonoBehaviour {


    private Vector3 targetPos;
    private Vector3 currentVelocity;
    private int maxVelocity;

    public float offset = 1.0f; // Offset of the circle
    public float radius = 1.0f; // radius of the circle which can next path can be selected from
    public float jitter = 0.2f; // circle scatter ratio
    public Vector3 desiredVelocity;

    private Vector3 seekPos;
    private Vector3 targetDir;
    private Vector3 randomDir;
    private Vector3 circlePos;
    private float slowingDistance;

    private Vector3 groupCenter;

    private SocialBehaviourScript closestsocialAgent;
    private TravellerBehaviourScript closestTraveller;

    GameManagerScript GMS;

    private bool socialAgentInRange;

    private bool wantSocial;
    private bool travellerInRange;

    Renderer rend;

    private float coolingOffTime;

    private void Awake()
    {
        maxVelocity = 5;
        slowingDistance = 6;

    }
    // Use this for initialization
    void Start()
    {
        GMS = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
        currentVelocity = new Vector3(0, 0, maxVelocity);
        socialAgentInRange = false;
        travellerInRange = false;
        wantSocial = true;
        rend = GetComponent<Renderer>();

    }

    // Update is called once per frame
    void Update()
    {
    }

    private void FixedUpdate()
    {
        if (wantSocial)
        {
            FindClosestSocialAgent();
            if (Vector3.Distance(transform.position, closestsocialAgent.transform.position) < 10)
            {
                socialAgentInRange = true;
            }
            else
            {
                socialAgentInRange = false;
            }

            if (socialAgentInRange)
            {
                groupCenter = (closestsocialAgent.groupCenter + transform.position) / 2.0f;
                closestsocialAgent.groupCenter = groupCenter;
                targetPos = groupCenter;
                maxVelocity = 5;
                ApplyVelocity(Arrival());
            }
            else
            {
                maxVelocity = 5;
                ApplyVelocity(Wander());
            }
        }
        else
        {

            FindClosestTraveller();
            if (Vector3.Distance(transform.position, closestTraveller.transform.position) < 6)
            {
                travellerInRange = true;
            }
            else
            {
                travellerInRange = false;
            }

            if (closestTraveller && travellerInRange && (Vector3.Distance(closestTraveller.targetPos, transform.position) < Vector3.Distance(closestTraveller.targetPos, closestTraveller.transform.position)))
            {
                targetPos = closestTraveller.transform.position + closestTraveller.currentVelocity * Time.deltaTime * 15;
                maxVelocity = 15;
                ApplyVelocity(Seek());
            }
            else
            {
                maxVelocity = Random.Range(4, 7);
                ApplyVelocity(Wander());
            }
        }
    }

    public IEnumerator SocialTimer()
    {
        wantSocial = true;
        yield return new WaitForSeconds(Random.Range(0.5f, 2.0f)); //randomized, 0.5s–2s
        wantSocial = false;
        rend.material.color = Color.green;
        transform.gameObject.tag = "wander";
        StartCoroutine(CoolingTimer());
    }

    public IEnumerator CoolingTimer()
    {
        wantSocial = false;
        yield return new WaitForSeconds(10f);
        wantSocial = true;
        rend.material.color = Color.yellow;
        transform.gameObject.tag = "social";
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

    private Vector3 Arrival()
    {
        // SET force to zero
        Vector3 steering = Vector3.zero;
        Vector3 desiredVelocity;

        // SET desiredForce to direction from target to owner's position
        desiredVelocity = targetPos - this.transform.position;
        // SET desiredForce y to zero 
        desiredVelocity.y = 0;

        float distance = desiredVelocity.magnitude;
        if (distance < slowingDistance)
        {
            desiredVelocity = desiredVelocity.normalized * maxVelocity * (distance / slowingDistance);
        } else
        {
            desiredVelocity = desiredVelocity.normalized * maxVelocity;
        }

        steering = desiredVelocity - currentVelocity;

        Debug.DrawRay(transform.position, currentVelocity.normalized * 5, Color.green);
        Debug.DrawRay(transform.position, desiredVelocity.normalized * 5, Color.magenta);

        return steering;
    }

    private Vector3 Wander()
    {
        // Set orce to zero
        Vector3 force = Vector3.zero;

        float randX = Random.Range(0, 0x7fff) - (0x7fff * 0.5f);
        float randZ = Random.Range(0, 0x7fff) - (0x7fff * 0.5f);

        randomDir = new Vector3(randX, 0, randZ);
        randomDir = randomDir.normalized;
        randomDir = randomDir * jitter;


        targetDir = targetDir + randomDir;
        targetDir = targetDir.normalized;
        targetDir = targetDir * radius;

        seekPos = transform.position + targetDir;
        seekPos = seekPos + transform.forward * offset;
        desiredVelocity = seekPos - transform.position;

        desiredVelocity.y = 0f;

        if (desiredVelocity != Vector3.zero)
        {
            desiredVelocity = desiredVelocity.normalized * maxVelocity;
            force = desiredVelocity - currentVelocity;
        }

        // Return force

        Debug.DrawRay(transform.position, currentVelocity.normalized * 5, Color.green);
        Debug.DrawRay(transform.position, desiredVelocity.normalized * 5, Color.magenta);

        return force;
    }

    void ApplyVelocity(Vector3 force)
    {
        currentVelocity = currentVelocity + force * Time.deltaTime * 2;

        if (currentVelocity.magnitude > maxVelocity)
        {
            currentVelocity = currentVelocity.normalized * maxVelocity;
        }

        if (currentVelocity != Vector3.zero)
        {
            transform.position = transform.position + currentVelocity * Time.deltaTime;
            transform.rotation = Quaternion.LookRotation(currentVelocity, Vector3.up);
        }

    }


    private Vector3 AvoidanceForce(GameObject obj)
    {
        Vector3 avoidance_force = new Vector3();
        Vector3 ahead = new Vector3();
        ahead = transform.position + currentVelocity.normalized * maxVelocity;
        avoidance_force = ahead - obj.transform.position;
        avoidance_force.y = 0;
        return avoidance_force.normalized * maxVelocity;
    }

    private void FindClosestSocialAgent()
    {
        float minDistance = float.MaxValue;

        for (int i = 0; i < GMS.socialAgents.Count; i++)
        {
            if (! (this.gameObject.GetInstanceID() == GMS.socialAgents[i].GetInstanceID()))
            {
                float distance = Vector3.Distance(transform.position, GMS.socialAgents[i].transform.position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    closestsocialAgent = GMS.socialAgents[i].GetComponent(typeof(SocialBehaviourScript)) as SocialBehaviourScript;
                }
            }
        }
    }


    private void FindClosestTraveller()
    {
        float minDistance = float.MaxValue;

        for (int i = 0; i < GMS.travellers.Count; i++)
        {
            float distance = Vector3.Distance(transform.position, GMS.travellers[i].transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                closestTraveller = GMS.travellers[i].GetComponent(typeof(TravellerBehaviourScript)) as TravellerBehaviourScript;
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // encounter obstacle
        if (collision.gameObject.tag == "obstacle")
        {
            currentVelocity = currentVelocity + AvoidanceForce(collision.gameObject) * Time.deltaTime * 10;
        }
        else if (collision.gameObject.tag == "Wall")
        {
            currentVelocity = -currentVelocity;
        } else if (collision.gameObject.tag == "social")
        {
            StartCoroutine(SocialTimer());
        }

    }

    private void OnCollisionStay(Collision collision)
    {
        // encounter obstacle
        if (collision.gameObject.tag == "obstacle")
        {
            currentVelocity = currentVelocity + AvoidanceForce(collision.gameObject) * Time.deltaTime * 10;
        }
        else if (collision.gameObject.tag == "Wall")
        {
            currentVelocity = -currentVelocity;
        }
    }
}
