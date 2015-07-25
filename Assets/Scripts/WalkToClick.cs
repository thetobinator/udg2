using UnityEngine;
using System.Collections;

public class WalkToClick : MonoBehaviour {
	public Camera m_camera;
	public GameObject managedObject;
	public GameObject[] taskObject;
	public GameObject[] FollowTarget;
	public GameObject[] furniture; // = GameObject.FindGameObjectsWithTag("Furniture");
	public GameObject[] humans; // = GameObject.FindGameObjectsWithTag("Human");
	public GameObject[] zombies; // = GameObject.FindGameObjectsWithTag("Zombie");
	public GameObject[] boxes; // = GameObject.FindGameObjectsWithTag ("Box");
	public GameObject[] glass; // = GameObject.FindGameObjectsWithTag ("Window");
	public GameObject[] door; // = GameObject.FindGameObjectsWithTag ("Door");
	public GameObject[] barricade; // = GameObject.FindGameObjectsWithTag ("Barricade");
	public string[] Taglist = new string[] {"Barricade","Box","Door","Furniture","Human","Zombie"};
	public string[] TargetTag;
	public GameObject[] previousObject;
	bool[] m_hasDestination;
	Vector3[] m_oldPosition;
	int managedIndex;

	//Transform[] hinges = GameObject.FindObjectsOfType (typeof(Transform)) as Transform[];

	void Start() {
	
	}

	// stop the character at a barricade
	void OnCollisionEnter(Collision collision) {
		//Debug.Log (collision.gameObject.tag);
		if (collision.gameObject.tag == "Human")
		{
			if (managedObject.tag == "Human"){
				m_hasDestination[managedIndex] = false;
				previousObject[managedIndex] = taskObject[managedIndex];
				taskObject[managedIndex] = null;
			}
		}
		// if hit a door, barricade or human go somewhere else. or if zombie target the object
		if(collision.gameObject.tag == "Barricade" || collision.gameObject.tag == "Door" || collision.gameObject.tag == "Human")
		{
			GetComponent<Animator> ().SetFloat ("speed", 0.0f );
		
			if (managedObject.tag == "Human")
				{
				previousObject[managedIndex] = taskObject[managedIndex];
				taskObject[managedIndex] = null;
				m_hasDestination[managedIndex] = false;
				}
				else
				{
				previousObject[managedIndex] = taskObject[managedIndex];
				taskObject[managedIndex] = collision.gameObject;
				m_hasDestination[managedIndex] = true;
				}

		}
	}


		void GoToTag(string Tag , GameObject managedObject){
		GameObject[] taggedObjects = GameObject.FindGameObjectsWithTag (Tag);
		if (taggedObjects.Length >= 1)
		{
		int RandNum =  Random.Range (1, taggedObjects.Length);
	//	print (Tag + "\t" + "R=" + RandNum + "\tLength=" + taggedObjects.Length);

			taskObject[managedIndex] = taggedObjects [RandNum];
		}
		}

	// Update is called once per frame
	void Update () {
		GameObject[] taggedObjects = GameObject.FindGameObjectsWithTag ("Zombie");

		for (int i=0;i<taggedObjects.Length;i++) {
			GameObject managedObject = taggedObjects[i];
			managedIndex = i; 

		if (Input.GetMouseButtonUp (0)) {
			previousObject = null;
			Ray ray = m_camera.GetComponent<Camera> ().ScreenPointToRay (Input.mousePosition);
			RaycastHit hit = new RaycastHit ();
			if (Physics.Raycast (ray, out hit)) {

					if (managedObject.tag == "Zombie"){
					if( hit.collider.tag != "Terrain" || hit.collider.tag != "Zombie")
					{
							previousObject[managedIndex] = taskObject[managedIndex];
							taskObject[managedIndex] = hit.collider.gameObject;
					}
					else
					{
					   GetComponent<NavMeshAgent> ().SetDestination (hit.point);
							previousObject[managedIndex] = taskObject[managedIndex];
							taskObject[managedIndex] = null;
					}
				}

					m_hasDestination[managedIndex] = true;
					m_oldPosition[managedIndex] = GetComponent<Transform> ().position;
			}
		}

		if (Input.GetKeyDown ("f")){
				if (tag == "Zombie"){
					taskObject[managedIndex] =  GameObject.FindWithTag("Player");
			}				
		}
		
		if (Input.GetKeyDown ("r")){
				if (managedObject.tag == "Zombie"){
				GoToTag ("Human",managedObject);
			
			}				
		}


			if (!m_hasDestination[managedIndex]) {
				if (managedObject.tag == "Human"){
					if (!taskObject[managedIndex]) {
					//do some tag decisions.
					//string[] Taglist = new string[] {"Barricade","Box","Door","Furniture","Human","Zombie"};
						TargetTag[managedIndex] = Taglist[Random.Range (0,Taglist.Length)];
					}
			}
			else
			{
					TargetTag[managedIndex] = "Human";
			}		
				if (taskObject[managedIndex] != null)
					{
					GoToTag (taskObject[managedIndex], taggedObjects[i]);//why won't this work? I must sleep.
					}
					
						
					
			}

			if (taskObject[managedIndex]) {
			GetComponent<NavMeshAgent> ().SetDestination (taskObject.transform.position);
				m_hasDestination[managedIndex] = true;
		}

	

			if (m_hasDestination[managedIndex]) {
			Vector3 movement = GetComponent<Transform> ().position - m_oldPosition;
				m_oldPosition[managedIndex] = GetComponent<Transform> ().position;
			Vector3 diff = GetComponent<Transform> ().position - GetComponent<NavMeshAgent> ().destination;
			if (GetComponent<Animator> ()) {
				if (diff.magnitude > 0.7f) {
					GetComponent<Animator> ().SetFloat ("speed", movement.magnitude / Time.deltaTime);
				} else {
					GetComponent<Animator> ().SetFloat ("speed", 0.0f);
					//print ( "REACHED" );
						m_hasDestination[managedIndex] = false;
						previousObject[managedIndex] = taskObject[managedIndex];
						taskObject[managedIndex] = null;
				}
			} else {
					managedObject.transform.Translate (Vector3.forward * Time.deltaTime);
			}

		}

	}// end update

			//m_oldPosition = GetComponent<Transform> ().position;
			/*
			if( diff.magnitude * 5.0f < 0.1f )
			{
				GetComponent<Animator> ().SetFloat ("speed", 0.0f );
				m_hasDestination = false;
			}
			*/
	}//end loop through all tagged objects
	//end WalkToClick.cs
}
