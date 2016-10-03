using UnityEngine;
using System.Collections;
using UMA;

public class UMACharGen : MonoBehaviour
{

    public UMAGeneratorBase generator;
    public SlotLibrary slotLibrary;
    public OverlayLibrary overlayLibrary;
    public RaceLibrary raceLibrary;
    public RuntimeAnimatorController animationController;
	public RuntimeAnimatorController secondaryAnimationController;

    private UMADynamicAvatar umaDynamicAvatar;
    private UMAData umaData;
    private UMADnaHumanoid umaDna;
    private UMADnaTutorial umaTutorialDNA;

    private int numberOfSlots = 10;

	public bool isMultiSpawn = false;

	private GameObject m_createdObject = null;
	private int m_spawnCount = 0;

    void Start()
    {
		setupFallbackObjects ();
		m_createdObject = GenerateUMA(name + "( generated object " + m_spawnCount + ")");
		++m_spawnCount;
		if (!isMultiSpawn) {
			Destroy (gameObject);
		}
    }

	void Update()
	{
		if (m_createdObject.GetComponent<HealthComponent> ().isDead ()) {
			m_createdObject = GenerateUMA (name + "( generated object " + m_spawnCount + ")");
			++m_spawnCount;
		}
	}

	void setupFallbackObjects()
	{
		if (generator == null) {
			generator = GameObject.Find("UMAGenerator").GetComponent<UMAGenerator>();
		}
		if (slotLibrary == null) {
			slotLibrary = GameObject.Find("SlotLibrary").GetComponent<SlotLibrary>();
		}
		if (overlayLibrary == null) {
			overlayLibrary = GameObject.Find("OverlayLibrary").GetComponent<OverlayLibrary>();
		}
		if (raceLibrary == null) {
			raceLibrary = GameObject.Find("RaceLibrary").GetComponent<RaceLibrary>();
		}
	}

