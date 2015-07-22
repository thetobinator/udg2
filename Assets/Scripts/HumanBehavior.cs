using UnityEngine;
using System.Collections;

public class HumanBehavior : MonoBehaviour {
	public Camera m_camera;
	public GameObject taskObject;
	public GameObject[] furniture;
	public GameObject[] humans;
	public GameObject[] zombies;
	public GameObject[] respawns;
	public GameObject FollowTarget;
	
	GameObject previousObject;
	bool m_hasDestination = false;
	Vector3 m_oldPosition;
	// Use this for initialization
	void Start () {
	
	}
	// stop the character at a barricade
	void OnCollisionEnter(Collision collision) {
		//Debug.Log (collision.gameObject.tag);
		if (collision.gameObject.tag == "Human")
		{
			if (this.tag == "Human"){GoToABarricade(taskObject);}
		}
		if(collision.gameObject.tag == "Barricade")
		{
			GetComponent<Animator> ().SetFloat ("speed", 0.2f );
			//m_oldPosition = this.GetComponent<Transform>().position;
			GetComponent<NavMeshAgent>().SetDestination(this.GetComponent<Transform>().position); //this.GetComponent<Transform>().position );
			
			m_hasDestination = true;
			if (this.tag == "Human"){GoToABarricade(taskObject);}
		}
	}
	
	void GoToABarricade(GameObject gameObject){
		//Debug.Log (  taskObject.transform.childCount);
		
		// if you want the transform component
		Transform[] childsT = new Transform[gameObject.transform.childCount];
		// if you want the underlying GameObject
		GameObject[] childsG = new GameObject[gameObject.transform.childCount];
		int i=0;
		int RNDObject =  Random.Range (0, gameObject.transform.childCount-1);
		
		if (childsG[RNDObject] == previousObject) {
			gameObject = furniture[Random.Range (1,furniture.Length)];
			RNDObject =  Random.Range (0, gameObject.transform.childCount-1);
		}
		foreach(Transform child in taskObject.transform)
		{
			
			childsT[i] = child.transform;
			childsG[i] = child.gameObject;
			i++;
		}
		
		previousObject = childsG[RNDObject];
		GetComponent<NavMeshAgent> ().SetDestination (childsT[RNDObject].transform.position);
		m_hasDestination = true;
		m_oldPosition = GetComponent<Transform> ().position;
	}
	
	void GoToAHuman(GameObject gameObject){
		//Debug.Log (  taskObject.transform.childCount);
		// if you want the transform component
		Transform[] childsT = new Transform[gameObject.transform.childCount];
		// if you want the underlying GameObject
		GameObject[] childsG = new GameObject[gameObject.transform.childCount];
		int i=0;
		int RNDObject =  Random.Range (0, gameObject.transform.childCount-1);
		foreach(Transform child in taskObject.transform)
		{
			childsT[i] = child.transform;
			childsG[i] = child.gameObject;
			i++;
		}
		previousObject = childsG [RNDObject];
		GetComponent<NavMeshAgent> ().SetDestination (childsT[RNDObject].transform.position);
		m_hasDestination = true;
		m_oldPosition = GetComponent<Transform> ().position;
	}
	// Update is called once per frame
	void Update () {
		// Update is called once per frame
	
		if (Input.GetMouseButtonUp (0)) {
			previousObject = null;
			Ray ray = m_camera.GetComponent<Camera> ().ScreenPointToRay (Input.mousePosition);
			RaycastHit hit = new RaycastHit ();
			if (Physics.Raycast (ray, out hit)) {
				GetComponent<NavMeshAgent> ().SetDestination (hit.point);
				m_hasDestination = true;
				m_oldPosition = GetComponent<Transform> ().position;
				previousObject = null;
			}
		}
			
		if (!m_hasDestination) {
			if (this.tag == "Human") {
				m_hasDestination = true;
				m_oldPosition = GetComponent<Transform> ().position;
				GoToABarricade (taskObject);
			}	
		}
			
		if (this.tag == "Zombie" && previousObject != null) {
			GetComponent<NavMeshAgent> ().SetDestination (previousObject.transform.position);
			m_hasDestination = true;
			//m_oldPosition = GetComponent<Transform> ().position;
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
				}
			} else {
				this.transform.Translate (Vector3.forward * Time.deltaTime);
			}
				
		}
	}//end update	
}
