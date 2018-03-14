using UnityEngine;
using System.Collections;
using UMA;
using System.IO;
using UMA.PoseTools;


// A practical guide to UMA Part 14b Using the "Asset" format https://youtu.be/Q6bLMusuhbo?t=8m13s
#if UNITY_EDITOR
using UnityEditor;
#endif



public class UMAMaker_PoliceMinimal : MonoBehaviour
{
    // UMA Maker for Police

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

    

    public bool hatState = false;
    private bool lastHatState = true;


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
      
    
        if (hatState && !lastHatState)
        {
            Debug.Log(string.Format("Hatstate {0}", hatState));
            lastHatState = hatState;
            SetSlot(9, "HumanMaleHatPolice");

            Debug.Log("Slot set. Attempting overlay");
            AddOverlay(9, "HumanMaleHatPolice");
            umaData.isMeshDirty = true; //processor expensive use all 'dirty' commands to regenerate mesh.
            umaData.isTextureDirty = true;
            umaData.isShapeDirty = true;
            umaData.Dirty();
        }


        if (!hatState && lastHatState)
        {
            lastHatState = hatState;
            Debug.Log("Removing Slot 9");
            RemoveSlot(9);
            umaData.isMeshDirty = true; //processor expensive use all 'dirty' commands to regenerate mesh.
            umaData.isTextureDirty = true;
            umaData.isShapeDirty = true;
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

        GO.AddComponent(typeof(UnityEngine.AI.NavMeshAgent));
        GO.AddComponent(typeof(CapsuleCollider));

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
        umaData.umaRecipe.slotDataList[6] = slotLibrary.InstantiateSlot("MaleFeet"); // mesh
        LinkOverlay(6, 3);
        // umaData.umaRecipe.slotDataList[6].SetOverlayList(umaData.umaRecipe.slotDataList[3].GetOverlayList());

        // add underwear
        umaData.umaRecipe.slotDataList[3].AddOverlay(overlayLibrary.InstantiateOverlay("MaleUnderwear01"));
        //umaData.umaRecipe.slotDataList[5].AddOverlay(overlayLibrary.InstantiateOverlay("MaleUnderwear01"));

        //Police hat slot is at 9 because I removed all the extra Secret Anorak /Wigifer work
        umaData.umaRecipe.slotDataList[9] = slotLibrary.InstantiateSlot("HumanMaleHatPolice"); // mesh
        //overlay doesn't work
        //AddOverlay(9, "HumanMaleHatPolice");
        //umaData.umaRecipe.slotDataList[9].AddOverlay(overlayLibrary.InstantiateOverlay("HumanMaleHatPolice"));
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

    void RemoveOverlay(int slotNumber, string overlayName)
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

    //practical guide to UMA part 15 intercepting uma events https://youtu.be/_k-SZRCvgIk?t=4m17s
    void CharacterCreatedCallback(UMAData umaData)
    {
   Debug.Log("UMA_Created");
    }

}
