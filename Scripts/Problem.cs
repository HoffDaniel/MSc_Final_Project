using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Problem : MonoBehaviour
{
    //The name of the problem
    private string Name;
    public Vector3 Pos_P;


    //The environment the problem is in/uses/tackles... problem can be the environment but not necessarily
    //public Environment environment;
    public GameObject Background;
    public List<GameObject> environment;
    public List<GameObject> agents;
    public List<GameObject> prefabs;

    //UI 
    //public GameObject gameObjectA;


    public int nbr_agentsFin = 0;

    public int min = 0;
    public int max = 750;
    
    public int seed = int.MinValue; //This value is used to generate the same random values
    public int previous_Seed;

    private void Awake()
    {
        SetName("This is a Problem");
    }

    void Start()
    {
        prefabs = new List<GameObject>();
        environment = new List<GameObject>();
        //environment.Add(new GameObject("Is this working?"));
        Debug.Log("Problem Initialised: " + Name);
    }

    void Update()
    {
        
    }

    public virtual void InitAgents(int PopSize)
    {
        Debug.Log("Overridden this should be - InitAgents");
    }
    public virtual void ResetEnvironment()
    {
        Debug.Log("Overridden this should be - ResetEnvironment");
    }

    public float Fitness()
    {
        float score = float.MinValue;

        return score;
    }

    public virtual float Fitness(List<Vector2> solution)
    {
        float score = float.MinValue;

        return score;
    }

    public void SetName(string newName)
    {
        Name = newName;
    }
    public string GetName()
    {
        return Name;
    }
}
