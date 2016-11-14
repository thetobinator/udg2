using UnityEngine;
using System.Collections;
using System.IO;
using UnityEngine.SceneManagement;
using System.Collections.Generic; // so we can use List<string> tText= new List<string>(); because that's 'easy' RRRRGgh



//[ExecuteInEditMode]


public class MainGameManager : MainGameInit
{


    public static MainGameManager instance; //local tCurrentLevel=ig3dGetLevelNames()

    // global score 
    private int currentScore;
    public string currentScene;
    public string scenePath;

    
    // Required game Object References
    
    public GameObject bullet;
    public GameObject gun;
    public GameObject destinationMarker;
    public GameObject ragdollTemplatePrefab;
    private GameObject m_ragdollTemplate;


    // global text box update
    public int showScreenText = 0;
    [Multiline]
    public List<string> screenText = new List<string>();
    // "ScreenText is the word.";

    //
    public GameObject[] newZombies;
    public GameObject[] newHumans;

    //  Parents for Spawns and deadbodies
    public  static GameObject ZombieParent;
    public static GameObject HumanParent;
    public static GameObject DeadBodies;

    // Coloring the GUI
    private float rr = 0.0f, gg = 0.0f, bb = 0.0f;
    private int rv = 1, gv = 1, bv = 1;
    private float slowColorTime = 0.0f;

    
    //startled humans alert the zombies
    private GameObject[] screamerObject;
    private Vector3[] screamVector3;
    private Vector3[] trackedPosition;


    #region Example UDGINIT.LUA as C#


    // UDGInit.lua to C# 07 25 2015
    // -- UDGInit with copies from RTS Init 01 03 09; 
    // 
    // 
    /* turning things off to  debug 08 27 2015
     * 
    /*this is old udeadgame Lua to C# should ineherit from the MainGameInit or Maybe Sub Manager scripts? Tracking.cs etc perhaps?
    private bool doShadows = true;
    private string masterAnimSource = "mcTrueBones.wtf";
    private string selectionRectModel = "selectionRect.wtf";
    private string statusString = "";
    private int trackingLight = 3;
    private int objCycle = 2;
    private int teamSelect = 0;
    private int lastClick = 0;
    private float doubleClickInterval = 0.4f;

    private int cameraSpeed = 5;
    private float velocityClamp = 1.16f;
    private bool levelIsLit = false;
    private bool handleUDGAI = true;
    private bool trackLightEnabled = false;
    private string nextUDGLevel = null;
    private bool CAPSLOCKKEY = false;
    */

    // -- counts how long the gun flare is on; 
    private int gunLightCount = 0;

    private bool reloadLevel = false; //-- added 02 08 2009 ; 
    private float startPositionTimer = 0.0f;

    // -- added 02 26 2009 keyboard teleport; 
    private bool teleportTimerActive = false;
    private float teleportTime = 0.0f;
    private int trackedObject = 3; //TrackedPosition.x,TrackedPosition.y,TrackedPosition.z = 0,0,0; 
    private bool displayLoading = false;
    private bool restartLevel = false;
    private float timeBeforeLoad = 0.0f;
    private int loadingTextBoxNum = 1;
    private string loadingBoxMessage = "";
    private float textualTimer = 0.0f;
    private int currentText = 1;
    private bool hideHUD = false;
    private bool wonThisLevel = false;
    //public string CurrentLevel = Application.loadedLevelName; //local tCurrentLevel=ig3dGetLevels()
    //--write currentlevel.lua to UDG folder
    public TextAsset udg2helpcontrolstext;
   public int zombieGroupSize = 1;
    //private char Quote = '\"';
    #endregion



    class ZombieCommander
	{
 