	private void setupDna (UMADnaHumanoid umaDna)
	{		
		umaDna.height = Random.Range (0.3f, 0.5f);
		umaDna.headSize = Random.Range (0.485f, 0.515f);
		umaDna.headWidth = Random.Range (0.4f, 0.6f);

		umaDna.neckThickness = Random.Range (0.495f, 0.51f);

		if (umaData.umaRecipe.raceData.raceName == "HumanMale") {
			umaDna.handsSize = Random.Range (0.485f, 0.515f);
			umaDna.feetSize = Random.Range (0.485f, 0.515f);
			umaDna.legSeparation = Random.Range (0.4f, 0.6f);
			umaDna.waist = 0.5f;
		} else {
			umaDna.handsSize = Random.Range (0.485f, 0.515f);
			umaDna.feetSize = Random.Range (0.485f, 0.515f);
			umaDna.legSeparation = Random.Range (0.485f, 0.515f);
			umaDna.waist = Random.Range (0.3f, 0.8f);
		}

		umaDna.armLength = Random.Range (0.485f, 0.515f);
		umaDna.forearmLength = Random.Range (0.485f, 0.515f);
		umaDna.armWidth = Random.Range (0.3f, 0.8f);
		umaDna.forearmWidth = Random.Range (0.3f, 0.8f);

		umaDna.upperMuscle = Random.Range (0.0f, 1.0f);
		umaDna.upperWeight = Random.Range (-0.2f, 0.2f) + umaDna.upperMuscle;
		if (umaDna.upperWeight > 1.0) {
			umaDna.upperWeight = 1.0f;
		}
		if (umaDna.upperWeight < 0.0) {
			umaDna.upperWeight = 0.0f;
		}

		umaDna.lowerMuscle = Random.Range (-0.2f, 0.2f) + umaDna.upperMuscle;
		if (umaDna.lowerMuscle > 1.0) {
			umaDna.lowerMuscle = 1.0f;
		}
		if (umaDna.lowerMuscle < 0.0) {
			umaDna.lowerMuscle = 0.0f;
		}

		umaDna.lowerWeight = Random.Range (-0.1f, 0.1f) + umaDna.upperWeight;
		if (umaDna.lowerWeight > 1.0) {
			umaDna.lowerWeight = 1.0f;
		}
		if (umaDna.lowerWeight < 0.0) {
			umaDna.lowerWeight = 0.0f;
		}

		umaDna.belly = umaDna.upperWeight;
		umaDna.legsSize = Random.Range (0.4f, 0.6f);
		umaDna.gluteusSize = Random.Range (0.4f, 0.6f);

		umaDna.earsSize = Random.Range (0.3f, 0.8f);
		umaDna.earsPosition = Random.Range (0.3f, 0.8f);
		umaDna.earsRotation = Random.Range (0.3f, 0.8f);

		umaDna.noseSize = Random.Range (0.3f, 0.8f);

		umaDna.noseCurve = Random.Range (0.3f, 0.8f);
		umaDna.noseWidth = Random.Range (0.3f, 0.8f);
		umaDna.noseInclination = Random.Range (0.3f, 0.8f);
		umaDna.nosePosition = Random.Range (0.3f, 0.8f);
		umaDna.nosePronounced = Random.Range (0.3f, 0.8f);
		umaDna.noseFlatten = Random.Range (0.3f, 0.8f);

		umaDna.chinSize = Random.Range (0.3f, 0.8f);
		umaDna.chinPronounced = Random.Range (0.3f, 0.8f);
		umaDna.chinPosition = Random.Range (0.3f, 0.8f);

		umaDna.mandibleSize = Random.Range (0.45f, 0.52f);
		umaDna.jawsSize = Random.Range (0.3f, 0.8f);
		umaDna.jawsPosition = Random.Range (0.3f, 0.8f);

		umaDna.cheekSize = Random.Range (0.3f, 0.8f);
		umaDna.cheekPosition = Random.Range (0.3f, 0.8f);
		umaDna.lowCheekPronounced = Random.Range (0.3f, 0.8f);
		umaDna.lowCheekPosition = Random.Range (0.3f, 0.8f);

		umaDna.foreheadSize = Random.Range (0.3f, 0.8f);
		umaDna.foreheadPosition = Random.Range (0.15f, 0.65f);

		umaDna.lipsSize = Random.Range (0.3f, 0.8f);
		umaDna.mouthSize = Random.Range (0.3f, 0.8f);
		umaDna.eyeRotation = Random.Range (0.3f, 0.8f);
		umaDna.eyeSize = Random.Range (0.3f, 0.8f);
		umaDna.breastSize = Random.Range (0.3f, 0.8f);
	}

    GameObject GenerateUMA(string name)
    {
		// Create a new game object and add UMA components to it
		GameObject GO = new GameObject(name);
		umaDynamicAvatar = GO.AddComponent<UMADynamicAvatar>();
		umaDynamicAvatar.animationController = animationController;
		GO.AddComponent<RagdollCreatorTest>();
		if (name.Contains ("Zombie")) {
			GO.AddComponent<ZombieBehavior> ();
			GO.tag = "Zombie";
		} else {
			HumanBehavior hb = GO.AddComponent<HumanBehavior> ();
			hb.zombieAnimationController = secondaryAnimationController;
			GO.tag = "Human";
		}
		GO.AddComponent<NavMeshAgent> ();
		GO.AddComponent<HealthComponent> ();
		GO.transform.position = transform.position;

        // Initialize Avatar and grab a reference to its data component
        umaDynamicAvatar.Initialize();
        umaData = umaDynamicAvatar.umaData;

        // Attach our generator
        umaDynamicAvatar.umaGenerator = generator;
        umaData.umaGenerator = generator;

        // Set up slot Array
        umaData.umaRecipe.slotDataList = new SlotData[numberOfSlots];

        // Set up our Morph references
        umaDna = new UMADnaHumanoid();
        umaTutorialDNA = new UMADnaTutorial();
        umaData.umaRecipe.AddDna(umaDna);
        umaData.umaRecipe.AddDna(umaTutorialDNA);


		if (name.Contains ("Female")) {
			CreateFemale ();
		} else {
			CreateMale ();
		}

        // Generate our UMA
		setupDna (umaDna);
        umaDynamicAvatar.UpdateNewRace();
		
		return GO;
    }

