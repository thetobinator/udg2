using UnityEngine;
using System;
using System.Collections;
using UMA;
using System.IO;
using UMA.PoseTools;


// A practical guide to UMA Part 14b Using the "Asset" format https://youtu.be/Q6bLMusuhbo?t=8m13s
#if UNITY_EDITOR
using UnityEditor;
#endif



public class UMAMaker1_Police: MonoBehaviour {
    //udeadgame UMA Maker for Police



    /* All UMA content that provides a mesh is a slot. Slots are basically containers
       holding all necessary data to be combined with the rest of an UMA avatar */
    public SlotLibrary slotLibrary;

    /*Each slot requires at least one overlay set but usually receives a list of them. 
      Overlays carries all the necessary textures to generate the final material(s) 
      and might have extra information on how they are mapped.*/
    public OverlayLibrary overlayLibrary;

    /*Each RaceData on the race library provides a prefab with all shared scripts, avatar data and etc, one or more 
      DNAConvertes and a list of the name of bones that require being updated.*/
    public RaceLibrary raceLibrary;
    public UMAGeneratorBase generator; // creates the characters
    public RuntimeAnimatorController animController;

    private UMADynamicAvatar umaDynamicAvatar; // must be on a gameObject to display characters
    private UMAData umaData; // all the uma slots/overlays/dna values
    private UMADnaHumanoid umaDna; // list of morphs
    private UMADnaTutorial umaTutorialDNA; // Extra DNA customizable values

    private int numberOfSlots = 20; // could be as big as we like
                                    //

    #region UMA Customizable Variables
    private string bloodName = "";
    private string lastBloodName = "";
    private bool lastHairState = false;
    private Color lastHairColor;
    private bool lastChestEmblemState = false;
    private bool lastVestState = false;
    private Color lastVestColor = Color.white;
    private Color lastTrenchcoatColor;
    private bool lastTrenchcoatState = false;
    private int footSlot = 6;
    private bool lastShoeState = true;
    private bool lastHatState = true;
    private bool lastBadgeState = true;
    private bool lastShirtPoliceState = true;
    private bool lastLegsPoliceState = true;
    private bool lastLegsNightStickState = true;
    private bool lastHandNightStickState = true;
    
    private bool lastBlood1 = true;
    private bool lastbloodBreastR = true;
    private bool lastbloodChest = true;
    private bool lastbloodGuts1 = true;
    private bool lastbloodShoulderBackL = true;
    private bool lastbloodShoulderBackR = true;
    private bool lastbloodShoulderR = true;
    private bool lastbloodWhatR = true;

    // this creates disclosure arrow

    [System.Serializable]
        public class Injury
    {       
        public bool Blood1 = false;  
        public bool bloodBreastR = false;
        public bool bloodChest = false;
        public bool bloodGuts1 = false;
        public bool bloodShoulderBackL = false;
        public bool bloodShoulderBackR = false;
        public bool bloodShoulderR = false;
        public bool bloodWhatR = false;      
    }

    [System.Serializable]
    public class Head
    {
        public bool hatState = false;
        public bool hairState = false;
        public Color hairColor;
        //slider 
        [Range(-1.0f, 1.0f)]
        public float happy = 0f;
    }

    [System.Serializable]
    public class Torso
    {
        public bool chestEmblemState = false;
        public bool vestState = false;
        public Color vestColor = Color.white;
        public bool badgeState = false;
        public bool shirtPoliceState = false;
        public bool trenchcoatState = false;
        public Color trenchcoatColor;
    }

    [System.Serializable]
    public class CustomUMA
    {      
        //slider
        [Range(0.0f, 1.0f)]
    public float bodyMass = 0.5f;

    // part 10b of practical guide to UMA https://youtu.be/jyboNBxVTQY?t=50s
    //slider
   /* [Range(0, 8)]
    public int injury = 0;*/

    // practical guide to uma part 17 expression player https://youtu.be/nJI-kUYYuWE?t=11m36s
    
        //Practical Guide to UMA part 1 Runtime Slot Changes. https://youtu.be/vB-tGGcFxDI?t=5m30s

        public Head head;
        public Torso torso;
        public Injury injury;
 

    //TrenchCoat UMA and Blender Content creation https://youtu.be/_c8lrr-BOnM

    public bool shoeState = false; 

    
    public bool legsPoliceState = false;
    public bool legsNightStickState = false;
        public bool handNightStickState = false;
 