        void zombieTargetUpdate(RaycastHit hit)
        {          
            GameObject[] zombies = GameObject.FindGameObjectsWithTag("Zombie");
            GameObject commandCandidate = null;
            bool acceptedTaskFlag = false;
            for (uint i = 0; i < 2; ++i)
            {
                float minSqrDistance = -1.0f;
                foreach (GameObject zombie in zombies)
                {
                    ZombieBehavior zb = zombie.GetComponent<ZombieBehavior>();
                    HealthComponent hc = zombie.GetComponent<HealthComponent>();
                    NavMeshAgent na = zombie.GetComponent<NavMeshAgent>();
                    if (zb != null && zb.enabled && zb.hasPlayerTask() == acceptedTaskFlag
                        && hc != null && hc.enabled && !hc.isDead() && na != null && na.enabled)
                    {
                        float sqrDistance = (hit.point - zb.transform.position).sqrMagnitude;
                        if (minSqrDistance == -1.0f || sqrDistance < minSqrDistance)
                        {
                            commandCandidate = zombie;
                            minSqrDistance = sqrDistance;
                        }
                    }
                }
             
                if (commandCandidate != null)
                {
                    // add dummy destination marker:
                    NavMeshPath path = new NavMeshPath();
                    NavMesh.CalculatePath(commandCandidate.transform.position, hit.point, NavMesh.AllAreas, path);
                    if (path.corners.Length > 0)
                    {
                        Vector3 destinationPosition = path.corners[path.corners.Length - 1];
                        GameObject destinationMarker = (GameObject)Instantiate(MainGameManager.instance.destinationMarker);
                        destinationMarker.transform.position = new Vector3(destinationPosition.x, destinationPosition.y + 0.01f, destinationPosition.z);
                        destinationMarker.transform.eulerAngles = new Vector3(90.0f, 0.0f, 0.0f);
                        commandCandidate.GetComponent<ZombieBehavior>().setTargetFromRaycastHit(hit);
                    }
                    break;
                }
                else
                {
                    acceptedTaskFlag = !acceptedTaskFlag;
                }
            }
        }

       

        public void update()
		{
            var kb = instance.GetComponent<UDG_KeyboardInput>();
         

            GameObject[] zombies = GameObject.FindGameObjectsWithTag("Zombie");
           
            for (int i = 0; i < zombies.Length; i++)
            {
                kb.zombieKeyboardInput(zombies[i]);
            }
            instance.zombieGroupSize = kb.multipleZombieCommand(instance.zombieGroupSize);
            if (Input.GetMouseButtonUp (0)) {            
				Ray ray = Camera.main.GetComponent<Camera> ().ScreenPointToRay (Input.mousePosition);
				RaycastHit hit = new RaycastHit ();
				if (Physics.Raycast (ray, out hit)) {
                                      
                    for (int i = 0; i < instance.zombieGroupSize; i++)
                    {                     
                        zombieTargetUpdate(hit);
                    }
				}
			}
		}
	}

	class MovementObserver
	{
		Hashtable m_oldObjectPositions = new Hashtable ();
		Hashtable m_newObjectPositions = new Hashtable ();
		float m_time = 0.0f;
		float m_queryInterval = 0.5f;

		public void update()
		{
			if( m_time == 0.0f || (int)(m_time / m_queryInterval) != (int)((m_time + Time.deltaTime) / m_queryInterval)) {
				string[] tags = { "Human", "Zombie" };
				m_oldObjectPositions.Clear ();
				foreach( DictionaryEntry d in m_newObjectPositions ) {
					m_oldObjectPositions.Add ( d.Key, d.Value );
				}
				m_newObjectPositions.Clear();
				for( uint tagIndex = 0; tagIndex < tags.Length; ++tagIndex ) {
					GameObject[] objects = GameObject.FindGameObjectsWithTag( tags[ tagIndex ] );
					foreach( GameObject o in objects ) {
						m_newObjectPositions.Add( o.GetInstanceID(), o.GetComponent<Transform>().position );
					}
				}

				if( m_time == 0.0f )
				{
					m_oldObjectPositions.Clear ();
					foreach( DictionaryEntry d in m_newObjectPositions ) {
						m_oldObjectPositions.Add ( d.Key, d.Value );
					}
				}
			}
			m_time += Time.deltaTime;
		}

		public float getObjectSpeed( GameObject obj )
		{
           
			int key = obj.GetInstanceID ();
			if (m_oldObjectPositions.ContainsKey (key)
				&& m_newObjectPositions.ContainsKey (key)) {
				Vector3 oldPos = (Vector3)m_oldObjectPositions[ key ];
				Vector3 newPos = (Vector3)m_newObjectPositions[ key ];
				newPos = newPos - oldPos;
				return newPos.magnitude / m_queryInterval;
			}

			return 0.0f;
		}
	}

	private MovementObserver m_movementObserver = new MovementObserver();
	private ZombieCommander m_zombieCommander = new ZombieCommander();

	public float getObjectSpeed( GameObject obj )
	{
		return m_movementObserver.getObjectSpeed (obj);
	}

