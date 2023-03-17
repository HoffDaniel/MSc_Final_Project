using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditor;

//MainController is the bridge between user and the algorithm
//Contains all the information that is going to be passed to the algorithm /the logic -> how and what to update 
//The controller of a MVC (Model View Controller) architecture

public class MainController : MonoBehaviour
{
    [Header("User - Settings")]
    public GameObject User;

    [Header("Algorithm - Settings")]
    public List<Algorithm> List_Algorithms;
    public GameObject Algorithm_One;
    public GameObject Algorithm_Two;

    [Header("Problem - Settings")]
    public List<Problem> List_Problems;
    public Problem Current_Problem;

    [Header("UI elements")]
    public bool toggle_UI;
    public GameObject main_Canvas_Container;
    public Canvas canvas_Main;    
    public GameObject UI_Algo_One;
    public GameObject UI_Algo_Two;
    public GameObject UI_Problem;
    public GameObject UI_Buttons;
    //public GameObject UI_seed;

    [Header("HUD elements")]
    public GameObject Algo_1_HUD;
    public GameObject Algo_2_HUD;

    void Awake()
    {
        Debug.Log("This is the first thing");
    }
    // Start is called before the first frame update
    void Start()
    {
        //main_Canvas_Container = new GameObject("Canvas: Main");

        Algorithm_One = new GameObject("Algorithm One");

        Algorithm_Two = new GameObject("Algorithm Two");
        //Algorithm_Two.AddComponent<GeneticAlgorithm>();
        //Algorithm_Two.GetComponent<GeneticAlgorithm>().enabled = false; ////---------

        //ADD Algorithm names/options here
        List_Algorithms = new List<Algorithm>
        {
            gameObject.AddComponent<GeneticAlgorithm>(),
            gameObject.AddComponent<Algorithm>()
        };

        foreach (var item in List_Algorithms)
        {
            item.enabled = false;
        }
        //Current_Algorithm = List_Algorithms[0];


        //ADD Problem names/options here
        List_Problems = new List<Problem>
        {
            gameObject.AddComponent<TSP>(),
            gameObject.AddComponent<Problem>()
        };

        foreach (var item in List_Problems)
        {
            item.enabled = false;
        }

        Current_Problem = List_Problems[0];
        Init_Main_Canvas();
        Init_HUD(); //Sets/get the HUD elements
        Init_UI();  //Links UI elements with the algorithm / problem (adding Listeners to user inputs )       
        Init_GA_HUD(Algo_1_HUD, Algorithm_One);//Link the HUD with the GA (this should eventually just be Init_Algorithm_HUD)
        Init_GA_HUD(Algo_2_HUD, Algorithm_Two);
        Debug.Log("MainController has Started");
    }


    // Update is called once per frame
    void Update()
    {
        //UI_Algo_One.transform.GetChild(0).GetComponent<TMP_Dropdown>().RefreshShownValue();
        //UI_Algo_Two.transform.GetChild(0).GetComponent<TMP_Dropdown>().RefreshShownValue();
        //UI_Problem.transform.GetChild(0).GetComponent<TMP_Dropdown>().RefreshShownValue();


        bool tmp_Bool = Input.GetKeyDown(KeyCode.Tab);
        if (tmp_Bool)
        {
            toggle_UI = !toggle_UI;
            //UI_canvas.SetActive(toggle_UI);
            main_Canvas_Container.transform.Find("UI_Panel_1").gameObject.SetActive(toggle_UI);
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit(); //Exiting (when program is build)
            //EditorApplication.isPlaying = false; //Exiting (from Unity editor /leaving play mode / when developing)
        }

        //User camera: isFree = true ==> camera can move around with a space shipe like movement
        if (toggle_UI) User.GetComponent<UserCamera>().isFree = false;
        else User.GetComponent<UserCamera>().isFree = false;//Disabled currently

    }

    public void UpdateAlgo(GameObject algorithm, Type newAlgo)
    {
        Destroy(algorithm.GetComponent<Algorithm>());
        algorithm.AddComponent(newAlgo);
    }

    public void UpdateProblem(GameObject algorithm, Type newProb)
    {
        Destroy(algorithm.GetComponent<Problem>());        
        algorithm.AddComponent(newProb);
        algorithm.GetComponent<Algorithm>().SetProblem(algorithm.GetComponent<Problem>());
    }