    //Practical Guide To UMA part 14 https://youtu.be/ZKRQ4wzp0ac
    public string SaveString = "";
    public bool saveText;
    public bool loadText;
    public bool saveAsset;
    public bool loadAsset;
    
    }

    public CustomUMA myCustomUMA;
    #endregion
   
    //practical guide to uma part 17 Expression player https://youtu.be/nJI-kUYYuWE?t=2m27s

    //public UMAExpressionPlayer expressionPlayer;

    void Awake()
    {
        slotLibrary = GameObject.Find("SlotLibrary").GetComponent<SlotLibrary>();
        overlayLibrary = GameObject.Find("OverlayLibrary").GetComponent<OverlayLibrary>();
        raceLibrary = GameObject.Find("RaceLibrary").GetComponent<RaceLibrary>();
        generator = GameObject.Find("UMAGenerator").GetComponent<UMAGenerator>();
    }

    // Part 4 of practical guide to UMA https://youtu.be/KZpvgiAdD9c
    void Start()
    {  
        GenerateUMA(); 
    }

    void Update()
    {
        if (myCustomUMA.bodyMass != umaDna.upperMuscle)
        {
            SetBodyMass(myCustomUMA.bodyMass);
            umaData.isShapeDirty = true;
            umaData.Dirty();
        }

        /* Expression play broke
    if (happy != expressionPlayer.midBrowUp_Down)
     {
         expressionPlayer.midBrowUp_Down = happy;
         expressionPlayer.leftMouthSmile_Frown = happy;
         expressionPlayer.rightMouthSmile_Frown = happy;
     }*/

        // part 10b of practical guid to UMA https://youtu.be/jyboNBxVTQY?t=1m6s
        if (myCustomUMA.torso.vestState && !lastVestState)
        {
            lastVestState = true;
            AddOverlay(3, "SA_Tee", myCustomUMA.torso.vestColor);     
            umaData.isTextureDirty = true;
            umaData.Dirty();
        }

        if (!myCustomUMA.torso.vestState && lastVestState)
        {
            lastVestState = false;
            RemoveOverlay(3, "SA_Tee");       
            umaData.isTextureDirty = true;
            umaData.Dirty();
        }


        if (myCustomUMA.torso.vestColor != lastVestColor && myCustomUMA.torso.vestState)
        {
            lastVestColor = myCustomUMA.torso.vestColor;
            ColorOverlay(3, "SA_Tee", myCustomUMA.torso.vestColor);
            umaData.isTextureDirty = true;
            umaData.Dirty();
        }

        if (myCustomUMA.torso.chestEmblemState && !lastChestEmblemState)
        {
            lastChestEmblemState = true;
            AddOverlay(3, "SA_Logo");
            umaData.isTextureDirty = true;
            umaData.Dirty();
        }

        if (!myCustomUMA.torso.chestEmblemState && lastChestEmblemState)
        {
            lastChestEmblemState = false;    
            RemoveOverlay(3, "SA_Logo");
            umaData.isTextureDirty = true;
            umaData.Dirty();
        }

        //bloodName = "";
        if (myCustomUMA.injury.Blood1 && !lastBlood1) { bloodName = "blood"; lastBlood1 = myCustomUMA.injury.Blood1; }
        if (myCustomUMA.injury.bloodBreastR && !lastbloodBreastR) { bloodName = "bloodBreastR"; lastbloodBreastR = myCustomUMA.injury.bloodBreastR; }
        if (myCustomUMA.injury.bloodChest && !lastbloodChest) { bloodName = "bloodChest"; lastbloodChest = myCustomUMA.injury.bloodChest; }
        if (myCustomUMA.injury.bloodGuts1 && !lastbloodGuts1) { bloodName = "bloodGuts1"; lastbloodGuts1 = myCustomUMA.injury.bloodGuts1; }
        if (myCustomUMA.injury.bloodShoulderBackL && !lastbloodShoulderBackL) { bloodName = "bloodShoulderBackL"; lastbloodShoulderBackL = myCustomUMA.injury.bloodShoulderBackL; }
        if (myCustomUMA.injury.bloodShoulderBackR && !lastbloodShoulderBackR) { bloodName = "bloodShoulderBackR"; lastbloodShoulderBackR = myCustomUMA.injury.bloodShoulderBackR; }
        if (myCustomUMA.injury.bloodShoulderR && !lastbloodShoulderR) { bloodName = "bloodShoulderR"; lastbloodShoulderR = myCustomUMA.injury.bloodShoulderR; }
        if (myCustomUMA.injury.bloodWhatR && !lastbloodWhatR) { bloodName = "bloodWhatR"; lastbloodWhatR = myCustomUMA.injury.bloodWhatR; }

        if (!myCustomUMA.injury.Blood1 && lastBlood1) { RemoveOverlay(3, "Blood1"); lastBlood1 = myCustomUMA.injury.Blood1; }
        if (!myCustomUMA.injury.bloodBreastR && lastbloodBreastR) { RemoveOverlay(3, "bloodBreastR"); lastbloodBreastR = myCustomUMA.injury.bloodBreastR; }
        if (!myCustomUMA.injury.bloodChest && lastbloodChest) { RemoveOverlay(3, "bloodChest"); lastbloodChest = myCustomUMA.injury.bloodChest; }
        if (!myCustomUMA.injury.bloodGuts1 && lastbloodGuts1) { RemoveOverlay(3, "bloodGuts1"); lastbloodGuts1 = myCustomUMA.injury.bloodGuts1; }
        if (!myCustomUMA.injury.bloodShoulderBackL && lastbloodShoulderBackL) { RemoveOverlay(3, "bloodShoulderBackL"); lastbloodShoulderBackL = myCustomUMA.injury.bloodShoulderBackL; }
        if (!myCustomUMA.injury.bloodShoulderBackR && lastbloodShoulderBackR) { RemoveOverlay(3, "bloodShoulderBackR"); lastbloodShoulderBackR = myCustomUMA.injury.bloodShoulderBackR; }
        if (!myCustomUMA.injury.bloodShoulderR && lastbloodShoulderR) { RemoveOverlay(3, "bloodShoulderR"); lastbloodShoulderR = myCustomUMA.injury.bloodShoulderR; }
        if (!myCustomUMA.injury.bloodWhatR && lastbloodWhatR) { RemoveOverlay(3, "bloodWhatR"); lastbloodWhatR = myCustomUMA.injury.bloodWhatR; }

        if (bloodName != lastBloodName)
        {
           // RemoveOverlay(3, lastBloodName);
            lastBloodName = bloodName;
            if (bloodName != "")
            {
                AddOverlay(3, bloodName);
            }
            DirtyUMAUpdate(umaData);
        }
        
        

        // practical guide to uma part 11 Change Slots At Runtime. https://youtu.be/vB-tGGcFxDI?t=5m30s

        if (myCustomUMA.head.hairState && !lastHairState)
        {
            lastHairState = myCustomUMA.head.hairState;
            SetSlot(7, "M_Hair_Shaggy");
            AddOverlay(7,"M_Hair_Shaggy", myCustomUMA.head.hairColor);
            DirtyUMAUpdate(umaData);
        }

        if (!myCustomUMA.head.hairState && lastHairState)
        {
            lastHairState = myCustomUMA.head.hairState;
            RemoveSlot(7);
            DirtyUMAUpdate(umaData);
        }

        //change haircolor at runtime.
        if (myCustomUMA.head.hairColor != lastHairColor && myCustomUMA.head.hairState)
        {           
            lastHairColor = myCustomUMA.head.hairColor;
            ColorOverlay(7, "M_Hair_Shaggy", myCustomUMA.head.hairColor);
            umaData.isTextureDirty = true;
            umaData.Dirty();           
        }


        if (myCustomUMA.shoeState && !lastShoeState)
        {
            lastShoeState = myCustomUMA.shoeState;
            SetSlot(footSlot, "Shoes");
           AddOverlay(footSlot, "Shoes");
            DirtyUMAUpdate(umaData);
        }

        if (!myCustomUMA.shoeState && lastShoeState)
        {
            lastShoeState = myCustomUMA.shoeState;
            //RemoveSlot(footSlot);
            SetSlot(footSlot, "MaleFeet");
            LinkOverlay(footSlot, 3);
            //AddOverlay(footSlot, "Shoes");
            DirtyUMAUpdate(umaData);
        }



        //practical guide to uma part 14 https://youtu.be/ZKRQ4wzp0ac
        if (myCustomUMA.saveText)
        {
            myCustomUMA.saveText = false;
            SaveText();
        }
        if (myCustomUMA.loadText)
        {
            myCustomUMA.loadText = false;
            LoadText();   
        }

        // practical guide to UMA part 14b https://youtu.be/Q6bLMusuhbo?t=10m18s
        if (myCustomUMA.saveAsset)
        {
            myCustomUMA.saveAsset = false;
            SaveAsset();
        }
        if (myCustomUMA.loadAsset)
        {
            myCustomUMA.loadAsset = false;
            LoadAsset();
        }



        //TrenchCoat UMA and Blender Content creation https://youtu.be/_c8lrr-BOnM
        if (myCustomUMA.torso.trenchcoatState && !lastTrenchcoatState)
        {
            lastTrenchcoatState = myCustomUMA.torso.trenchcoatState;
            SetSlot(8, "UMA_Human_Male_Trenchcoat");
            AddOverlay(8, "UMA_Human_Male_Trenchcoat", myCustomUMA.torso.trenchcoatColor);
            DirtyUMAUpdate(umaData);
        }


        if (!myCustomUMA.torso.trenchcoatState && lastTrenchcoatState)
        {
            lastTrenchcoatState = myCustomUMA.torso.trenchcoatState;
            RemoveSlot(8);
            DirtyUMAUpdate(umaData);
        }

        //change trenchcolor at runtime.
        if (myCustomUMA.torso.trenchcoatColor != lastTrenchcoatColor && myCustomUMA.torso.trenchcoatState)
        {
            lastTrenchcoatColor = myCustomUMA.torso.trenchcoatColor;
            ColorOverlay(8, "UMA_Human_Male_Trenchcoat", myCustomUMA.torso.trenchcoatColor);
            umaData.isTextureDirty = true;
            umaData.Dirty();
        }


        //TrenchCoat UMA and Blender Content creation https://youtu.be/_c8lrr-BOnM
        if (myCustomUMA.head.hatState && !lastHatState)
        {
            Debug.Log(string.Format("Hatstate {0}", myCustomUMA.head.hatState));
            lastHatState = myCustomUMA.head.hatState;
            SetSlot(9, "HumanMaleHatPolice");

            Debug.Log("Slot set attempting overlay");
            AddOverlay(9, "HumanMaleHatPolice");
            DirtyUMAUpdate(umaData);
        }


        if (!myCustomUMA.head.hatState && lastHatState)
        {
            lastHatState = myCustomUMA.head.hatState;
          //  Debug.Log(string.Format("Hatstate {0}", hatState));
            RemoveSlot(9);
            DirtyUMAUpdate(umaData);
        }

        //badge
        if (myCustomUMA.torso.badgeState && !lastBadgeState)
        {
            lastBadgeState = myCustomUMA.torso.badgeState;
            SetSlot(10, "HumanMaleBadgePolice");      
            AddOverlay(10, "HumanMaleBadge", Color.yellow);
            DirtyUMAUpdate(umaData);
        }


        if (!myCustomUMA.torso.badgeState && lastBadgeState)
        {
            lastBadgeState = myCustomUMA.torso.badgeState;       
            RemoveSlot(10);
            DirtyUMAUpdate(umaData);
        }


        //police Shirt
        if (myCustomUMA.torso.shirtPoliceState && !lastShirtPoliceState && myCustomUMA.legsPoliceState)
        {
            lastShirtPoliceState = myCustomUMA.torso.shirtPoliceState;
            SetSlot(3, "HumanMaleShirtPoliceWPants");
            AddOverlay(3, "HumanMaleShirtPolice");
            SetSlot(5, "HumanMaleLegsPolice");
            AddOverlay(5, "HumanMaleLegsPolice");
            RemoveSlot(footSlot);
            SetSlot(12, "HumanMaleArms"); // mesh
            AddOverlay(12, "HumanMaleArms");
            DirtyUMAUpdate(umaData);
        }

        if (myCustomUMA.torso.shirtPoliceState && !lastShirtPoliceState && !myCustomUMA.legsPoliceState)
        {
            
            lastShirtPoliceState = myCustomUMA.torso.shirtPoliceState;
            SetSlot(3, "HumanMaleShirtPoliceNoPants");
            AddOverlay(3, "HumanMaleShirtPolice");
            SetSlot(5, "MaleLegs");
            LinkOverlay(5, 2);
            AddOverlay(5, "MaleUnderwear01");
            LinkOverlay(footSlot,2);
            SetSlot(12, "HumanMaleArms"); // mesh
            LinkOverlay(12, 2);
            DirtyUMAUpdate(umaData);
        }


  
        if (!myCustomUMA.torso.shirtPoliceState && lastShirtPoliceState && !myCustomUMA.legsPoliceState)
        {
            lastShirtPoliceState = myCustomUMA.torso.shirtPoliceState;
            SetSlot(3, "MaleTorso");
            AddOverlay(3, "MaleBody02");
            SetSlot(5, "MaleLegs");
            LinkOverlay(5, 3);
            AddOverlay(5, "MaleUnderwear01");
            SetSlot(footSlot, "MaleFeet");
            LinkOverlay(footSlot, 3);
            DirtyUMAUpdate(umaData);
        }


        //police Legs
        if (myCustomUMA.legsPoliceState && !lastLegsPoliceState )
        {
            lastLegsPoliceState = myCustomUMA.legsPoliceState;     
            SetSlot(5, "HumanMaleLegsPoliceNoNightStick");
            AddOverlay(5, "HumanMaleLegsPolice");
            SetSlot(13, "HumanMaleLegsPoliceNightStick");
            RemoveSlot(footSlot);
            DirtyUMAUpdate(umaData);
        }

        if (!myCustomUMA.legsPoliceState && lastLegsPoliceState)
        {
            lastLegsPoliceState = myCustomUMA.legsPoliceState;    
            SetSlot(5, "MaleLegs");
            LinkOverlay(5, 3);
            AddOverlay(5, "MaleUnderwear01");
            SetSlot(footSlot, "MaleFeet");
            LinkOverlay(footSlot, 3);
            RemoveSlot(13);
            DirtyUMAUpdate(umaData);
        }

        if (myCustomUMA.legsNightStickState && !lastLegsNightStickState)
        {
            lastLegsNightStickState = myCustomUMA.legsNightStickState;
            SetSlot(13, "HumanMaleLegsPoliceNightStick");
            AddOverlay(13, "NightstickUV");
            DirtyUMAUpdate(umaData);
        }

        if (!myCustomUMA.legsNightStickState && lastLegsNightStickState)
        {
            lastLegsNightStickState = myCustomUMA.legsNightStickState;
            RemoveSlot(13);
            DirtyUMAUpdate(umaData);
        }

        if (myCustomUMA.handNightStickState && !lastHandNightStickState)
        {
            lastHandNightStickState = myCustomUMA.handNightStickState;
            SetSlot(4, "HumanMaleLeftHandNightStick");
            LinkOverlay(4, 3);
            DirtyUMAUpdate(umaData);
        }
        if (!myCustomUMA.handNightStickState && lastHandNightStickState)
        {
            lastHandNightStickState = myCustomUMA.handNightStickState;
            SetSlot(4,"MaleHands");
            LinkOverlay(4, 3);
            DirtyUMAUpdate(umaData);
        }
    }

