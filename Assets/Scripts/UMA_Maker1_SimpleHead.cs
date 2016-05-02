using UnityEngine;
using System.Collections;
using UMA;

public class UMAMaker1 : MonoBehaviour {

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


    private UMADynamicAvatar umaDynamicAvatar; // must be on a gameObject to display characters
    private UMAData umaData; // all the uma slots/overlays/dna values
    private UMADnaHumanoid umaDna; // list of morphs
    private UMADnaTutorial umaTutorialDNA; // Extra DNA customizable values

    private int numberOfSlots = 20; // could be as big as we like

    // Part 4 of practical guide to UMA https://youtu.be/KZpvgiAdD9c
    void Start()
    {
        GenerateUMA();
    }

    void GenerateUMA()
    {
        // Create a new game object and add UMA components to it
        GameObject GO = new GameObject("MyUMA");
        umaDynamicAvatar = GO.AddComponent<UMADynamicAvatar>();

        // Initialise Avatar and grab a reference to it's data component
        umaDynamicAvatar.Initialize();
        umaData = umaDynamicAvatar.umaData;

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

        // Generate Our UMA
        umaDynamicAvatar.UpdateNewRace();
    }

    // Practical Guide to UMA part 5 https://youtu.be/N-NlNJv1ESE
    void CreateMale()
    {
        // Grab a reference to our recipe
        var umaRecipe = umaDynamicAvatar.umaData.umaRecipe;
        umaRecipe.SetRace(raceLibrary.GetRace("HumanMale"));

        umaData.umaRecipe.slotDataList[1] = slotLibrary.InstantiateSlot("MaleFace");

        //add texture
        umaData.umaRecipe.slotDataList[1].AddOverlay(overlayLibrary.InstantiateOverlay("MaleHead02"));


    }
}