        // what does populationData do? Looks like it sets up the singleton?
        struct PopulationData
        {
            uint m_poolSize;
            uint m_targetCount;
            float m_spawnInterval;
            float m_lastSpawnTime;
            float m_time;
            string m_factionTag;
            string m_spawnPointTag;
            GameObject[] m_entities;
            GameObject[] m_prefabs;

/* Unity Rougelike tutorial item spawing singleton setup?

            public void setup(uint poolSize, uint targetCount, float spawnInterval, string factionTag, GameObject[] prefabs, string spawnPointTag)
            {
                m_poolSize = poolSize;
                m_targetCount = targetCount;
                m_spawnInterval = spawnInterval;
                m_factionTag = factionTag;
                m_spawnPointTag = spawnPointTag;
                m_prefabs = prefabs;
                m_lastSpawnTime = -m_spawnInterval;
                m_time = 0.0f;
                if (prefabs.Length == 0)
                {
                    print("There are no prefabs, spawning is disabled");
                    m_poolSize = 0;
                }
            }
*/      

        private void spawnOneAtEachSpawnpoint()
		{
			GameObject[] spawnPoints = GameObject.FindGameObjectsWithTag(m_spawnPointTag);
			if (spawnPoints.Length == 0)
			{
				print("No spawn points with tag '" + m_spawnPointTag + "' found in scene! Cannot spawn any '" + m_factionTag + "s'!");
			}

			for( uint i = 0u; i < spawnPoints.Length; ++i )
			{
				GameObject spawnPoint = spawnPoints[ i ];
				Instantiate(m_prefabs[Random.Range(0, m_prefabs.Length)], spawnPoint.transform.position, spawnPoint.transform.rotation);
			}
		}

        public void update(float timeStep)
        {
			if( m_time == 0.0f  && m_prefabs.Length > 0u )
			{
				m_time = 1.0f;// *sigh*
                // prefabbed UMA self spawns.
				//spawnOneAtEachSpawnpoint();
			}
            #region Test old delayed spawning code
            // :TO: enable below code for testing the old delayed spawning code            
            /*
            m_time += timeStep;
            m_entities = GameObject.FindGameObjectsWithTag(m_factionTag);

            if (m_entities.Length < m_targetCount && m_poolSize != 0u)
            {
                if (m_time - m_lastSpawnTime >= m_spawnInterval)
                {
                    m_lastSpawnTime += m_spawnInterval;
                    GameObject[] spawnPoints = GameObject.FindGameObjectsWithTag(m_spawnPointTag);
                    if (spawnPoints.Length == 0)
                    {
                        print("No spawn points with tag '" + m_spawnPointTag + "' found in scene! Cannot spawn any '" + m_factionTag + "s'!");
                    }
                    else
                    {
                        GameObject spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
                        Instantiate(m_prefabs[Random.Range(0, m_prefabs.Length)], spawnPoint.transform.position, spawnPoint.transform.rotation);
                        --m_poolSize;
                    }

                }
            }
            */
            #endregion
        }
    }

    private PopulationData m_zombies;
    private PopulationData m_humans;

    
    // color the GUI
    public Color slowColor()
    {
        Color GUITextColor = new Color(rr, gg, bb);
        slowColorTime = (Time.deltaTime * 0.05f);
        // this little bit here cycles through colors very very slowly
        if (rr <= 0.0f) { rv = 1; }
        if (gg <= 0.0f) { gv = 1; }
        if (bb <= 0.0f) { bv = 1; }
        if (rr >= 0.99f) { rv = -1; }
        if (gg >= 0.99f) { gv = -1; }
        if (bb >= 0.99f) { bv = -1; }
        if (rr <= 1.0f && gg <= 0.9f) { rr += (slowColorTime * rv); }
        if (gg <= 1.0f && rr >= 0.5f) { gg += (slowColorTime * gv); }
        if (bb <= 1.0f && gg >= 0.5f) { bb += (slowColorTime * bv); }
        return GUITextColor;
    }

    public void updateScreenText(int itemNumber)
    {
       
        screenText.Clear();
        screenText.Add(zombieCount().ToString() + " Flesheaters");
        screenText.Add(humanCount().ToString() + " Meals");
       
        screenText.Add("\nCommanding: " + instance.zombieGroupSize + "\nStalking:");
        string groupsText = "";
        foreach (GameObject z in GameObject.FindGameObjectsWithTag("Zombie"))
        {
            ZombieBehavior zb = z.GetComponent<ZombieBehavior>();
            if (zb != null && zb.taskObject != null)
            {
                string n = zb.taskObject.name;
                if (!groupsText.Contains(n))
                {
                    groupsText = groupsText + "    " + n +" \n";
                }
            }
        }
        screenText.Add(groupsText );
        screenText.Add("\n\n\n" + udg2helpcontrolstext.text);
        string screenTextOut = "";
        for (int i = 0; i < screenText.Count; i++)
        {
            screenTextOut = screenTextOut + "\n" + screenText[i];
        }
        screenText.Add(screenTextOut);
            GUI.contentColor = slowColor();
            GUI.Label(new Rect(10, 10, 700, 400), screenText[screenText.Count-1]);     
    }

