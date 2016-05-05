using UnityEngine;
using System.Collections;
using UMA;
using System.IO;
using UMA.PoseTools;


// A practical guide to UMA Part 14b Using the "Asset" format https://youtu.be/Q6bLMusuhbo?t=8m13s
#if UNITY_EDITOR
using UnityEditor;
#endif



public class UMAMaker1_Trenchcoat: MonoBehaviour {

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

    //slider
    [Range(0.0f, 1.0f)]
    public float bodyMass = 0.5f;

    // part 10b of practical guide to UMA https://youtu.be/jyboNBxVTQY?t=50s
    //slider
    [Range(0, 8)]
    public int injury;
    private string bloodName;
    private string lastBloodName;
    // private bool bloodState = false;
    // private bool lastBloodState = false;

    // practical guide to uma part 17 expression player https://youtu.be/nJI-kUYYuWE?t=11m36s
    //slider 
    [Range(-1.0f, 1.0f)]
    public float happy = 0f; 

    public bool chestEmblemState = false;
    private bool lastChestEmblemState = false;

    public bool vestState = false;
    private bool lastVestState = false;

    public Color vestColor = Color.white;
    private Color lastVestColor = Color.white;

    //Practical Guide to UMA part 1 Runtime Slot Changes. https://youtu.be/vB-tGGcFxDI?t=5m30s
    public bool hairState = false;
    private bool lastHairState = false;

    public Color hairColor;
    private Color lastHairColor;

    private int footSlot = 6;
    public bool shoeState = false;
    private bool lastShoeState = true;

    //Practical Guide To UMA part 14 https://youtu.be/ZKRQ4wzp0ac
    public string SaveString = "";
    public bool saveText;
    public bool loadText;
    public bool saveAsset;
    public bool loadAsset;

    //practical guide to uma part 17 Expression player https://youtu.be/nJI-kUYYuWE?t=2m27s

    public UMAExpressionPlayer expressionPlayer;

    //TrenchCoat UMA and Blender Content creation https://youtu.be/_c8lrr-BOnM
    public bool trenchcoatState = false;
    public Color trenchcoatColor;
    private Color lastTrenchcoatColor;
    private bool lastTrenchcoatState = false;
    


    // Part 4 of practical guide to UMA https://youtu.be/KZpvgiAdD9c
    void Start()
    {
        GenerateUMA();
    }

