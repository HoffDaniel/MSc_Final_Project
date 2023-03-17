using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GeneticAlgorithm : Algorithm
{
    //========== VARIABLES in Algorithm ===========
    //private string Name
    //public int Algo_ID = 0;
    //public Vector3 Algo_Pos;
    //public GameObject Algo_HUD;
    //public bool hasInit;
    //public bool hasFin;
    //public bool hasPause;
    //public bool hasAnimation;
    //public Problem problem;
    //public float fittest;
    //public int populationSize;
    //public int MaxEpochs = 500;
    //public int currentEpoch = 0;

    //GA - BEHAVIOUR PARAMETERS
    [Header("GA - Behaviour Settings")]
    public float cutoff;
    public int survivorKeep;

    [Range(0f, 1f)]
    public float mutationRate;  
    public DNA.muta Mutation;

    [Range(0f, 1f)]
    public float crossoverRate;
    public DNA.CO Crossover = DNA.CO.Percent25;
    //[ReadOnly]
    float crossoverSection;

    public float creatureSpeed = 1000;
    public List<int> agents_PathIndex;

    
    public List<Gene> GenePool;    
    public List<DNA> population; //Solutions
    

    void Awake()
    {
        SetName("Genetic Algorithm");
        Debug.Log("GA is awake");
       
    }

    void Start()
    {
        population = new List<DNA>();
        GenePool = new List<Gene>();
        agents_PathIndex = new List<int>();
        problem = GetComponent<Problem>();

    }
    void Update()
    {
        //crossoverSection = ((float)Crossover) / 100f;
        if (Algo_HUD != null)
        {
            //Debug.Log(Algo_HUD);
            Algo_HUD.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "[ " + Algo_ID + "| Generation: " + currentEpoch;
            Algo_HUD.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "Fittest: " + fittest + " ]";
        }
        
        //Handel script start - only start algorithm when user inputs
        if (!isRunning) // If UI "Stop" button is pressed  
        {
            //Debug.Log("Is not running...");
            Reset_Algorithm(); //Resets relevant lists /=> Destroy() game objects in lists and clear() lists
            ResetData(GetResultPath());
            hasInit = false;
            return;
        }
        else if (Handel_End())
        { 
            return; 
        }
        else if (hasPause)
        {
            return;
        }
        else if (!hasInit)
        {
            Debug.Log("GA INIT");

            InitGenePool();
            InitPopulation();
            //problem = gameObject.GetComponent<Problem>();
            hasInit = true;
            return;
        }
        

        //Debug.Log("Updating...");
        Handel_Next();
    }
    
    void LateUpdate()
    {
        //update probleme environment
        //Debug.Log("LateUpdating...");
        Handel_Animation();
    }

    void InitGenePool()
    {
        //Debug.Log(GenePool.Count + " <-- This should be zero");
        if (problem.GetType().Name != "TSP")
        {
            Debug.Log("Test: " + problem.GetType().Name);

            return;
        }
        problem.ResetEnvironment();
        for (int i = 0; i < problem.environment.Count; i++)
        {
            //update environment depending on the algorithms unique position
            problem.environment[i].transform.position = problem.environment[i].transform.position + Algo_Pos;
            GenePool.Add(new Gene(i, problem.environment[i].transform.position));
        }
        // Debug.Log(problem.GetType().ToString());       
        
    }

    void InitPopulation()
    {

        problem.InitAgents(populationSize);
        int startingPath = 0;
        for (int i = 0; i < populationSize; i++)//initiate the creatures
        {
            population.Add(new DNA(GenePool.Count));            
            agents_PathIndex.Add(startingPath);

            int tmp_geneIndex = population[i].genes[agents_PathIndex[i]];
            problem.agents[i].transform.position = GetGeneValue(tmp_geneIndex);
        }
        fittest = GetScore(GetFittestIndex());
    }
    

    public void Handel_Next()
    {
       
        if (!HasActive())
        {
            
            SaveData(currentEpoch, fittest, GetResultPath());

            currentEpoch++;
            NextGeneration();
        }
    }

    public void NextGeneration()
    {
        //Prepare agents for next generation
        problem.InitAgents(populationSize); //called every gen to make sure number of agents is updated
        agents_PathIndex.Clear();
        int startingPath = 0;
        for (int i = 0; i < populationSize; i++)
        {   
            agents_PathIndex.Add(startingPath);

            int tmp_geneIndex = population[i].genes[agents_PathIndex[i]]; //pathindex = 0 -> gene[0]
            //((TSP)problem).SetAgentPos(i, GetGeneValue(tmp_geneIndex));
            problem.agents[i].transform.position = GetGeneValue(tmp_geneIndex);
            
        }

        List<DNA> survivors = new(); //To store the "survivors
        int survivorCut = Mathf.RoundToInt(populationSize * cutoff);
        int tmp_index = GetFittestIndex();
        fittest = GetScore(tmp_index);
        
        problem.agents[tmp_index].GetComponent<LineRenderer>().sortingLayerID = SortingLayer.NameToID("Foreground");
        problem.agents[tmp_index].GetComponent<LineRenderer>().startWidth = 6f;
        problem.agents[tmp_index].GetComponent<LineRenderer>().endWidth = 6f;


        //Get the fittest survivors from the population (up to the cutoff)
        for (int i = 0; i < survivorCut; i++)
        {
            
            survivors.Add(GetFittestDNA());
            population.RemoveAt(GetFittestIndex());
            //i = 0 fittest creature -> store or display etc    
        }
        population.Clear();

        //Generate the next generation
        //Elitism
        if (survivorKeep < populationSize) //Make sure survivor keep is lower than population size
        {
            for (int i = 0; i < survivorKeep; i++)
            {
                //Debug.Log("Nbr of survivors" + survivors.Count);
                population.Add(survivors[i]);
            }
        }

        
        //The rest
        int temp_Sur_Count = survivors.Count - 1;
        while (population.Count < populationSize)
        {
            for (int i = 0; i < survivors.Count; i++)
            {
                int temp_RandA = Random.Range(0, temp_Sur_Count);
                int temp_RandB = Random.Range(0, temp_Sur_Count);
                if (temp_RandA == temp_RandB)
                {
                    if (temp_RandA != 0)
                    {
                        temp_RandA--;
                    }
                    else //if = 0
                    {
                        temp_RandA++;
                    }

                }

                DNA temp_DNA = new(survivors[temp_RandA], survivors[temp_RandB], Mutation, Crossover, mutationRate, crossoverRate);
                //DNA temp_DNA = new(survivors[temp_RandA], survivors[temp_RandB], mutationRate, crossoverRate, crossoverSection);

                population.Add(temp_DNA);

                if (population.Count >= populationSize)
                {
                    break;
                }
            }
        }

    }
    public bool Handel_End()
    {
        
        if (MaxEpochs <= currentEpoch)
        {
            hasFin = true;
        }

        if (hasFin)
        {
            //Save results or something here 
            DNA winner = GetFittestDNA();
            population.Clear();
            population.Add(winner);
            problem.InitAgents(population.Count);

            problem.agents[0].GetComponent<LineRenderer>().startColor = new Color(1f, 0f, 0f, 1f);
            problem.agents[0].GetComponent<LineRenderer>().endColor = new Color(0f, 1f, 0f, 1f); 
            problem.agents[0].GetComponent<LineRenderer>().startWidth = 6f;
            problem.agents[0].GetComponent<LineRenderer>().endWidth = 6f;
            Debug.Log("!END THE WINNER IS :: " + GetFittestDNA().genes.ToString());

        }
        return hasFin;
    }
    public void Handel_Animation()
    {
        if (population.Count == 0) { return; }
        if (hasPause) { return; }

        if (!hasAnimation)
        {
            for (int i = 0; i < population.Count; i++)
            {
                List<Vector3> linePoints = new();
                for (int j = 0; j < population[i].genes.Count; j++)
                {
                    linePoints.Add(GetGeneValue(population[i].genes[j]));
                }
                linePoints.Add(GetGeneValue(population[i].genes[0]));
                problem.agents[i].GetComponent<LineRenderer>().positionCount = 0;
                problem.agents[i].GetComponent<LineRenderer>().positionCount = linePoints.Count;
                problem.agents[i].GetComponent<LineRenderer>().SetPositions(linePoints.ToArray());
            }
            return;
        }
        for (int i = 0; i < population.Count; i++)
        {
            Vector2 tmp_agentpos = problem.agents[i].transform.position;

            //Debug.Log("Agent: " + i + " | path index: " + agents_PathIndex[i] + " | Genes: " + population[i].genes.Count);

            int nextPointIndex = population[i].genes[agents_PathIndex[i]];

            List<Vector3> linePoints = new();
            for (int j = 0; j < agents_PathIndex[i]; j++)
            {
                linePoints.Add(GetGeneValue(population[i].genes[j]));
                //Debug.Log("linePoints: " + linePoints);
            }
            linePoints.Add(problem.agents[i].transform.position);
            problem.agents[i].GetComponent<LineRenderer>().positionCount = linePoints.Count;
            problem.agents[i].GetComponent<LineRenderer>().SetPositions(linePoints.ToArray());

            if (tmp_agentpos != GetGeneValue(nextPointIndex))
            {
                problem.agents[i].transform.position = Vector2.MoveTowards(tmp_agentpos, GetGeneValue(nextPointIndex), creatureSpeed * Time.deltaTime);
                
            }
            else
            {
                //Debug.Log("population[" + i + "].genes.Count = " + population[i].genes.Count);
                if (agents_PathIndex[i] != GenePool.Count) //or Ge
                {
                    agents_PathIndex[i]++;
                }
            }
        }
    }
    
    bool HasActive()
    {
        
        if (!hasAnimation) return false;

        for (int i = 0; i < population.Count; i++)
        {
            if (agents_PathIndex[i] != GenePool.Count )
            {
                //Debug.Log("At least 1 agent has not reached the end of the path");
                return true;
            }     
        }

        return false;
    }

    Vector2 GetGeneValue(int ID)
    {
        return GenePool[ID].GeneValue;
        //return dna.GenePool[ID].geneValue(problem.Name); //specific genevalues for specific problems
    }

    //Get the fitness from the problems fitness function
    public float GetFitness(List<Vector2> solution)
    {
        float Sum = problem.Fitness(solution);
        return Sum;
    }

    public DNA GetFittestDNA()
    {
        return population[GetFittestIndex()];
    }
    public int GetFittestIndex()
    {
        int key = -1;
        float best = float.MaxValue;
        for (int i = 0; i < population.Count; i++)
        {
            List<Vector2> SingleSolution = new();
            float score;
            for (int j = 0; j < population[i].genes.Count ; j++)
            {
                SingleSolution.Add(GetGeneValue(population[i].genes[j]));
            }
            score = GetFitness(SingleSolution);
            if (score < best)
            {
                best = score;
                key = i;
            }
        }
        return key;
    }

    public float GetScore(int Index)
    {
        float score;
        List<Vector2> SingleSolution = new();
        for (int j = 0; j < population[Index].genes.Count; j++)
        {
            SingleSolution.Add(GetGeneValue(population[Index].genes[j]));
        }
        score = GetFitness(SingleSolution);

        return score;
    }

    public override void Reset_Algorithm()
    {
        //Making sure list are not empty else error...
        if (population != null) population.Clear();
        if (GenePool != null) GenePool.Clear();
        if (agents_PathIndex != null) agents_PathIndex.Clear();
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


}