    // OnGUI is auto updating.
    public void OnGUI()
    {
         updateScreenText(0);
   
    }

    public void AdjustScore(int num)
    {
        currentScore += num;
    }


    void Awake()
    {
        instance = this;
        scenePath = SceneManager.GetActiveScene().path;
        currentScene = SceneManager.GetActiveScene().name;      

        // create parent objects
        HumanParent = new GameObject("HumanParent");
        HumanParent.transform.parent = this.transform;
        ZombieParent = new GameObject("ZombieParent");
        ZombieParent.transform.parent = this.transform;
        DeadBodies = new GameObject("DeadBodies");
        DeadBodies.transform.parent = this.transform;    
    }

  
    public GameObject getRagdollTemplate()
	{
		return m_ragdollTemplate;
	}

    static public GameObject getRootObject(GameObject obj)
    {
        GameObject root = obj;
        for (; root.transform.parent != null; root = root.transform.parent.gameObject) { }
        return root;
    }

    public int zombieCount()
    {
       // newZombies = GameObject.FindGameObjectsWithTag("Zombie");
        foreach(GameObject z in GameObject.FindGameObjectsWithTag("Zombie"))
        {
            if (z.GetComponent<HealthComponent>().current_health <= 0.0f && !z.GetComponent<NavMeshAgent>().enabled)
            {
                z.tag = "Dead";
                z.name = "Dead";
                z.transform.parent = GameObject.Find("DeadBodies").transform;
            }
           /* else
            {
              Vector3 ztrans = z.transform.localPosition;
                GameObject p = GameObject.Find("ZombieParent");
                p.transform.localPosition = ztrans;
                if (getRootObject(z).name != "ZombieParent")
                {
                   
                    z.transform.SetParent(GameObject.Find("ZombieParent").transform,false);
                   
                }
                z.transform.localPosition = ztrans;
            }*/
        }
        newZombies = GameObject.FindGameObjectsWithTag("Zombie");
        return newZombies.Length;
    }

    public int humanCount()
    {
        foreach (GameObject h in GameObject.FindGameObjectsWithTag("Human"))
        {
            HealthComponent hc = h.GetComponent<HealthComponent>();
            if (hc != null && hc.current_health <= 0.0f)
            {
                h.tag = "Zombie";
                h.name = "Zombie" + GameObject.FindGameObjectsWithTag("Zombie").Length;
                ZombieBehavior z =  h.AddComponent<ZombieBehavior>();
                hc.current_health = 100.0f;
                HumanBehavior hb = GetComponent<HumanBehavior>();
                Destroy(hb);
            }
          //parenting disabled because of position shift.
            //h.transform.SetParent(GameObject.Find("HumanParent").transform, false);
        }
        newHumans = GameObject.FindGameObjectsWithTag("Human");
        return newHumans.Length;
    }

    public void Update()
    {
       // m_humans.update(Time.deltaTime);
        //m_zombies.update(Time.deltaTime);
		m_movementObserver.update ();
		m_zombieCommander.update ();

    }

    private void Start()
    {
        // keyboard input script
        this.gameObject.AddComponent<UDG_KeyboardInput>();

        // help text file 
        udg2helpcontrolstext = (TextAsset)Resources.Load("TextAssets/udg2_help_controls");
        GameObject TagList = GameObject.Find("UDG_Tag_List");

        // don't confuse the game mode with the taglist
        if (TagList) { TagList.SetActive(false); }
        
        m_ragdollTemplate = (GameObject)Instantiate(ragdollTemplatePrefab, transform.position, transform.rotation);
        //Unity  rouguelike tutorial spawning.
        //setup(uint poolSize, uint targetCount, float spawnInterval, string factionTag, GameObject[] prefabs, string spawnPointTag)
        /*  m_humans.setup(10u, 10u, 1.0f, "Human", humans, "SpawnPoint_Human");
           m_zombies.setup(7u, 7u, 1.0f, "Zombie", zombies, "SpawnPoint_Zombie");
           */
    }

    //end MainGameManager
}