    void CreateFemale()
    {
        // Grab a reference to our recipe
        var umaRecipe = umaDynamicAvatar.umaData.umaRecipe;
        umaRecipe.SetRace(raceLibrary.GetRace("HumanFemale"));

        umaData.umaRecipe.slotDataList[0] = slotLibrary.InstantiateSlot("FemaleHead_Head");
        umaData.umaRecipe.slotDataList[0].AddOverlay(overlayLibrary.InstantiateOverlay("FemaleHead01"));

        umaData.umaRecipe.slotDataList[1] = slotLibrary.InstantiateSlot("FemaleTorso");
        umaData.umaRecipe.slotDataList[1].AddOverlay(overlayLibrary.InstantiateOverlay("FemaleBody01"));

        umaData.umaRecipe.slotDataList[2] = slotLibrary.InstantiateSlot("FemaleTshirt01");
        umaData.umaRecipe.slotDataList[2].AddOverlay(overlayLibrary.InstantiateOverlay("FemaleShirt01"));

        umaData.umaRecipe.slotDataList[3] = slotLibrary.InstantiateSlot("FemaleFace");
        umaData.umaRecipe.slotDataList[3].AddOverlay(overlayLibrary.InstantiateOverlay("FemaleHead01"));

        umaData.umaRecipe.slotDataList[4] = slotLibrary.InstantiateSlot("FemaleLongHair01");
        umaData.umaRecipe.slotDataList[4].AddOverlay(overlayLibrary.InstantiateOverlay("FemaleLongHair01_Module"));

        umaData.umaRecipe.slotDataList[5] = slotLibrary.InstantiateSlot("FemaleHands");
        umaData.umaRecipe.slotDataList[5].AddOverlay(overlayLibrary.InstantiateOverlay("FemaleBody01"));

        umaData.umaRecipe.slotDataList[6] = slotLibrary.InstantiateSlot("FemaleLegs");
        umaData.umaRecipe.slotDataList[6].AddOverlay(overlayLibrary.InstantiateOverlay("FemaleJeans01"));

        umaData.umaRecipe.slotDataList[7] = slotLibrary.InstantiateSlot("FemaleFeet");
        umaData.umaRecipe.slotDataList[7].AddOverlay(overlayLibrary.InstantiateOverlay("FemaleBody01"));
    }

	void CreateMale()
	{
		// Grab a reference to our recipe
		var umaRecipe = umaDynamicAvatar.umaData.umaRecipe;
		umaRecipe.SetRace(raceLibrary.GetRace("HumanMale"));

		umaData.umaRecipe.slotDataList[0] = slotLibrary.InstantiateSlot("MaleHead_Head");
		umaData.umaRecipe.slotDataList[0].AddOverlay(overlayLibrary.InstantiateOverlay("MaleHead01"));

		umaData.umaRecipe.slotDataList[1] = slotLibrary.InstantiateSlot("MaleTorso");
		umaData.umaRecipe.slotDataList[1].AddOverlay(overlayLibrary.InstantiateOverlay("MaleBody01"));

		umaData.umaRecipe.slotDataList[3] = slotLibrary.InstantiateSlot("MaleFace");
		umaData.umaRecipe.slotDataList[3].AddOverlay(overlayLibrary.InstantiateOverlay("MaleHead01"));

		umaData.umaRecipe.slotDataList[4] = slotLibrary.InstantiateSlot("MaleHead_Head");
		umaData.umaRecipe.slotDataList[4].AddOverlay(overlayLibrary.InstantiateOverlay("MaleHair01"));

		umaData.umaRecipe.slotDataList[5] = slotLibrary.InstantiateSlot("MaleHands");
		umaData.umaRecipe.slotDataList[5].AddOverlay(overlayLibrary.InstantiateOverlay("MaleBody01"));

		umaData.umaRecipe.slotDataList[6] = slotLibrary.InstantiateSlot("MaleLegs");
		umaData.umaRecipe.slotDataList[6].AddOverlay(overlayLibrary.InstantiateOverlay("MaleJeans01"));

		umaData.umaRecipe.slotDataList[7] = slotLibrary.InstantiateSlot("MaleFeet");
		umaData.umaRecipe.slotDataList[7].AddOverlay(overlayLibrary.InstantiateOverlay("MaleBody01"));
	}
}
