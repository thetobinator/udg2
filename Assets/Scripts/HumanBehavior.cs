using UnityEngine;
using System.Collections;

public class HumanBehavior : MonoBehaviour {
	public Camera m_camera;
	public GameObject taskObject;
	public GameObject FollowTarget;
	public GameObject[] furniture; // = GameObject.FindGameObjectsWithTag("Furniture");
	public GameObject[] humans; // = GameObject.FindGameObjectsWithTag("Human");
	public GameObject[] zombies; // = GameObject.FindGameObjectsWithTag("Zombie");
	public GameObject[] boxes; // = GameObject.FindGameObjectsWithTag ("Box");
	public GameObject[] glass; // = GameObject.FindGameObjectsWithTag ("Window");
	public GameObject[] door; // = GameObject.FindGameObjectsWithTag ("Door");
	public GameObject[] barricade; // = GameObject.FindGameObjectsWithTag ("Barricade");
	string[] Taglist = new string[] {"Barricade","Box","Door","Furniture","Human","Zombie"};
	string TargetTag;
	GameObject previousObject;
	bool m_hasDestination = false;
	Vector3 m_oldPosition;
	
	//Transform[] hinges = GameObject.FindObjectsOfType (typeof(Transform)) as Transform[];
	
	void Start() {
		
	}
	
	// stop the character at a barricade
	void OnCollisionEnter(Collision collision) {
		//Debug.Log (collision.gameObject.tag);
		if (collision.gameObject.tag == "Human")
		{
			if (this.tag == "Human"){
				m_hasDestination = false;
				previousObject = taskObject;
				taskObject = null;
			}
		}
		// if hit a door, barricade or human go somewhere else. or if zombie target the object
		if(collision.gameObject.tag == "Barricade" || collision.gameObject.tag == "Door" || collision.gameObject.tag == "Human")
		{
			GetComponent<Animator> ().SetFloat ("speed", 0.0f );
			
			if (this.tag == "Human")
			{
				previousObject = taskObject;
				taskObject = null;
				m_hasDestination = false;
			}
			else
			{
				previousObject = taskObject;
				taskObject = collision.gameObject;
				m_hasDestination = true;
			}
			
		}
	}

	void GoToTag(string Tag){
		GameObject[] taggedObjects = GameObject.FindGameObjectsWithTag (Tag);
		if (taggedObjects.Length >= 1)
		{
			int RandNum =  Random.Range (1, taggedObjects.Length);
			//	print (Tag + "\t" + "R=" + RandNum + "\tLength=" + taggedObjects.Length);
			
			taskObject = taggedObjects [RandNum];
		}
	}
	
	// Update is called once per frame
	void Update () {
		
		/*if (Input.GetMouseButtonUp (0)) {
			previousObject = null;
			Ray ray = m_camera.GetComponent<Camera> ().ScreenPointToRay (Input.mousePosition);
			RaycastHit hit = new RaycastHit ();
			if (Physics.Raycast (ray, out hit)) {
				
				if (this.tag == "Zombie"){
					if( hit.collider.tag != "Terrain" || hit.collider.tag != "Zombie")
					{
						previousObject = taskObject;
						taskObject = hit.collider.gameObject;
					}
					else
					{
						GetComponent<NavMeshAgent> ().SetDestination (hit.point);
						previousObject = taskObject;
						taskObject = null;
					}
				}
				
				m_hasDestination = true;
				m_oldPosition = GetComponent<Transform> ().position;
			}
		}
		
		if (Input.GetKeyDown ("f")){
			if (this.tag == "Zombie"){
				taskObject =  GameObject.FindWithTag("Player");
			}				
		}
		
		if (Input.GetKeyDown ("r")){
			if (this.tag == "Zombie"){
				GoToTag ("Human");
				
			}				
		}
*/
		
		if (!m_hasDestination) {
			if (this.tag == "Human"){
				if (!taskObject) {
					//do some tag decisions.
					//string[] Taglist = new string[] {"Barricade","Box","Door","Furniture","Human","Zombie"};
					TargetTag = Taglist[Random.Range (0,Taglist.Length)];
				}
			}
			else
			{
				TargetTag = "Human";
			}		
			if (TargetTag != null)
			{
				GoToTag (TargetTag);
			}

		}
		
		if (taskObject) {
			GetComponent<NavMeshAgent> ().SetDestination (taskObject.transform.position);
			m_hasDestination = true;
		}

		if (m_hasDestination) {
			Vector3 movement = GetComponent<Transform> ().position - m_oldPosition;
			m_oldPosition = GetComponent<Transform> ().position;
			Vector3 diff = GetComponent<Transform> ().position - GetComponent<NavMeshAgent> ().destination;
			if (GetComponent<Animator> ()) {
				if (diff.magnitude > 0.7f) {
					GetComponent<Animator> ().SetFloat ("speed", movement.magnitude / Time.deltaTime);
				} else {
					GetComponent<Animator> ().SetFloat ("speed", 0.0f);
					//print ( "REACHED" );
					m_hasDestination = false;
					previousObject = taskObject;
					taskObject = null;
				}
			} else {
				this.transform.Translate (Vector3.forward * Time.deltaTime);
			}
			
		}
		
	}// end update
	

	
	//end HumanBehavior.cs
}