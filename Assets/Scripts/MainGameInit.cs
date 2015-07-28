using UnityEngine;
using System; // allows us to use Serializable
using System.Collections;
using System.Collections.Generic; // so we can use List<string> tText= new List<string>(); because that's 'easy' RRRRGgh
using System.IO;
using System.Linq;
//using System.Linq;
// this is like BoardManager RogueLike Tutorial
// lets assume aroguemaze maker.
public class MainGameInit : MonoBehaviour
{
    // Singleton;
    public static MainGameInit mainGameInit;
    //
    public MainGameManager mainGameManager;
    public List<GameObject> zombies;
    public GameObject[] humans;
    public List<GameObject> levelMaps;
    public GameObject loadedLevel;
    public int currentMap = 1;

    // UDGInit.lua to C# 07 25 2015
    // -- UDGInit with copies from RTS Init 01 03 09; 
    // 
    // 
    public string gameroot;//= Application.dataPath;
    public string levelPath;// = Application.dataPath  + "/Data/Levels/"; 
    public string gameRoot;// = gameroot; //break and fix this all at once ...later;
    public string currentLevel;// = Application.loadedLevelName;
    /*
    public bool doShadows = true;
    public string masterAnimSource = "mcTrueBones.wtf";
    public string selectionRectModel = "selectionRect.wtf";
    public string statusString = "";
    public int trackingLight = 3;
    public int objCycle = 2;
    public int teamSelect = 0;
    public int lastClick = 0;
    public float doubleClickInterval = 0.4f;

    public int cameraSpeed = 5;
    public float velocityClamp = 1.16f;
    public bool levelIsLit = false;
    public bool handleUDGAI = true;
    public bool trackLightEnabled = false;
    public string nextUDGLevel = null;
    public bool CAPSLOCKKEY = false;

    // -- this tracks the screamer with  light 5; 
    public GameObject[] screamerObject;
    public Vector3[] screamVector3;
    //ScreamX,ScreamY,gScreamZ = new Transform.position(); // nil,nil,nil; 
    public Vector3[] trackedPosition;

    // -- counts how long the gun flare is on; 
    public int gunLightCount = 0;

    public bool reloadLevel = false; //-- added 02 08 2009 ; 
    public float startPositionTimer = 0.0f;

    // -- added 02 26 2009 keyboard teleport; 
    public bool teleportTimerActive = false;
    public float teleportTime = 0.0f;

    // int because trackedObject is in a list somewhere?
    public int trackedObject = 3; //TrackedPosition.x,TrackedPosition.y,TrackedPosition.z = 0,0,0; 
    public bool displayLoading = false;
    public bool restartLevel = false;
    public float timeBeforeLoad = 0.0f;
    public int loadingTextBoxNum = 1;
    public string loadingBoxMessage = "";
    public float textualTimer = 0.0f;
    public int currentText = 1;
    public bool hideHUD = false;
    public bool wonThisLevel = false;
  
    // end of old igame3d udeadgame variables.
    */
    public char Q = '\"';



    // global text box update
    public  List<string> screenText = new List<string>();
    // these were those files
    // USING RTS SCRIPTS?!? ARGH Lets move what we can to UDG specifics!
    public List<string> RTSFUNCTIONS = new List<string>(new string[] { "Attributes", "Utilities", "Sounds", "Tasks", "Behaviours", "Senses", "Vehicles", "Weapons", "Orders", "Events", "AI", "Collisions", "Keyboard", "astar", "Game" });
    public List<string> UDGFUNCTIONS = new List<string>(new string[] { "Editor", "Lights", "AI", "Utilities", "Sounds", "Fallback", "Textboxes", "Keyboard", "Game", "Music" });
    // assign UDG entities behavior Now in its own UDG Folder
    public List<string> UDGENTITIES = new List<string>(new string[] { "Teams", "Emitters", "Human", "Zombie", "Building", "Gun", "Car", "GUI", "Window", "Door", "Scenery", "BloodPencil", "MeleeWeapons", "LevelExits", "Furniture", "Grenade", "Menu" });

    // List of old uDeadGame Lua files at Init // perhaps use again?
    public List<string> initFileList = new List<string>();