    void DirtyUMAUpdate(UMAData umaData)
    {
        umaData.isMeshDirty = true; //processor expensive use all 'dirty' commands to regenerate mesh.
        umaData.isTextureDirty = true;
        umaData.isShapeDirty = true;
        umaData.Dirty();
    }

    void GenerateUMA()
    {
        // Create a new game object and add UMA components to it
        GameObject GO = new GameObject("MyUMA");


        // parent the new uma into the host game object
        GO.transform.parent = this.gameObject.transform;
        GO.transform.localPosition = Vector3.zero;
        GO.transform.localRotation = Quaternion.identity;

        GO.AddComponent(typeof(NavMeshAgent));
        GO.AddComponent(typeof(CapsuleCollider));
        var goCol = GO.GetComponent<CapsuleCollider>();
        goCol.center = new Vector3(0f, 0.78f, 0f);
        goCol.height = 1.7f;
        goCol.radius = 0.2f;

        umaDynamicAvatar = GO.AddComponent<UMADynamicAvatar>();

        // Initialise Avatar and grab a reference to it's data component
        umaDynamicAvatar.Initialize();
        umaData = umaDynamicAvatar.umaData;
       
        //practical guide to uma part 15 uma events https://youtu.be/_k-SZRCvgIk?t=8m20s
        umaData.OnCharacterCreated += CharacterCreatedCallback;

        // Attach our generator
        umaDynamicAvatar.umaGenerator = generator;
        umaData.umaGenerator = generator;

        // Set up slot Array
        umaData.umaRecipe.slotDataList = new SlotData[numberOfSlots];

        // Set up our Morph reference
        umaDna = new UMADnaHumanoid();
        umaTutorialDNA = new UMADnaTutorial();
        umaData.umaRecipe.AddDna(umaDna);
        umaData.umaRecipe.AddDna(umaTutorialDNA);

        // Grab a reference to our recipe
        // var umaRecipe = umaDynamicAvatar.umaData.umaRecipe; //moved to subroutine MakeMale

        // >>> This is where the fun will happen according to Secret Anorak <<<<
        CreateMale();        

        // dynamic animation controller 
        umaDynamicAvatar.animationController = animController;

        // Generate Our UMA
        umaDynamicAvatar.UpdateNewRace();
    
    }