    public void Alocate_Algo_Unique()
    {
        int Space_Between_Algos = 100;
        int PosOne = Algorithm_One.GetComponent<Problem>().min;
        int PosTwo = PosOne + Algorithm_One.GetComponent<Problem>().max + Space_Between_Algos;
        
        //Assign position
        Algorithm_One.GetComponent<Algorithm>().Algo_Pos = new Vector3(PosOne, PosOne, 0f);
        Algorithm_Two.GetComponent<Algorithm>().Algo_Pos = new Vector3(PosTwo, PosOne, 0f);

        //Assign ID
        Algorithm_One.GetComponent<Algorithm>().Algo_ID = 1;
        Algorithm_Two.GetComponent<Algorithm>().Algo_ID = 2;

        Algorithm_One.transform.position = new Vector3(PosOne, PosOne, 0f);
        Algorithm_Two.transform.position = new Vector3(PosTwo, PosOne, 0f);




    }

    public void Update_AlgoSelection(TMP_Dropdown dropdown, GameObject Container) {
        int i = dropdown.value;
        //Current_Algo_Name = List_Algorithms[i].GetName();
        Type type = List_Algorithms[i].GetType();
        UpdateAlgo(Container, type);          
    }

    public void Update_ProbSelection(TMP_Dropdown dropdown) {
        int i = dropdown.value;
        Current_Problem = List_Problems[i];
        Type type = Current_Problem.GetType();
        UpdateProblem(Algorithm_One, type);
        UpdateProblem(Algorithm_Two, type);
    }


    public void Init_Main_Canvas()
    {
        GameObject Canvas_Prefab = Resources.Load("UI/UI (Canvas)") as GameObject;
        main_Canvas_Container = Instantiate(Canvas_Prefab);
        //main_Canvas_Container.GetComponent<Canvas>().transform.GetChild(0).transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "THis is nice";
        canvas_Main = main_Canvas_Container.GetComponent<Canvas>();
    }

    public void Init_HUD()
    {
        //if GA
        Transform HUD_Panel = canvas_Main.transform.Find("HUD_Panel").transform;
        Algo_1_HUD = HUD_Panel.GetChild(0).gameObject;
        Algo_2_HUD = HUD_Panel.GetChild(1).gameObject;
    }

    public void Init_UI(GameObject main_Canvas)
    {
        //Algorithm_One.Init_UI()
        //Algorithm_Two.Init_UI()

    }
    public void Init_UI()
    {
        Transform panel_1 = canvas_Main.transform.Find("UI_Panel_1").transform;
        Transform panel_2 = canvas_Main.transform.Find("UI_Panel_2").transform;

        UI_Algo_One = panel_1.Find("Algorithm_One").gameObject;        
        UI_Algo_Two = panel_1.Find("Algorithm_Two").gameObject;
        UI_Problem = panel_1.Find("Problem").gameObject;
        UI_Buttons = panel_2.Find("StateButtons").gameObject;       
        
        //Init_UI_Algo_Selections();
        Init_UI_Algo_Selections(Algorithm_One, UI_Algo_One, Algo_1_HUD);
        Init_UI_Algo_Selections(Algorithm_Two, UI_Algo_Two, Algo_2_HUD);

        Init_UI_Prob_Selections();
        Init_UI_toggle_Animated();
        Init_UI_buttons();
        Alocate_Algo_Unique();
        toggle_UI = true;

        //INit
    }
    public void Init_UI_buttons()
    {
        Button button_Start = UI_Buttons.transform.GetChild(0).GetComponent<Button>();
        Button button_Pause = UI_Buttons.transform.GetChild(1).GetComponent<Button>();
        button_Pause.interactable = false;

        button_Start.onClick.AddListener(delegate {
            //Current_Algorithm.isRunning = !Current_Algorithm.isRunning;
            Algorithm_One.GetComponent<Algorithm>().isRunning = !Algorithm_One.GetComponent<Algorithm>().isRunning;
            Algorithm_Two.GetComponent<Algorithm>().isRunning = Algorithm_One.GetComponent<Algorithm>().isRunning; //Should be same as algo one

            //Elements that should be blocked 
            TMP_InputField tmp_Input = UI_Problem.transform.Find("Seed (TMP)").GetComponent<TMP_InputField>();
            Slider tmp_Slider = UI_Problem.transform.Find("NbrPoints(Slider)").GetComponent<Slider>();

            if (Algorithm_One.GetComponent<Algorithm>().isRunning) //When started
            { 
                button_Start.GetComponentInChildren<TextMeshProUGUI>().text = "Stop";
                tmp_Input.interactable = false;
                tmp_Slider.interactable = false;
                button_Pause.interactable = true;

            } 
            else //When stopped 
            {
                button_Start.GetComponentInChildren<TextMeshProUGUI>().text = "Start";
                tmp_Input.interactable = true;
                tmp_Slider.interactable = true;
                button_Pause.interactable = false;
                if (button_Pause.GetComponentInChildren<TextMeshProUGUI>().text == "Resume")
                {
                    button_Pause.onClick.Invoke();
                }
                

                Alocate_Algo_Unique();
            } 
        });

        //Add listener to the pause button
        button_Pause.GetComponent<Button>().onClick.AddListener(delegate {
            Algorithm_One.GetComponent<Algorithm>().hasPause = !Algorithm_One.GetComponent<Algorithm>().hasPause;
            Algorithm_Two.GetComponent<Algorithm>().hasPause = !Algorithm_Two.GetComponent<Algorithm>().hasPause;

            if (Algorithm_One.GetComponent<Algorithm>().hasPause) { button_Pause.GetComponentInChildren<TextMeshProUGUI>().text = "Resume"; } //When paused
            else { button_Pause.GetComponentInChildren<TextMeshProUGUI>().text = "Pause"; }//When un paused
        });
    }