    void Update()
    {
        if (bodyMass != umaDna.upperMuscle)
        {
            SetBodyMass(bodyMass);
            umaData.isShapeDirty = true;
            umaData.Dirty();
        }


        if (happy != expressionPlayer.midBrowUp_Down)
        {
            expressionPlayer.midBrowUp_Down = happy;
            expressionPlayer.leftMouthSmile_Frown = happy;
            expressionPlayer.rightMouthSmile_Frown = happy;
        }
        // part 10b of practical guid to UMA https://youtu.be/jyboNBxVTQY?t=1m6s
        if(vestState && !lastVestState)
        {
            lastVestState = true;
            AddOverlay(3, "SA_Tee", vestColor);     
            umaData.isTextureDirty = true;
            umaData.Dirty();
        }

        if (!vestState && lastVestState)
        {
            lastVestState = false;
            RemoveOverlay(3, "SA_Tee");       
            umaData.isTextureDirty = true;
            umaData.Dirty();
        }


        if (vestColor != lastVestColor && vestState)
        {
            lastVestColor = vestColor;
            ColorOverlay(3, "SA_Tee", vestColor);
            umaData.isTextureDirty = true;
            umaData.Dirty();
        }

        if (chestEmblemState && !lastChestEmblemState)
        {
            lastChestEmblemState = true;
            AddOverlay(3, "SA_Logo");
            umaData.isTextureDirty = true;
            umaData.Dirty();
        }

        if (!chestEmblemState && lastChestEmblemState)
        {
            lastChestEmblemState = false;    
            RemoveOverlay(3, "SA_Logo");
            umaData.isTextureDirty = true;
            umaData.Dirty();
        }

        if (injury != 0)
        {
            //bloodState = true;
            switch (injury)
            {
                case 1:
                    bloodName = "blood";
                    break;
                case 2:
                    bloodName = "bloodBreastR";
                    break;
                case 3:
                    bloodName = "bloodChest";
                    break;
                case 4:
                    bloodName = "bloodGuts1";
                    break;
                case 5:
                    bloodName = "bloodShoulderBackL";
                    break;
                case 6:
                    bloodName = "bloodShoulderBackR";
                    break;

                case 7:
                    bloodName = "bloodShoulderR";
                    break;

                case 8:
                    bloodName = "bloodWhatR";
                    break;
            }

          if (lastBloodName != "") { RemoveOverlay(3, lastBloodName); } 
           
            lastBloodName = bloodName;
            AddOverlay(3, bloodName);
            umaData.isTextureDirty = true;
            umaData.Dirty();
        }
        else
        {
            if (lastBloodName != "") { RemoveOverlay(3, lastBloodName); }
        
            umaData.isTextureDirty = true;
            umaData.Dirty();
            lastBloodName = "";
            bloodName = "";
        }

        // practical guide to uma part 11 Change Slots At Runtime. https://youtu.be/vB-tGGcFxDI?t=5m30s

        if (hairState && !lastHairState)
        {
            lastHairState = hairState;
            SetSlot(7, "M_Hair_Shaggy");
            AddOverlay(7,"M_Hair_Shaggy", hairColor);
            umaData.isMeshDirty = true; //processor expensive use all 'dirty' commands to regenerate mesh.
            umaData.isTextureDirty = true;
            umaData.isShapeDirty = true;
            umaData.Dirty();
        }

        if (!hairState && lastHairState)
        {
            lastHairState = hairState;
            RemoveSlot(7);
           
            umaData.isMeshDirty = true; //processor expensive use all 'dirty' commands to regenerate mesh.
            umaData.isTextureDirty = true;
            umaData.isShapeDirty = true;
            umaData.Dirty();
        }

        //change haircolor at runtime.
        if ( hairColor != lastHairColor && hairState)
        {           
            lastHairColor = hairColor;
            ColorOverlay(7, "M_Hair_Shaggy", hairColor);
            umaData.isTextureDirty = true;
            umaData.Dirty();           
        }


        if (shoeState && !lastShoeState)
        {
            lastShoeState = shoeState;
            SetSlot(footSlot, "Shoes");
           AddOverlay(footSlot, "Shoes");
            umaData.isMeshDirty = true; //processor expensive use all 'dirty' commands to regenerate mesh.
            umaData.isTextureDirty = true;
            umaData.isShapeDirty = true;
            umaData.Dirty();
        }

        if (!shoeState && lastShoeState)
        {
            lastShoeState = shoeState;
            //RemoveSlot(footSlot);
            SetSlot(footSlot, "MaleFeet");
            LinkOverlay(footSlot, 3);
            //AddOverlay(footSlot, "Shoes");
            umaData.isMeshDirty = true; //processor expensive use all 'dirty' commands to regenerate mesh.
            umaData.isTextureDirty = true;
            umaData.isShapeDirty = true;
            umaData.Dirty();
        }



        //practical guide to uma part 14 https://youtu.be/ZKRQ4wzp0ac
        if (saveText)
        {
            saveText = false;
            SaveText();
        }
        if (loadText)
        {
            loadText = false;
            LoadText();   
        }

        // practical guide to UMA part 14b https://youtu.be/Q6bLMusuhbo?t=10m18s
        if (saveAsset)
        {
            saveAsset = false;
            SaveAsset();
        }
        if (loadAsset)
        {
            loadAsset = false;
            LoadAsset();
        }



        //TrenchCoat UMA and Blender Content creation https://youtu.be/_c8lrr-BOnM
        if (trenchcoatState && !lastTrenchcoatState)
        {
            lastTrenchcoatState = trenchcoatState;
            SetSlot(8, "UMA_Human_Male_Trenchcoat");
            AddOverlay(8, "UMA_Human_Male_Trenchcoat", trenchcoatColor);
            umaData.isMeshDirty = true; //processor expensive use all 'dirty' commands to regenerate mesh.
            umaData.isTextureDirty = true;
            umaData.isShapeDirty = true;
            umaData.Dirty();
        }


        if (!trenchcoatState && lastTrenchcoatState)
        {
            lastTrenchcoatState = trenchcoatState;
            RemoveSlot(8);
            umaData.isMeshDirty = true; //processor expensive use all 'dirty' commands to regenerate mesh.
            umaData.isTextureDirty = true;
            umaData.isShapeDirty = true;
            umaData.Dirty();
        }

        //change trenchcolor at runtime.
        if (trenchcoatColor != lastTrenchcoatColor && trenchcoatState)
        {
            lastTrenchcoatColor = trenchcoatColor;
            ColorOverlay(8, "UMA_Human_Male_Trenchcoat", trenchcoatColor);
            umaData.isTextureDirty = true;
            umaData.Dirty();
        }


    }