    // Practical Guide to UMA part 5 https://youtu.be/N-NlNJv1ESE
    void CreateMale()
    {
        // Grab a reference to our recipe
        var umaRecipe = umaDynamicAvatar.umaData.umaRecipe;
        umaRecipe.SetRace(raceLibrary.GetRace("HumanMale"));

        // practical UMA part 6 https://youtu.be/cpstGcNN5e0
        // seven basic slots
        umaData.umaRecipe.slotDataList[0] = slotLibrary.InstantiateSlot("MaleEyes"); // mesh
        AddOverlay(0, "EyeOverlay"); // texture

        umaData.umaRecipe.slotDataList[1] = slotLibrary.InstantiateSlot("MaleInnerMouth"); // mesh
        AddOverlay(1, "InnerMouth"); // texture

        umaData.umaRecipe.slotDataList[2] = slotLibrary.InstantiateSlot("MaleFace"); // mesh
        AddOverlay(2, "MaleHead02"); // texture

        umaData.umaRecipe.slotDataList[3] = slotLibrary.InstantiateSlot("MaleTorso"); // mesh
        AddOverlay(3, "MaleBody02"); // texture

        umaData.umaRecipe.slotDataList[4] = slotLibrary.InstantiateSlot("MaleHands"); // mesh
        LinkOverlay(4, 3);

        umaData.umaRecipe.slotDataList[5] = slotLibrary.InstantiateSlot("MaleLegs"); // mesh
        LinkOverlay(5, 3);
    
        //footSlot = 6
        umaData.umaRecipe.slotDataList[footSlot] = slotLibrary.InstantiateSlot("MaleFeet"); // mesh
        LinkOverlay(footSlot, 3); 

        // add underwear
        umaData.umaRecipe.slotDataList[3].AddOverlay(overlayLibrary.InstantiateOverlay("MaleUnderwear01"));

      // Practical Guide to UMA part 7 https://youtu.be/Czg1U-hlXn0
        //add eyebrow and set color
        umaData.umaRecipe.slotDataList[2].AddOverlay(overlayLibrary.InstantiateOverlay("MaleEyebrow01", Color.black));

        #region Tutorials_for_overlays_umaDna_custom_content
        // Practical Guide to UMA part 10 https://youtu.be/vNqBg-IZuQc?t=6m4s
        //  AddOverlay(3,"SA_Tee"); // t-shirt
        // AddOverlay(3, "SA_chestEmblem");// t-shirt chestEmblem
        // adding SA_Tee at runtime after lesson part 10b
        // Practical Guide to UMA part 12 https://youtu.be/JgHYjUZ4yWI?t=7m8s
        //SetSlot(footSlot, "Shoes");
        //AddOverlay(footSlot, "Shoes");
        // Modify umaDNA via script
        // max head room
        // umaDna.headSize = 1f;
        // UMA 2.0 & Blender 2.76b Custom Content Creation Part 5: Getting our Model/Slot in Unity https://youtu.be/NAw7z_x8Mos?t=11m
        //umaData.umaRecipe.slotDataList[8] = slotLibrary.InstantiateSlot("UMA_Human_Male_Trenchcoat"); // mesh 
        #endregion

    }