    public void Init_UI_toggle_Animated()
    {
        Toggle toggle_Anim = canvas_Main.transform.GetChild(1).GetChild(1).GetComponent<Toggle>();
        //
        Algorithm_One.GetComponent<Algorithm>().hasAnimation = toggle_Anim.isOn;
        Algorithm_Two.GetComponent<Algorithm>().hasAnimation = toggle_Anim.isOn;

        //Current_Algorithm.hasAnimation = toggle_Animated.isOn;
        toggle_Anim.onValueChanged.AddListener(delegate {
            Algorithm_One.GetComponent<Algorithm>().hasAnimation = toggle_Anim.isOn;
            Algorithm_Two.GetComponent<Algorithm>().hasAnimation = toggle_Anim.isOn;
        });
    }

    public void Init_UI_Algo_Selections(GameObject Algorithm, GameObject UI_Algorithm, GameObject HUD_Algo)
    {
        //======== Dropdowns =======
        //=== Algorithm selection
        TMP_Dropdown dropdown_Algo= UI_Algorithm.transform.GetChild(0).GetComponent<TMP_Dropdown>();
        dropdown_Algo.ClearOptions();
        int count = List_Algorithms.Count;
        List<string> elems = new();

        string tmp_Name;
        for (int i = 0; i < count; i++)
        {
            tmp_Name = List_Algorithms[i].GetName();
            elems.Add(tmp_Name);
        }
        dropdown_Algo.AddOptions(elems);
        Update_AlgoSelection(dropdown_Algo, Algorithm);

        dropdown_Algo.onValueChanged.AddListener(delegate{Update_AlgoSelection(dropdown_Algo, Algorithm);});
        

        //======= Sliders =======
        //=== Nbr of agents
        Slider Agents_nbr_Slider = UI_Algorithm.transform.GetChild(1).GetComponent<Slider>();
        Init_UI_NbrAgents(Agents_nbr_Slider, Algorithm, 100);


        //======= Algorithm specific ========
        //==== Genetic Algorithm       
        Init_GA_UI(UI_Algorithm, Algorithm);
        //=== CutOff
        //=== SurvivorKeep
        //=== Mutation Rate
        //=== Mutation
        //=== Crossover Rate
        //=== Crossover 


    }

    public void Init_UI_Prob_Selections()
    {
        //======= Dropdown =======
        //=== Problem selection
        TMP_Dropdown dropdown_Prob = UI_Problem.transform.Find("Problems").GetComponent<TMP_Dropdown>();

        dropdown_Prob.ClearOptions();
        int count = List_Problems.Count;
        List<string> elems = new();

        string tmp_Name;
        for (int i = 0; i < count; i++)
        {
            tmp_Name = List_Problems[i].GetName();
            elems.Add(tmp_Name);
        }

        foreach (var item in elems)
        {
            dropdown_Prob.options.Add(new TMP_Dropdown.OptionData() { text = item });
        }

        Update_ProbSelection(dropdown_Prob);

        dropdown_Prob.onValueChanged.AddListener(delegate {Update_ProbSelection(dropdown_Prob); });

        //=== SeeD value
        Init_UI_Seed(Algorithm_One);
        Init_UI_Seed(Algorithm_Two);

        //=== Max number of Epochs value
        Init_UI_Epoch(Algorithm_One);
        Init_UI_Epoch(Algorithm_Two);

        //=== Nbr of cities
        Init_UI_Prob_Environment(Algorithm_One.GetComponent<Problem>(), 50);
        Init_UI_Prob_Environment(Algorithm_Two.GetComponent<Problem>(), 50);

    }

