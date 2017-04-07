using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UMA;

public class UMACharGen : MonoBehaviour
{

	public UMAGeneratorBase generator;
	public SlotLibrary slotLibrary;
	public OverlayLibrary overlayLibrary;
	public RaceLibrary raceLibrary;
	public RuntimeAnimatorController animationController;

	private UMADynamicAvatar umaDynamicAvatar;
	private UMAData umaData;
	private UMADnaHumanoid umaDna;
	private UMADnaTutorial umaTutorialDNA;

	private int numberOfSlots = 2; // <- fix this

	public bool isMultiSpawn = false;

	private GameObject m_createdObject = null;
	private int m_spawnCount = 0;

	static Dictionary<string,string> s_slotMap = new Dictionary<string,string>();
	static Dictionary<string,string> s_overlayMap = new Dictionary<string,string>();

	void Start()
	{
		setupFallbackObjects ();
		m_createdObject = GenerateUMA(name + "( generated object " + m_spawnCount + ")");
		++m_spawnCount;

		if (!isMultiSpawn) {
			Destroy (gameObject);
		}
	}

	// is this spawning new ones when they die? -bill I'll see.
	void Update()
	{
		if (m_createdObject.GetComponent<HealthComponent> ().isDead ()) {
			m_createdObject = GenerateUMA (name + "( generated object " + m_spawnCount + ")");

			++m_spawnCount;
		}
	}

	string getMappedSlotName(string name) {
		// :TODO: :TO: fix clothing
		if (s_slotMap.Count == 0) {
			s_slotMap ["FemaleEyelash"] = "";
			s_slotMap ["FemaleTshirt01"] = "";
			s_slotMap ["FemaleShortHair01"] = "";
			s_slotMap ["FemaleLongHair01"] = "";
		}

		if (s_slotMap.ContainsKey (name)) {
			return s_slotMap [name];
		}

		return name;
	}