    ///////////////   UMA Morph Routines   /////////////
    // Practical Guide to UMA Part 9 Changing Shape at Runtime  https://youtu.be/FaPPR7hdZy8
    void SetBodyMass(float mass)
    {
        umaDna.upperMuscle = mass;
        umaDna.upperWeight = mass;
        umaDna.lowerMuscle = mass;
        umaDna.lowerWeight = mass;
        umaDna.armWidth = mass;
        umaDna.forearmWidth = mass;
    }
    ///////////////   Overlay Helpers   ///////////////

    // Practical Guide to UMA part 10 https://youtu.be/vNqBg-IZuQc?t=7m4s
    void AddOverlay(int slot, string overlayName)
    {
        umaData.umaRecipe.slotDataList[slot].AddOverlay(overlayLibrary.InstantiateOverlay(overlayName)); 
    }

    //overload
    void AddOverlay(int slot, string overlayName, Color color)
    {
        umaData.umaRecipe.slotDataList[slot].AddOverlay(overlayLibrary.InstantiateOverlay(overlayName, color));
    }

    void LinkOverlay(int slotNumber, int slotToLink)
    {
        umaData.umaRecipe.slotDataList[slotNumber].SetOverlayList(umaData.umaRecipe.slotDataList[slotToLink].GetOverlayList());
    }