    //maybe this is a per level thing
    public GameObject[] furniture; 
    public GameObject[] windows;
    public GameObject[] boxes;
    public GameObject[] glass; 
    public GameObject[] door;
    public GameObject[] barricade; 

    private Transform mapHolder; //parent the map to this

    private Transform SpawnHolder; // this will parent spawns in the editor
    
    private List <Vector3> gridPositions = new List<Vector3>(); //Track all objects 3D position and avoid spawn same spot
    
  
    //
    //
    //
    void Awake()
    {
        mainGameInit = this;
        gameroot = Application.dataPath;
        gameRoot = gameroot; // fix them all at once later.
        levelPath = Application.dataPath + "/Data/Levels/";
        currentLevel = Application.loadedLevelName;
        InitialiseList();
        
            //Instantiate gameManager prefab
            Instantiate(mainGameManager);  
        
        
    }
    // Use this for initialization
    void Start()
    {
        // This Seems to creat dynamic, ie,whatever is in Resources/<path> load and become part of
        // the component
        print("Attaching Resources/Prefabs/zombies/ to this :" + this.name);
        object[] zombs = Resources.LoadAll("Prefabs/zombies");
            foreach (GameObject z in zombs)
            {
            string t = z.ToString();
            zombies.Add(z);    
             }
    }

    void listPrefabs(string tFolder)
    {
        char[] delimiterChars = { '\\' }; // { ' ', ',', '.', ':', '\t' };
        string myPath = Application.dataPath + "/Prefabs/" + tFolder + "/";
        DirectoryInfo dir = new DirectoryInfo(myPath);
        FileInfo[] listFiles = dir.GetFiles("*.prefab");
        foreach (FileInfo f in listFiles)
        {
            string t = f.ToString();
            string[] splits = t.Split(delimiterChars);
            foreach (string s in splits)
            {
                if (s.EndsWith("prefab"))
                {
                    string[] tfiles = s.Split('.');

                    print(tfiles[0]);
                }
            }
        }
    }
    void InitialiseList()
    {
        // gridPositions is for identifying spawn locations
        gridPositions.Clear(); // empty the list
        //old igame3d udeadgame lua initialisation scripts LIST, just a list for now.
        // probably need to    load script component on runtime
        foreach (string tFile in RTSFUNCTIONS) { initFileList.Add(gameroot + "Data/Scripts/RTS_Functions/RTS_" + tFile + ".lua"); }
        foreach (string tFile in UDGFUNCTIONS) { initFileList.Add(gameroot + "Data/Scripts/UDG_Functions/RTS_" + tFile + ".lua"); }
        foreach (string tFile in UDGENTITIES)
        { initFileList.Add(gameroot + "Data/Scripts/UDG_Functions/UDG_Entities/UDG_" + tFile + ".lua"); }
        // concatenate the list to an array string with delimiter
        print("OLD _Initalization Files List:\n\t" + String.Join("\n\t", initFileList.ToArray()));
    }

    // loads a game object prefab as a whole level
  public  void LoadLevel(GameObject gameObject)
    {
          //GameObject loadLevel =
              Instantiate(gameObject, new Vector3(0f, 0f, 0f), Quaternion.identity);
      //as GameObject;
    }
    // a little redundant?
    public void SetupScene()
    {
        LoadLevel(levelMaps[currentMap].gameObject);
    }

    

    // Update is called once per frame
    void Update()
    {

    }


    // Until I know better how to move the functions into other files and not cause a jam
    public void writeCurrentLevel(string currentLevel)
    {// Add some text to the file.
        using (StreamWriter currentLevelFile = new StreamWriter(gameroot + "/Data/Levels/UDG/" + currentLevel + ".txt"))
        {
            string thislevelout = "currentLevel =" + Q + currentLevel + Q + "\n";
            currentLevelFile.Write(thislevelout);
            if (System.IO.File.Exists(gameroot + "/Data/Levels/UDG/" + currentLevel + ".txt"))
            {
                print(thislevelout); //do stuff
            }
        }
    }// end writecurrentlevel

   // end UDG_INIT
}
