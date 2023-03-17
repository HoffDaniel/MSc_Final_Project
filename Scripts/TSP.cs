using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TSP : Problem
{
    //Problem variables
    //private string Name;
    //public Vector3 Pos;
    //public List<GameObject> environment;
    //public List<GameObject> agents;
    //public List<GameObject> prefabs;
    //public int nbr_agentsFin = 0;
    //public int nbr_agentsFin = 0;
    //public int min = 0;
    //public int max = 750;
    //public int seed = int.MinValue; 

    public int nbr_Points;
    int previous_Nbr; //used to check if the number of points changed
    public List<Vector2> travel_Points;    

    public int pathIndex = 0;    

    private void Awake()
    {
        SetName("Traveling Salesman (TSP)");
        agents = new();

        

    }
    // Start is called before the first frame update
    void Start()
    {
        //initialise some of the lists
        Background = new GameObject();
        environment = new List<GameObject>();
        prefabs = new List<GameObject>();
        travel_Points = new List<Vector2>();

        previous_Nbr = nbr_Points + 1; // just making sure they are not equivalent
        previous_Seed = seed;

        //Adding prefabs which are going to be used to create the environment        
        prefabs.Add(Resources.Load("Prefabs/City") as GameObject);
        prefabs.Add(Resources.Load("Prefabs/Agent2") as GameObject);
        prefabs.Add(Resources.Load("Prefabs/Ground") as GameObject);



        GenerateEnvironment(nbr_Points);

        Debug.Log("Problem Initialised: " + GetName());

    }

    // Update is called once per frame
    void Update()
    {
        //========= TRAVEL POINTS
        //Update enviroment if nbr of points changes or seed changes 
        //Code below can be removed -> added a ui blocker while algorithm is running => the user cannot change the value of the seed during that
        if (previous_Nbr == nbr_Points)
        {
            if (previous_Seed != seed)
            {
                ResetEnvironment();
                previous_Seed = seed;
            }
            return; 
        }                  
        ResetEnvironment();
        previous_Nbr = nbr_Points;
    }

    public override void ResetEnvironment()
    {
        //Debug.Log("TSP - ResetEnvironment");
        //Reset environment
        for (int i = 0; i < environment.Count; i++)
        {
            Destroy(environment[i]);
        }
        environment.Clear();
        travel_Points.Clear();
        environment = new List<GameObject>();
        //Generate new environment
        GenerateEnvironment(nbr_Points);
    }
    public void GenerateEnvironment(int nbr)
    {
        Debug.Log("Random");
        Destroy(Background);

        System.Random RNG = new System.Random(seed); //Allways the same randomness for the specific seed  
        for (int i = 0; i < nbr; i++)
        {
            Vector2 temp_point = new Vector2(RNG.Next(min, max) + Pos_P.x, RNG.Next(min, max) + Pos_P.y);
            if (travel_Points.Contains(temp_point)){ i--; }
            else{ travel_Points.Add(temp_point); }
        }

        for (int i = 0; i < nbr; i++)
        {
            GameObject go = Instantiate(prefabs[0], travel_Points[i], Quaternion.identity, gameObject.transform);
            environment.Add(go);
        }

        float X = max * 0.5f + gameObject.transform.position.x;
        float Y = max * 0.5f + gameObject.transform.position.y;
        float Z = 10f;
        Vector3 tmp_Pos = new Vector3(X, Y, Z);
        //Background = Instantiate(prefabs[2], tmp_Pos, Quaternion.identity, gameObject.transform);
        //Background = Instantiate(prefabs[2], tmp_Pos, prefabs[2].transform.rotation, gameObject.transform);
        Background = Instantiate(prefabs[2], gameObject.transform);
        Background.transform.position = tmp_Pos;
        Background.transform.rotation = prefabs[2].transform.rotation;


        float tmp_scale = max + 75;
        tmp_Pos = new Vector3(tmp_scale, tmp_scale, 1f);
        Background.transform.localScale = tmp_Pos;
        //Background.transform.rotation = prefabs[2].transform.rotation;


    }

    public override float Fitness(List<Vector2> solution) //Make sure the solution returns to the departing point
    {
        float Sum = 0f;
        for (int i = 0; i < solution.Count - 1; i++)
        {
            Sum += Vector2.Distance(solution[i], solution[i + 1]);
        }
        
        return Sum;
    }

    public override void InitAgents(int nbr_Agents)
    {
        //Debug.Log("TSP - Init Agents");
        foreach (var item in agents)
        {
            Destroy(item);
        }
        agents.Clear();

        for (int i = 0; i < nbr_Agents; i++)
        {
            agents.Add(Instantiate(prefabs[1],gameObject.transform));
            agents[i].GetComponent<LineRenderer>().startColor = new Color(0f, 1f, 0f, 0.25f);// Color.green;
            agents[i].GetComponent<LineRenderer>().endColor = new Color(1f, 0f, 0f, 0.25f); //Color.red;
        }
    }

    public void SetAgentPos(int Index, Vector3 Pos )
    {        
        agents[Index].transform.position = Pos;
    }
    public void UpdateAgents(List<Vector2> AgentPositions)
    {       
        for (int i = 0; i < AgentPositions.Count; i++)
        {
            agents[i].transform.position = AgentPositions[i];
        }        
    }
    
}