    void RemoveOverlay( int slotNumber, string overlayName)
    {
        umaData.umaRecipe.slotDataList[slotNumber].RemoveOverlay(overlayName);
    }

    void ColorOverlay(int slotNumber, string overlayName, Color color)
    {
        umaData.umaRecipe.slotDataList[slotNumber].SetOverlayColor(color, overlayName);
    }

    ///////////////    Slot Helpers      ///////////////
    void SetSlot(int slotnumber, string SlotName)
    {
        umaData.umaRecipe.slotDataList[slotnumber] = slotLibrary.InstantiateSlot(SlotName);
    }
    void RemoveSlot(int slotNumber)
    {
        umaData.umaRecipe.slotDataList[slotNumber] = null;
    }


    //practical guide to uma part 14 https://youtu.be/ZKRQ4wzp0ac
    ////////////////// Load and Save //////////////////////

    void SaveText()
    {
        //Generate UMA String
        UMATextRecipe recipe = ScriptableObject.CreateInstance<UMATextRecipe>();
        recipe.Save(umaDynamicAvatar.umaData.umaRecipe, umaDynamicAvatar.context);
        myCustomUMA.SaveString = recipe.recipeString;
        Destroy(recipe);

        //Save string to text file
        string fileName = "Assets/UMASavedTextFile.txt";
            StreamWriter stream = File.CreateText(fileName);
        stream.WriteLine(myCustomUMA.SaveString);
        stream.Close();
    }