	string getMappedOverlayName(string name) {
		// :TODO: :TO: fix clothing
		if (s_overlayMap.Count == 0) {
			s_overlayMap ["FemaleHead01"] = "F_H_Head";
			s_overlayMap ["FemaleEyebrow01"] = "";
			s_overlayMap ["FemaleBody01"] = "";
			s_overlayMap ["FemaleBody02"] = "";
			s_overlayMap ["FemaleUnderwear01"] = "";
			s_overlayMap ["FemaleShirt01"] = "";
			s_overlayMap ["FemaleShirt02"] = "";
			s_overlayMap ["FemaleJeans01"] = "";
		}

		if (s_overlayMap.ContainsKey (name)) {
			return s_overlayMap [name];
		}
		return name;
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



	// this is totally not working as expected 10/6/2016
	void parentAtPosition(GameObject obj)
	{
		Vector3 currentPos = obj.transform.position;
		Vector3 mainPos = GameObject.Find("MainGameManager").transform.position;
		float x = currentPos.x - mainPos.x;
		float y = currentPos.y - mainPos.y;
		float z = currentPos.z - mainPos.z;
		obj.transform.SetParent(GameObject.Find("MainGameManager").transform);
		Vector3 newpos = new Vector3(x, y, z);
		obj.transform.localPosition = newpos;
	}


	GameObject GenerateUMA(string name)
	{
		// Create a new game object and add UMA components to it

		GameObject GO = new GameObject(name);
		GO.transform.position = this.transform.position;
		//parentAtPosition(GO);


		umaDynamicAvatar = GO.AddComponent<UMADynamicAvatar>();
		umaDynamicAvatar.animationController = animationController;
		GO.AddComponent<RagdollCreatorTest>();
		if (name.Contains ("Zombie")) {

			ZombieBehavior zbh = GO.AddComponent<ZombieBehavior> ();
			zbh.speedMultiplier = Random.Range (0.5f, 2.5f);
			GO.tag = "Zombie";
			GameObject[] zombies = GameObject.FindGameObjectsWithTag("Zombie");
			GO.name = "Zombie" + zombies.Length;

		} else {
			HumanBehavior hb = GO.AddComponent<HumanBehavior> ();
			GO.tag = "Human";
			//  GameObject[] humans = GameObject.FindGameObjectsWithTag("Human");
			//GO.name = "Human" + humans.Length;

		}

		GO.AddComponent<UnityEngine.AI.NavMeshAgent> ();
		GO.AddComponent<HealthComponent> ();

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

		if (name.Contains("Human"))
		{
			if (name.Contains("Female"))
			{
				GO.name = createName("Female");
			}
			else
			{
				GO.name = createName("Male");
			}
		}

		return GO;
	}

	private SlotLibrary GetSlotLibrary()
	{
		return slotLibrary;
	}

	private OverlayLibrary GetOverlayLibrary()
	{
		return overlayLibrary;
	}

	string createName(string sex)
	{   string newName = "";
		string firstName = "";
		HumanNameLists humanNameList = GameObject.Find("HumanNamesList").GetComponent<HumanNameLists>();
		string lastName = humanNameList.lastNames[Random.Range(0, humanNameList.lastNames.Count - 1)];
		if (sex == "Female")
		{        
			firstName = humanNameList.femaleNames[Random.Range(0, humanNameList.femaleNames.Count - 1)];        
		}
		else
		{
			firstName = humanNameList.maleNames[Random.Range(0, humanNameList.maleNames.Count - 1)];
		}
		newName = firstName + " " + lastName;
		return newName;
	}

	void CreateNeutrum()
	{
		var umaRecipe = umaDynamicAvatar.umaData.umaRecipe;
		umaRecipe.SetRace (raceLibrary.GetRace ("HumanMale"));

		umaData.umaRecipe.slotDataList = new SlotData[2];

		umaData.umaRecipe.slotDataList[0] = GetSlotLibrary().InstantiateSlot(getMappedSlotName("MaleTorso"));
		umaData.umaRecipe.slotDataList[1] = GetSlotLibrary().InstantiateSlot(getMappedSlotName("MaleLegs"));


		//umaData.umaRecipe.slotDataList[1] = GetSlotLibrary().InstantiateSlot(getMappedSlotName("MaleFace"));

			//umaData.umaRecipe.slotDataList[1] = GetSlotLibrary().InstantiateSlot(getMappedSlotName("MaleFace"));


		/*
			umaData.umaRecipe.slotDataList[1] = GetSlotLibrary().InstantiateSlot(getMappedSlotName("MaleHead_Head"));


			umaData.umaRecipe.slotDataList[6] = GetSlotLibrary().InstantiateSlot(getMappedSlotName("MaleHead_Eyes"), umaData.umaRecipe.slotDataList[1].GetOverlayList());
			umaData.umaRecipe.slotDataList[8] = GetSlotLibrary().InstantiateSlot(getMappedSlotName("MaleHead_Mouth"), umaData.umaRecipe.slotDataList[1].GetOverlayList());


			umaData.umaRecipe.slotDataList[9] = GetSlotLibrary().InstantiateSlot(getMappedSlotName("MaleHead_Nose"), umaData.umaRecipe.slotDataList[1].GetOverlayList());


			umaData.umaRecipe.slotDataList[7] = GetSlotLibrary().InstantiateSlot(getMappedSlotName("MaleHead_Ears"), umaData.umaRecipe.slotDataList[1].GetOverlayList());
		







		umaData.umaRecipe.slotDataList[3] = GetSlotLibrary().InstantiateSlot(getMappedSlotName("MaleHands"), umaData.umaRecipe.slotDataList[2].GetOverlayList());

		umaData.umaRecipe.slotDataList[4] = GetSlotLibrary().InstantiateSlot(getMappedSlotName("MaleInnerMouth"));




		umaData.umaRecipe.slotDataList[5] = GetSlotLibrary().InstantiateSlot(getMappedSlotName("MaleFeet"), umaData.umaRecipe.slotDataList[2].GetOverlayList());
		*/
	}

	void CreateFemale()
	{
		CreateNeutrum ();return;//remove me
		var umaRecipe = umaDynamicAvatar.umaData.umaRecipe;
		umaRecipe.SetRace (raceLibrary.GetRace ("HumanFemale"));

		// copied from Bill's code

		Color skinColor = new Color (1, 1, 1, 1);
		float skinTone;

		skinTone = Random.Range (0.1f, 0.6f);
		skinColor = new Color (skinTone + Random.Range (0.35f, 0.4f), skinTone + Random.Range (0.25f, 0.4f), skinTone + Random.Range (0.35f, 0.4f), 1);

		Color HairColor = new Color (Random.Range (0.1f, 0.9f), Random.Range (0.1f, 0.9f), Random.Range (0.1f, 0.9f), 1);

		int randomResult = 0;
		//Female Avatar

		//Example of dynamic list
		List<SlotData> tempSlotList = new List<SlotData> ();


		//tempSlotList.Add(GetSlotLibrary().InstantiateSlot(getMappedSlotName("FemaleEyes")));
		//tempSlotList[tempSlotList.Count - 1].AddOverlay(GetOverlayLibrary().InstantiateOverlay(getMappedOverlayNameName("EyeOverlay")));
		//tempSlotList[tempSlotList.Count - 1].AddOverlay(GetOverlayLibrary().InstantiateOverlay(getMappedOverlayName("EyeOverlayAdjust", new Color(Random.Range(0.1f, 0.9f), Random.Range(0.1f, 0.9f), Random.Range(0.1f, 0.9f), 1))));
		tempSlotList.Add (GetSlotLibrary ().InstantiateSlot (getMappedSlotName ("FemaleTorso")));
		int headIndex = 0;

		string mappedEyeBrowOverlayName = getMappedOverlayName ("FemaleEyebrow01");
		string mappedHead1OverlayName = getMappedOverlayName ("FemaleHead01");

		randomResult = Random.Range (0, 2);
		if (randomResult == 0) {

			tempSlotList.Add (GetSlotLibrary ().InstantiateSlot (getMappedSlotName ("FemaleFace")));
			headIndex = tempSlotList.Count - 1;
			tempSlotList [headIndex].AddOverlay (GetOverlayLibrary ().InstantiateOverlay (getMappedOverlayName ("FemaleHead01"), skinColor));
			if (mappedEyeBrowOverlayName != "") {
				tempSlotList [headIndex].AddOverlay (GetOverlayLibrary ().InstantiateOverlay (mappedEyeBrowOverlayName, new Color (0.125f, 0.065f, 0.065f, 1.0f)));
			}

			randomResult = Random.Range (0, 2);
			if (randomResult == 0) {
				tempSlotList [headIndex].AddOverlay (GetOverlayLibrary ().InstantiateOverlay (getMappedOverlayName ("FemaleLipstick01"), new Color (skinColor.r + Random.Range (0.0f, 0.3f), skinColor.g, skinColor.b + Random.Range (0.0f, 0.2f), 1)));
			}
		} else if (randomResult == 1) {
			tempSlotList.Add (GetSlotLibrary ().InstantiateSlot (getMappedSlotName ("FemaleHead_Head")));
			headIndex = tempSlotList.Count - 1;
			if (mappedHead1OverlayName != "") {
				tempSlotList [headIndex].AddOverlay (GetOverlayLibrary ().InstantiateOverlay (getMappedOverlayName ("FemaleHead01"), skinColor));
			}
			if (mappedEyeBrowOverlayName != "") {
				tempSlotList [headIndex].AddOverlay (GetOverlayLibrary ().InstantiateOverlay (mappedEyeBrowOverlayName, new Color (0.125f, 0.065f, 0.065f, 1.0f)));
			}

			tempSlotList.Add (GetSlotLibrary ().InstantiateSlot (getMappedSlotName ("FemaleHead_Eyes"), tempSlotList [headIndex].GetOverlayList ()));
			tempSlotList.Add (GetSlotLibrary ().InstantiateSlot (getMappedSlotName ("FemaleHead_Mouth"), tempSlotList [headIndex].GetOverlayList ()));
			tempSlotList.Add (GetSlotLibrary ().InstantiateSlot (getMappedSlotName ("FemaleHead_Nose"), tempSlotList [headIndex].GetOverlayList ()));


			randomResult = Random.Range (0, 2);
			if (randomResult == 0) {
				tempSlotList.Add (GetSlotLibrary ().InstantiateSlot (getMappedSlotName ("FemaleHead_Ears"), tempSlotList [headIndex].GetOverlayList ()));

				//   tempSlotList.Add(GetSlotLibrary().InstantiateSlot(getMappedSlotName("FemaleHead_ElvenEars")));
				//tempSlotList[tempSlotList.Count - 1].AddOverlay(GetOverlayLibrary().InstantiateOverlay(getMappedOverlayName("ElvenEars", skinColor)));
			} else if (randomResult == 1) {
				tempSlotList.Add (GetSlotLibrary ().InstantiateSlot (getMappedSlotName ("FemaleHead_Ears"), tempSlotList [headIndex].GetOverlayList ()));
			}

			randomResult = Random.Range (0, 2);
			if (randomResult == 0) {
				tempSlotList [headIndex].AddOverlay (GetOverlayLibrary ().InstantiateOverlay (getMappedOverlayName ("FemaleLipstick01"), new Color (skinColor.r + Random.Range (0.0f, 0.3f), skinColor.g, skinColor.b + Random.Range (0.0f, 0.2f), 1)));
			}
		}

		string eyeLashSlotName = getMappedSlotName ("FemaleEyelash");
		if (eyeLashSlotName != "") {
			tempSlotList.Add (GetSlotLibrary ().InstantiateSlot (eyeLashSlotName));
			tempSlotList [tempSlotList.Count - 1].AddOverlay (GetOverlayLibrary ().InstantiateOverlay (getMappedOverlayName ("FemaleEyelash"), Color.black));
		}

		// zombiegirl slot models  broken disabled 10 - 09 - 2016 .
		/*	if (this.gameObject.tag == "SpawnPoint_Zombie")
		{
			tempSlotList.Add(GetSlotLibrary().InstantiateSlot(getMappedSlotName("ZombieGirl_BodyNew")));
			tempSlotList.Add(GetSlotLibrary().InstantiateSlot(getMappedSlotName("ZombieGirl_TopNew")));
			//tempSlotList[bodyIndex].AddOverlay(GetOverlayLibrary().InstantiateOverlay(getMappedOverlayName("ZombieGirlTop", new Color(Random.Range(0.1f, 0.9f), Random.Range(0.1f, 0.9f), Random.Range(0.1f, 0.9f), 1))));

			tempSlotList.Add(GetSlotLibrary().InstantiateSlot(getMappedSlotName("ZombieGirl_PantsNew")));
		}*/


		int bodyIndex = 0;
		randomResult = Random.Range(0, 2);

		string mappedBody1OverlayName = getMappedOverlayName ("FemaleBody01");
		string mappedBody2OverlayName = getMappedOverlayName ("FemaleBody02");
		string mappedUnderwearOverlayName = getMappedOverlayName ("FemaleUnderwear01");
		string mappedTshirt1OverlayName = getMappedOverlayName ("FemaleShirt01");
		string mappedTshirt2OverlayName = getMappedOverlayName ("FemaleShirt02");
		string mappedJeansOverlayName = getMappedOverlayName ("FemaleJeans01");
		string mappedTshirtSlotName = getMappedSlotName ("FemaleTshirt01");

		if (randomResult == 0 && mappedBody1OverlayName != "")
		{
			tempSlotList [bodyIndex].AddOverlay (GetOverlayLibrary ().InstantiateOverlay (mappedBody1OverlayName, skinColor));
		}
		if (randomResult == 1 && mappedBody2OverlayName != "")
		{
			tempSlotList [bodyIndex].AddOverlay (GetOverlayLibrary ().InstantiateOverlay (mappedBody2OverlayName, skinColor));
		}

		if (mappedUnderwearOverlayName != "") {
			tempSlotList [bodyIndex].AddOverlay (GetOverlayLibrary ().InstantiateOverlay (mappedUnderwearOverlayName, new Color (Random.Range (0.1f, 0.9f), Random.Range (0.1f, 0.9f), Random.Range (0.1f, 0.9f), 1)));
		}

		randomResult = Random.Range(0, 4);
		if (randomResult == 0 && mappedTshirt1OverlayName != "")
		{
			tempSlotList[bodyIndex].AddOverlay(GetOverlayLibrary().InstantiateOverlay(mappedTshirt1OverlayName, new Color(Random.Range(0.1f, 0.9f), Random.Range(0.1f, 0.9f), Random.Range(0.1f, 0.9f), 1)));
		}
		else if (randomResult == 1 && mappedTshirt2OverlayName != "")
		{
			tempSlotList[bodyIndex].AddOverlay(GetOverlayLibrary().InstantiateOverlay(mappedTshirt2OverlayName, new Color(Random.Range(0.1f, 0.9f), Random.Range(0.1f, 0.9f), Random.Range(0.1f, 0.9f), 1)));
		}
		else if (randomResult == 2 && mappedTshirtSlotName != "")
		{
			tempSlotList.Add(GetSlotLibrary().InstantiateSlot(mappedTshirtSlotName));
			if (mappedTshirt1OverlayName != "") {
				tempSlotList [tempSlotList.Count - 1].AddOverlay (GetOverlayLibrary ().InstantiateOverlay (mappedTshirt1OverlayName, new Color (Random.Range (0.1f, 0.9f), Random.Range (0.1f, 0.9f), Random.Range (0.1f, 0.9f), 1)));
			}
		}

		tempSlotList.Add(GetSlotLibrary().InstantiateSlot(getMappedSlotName("FemaleHands"), tempSlotList[bodyIndex].GetOverlayList()));

		tempSlotList.Add(GetSlotLibrary().InstantiateSlot(getMappedSlotName("FemaleInnerMouth")));
		tempSlotList[tempSlotList.Count - 1].AddOverlay(GetOverlayLibrary().InstantiateOverlay(getMappedOverlayName("InnerMouth")));

		tempSlotList.Add(GetSlotLibrary().InstantiateSlot(getMappedSlotName("FemaleFeet"), tempSlotList[bodyIndex].GetOverlayList()));

		randomResult = Random.Range(0, 2);
		randomResult = 1; // :TO: always some trousers for women for now, add skirts etc. later
		if (randomResult == 0)
		{
			tempSlotList.Add(GetSlotLibrary().InstantiateSlot(getMappedSlotName("FemaleLegs"), tempSlotList[bodyIndex].GetOverlayList()));
		}
		else if (randomResult == 1)
		{
			tempSlotList.Add(GetSlotLibrary().InstantiateSlot(getMappedSlotName("FemaleLegs"), tempSlotList[bodyIndex].GetOverlayList()));
			if (mappedJeansOverlayName != "") {
				tempSlotList [bodyIndex].AddOverlay (GetOverlayLibrary ().InstantiateOverlay (mappedJeansOverlayName, new Color (Random.Range (0.1f, 0.9f), Random.Range (0.1f, 0.9f), Random.Range (0.1f, 0.9f), 1)));
			}
		}

		string mappedShortHairSlotName = getMappedSlotName ("FemaleShortHair01");
		string mappedLongHairSlotName = getMappedSlotName ("FemaleLongHair01");
		randomResult = Random.Range(0, 3);
		if (randomResult == 0 && mappedShortHairSlotName != "")
		{
			tempSlotList.Add(GetSlotLibrary().InstantiateSlot(mappedShortHairSlotName, tempSlotList[headIndex].GetOverlayList()));
			tempSlotList[headIndex].AddOverlay(GetOverlayLibrary().InstantiateOverlay(getMappedOverlayName("FemaleShortHair01"), HairColor));
		}
		else if (randomResult == 1 && mappedLongHairSlotName != "")
		{
			tempSlotList.Add(GetSlotLibrary().InstantiateSlot(mappedLongHairSlotName, tempSlotList[headIndex].GetOverlayList()));
			tempSlotList[headIndex].AddOverlay(GetOverlayLibrary().InstantiateOverlay(getMappedOverlayName("FemaleLongHair01"), HairColor));
		}
		else if( mappedLongHairSlotName != "")
		{
			tempSlotList.Add(GetSlotLibrary().InstantiateSlot(mappedLongHairSlotName, tempSlotList[headIndex].GetOverlayList()));
			tempSlotList[headIndex].AddOverlay(GetOverlayLibrary().InstantiateOverlay(getMappedOverlayName("FemaleLongHair01"), HairColor));

			tempSlotList.Add(GetSlotLibrary().InstantiateSlot(getMappedSlotName("FemaleLongHair01_Module")));
			tempSlotList[tempSlotList.Count - 1].AddOverlay(GetOverlayLibrary().InstantiateOverlay(getMappedOverlayName("FemaleLongHair01_Module"), HairColor));
		}

		umaData.SetSlots(tempSlotList.ToArray());
	}

	void CreateMale()
	{
		CreateNeutrum ();return;//remove me
		var umaRecipe = umaDynamicAvatar.umaData.umaRecipe;
		umaRecipe.SetRace(raceLibrary.GetRace("HumanMale"));


		// Copied from Bill's code
		int randomResult = 0;
		//Male Avatar

		Color skinColor = new Color(1, 1, 1, 1);
		float skinTone;

		skinTone = Random.Range(0.1f, 0.6f);
		skinColor = new Color(skinTone + Random.Range(0.35f, 0.4f), skinTone + Random.Range(0.25f, 0.4f), skinTone + Random.Range(0.35f, 0.4f), 1);

		Color HairColor = new Color(Random.Range(0.1f, 0.9f), Random.Range(0.1f, 0.9f), Random.Range(0.1f, 0.9f), 1);

		umaData.umaRecipe.slotDataList = new SlotData[15];

		umaData.umaRecipe.slotDataList[0] = GetSlotLibrary().InstantiateSlot(getMappedSlotName("MaleEyes"));
		//umaData.umaRecipe.slotDataList[0].AddOverlay(GetOverlayLibrary().InstantiateOverlay(getMappedOverlayNameName("EyeOverlay")));
		//umaData.umaRecipe.slotDataList[0].AddOverlay(GetOverlayLibrary().InstantiateOverlay(getMappedOverlayName("EyeOverlayAdjust", new Color(Random.Range(0.1f, 0.9f), Random.Range(0.1f, 0.9f), Random.Range(0.1f, 0.9f), 1)))=;

		randomResult = Random.Range(0, 2);
		if (randomResult == 0)
		{
			umaData.umaRecipe.slotDataList[1] = GetSlotLibrary().InstantiateSlot(getMappedSlotName("MaleFace"));

			randomResult = Random.Range(0, 2);

			if (randomResult == 0)
			{
				umaData.umaRecipe.slotDataList[1].AddOverlay(GetOverlayLibrary().InstantiateOverlay(getMappedOverlayName("MaleHead01"), skinColor));
			}
			else if (randomResult == 1)
			{
				umaData.umaRecipe.slotDataList[1].AddOverlay(GetOverlayLibrary().InstantiateOverlay(getMappedOverlayName("MaleHead02"), skinColor));
			}
		}
		else if (randomResult == 1)
		{
			umaData.umaRecipe.slotDataList[1] = GetSlotLibrary().InstantiateSlot(getMappedSlotName("MaleHead_Head"));

			randomResult = Random.Range(0, 2);
			if (randomResult == 0)
			{
				umaData.umaRecipe.slotDataList[1].AddOverlay(GetOverlayLibrary().InstantiateOverlay(getMappedOverlayName("MaleHead01"), skinColor));
			}
			else if (randomResult == 1)
			{
				umaData.umaRecipe.slotDataList[1].AddOverlay(GetOverlayLibrary().InstantiateOverlay(getMappedOverlayName("MaleHead02"), skinColor));
			}

			umaData.umaRecipe.slotDataList[7] = GetSlotLibrary().InstantiateSlot(getMappedSlotName("MaleHead_Eyes"), umaData.umaRecipe.slotDataList[1].GetOverlayList());
			umaData.umaRecipe.slotDataList[9] = GetSlotLibrary().InstantiateSlot(getMappedSlotName("MaleHead_Mouth"), umaData.umaRecipe.slotDataList[1].GetOverlayList());

			randomResult = Random.Range(0, 2);
			if (randomResult == 0)
			{
				//umaData.umaRecipe.slotDataList[10] = GetSlotLibrary().InstantiateSlot(getMappedSlotName("MaleHead_PigNose", umaData.umaRecipe.slotDataList[1].GetOverlayList()));
				//umaData.umaRecipe.slotDataList[1].AddOverlay(GetOverlayLibrary().InstantiateOverlay(getMappedOverlayName("MaleHead_PigNose", skinColor)));
				umaData.umaRecipe.slotDataList[10] = GetSlotLibrary().InstantiateSlot(getMappedSlotName("MaleHead_Nose"), umaData.umaRecipe.slotDataList[1].GetOverlayList());

			}
			else if (randomResult == 1)
			{
				umaData.umaRecipe.slotDataList[10] = GetSlotLibrary().InstantiateSlot(getMappedSlotName("MaleHead_Nose"), umaData.umaRecipe.slotDataList[1].GetOverlayList());
			}

			randomResult = Random.Range(0, 2);
			if (randomResult == 0)
			{
				umaData.umaRecipe.slotDataList[8] = GetSlotLibrary().InstantiateSlot(getMappedSlotName("MaleHead_Ears"), umaData.umaRecipe.slotDataList[1].GetOverlayList());
				// umaData.umaRecipe.slotDataList[8] = GetSlotLibrary().InstantiateSlot(getMappedSlotName("MaleHead_ElvenEars"));
				//	umaData.umaRecipe.slotDataList[8].AddOverlay(GetOverlayLibrary().InstantiateOverlay(getMappedOverlayName("ElvenEars"), skinColor));
			}
			else if (randomResult == 1)
			{
				umaData.umaRecipe.slotDataList[8] = GetSlotLibrary().InstantiateSlot(getMappedSlotName("MaleHead_Ears"), umaData.umaRecipe.slotDataList[1].GetOverlayList());
			}
		}


		randomResult = Random.Range(0, 3);
		if (randomResult == 0)
		{
			umaData.umaRecipe.slotDataList[1].AddOverlay(GetOverlayLibrary().InstantiateOverlay(getMappedOverlayName("MaleHair01"), HairColor * 0.25f));
		}
		else if (randomResult == 1)
		{
			umaData.umaRecipe.slotDataList[1].AddOverlay(GetOverlayLibrary().InstantiateOverlay(getMappedOverlayName("MaleHair02"), HairColor * 0.25f));
		}
		else
		{

		}


		randomResult = Random.Range(0, 4);
		if (randomResult == 0)
		{
			umaData.umaRecipe.slotDataList[1].AddOverlay(GetOverlayLibrary().InstantiateOverlay(getMappedOverlayName("MaleBeard01"), HairColor * 0.15f));
		}
		else if (randomResult == 1)
		{
			umaData.umaRecipe.slotDataList[1].AddOverlay(GetOverlayLibrary().InstantiateOverlay(getMappedOverlayName("MaleBeard02"), HairColor * 0.15f));
		}
		else if (randomResult == 2)
		{
			umaData.umaRecipe.slotDataList[1].AddOverlay(GetOverlayLibrary().InstantiateOverlay(getMappedOverlayName("MaleBeard03"), HairColor * 0.15f));
		}
		else
		{

		}

		//Extra beard composition
		randomResult = Random.Range(0, 4);
		if (randomResult == 0)
		{
			umaData.umaRecipe.slotDataList[1].AddOverlay(GetOverlayLibrary().InstantiateOverlay(getMappedOverlayName("MaleBeard01"), HairColor * 0.15f));
		}
		else if (randomResult == 1)
		{
			umaData.umaRecipe.slotDataList[1].AddOverlay(GetOverlayLibrary().InstantiateOverlay(getMappedOverlayName("MaleBeard02"), HairColor * 0.15f));
		}
		else if (randomResult == 2)
		{
			umaData.umaRecipe.slotDataList[1].AddOverlay(GetOverlayLibrary().InstantiateOverlay(getMappedOverlayName("MaleBeard03"), HairColor * 0.15f));
		}
		else
		{

		}

		randomResult = Random.Range(0, 2);
		if (randomResult == 0)
		{
			umaData.umaRecipe.slotDataList[1].AddOverlay(GetOverlayLibrary().InstantiateOverlay(getMappedOverlayName("MaleEyebrow01"), HairColor * 0.05f));
		}
		else
		{
			umaData.umaRecipe.slotDataList[1].AddOverlay(GetOverlayLibrary().InstantiateOverlay(getMappedOverlayName("MaleEyebrow02"), HairColor * 0.05f));
		}

		umaData.umaRecipe.slotDataList[2] = GetSlotLibrary().InstantiateSlot(getMappedSlotName("MaleTorso"));

		randomResult = Random.Range(0, 2);
		if (randomResult == 0)
		{
			umaData.umaRecipe.slotDataList[2].AddOverlay(GetOverlayLibrary().InstantiateOverlay(getMappedOverlayName("MaleBody01"), skinColor));
		}
		else
		{
			umaData.umaRecipe.slotDataList[2].AddOverlay(GetOverlayLibrary().InstantiateOverlay(getMappedOverlayName("MaleBody02"), skinColor));
		}


		randomResult = Random.Range(0, 2);
		if (randomResult == 0)
		{
			umaData.umaRecipe.slotDataList[2].AddOverlay(GetOverlayLibrary().InstantiateOverlay(getMappedOverlayName("MaleShirt01"), new Color(Random.Range(0.1f, 0.9f), Random.Range(0.1f, 0.9f), Random.Range(0.1f, 0.9f), 1)));
		}

		umaData.umaRecipe.slotDataList[3] = GetSlotLibrary().InstantiateSlot(getMappedSlotName("MaleHands"), umaData.umaRecipe.slotDataList[2].GetOverlayList());

		umaData.umaRecipe.slotDataList[4] = GetSlotLibrary().InstantiateSlot(getMappedSlotName("MaleInnerMouth"));
		umaData.umaRecipe.slotDataList[4].AddOverlay(GetOverlayLibrary().InstantiateOverlay(getMappedOverlayName("InnerMouth")));


		randomResult = Random.Range(0, 2);
		randomResult = 1; // :TO: always trousers for now
		if (randomResult == 0)
		{
			umaData.umaRecipe.slotDataList[4] = GetSlotLibrary().InstantiateSlot(getMappedSlotName("MaleLegs"), umaData.umaRecipe.slotDataList[2].GetOverlayList());
			umaData.umaRecipe.slotDataList[4].AddOverlay(GetOverlayLibrary().InstantiateOverlay(getMappedOverlayName("MaleUnderwear01"), new Color(Random.Range(0.1f, 0.9f), Random.Range(0.1f, 0.9f), Random.Range(0.1f, 0.9f), 1)));
		}
		else
		{
			umaData.umaRecipe.slotDataList[4] = GetSlotLibrary().InstantiateSlot(getMappedSlotName("MaleJeans01"));
			umaData.umaRecipe.slotDataList[4].AddOverlay(GetOverlayLibrary().InstantiateOverlay(getMappedOverlayName("MaleJeans01"), new Color(Random.Range(0.1f, 0.9f), Random.Range(0.1f, 0.9f), Random.Range(0.1f, 0.9f), 1)));
		}

		umaData.umaRecipe.slotDataList[5] = GetSlotLibrary().InstantiateSlot(getMappedSlotName("MaleFeet"), umaData.umaRecipe.slotDataList[2].GetOverlayList());
	}



	// copied from Bill's code
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
}