    void GenerateUMA()
    {
        // Create a new game object and add UMA components to it
        GameObject GO = new GameObject("MyUMA");
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

        // >>> This is whee the fun will happen according to Secret Anorak <<<<
        CreateMale();
        
        // dynamic animation controller 
        umaDynamicAvatar.animationController = animController;

        // Generate Our UMA
        umaDynamicAvatar.UpdateNewRace();

        // parent the new uma into the host game object
        GO.transform.parent = this.gameObject.transform;
        GO.transform.localPosition = Vector3.zero;
        GO.transform.localRotation = Quaternion.identity;
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
       // umaData.umaRecipe.slotDataList[4].SetOverlayList(umaData.umaRecipe.slotDataList[3].GetOverlayList());

        umaData.umaRecipe.slotDataList[5] = slotLibrary.InstantiateSlot("MaleLegs"); // mesh
        LinkOverlay(5, 3);
        //umaData.umaRecipe.slotDataList[5].SetOverlayList(umaData.umaRecipe.slotDataList[3].GetOverlayList()); 

        //footSlot = 6
        umaData.umaRecipe.slotDataList[footSlot] = slotLibrary.InstantiateSlot("MaleFeet"); // mesh
        LinkOverlay(footSlot, 3);
       // umaData.umaRecipe.slotDataList[6].SetOverlayList(umaData.umaRecipe.slotDataList[3].GetOverlayList());

        // add underwear
        umaData.umaRecipe.slotDataList[3].AddOverlay(overlayLibrary.InstantiateOverlay("MaleUnderwear01"));
        //umaData.umaRecipe.slotDataList[5].AddOverlay(overlayLibrary.InstantiateOverlay("MaleUnderwear01"));

        // Practical Guide to UMA part 7 https://youtu.be/Czg1U-hlXn0
        //add eyebrow and set color
        umaData.umaRecipe.slotDataList[2].AddOverlay(overlayLibrary.InstantiateOverlay("MaleEyebrow01", Color.black));

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
        umaData.umaRecipe.slotDataList[8] = slotLibrary.InstantiateSlot("UMA_Human_Male_Trenchcoat"); // mesh                                                                                                      //umaData.umaRecipe.slotDataList[8].AddOverlay(overlayLibrary.InstantiateOverlay("UMA_Human_Male_Trenchcoat"));

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
        SaveString = recipe.recipeString;
        Destroy(recipe);

        //Save string to text file
        string fileName = "Assets/Test.txt";
            StreamWriter stream = File.CreateText(fileName);
        stream.WriteLine(SaveString);
        stream.Close();
    }

    void LoadText()
    {
        //Save string to text file
        string fileName = "Assets/Test.txt";
            StreamReader stream = File.OpenText(fileName);
        SaveString = stream.ReadLine();
        stream.Close();

        //Generate UMA String
        UMATextRecipe recipe = ScriptableObject.CreateInstance<UMATextRecipe>();        
        recipe.recipeString = SaveString;
        umaDynamicAvatar.Load(recipe);
        Destroy(recipe);
    }

    // practical guide to UMA part 14b https://youtu.be/Q6bLMusuhbo?t=10m18s
    void LoadAsset()
    {
        UMARecipeBase recipe = Resources.Load("Troll") as UMARecipeBase;
        umaDynamicAvatar.Load(recipe);
    }

    void SaveAsset()
    {
        #if UNITY_EDITOR
        var asset = ScriptableObject.CreateInstance<UMATextRecipe>();
        asset.Save(umaDynamicAvatar.umaData.umaRecipe, umaDynamicAvatar.context);
        AssetDatabase.CreateAsset(asset, "Assets/Boom.asset");
        AssetDatabase.SaveAssets();
        #endif
    }

    //practical guide to UMA part 15 intercepting uma events https://youtu.be/_k-SZRCvgIk?t=4m17s
    void CharacterCreatedCallback(UMAData umaData)
    {
        //Debug.Log("UMA_Created");
        GrabStaff();

        // A Practical Guide To UMA - Part 17 - Using the Expression Player  https://youtu.be/nJI-kUYYuWE
        UMAExpressionSet expressionSet = umaData.umaRecipe.raceData.expressionSet;
        expressionPlayer = umaData.gameObject.AddComponent<UMAExpressionPlayer>();
        expressionPlayer.expressionSet = expressionSet;
        expressionPlayer.umaData = umaData;
        expressionPlayer.Initialize();
        // automated expressions to look life like
        expressionPlayer.enableBlinking = true;
        expressionPlayer.enableSaccades = true;
    }


    //practical guide to UMA part 16 attaching props https://youtu.be/0Iy_G_IufKU?t=6m48s
    void GrabStaff()
    {
        GameObject staff = GameObject.Find("staff");
        //Transform hand = umaDynamicAvatar.gameObject.transform.FindChild("Root/Global/Position/Hips..etc hiearchy style");
        Transform hand = umaData.skeleton.GetBoneGameObject(UMASkeleton.StringToHash("LeftHand")).transform﻿; //eli curtz style.
        staff.transform.SetParent(hand);
        staff.transform.localPosition = Vector3.zero;
        staff.transform.localPosition = new Vector3(-0.117f, -0.543f, -0.017f);
        staff.transform.localRotation = Quaternion.Euler(new Vector3(0.0f, 344.0f, 0.0f));
    }
}