    void LoadText()
    {
        //Save string to text file
        string fileName = "Assets/UMASavedTextFile.txt";
            StreamReader stream = File.OpenText(fileName);
        myCustomUMA.SaveString = stream.ReadLine();
        stream.Close();

        //Generate UMA String
        UMATextRecipe recipe = ScriptableObject.CreateInstance<UMATextRecipe>();        
        recipe.recipeString = myCustomUMA.SaveString;
        umaDynamicAvatar.Load(recipe);
        Destroy(recipe);
    }

    // practical guide to UMA part 14b https://youtu.be/Q6bLMusuhbo?t=10m18s
    void LoadAsset()
    {
        UMARecipeBase recipe = Resources.Load("Assets/UMASavedAsAsset.asset") as UMARecipeBase;
        umaDynamicAvatar.Load(recipe);
    }

    void SaveAsset()
    {
        #if UNITY_EDITOR
        var asset = ScriptableObject.CreateInstance<UMATextRecipe>();
        
        asset.Save(umaDynamicAvatar.umaData.umaRecipe, umaDynamicAvatar.context);
        AssetDatabase.CreateAsset(asset, "Assets/UMASavedAsAsset.asset");
        AssetDatabase.SaveAssets();
        #endif
    }

    //practical guide to UMA part 15 intercepting uma events https://youtu.be/_k-SZRCvgIk?t=4m17s
    void CharacterCreatedCallback(UMAData umaData)
    {
        //GameObject myUMA = GameObject.Find("myUMA");
        //Debug.Log("UMA_Created");
       

        //attach scripts after creation
        
      umaData.gameObject.transform.position =  new Vector3(this.transform.position.x, this.transform.position.y, this.transform.position.z);
       Debug.Log("positioned Police!");
        umaData.gameObject.tag = "Zombie";
        umaData.gameObject.AddComponent<HealthComponent>();
        umaData.gameObject.AddComponent<ZombieBehavior>();
        // hand collision to prevent slide through
        LeftFistBox();
        RightFistBox();
        SpawnStaff();
        SpawnPistolUO();
        #region    ---------------------   Expression Player Callback broken
        /*
        // A Practical Guide To UMA - Part 17 - Using the Expression Player  https://youtu.be/nJI-kUYYuWE
        UMAExpressionSet expressionSet = umaData.umaRecipe.raceData.expressionSet;
        expressionPlayer = umaData.gameObject.AddComponent<UMAExpressionPlayer>();
        expressionPlayer.expressionSet = expressionSet;
        expressionPlayer.umaData = umaData;
        expressionPlayer.Initialize();
        // automated expressions to look life like
        expressionPlayer.enableBlinking = true;
        expressionPlayer.enableSaccades = true;*/
        #endregion
    }

    
    //practical guide to UMA part 16 attaching props https://youtu.be/0Iy_G_IufKU?t=6m48s
    void SpawnStaff()
    {
     // right handed staff/sword/stick wielders will stab themselvse while running.
        Transform hand = umaData.skeleton.GetBoneGameObject(UMASkeleton.StringToHash("LeftHand")).transform﻿; //eli curtz style.
        GameObject staff = GameObject.CreatePrimitive(PrimitiveType.Cube);
     
        staff.name = "staff";
        staff.tag = "Staff";
        staff.transform.SetParent(hand);   
        MeshRenderer cubeRenderer = staff.GetComponent<MeshRenderer>();
        Color red = new Color(255, 0, 0, 1);
        cubeRenderer.material.color = red;
        staff.transform.localPosition = Vector3.zero;
        staff.transform.localPosition = new Vector3(-0.117f, -0.543f, -0.017f);
        staff.transform.localRotation = Quaternion.Euler(new Vector3(0.0f, 344.0f, 0.0f));
        staff.transform.localScale = new Vector3(0.02f, 1.0f, 0.02f);
        staff.AddComponent(typeof(BoxCollider));
    }
    void SpawnPistolUO()
    {

        Transform hand = umaData.skeleton.GetBoneGameObject(UMASkeleton.StringToHash("LeftHandFinger03_01")).transform﻿; //eli curtz style.

        if (umaData.transform != null)
        {  
            GameObject pistol = Instantiate(Resources.Load("Prefabs/weapons/Pistol1_uo", typeof(GameObject))) as GameObject;  
            pistol.transform.parent = umaData.transform;
            pistol.transform.Translate(umaData.transform.position);
            pistol.gameObject.transform.tag = "Pistol";
            pistol.transform.SetParent(hand);
            pistol.transform.localPosition = Vector3.zero;
            pistol.transform.localPosition = new Vector3(-0.02f, -0.0f, -0.05f);
            pistol.transform.localRotation = Quaternion.Euler(new Vector3(340.0f,250.0f,200.0f));
          //  pistol.transform.localRotation = Quaternion.Euler(new Vector3(0.0568f, 0.0590f, 0.0581f));
        }
    }

