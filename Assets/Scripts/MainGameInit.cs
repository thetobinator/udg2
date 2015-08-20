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
    public static MainGameManager mainGameManager;
    //

    /*  08 17 2015 conflict with entityspawning
   
    public List<GameObject> zombies;
    public GameObject[] humans;
     */

    public List<GameObject> levelMaps;
    public GameObject loadedLevel;
    public int currentMap = 1; // 

    // UDGInit.lua to C# 08 17 2015
    // -- UDGInit with copies from RTS Init 01 03 09; 
    // 
    // 
    public string gameroot;//= Application.dataPath;
    public string levelPath;// = Application.dataPath  + "/Data/Levels/"; 
    public string gameRoot;// = gameroot; //break and fix this all at once ...later;
    public string currentLevel;// = Application.loadedLevelName;

    /*    // variables off for working clean inspector 08 17 2015
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
  

   
   // these were those files
   // USING RTS SCRIPTS?!? ARGH Lets move what we can to UDG specifics!
   public List<string> RTSFUNCTIONS = new List<string>(new string[] { "Attributes", "Utilities", "Sounds", "Tasks", "Behaviours", "Senses", "Vehicles", "Weapons", "Orders", "Events", "AI", "Collisions", "Keyboard", "astar", "Game" });
   public List<string> UDGFUNCTIONS = new List<string>(new string[] { "Editor", "Lights", "AI", "Utilities", "Sounds", "Fallback", "Textboxes", "Keyboard", "Game", "Music" });
   // assign UDG entities behavior Now in its own UDG Folder
   public List<string> UDGENTITIES = new List<string>(new string[] { "Teams", "Emitters", "Human", "Zombie", "Building", "Gun", "Car", "GUI", "Window", "Door", "Scenery", "BloodPencil", "MeleeWeapons", "LevelExits", "Furniture", "Grenade", "Menu" });
   // List of old uDeadGame Lua files at Init // perhaps use again?
   public List<string> initFileList = new List<string>();
    //end of old igame3d udeadgame variables.

   //maybe these gameobject arrays is a per level/character thing, maybe shoulds be List<string>s?
   public GameObject[] furniture; 
   public GameObject[] windows;
   public GameObject[] boxes;
   public GameObject[] glass; 
   public GameObject[] door;
   public GameObject[] barricade; 
   */

    // global text box update
    [Multiline]
    public List<string> screenText = new List<string>();
    private char Quote = '\"';
    private Transform mapHolder; //parent the map to this...eventually?
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
        /*
        // This Seems to creat dynamic, ie,whatever is in Resources/<path> load and become part of
        // the component
        print("Attaching Resources/Prefabs/zombies/ to this :" + this.name);
        object[] zombs = Resources.LoadAll("Prefabs/zombies");
            foreach (GameObject z in zombs)
            {
            string t = z.ToString();
            zombies.Add(z);    
             }
         */
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
   
    }


  /* no need to run this really 08 17 2015
   * void OldUDGLuaFilesList()
    { //old igame3d udeadgame lua initialisation scripts LIST, just a list for now.
        // probably need to    load script component on runtime
        foreach (string tFile in RTSFUNCTIONS) { initFileList.Add(gameroot + "Data/Scripts/RTS_Functions/RTS_" + tFile + ".lua"); }
        foreach (string tFile in UDGFUNCTIONS) { initFileList.Add(gameroot + "Data/Scripts/UDG_Functions/RTS_" + tFile + ".lua"); }
        foreach (string tFile in UDGENTITIES) { initFileList.Add(gameroot + "Data/Scripts/UDG_Functions/UDG_Entities/UDG_" + tFile + ".lua"); }
        // concatenate the list to an array string with delimiter
        print("OLD _Initalization Files List:\n\t" + String.Join("\n\t", initFileList.ToArray()));
    }
   */

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
            string thislevelout = "currentLevel =" + Quote + currentLevel + Quote + "\n";
            currentLevelFile.Write(thislevelout);
            if (System.IO.File.Exists(gameroot + "/Data/Levels/UDG/" + currentLevel + ".txt"))
            {
                print("writeCurrentLevel  =" +  thislevelout); //do stuff
            }
        }
    }// end writecurrentlevel




    //  -- init_all_objects from RTS.Attributes.lua came before the udg_init


    //function udg_init()
    /*
     *  void udg_init(){
	setObjectInfo(playerBox, IG3D_CAST_SHADOW, false)
	gObjects[playerBox].alive=false
	gObjects[playerBox].team=12
	-- further initialization
	randomWalking= walk_between_safe_places --replace the random walking
	standard_flee= flee_to_safe_place --replace flee
	
	gZombieBehaviour=zombie_hold_behaviour

	
	playerGunSound = assign_free_sound_emitter()
	setSound_emitterInfo(playerGunSound, IG3D_SAMPLE, "Data/Sounds/rifle.ogg")
	setSound_emitterInfo(playerGunSound, IG3D_LOOP, false)--true
	
	local i
	for i=1,#gObjects,1 do
		setObjectInfo(gObjects[i].cObj, IG3D_BONE_COLL, false)--no bone colls for heavens sake!
		if gObjects[i].team==0 or gObjects[i].team==1 then
			setObjectInfo(gObjects[i].cObj, IG3D_SHAPE, ig3d_sphere)
			setObjectInfo(gObjects[i].cObj, IG3D_COLLBOX_MULTIPLICATORS, 0.5,1,1)--brave new world!
		end
		
		if gObjects[i].team==1 and gObjects[i].alive then
			gObjects[i].behaviour= gZombieBehaviour
			gObjects[i].noise=0.2
			--gObjects[i].meleeDistance=2
		end
		
		if gObjects[i].team==0 then
			--dead on a papercut
			--gObjects[i].behaviour=stand_and_cry
			if string.sub(tCurrentLevel,-4,-1) ~= "Hard" then 
			gObjects[i].behaviour= idle_until_zombie_in_sight
			end
			if isFemale(gObjects[i]) then
				--gObjects[i].health=1
			end
			gObjects[i].health=1
			gObjects[i].voiceSndEmt=assign_free_sound_emitter()--people need to scream etc.
			
		end
		setSound_emitterInfo(gObjects[i].voiceSndEmt, IG3D_VOLUME, 80)--3
		setSound_emitterInfo(gObjects[i].noiseSndEmt, IG3D_VOLUME, 80)--3
	end
	
	setObjectInfo(playerBox, IG3D_COLLBOX_MULTIPLICATORS, 1,1,1)--brave new world!
	setObjectInfo(playerBox, IG3D_SHAPE, ig3d_sphere)--sphere
	
	xa,ya,za=getCameraInfo(IG3D_ROTATION)
	setLightInfo(2, IG3D_POSITION, 0,5,7,1)
	setCameraInfo(IG3D_POSITION, 0,15,0)
	setObjectInfo(playerBox, IG3D_POSITION, getCameraInfo(IG3D_POSITION))
	
	--setSceneInfo(IG3D_RECEIVE_SHADOW, gDoShadows)--do we cast shadows?

	--ignore reset
	for i=1,#gObjectWTFs,1 do
	setObjectInfo(gObjects[i].cObj, IG3D_POSITION, gObjectPositions[i][1],gObjectPositions[i][2],gObjectPositions[i][3])
end
	

	
	--setObjectInfo(playerBox, IG3D_IGNORE_OBJECT_COLLS, true)
	crosshair=gObjects[get(ig3d_object, "crosshair1")]
	crosshair.alive=false
	setObjectInfo(crosshair.cObj, IG3D_GRAVITY_MULTIPLIER, 0)
	crosshair.team=12
	setObjectInfo(crosshair.cObj, IG3D_POSITION, 0,0,-0.1)
	bloodonfloor=get(ig3d_particle_emitter, "bloodonfloor")
end

-----------------------------------------------------------
-- if not game mode don't do all this stuff
if ig3d_GetMode__i() == 4 then 	
HumansWithGuns()
HumansWithMeleeWeapons()
SoldiersWithGrenades()
--set hit sound and level of depth
allEntitySoundCollLOD() -- see RTS_Attributes.lua
setupAStarGrid()
end

-------------------------------------------------------------
-- Copied from FPS Init 
FPSgun = get(ig3d_particle_emitter,"fpsgun")
playerBox = get(ig3d_object, "playerbox")
--

setObjectInfo(playerBox,IG3D_POSITION,0.0,1,0.0)
setObjectInfo(playerBox,IG3D_SIZE,50,50,50)
looker=get(ig3d_bone, playerBox, "None")
setBoneInfo(looker, IG3D_ENABLED, false)
first=true
if FPSgun ==nil then print("Missing \"fpsgun\" emitter\n") end
if playerBox ==nil then print("Missing \"playerbox\" object\n") end
setSceneInfo(IG3D_MOUSE_VIEW, true, 1)
setObjectInfo(playerBox, IG3D_VECTOR_CONSTRAINT, 0,1,0)
setObjectInfo(playerBox, IG3D_GRAVITY_MULTIPLIER, 0.1)
setObjectInfo(playerBox, IG3D_IGNORE_COLL, true, ig3d_particle_emitter, FPSgun)
setParticle_emitterInfo(FPSgun, IG3D_IGNORE_COLL, true, ig3d_particle_emitter, FPSgun)--dont collide bullets with bullets
setObjectInfo(playerBox, IG3D_SHAPE, ig3d_sphere)
xa,ya,za=getCameraInfo(IG3D_ROTATION)
setLightInfo(2, IG3D_POSITION, 0,5,7,1)

-------------------------------------------------------------		



if ig3d_GetMode__i() == 4 then
--init phase
udg_init()
udg_place_objects_randomly()--haha!
udg_create_safe_places_randomly()--ho ho ho

rts_update_living_and_dead_counts()
end  -- if not game mode don't do all this stuff
     */
    // end UDG_INIT.lua
}
