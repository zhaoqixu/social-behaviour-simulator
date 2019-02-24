using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManagerScript : MonoBehaviour {


    //public GameObject shape1, shape2, shape3, shape4, shape5, shape6;
    private Vector3[] obstaclePositions = new Vector3[6];
    public List<GameObject> travellers;
    public List<GameObject> socialAgents;

    public int numOfObstacle = 1; // 3 - 5
    public int numOfTraveller = 1;
    public int numOfWander = 1;
    public int numOfSocial = 2;

    void Awake ()
    {
        // initialize obstacle position
        obstaclePositions[0] = new Vector3(13 + Random.Range(-3f, 3f), 4, 4 + Random.Range(-3f, 3f));
        obstaclePositions[1] = new Vector3(-16 + Random.Range(-3f, 3f), 4, -2 + Random.Range(-3f, 3f));
        obstaclePositions[2] = new Vector3(0 + Random.Range(-15f, 15f), 4, -40 + Random.Range(-3f, 3f));
        obstaclePositions[3] = new Vector3(18 + Random.Range(-3f, 3f), 4, -82 + Random.Range(-3f, 3f));
        obstaclePositions[4] = new Vector3(-17 + Random.Range(-3f, 3f), 4, -72 + Random.Range(-3f, 3f));
    }

    IEnumerator PopTraveller()
    {
        for (int i = 0; i < numOfTraveller; i++)
        {
            createTraveller();
            yield return new WaitForSecondsRealtime(Random.Range(1.0f, 5.0f));
        }
        Debug.Log("Traveller Population Completed!");
        //travellerThroughput = 0; // initialize traveller throughput
    }

    // Use this for initialization
    void Start () {


        // populate obstacle
        if (numOfObstacle > 5) numOfObstacle = 5;
        populateObstable();


        // populate wander
        for (int i = 0; i< numOfWander; i++)
        {
            createWander();
        }

        // populate social agent
        for (int i = 0; i < numOfSocial; i++)
        {
            createSocial();
        }

        // populate traveller, need some time to populate, so populate at last
        StartCoroutine(PopTraveller());
    }

    // Update is called once per frame
    void Update () {
    }

    void populateObstable()
    {
        List<int> pos = randomSelector(numOfObstacle, 5);
        List<int> shapes = randomSelector(numOfObstacle, 6);

        for (int i = 0; i < numOfObstacle; i++)
        {
            string nameOfShape = "Prefabs/Shape" + (shapes[i] + 1);
            GameObject shape1 = Instantiate(Resources.Load(nameOfShape, typeof(GameObject))) as GameObject;
            shape1.transform.position = obstaclePositions[pos[i]];
            shape1.transform.Rotate(new Vector3(0, Random.Range(-90, 90), 0));
        }
    }

    public void createTraveller()
    {
        GameObject traveller = Instantiate(Resources.Load("Prefabs/Traveller", typeof(GameObject))) as GameObject;
        traveller.transform.position = new Vector3(0, 2, -105);
        travellers.Add(traveller);
    }

    public void createWander()
    {
        GameObject wander = Instantiate(Resources.Load("Prefabs/Wander", typeof(GameObject))) as GameObject;
        wander.transform.position = new Vector3(Random.Range(-28.0f, 28.0f), 2, Random.Range(-96.0f, 26.0f));
    }

    public void createSocial()
    {
        GameObject social = Instantiate(Resources.Load("Prefabs/Social", typeof(GameObject))) as GameObject;
        social.transform.position = new Vector3(Random.Range(-28.0f, 28.0f), 2, Random.Range(-96.0f, 26.0f));
        socialAgents.Add(social);
    }

    List<int> randomSelector(int n, int range)
    {
        List<int> pos = new List<int>(new int[n]);
        for (int i = 0; i < n; i++)
        {
            pos[i] = -1;
        }
        int Rand;
        for (int i = 0; i < n; i++)
        {
            Rand = UnityEngine.Random.Range(0, range);
            while (pos.Contains(Rand))
            {
                Rand = UnityEngine.Random.Range(0, range);
            }

            pos[i] = Rand;
        }
        return pos;
    }
}