    // Create Empty LeftFist and RightFist and add Capsule Collider to detect hits.
    void LeftFistBox()
    {
        Transform hand = umaData.skeleton.GetBoneGameObject(UMASkeleton.StringToHash("LeftHandFinger03_01")).transform﻿; //eli curtz style.

        if (umaData.transform != null)
        {
            GameObject LeftFist = new GameObject("LeftFist");
            LeftFist.transform.parent = umaData.transform;
            LeftFist.transform.Translate(umaData.transform.position);
            LeftFist.AddComponent(typeof(CapsuleCollider));
            LeftFist.gameObject.tag = "Fist";

            CapsuleCollider LeftHandCol = LeftFist.GetComponent<CapsuleCollider>();      
            LeftHandCol.height = .2f;
            LeftHandCol.radius = 0.1f;
            LeftHandCol.transform.SetParent(hand);
            LeftHandCol.transform.localPosition = Vector3.zero;
            LeftHandCol.transform.localPosition = new Vector3(-0.02f, -0.0f, -0.05f);
            //leftHandCol.transform.localRotation = Quaternion.Euler(new Vector3(0.0f, 344.0f, 0.0f));
        }
    }

    void RightFistBox()
    {
        Transform hand = umaData.skeleton.GetBoneGameObject(UMASkeleton.StringToHash("RightHandFinger03_01")).transform﻿; //eli curtz style.

        if (umaData.transform != null)

        {
            GameObject RightFist = new GameObject("RightFist");
            RightFist.transform.parent = umaData.transform;
            RightFist.transform.Translate(umaData.transform.position);
            RightFist.AddComponent(typeof(CapsuleCollider));
            RightFist.gameObject.tag = "Fist";

            CapsuleCollider RightHandCol = RightFist.GetComponent<CapsuleCollider>();
            RightHandCol.height = .2f;
            RightHandCol.radius = 0.1f;
            RightHandCol.transform.SetParent(hand);
            RightHandCol.transform.localPosition = Vector3.zero;
             RightHandCol.transform.localPosition = new Vector3(-0.02f, -0.0f, -0.05f);
            // RightHandCol.transform.localRotation = Quaternion.Euler(new Vector3(0.0f, 344.0f, 0.0f));
        }
    }
}
