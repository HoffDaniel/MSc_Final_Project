using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using TMPro;

public class Algorithm : MonoBehaviour
{
    //An algorithm as a name
    private string Name;
    public int Algo_ID = 0;
    public Vector3 Algo_Pos;
    public GameObject Algo_HUD;

    public bool isRunning;
    public bool hasInit;    
    public bool hasFin;
    public bool hasPause;
    public bool hasAnimation;
    public int MaxEpochs;
    public int currentEpoch;


    //The problem to be tackled 
    //contains the environment -> the visual (without the UI) public List<GameObject> environment;
    //we update the environment of the problem with this algorithm
    public Problem problem;
    
    //Best solution score/value
    public float fittest;
    public int populationSize;
    //List of solutions as score/value || how the solution was archieved (i.e. TSP a solution has a score but it is also a specific path) 
    // OR list of agents/ agents have 
    //public float population; 
   

    //TO DO:
    //FSM -> Initialise, Run pause etc...
    //Add a data results option

    void Awake()
    {
        SetName("This is an Algorithm");
    }

    // Start is called before the first frame update
    void Start()
    {
        //Name = "This is an Algorithm";
        //problem = gameObject.AddComponent<TSP>();
        problem = gameObject.GetComponent<Problem>();
        Debug.Log("Algorithm Initialised: " + Name);
        Debug.Log("Solving:" + problem.GetName()); //Name will only be shown properly after <==> this line of code executes before the NAME has been initialised
        hasInit = true;
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log("THIS is just an Algorithm ");
    }
    public virtual void Reset_Algorithm()
    {
        Debug.Log("This should not be displayed/logged ");
        if (problem.agents != null)
        {
            foreach (var item in problem.agents)
            {
                Destroy(item);
            }
            problem.agents.Clear();
        }
        currentEpoch = 0;
        hasInit = false;
        hasPause = false;
        hasFin = false;
    }

    //public virtual void NextGen() || HandelNext()

    //Save a line of Data unto specified file
    public void SaveData(int Generation, float Fittest, string filepath)
    {
        StreamWriter file = new StreamWriter(@filepath, true);
        file.WriteLine(Generation + "," + Fittest);
        //file.Flush();
        file.Close();      

    }
    
    //Clears the specified file 
    public void ResetData(string filepath)
    {
        StreamWriter file = new StreamWriter(@filepath, false);
        //file.WriteLine("");
        file.Flush();
        file.Close();
    }

    public string GetResultPath()
    {
        string path = string.Format("Assets/Data/Results_Algo_{0}.csv", Algo_ID.ToString());
        return path;
    }

    public void SetProblem(Problem newProblem)
    {
        problem = newProblem;
    }

    public void SetPopSize(int Size)
    {
        populationSize = Size;
    }
    public void SetName(string newName)
    {
        Name = newName;
    }
    public string GetName()
    {
        return Name;
    }

    public void SetHUD(GameObject Container)
    {
        Algo_HUD = Container;
    }
}