    //public void Init_UI_FileExplorer(GameObject Container)
    //string path = EditorUtility.OpenFilePanel();
    public void Init_UI_Seed(GameObject Container) //,String "Name" (of the object containing the seed input field
    {
        
        TMP_InputField tmp_Input = UI_Problem.transform.Find("Seed (TMP)").GetComponent<TMP_InputField>();
        tmp_Input.text = "7";
        Container.GetComponent<Problem>().seed = int.Parse(tmp_Input.text);

        tmp_Input.onValueChanged.AddListener(delegate { Container.GetComponent<Problem>().seed = int.Parse(tmp_Input.text); });
    }

    public void Init_UI_Epoch(GameObject Container)
    {

        TMP_InputField tmp_Input = UI_Problem.transform.Find("Epoch (TMP)").GetComponent<TMP_InputField>();
        tmp_Input.text = "5000";
        Container.GetComponent<Algorithm>().MaxEpochs = int.Parse(tmp_Input.text);

        tmp_Input.onValueChanged.AddListener(delegate { Container.GetComponent<Algorithm>().MaxEpochs= int.Parse(tmp_Input.text); });
    }


    public void Init_UI_Prob_Environment(Problem problem, int init_nbr)
    {
               
        if (problem.GetType().Name == "TSP")
        {
            Slider UI_Nbr_Points_Slider = UI_Problem.transform.Find("NbrPoints(Slider)").GetComponent<Slider>();
            TextMeshProUGUI UI_NbrPoints_value = UI_Nbr_Points_Slider.transform.Find("NbrPoints (Value)").GetComponent<TextMeshProUGUI>();

            problem.GetComponent<TSP>().nbr_Points = init_nbr;
            UI_Nbr_Points_Slider.value = init_nbr;
            UI_NbrPoints_value.text = init_nbr.ToString();

            UI_Nbr_Points_Slider.onValueChanged.AddListener(delegate
            {
                int tmp_value = (int)UI_Nbr_Points_Slider.value;
                UI_NbrPoints_value.text = tmp_value.ToString();
                problem.GetComponent<TSP>().nbr_Points = tmp_value;
            });

        }
    }

    public void Init_UI_NbrAgents(Slider UI_agents_slider, GameObject Algo_Container, int init_nbr)
    {
        TextMeshProUGUI UI_slider_value= UI_agents_slider.transform.GetChild(3).GetComponent<TextMeshProUGUI>();
        
        UI_agents_slider.value = init_nbr; 
        
        Algo_Container.GetComponent<Algorithm>().SetPopSize(init_nbr);


        UI_agents_slider.onValueChanged.AddListener(delegate
        {
            int tmp_value = (int)UI_agents_slider.value;
            UI_slider_value.text = tmp_value.ToString();
            Algo_Container.GetComponent<Algorithm>().SetPopSize(tmp_value);
        });

    }

    public void Init_GA_HUD(GameObject Algo_Hud, GameObject Algo_Container)
    {
        Algo_Container.GetComponent<Algorithm>().SetHUD(Algo_Hud); 
    }
    public void Init_GA_UI(GameObject UI_Algorithm, GameObject Algo_Container)
    {
        //===Cutoff
        float init_Cutoff = 30f;
        Slider Cutoff_slider = UI_Algorithm.transform.GetChild(2).GetComponent<Slider>();
        TextMeshProUGUI UI_Cutoff_value = Cutoff_slider.transform.GetChild(3).GetComponent<TextMeshProUGUI>();
        Cutoff_slider.value = init_Cutoff;
        UI_Cutoff_value.text = (init_Cutoff*0.01).ToString();
        Algo_Container.GetComponent<GeneticAlgorithm>().cutoff = init_Cutoff*0.01f;

        Cutoff_slider.onValueChanged.AddListener(delegate
        {
            float tmp_value = Cutoff_slider.value * 0.01f;
            UI_Cutoff_value.text = tmp_value.ToString();
            Algo_Container.GetComponent<GeneticAlgorithm>().cutoff = tmp_value;
        });

        //===SurvivorKeep
        int init_SurvivorKeep = 5;
        Slider SurvivorKeep_slider = UI_Algorithm.transform.GetChild(3).GetComponent<Slider>();
        TextMeshProUGUI UI_SurvivorKeep_value = SurvivorKeep_slider.transform.GetChild(3).GetComponent<TextMeshProUGUI>();
        SurvivorKeep_slider.value = init_SurvivorKeep;
        Algo_Container.GetComponent<GeneticAlgorithm>().survivorKeep = init_SurvivorKeep;

        SurvivorKeep_slider.onValueChanged.AddListener(delegate
        {
            int tmp_value = (int)SurvivorKeep_slider.value;
            UI_SurvivorKeep_value.text = tmp_value.ToString();
            Algo_Container.GetComponent<GeneticAlgorithm>().survivorKeep = tmp_value;
        });

        //===Mutation Rate
        float init_MutationRate = 50f;
        Slider MutationRate_slider = UI_Algorithm.transform.GetChild(4).GetComponent<Slider>();
        TextMeshProUGUI UI_MutationRate_value = MutationRate_slider.transform.GetChild(3).GetComponent<TextMeshProUGUI>();

        MutationRate_slider.value = init_MutationRate;
        UI_MutationRate_value.text = (init_MutationRate *0.01).ToString();
        Algo_Container.GetComponent<GeneticAlgorithm>().mutationRate = init_MutationRate * 0.01f;

        MutationRate_slider.onValueChanged.AddListener(delegate
        {
            float tmp_value = MutationRate_slider.value * 0.01f;
            UI_MutationRate_value.text = tmp_value.ToString();
            Algo_Container.GetComponent<GeneticAlgorithm>().mutationRate = tmp_value;
        });

        //=== Mutation 
        TMP_Dropdown dropdown_Mutation = UI_Algorithm.transform.GetChild(5).GetComponent<TMP_Dropdown>();
        dropdown_Mutation.ClearOptions();
        string [] mutationTypes = Enum.GetNames(typeof(DNA.muta));
        List<string> muta_elems = new(mutationTypes);
        dropdown_Mutation.AddOptions(muta_elems);

        dropdown_Mutation.value = (int)DNA.muta.Inversion;
        Algo_Container.GetComponent<GeneticAlgorithm>().Mutation = DNA.muta.Inversion; //Initiate with this mutation type
        dropdown_Mutation.onValueChanged.AddListener(delegate { Algo_Container.GetComponent<GeneticAlgorithm>().Mutation = (DNA.muta)dropdown_Mutation.value; });
        
        //===CrossoverRate
        float init_CrossoverRate = 0f;
        Slider CrossoverRate_slider = UI_Algorithm.transform.GetChild(6).GetComponent<Slider>();
        TextMeshProUGUI UI_CrossoverRate_value = CrossoverRate_slider.transform.GetChild(3).GetComponent<TextMeshProUGUI>();

        CrossoverRate_slider.value = init_CrossoverRate;
        UI_CrossoverRate_value.text = (init_CrossoverRate*0.01f).ToString();
        Algo_Container.GetComponent<GeneticAlgorithm>().crossoverRate = init_CrossoverRate * 0.01f;        

        CrossoverRate_slider.onValueChanged.AddListener(delegate
        {
            float tmp_value = CrossoverRate_slider.value * 0.01f;
            UI_CrossoverRate_value.text = tmp_value.ToString();
            Algo_Container.GetComponent<GeneticAlgorithm>().crossoverRate = tmp_value;
        });

        //===Crossover
        TMP_Dropdown dropdown_Crossover = UI_Algorithm.transform.GetChild(7).GetComponent<TMP_Dropdown>();
        dropdown_Crossover.ClearOptions();
        string[] crossoverTypes = Enum.GetNames(typeof(DNA.CO));
        List<string> CO_elems = new(crossoverTypes);
        dropdown_Crossover.AddOptions(CO_elems);

        dropdown_Crossover.value = (int)DNA.CO.Percent25;
        Algo_Container.GetComponent<GeneticAlgorithm>().Crossover = DNA.CO.Percent25; //Initiate with this mutation type
        dropdown_Crossover.onValueChanged.AddListener(delegate { Algo_Container.GetComponent<GeneticAlgorithm>().Crossover = (DNA.CO)dropdown_Crossover.value; });
        
    }
}   
