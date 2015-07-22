using UnityEngine;
using System.Collections;

public class ZombieBehavior : MonoBehaviour {
	public Camera m_camera;
	public GameObject taskObject;
	//public GameObject[] furniture;
	//public GameObject[] humans;
	//public GameObject[] zombies;
	//public GameObject[] respawns;
	public GameObject TargetObject;
	GameObject previousObject;
	bool m_hasDestination = false;
	Vector3 m_oldPosition;

	// Use this for initialization
	void Start () {
	
	}
	void ZombieWalk(){
		if (TargetObject != null){
			GetComponent<NavMeshAgent> ().SetDestination (TargetObject.transform.position);
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

	}// end zombiewalk

	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown ("f")){
	
			GameObject[] TargetObjects =   GameObject.FindGameObjectsWithTag("Player");
			GetComponent<NavMeshAgent> ().SetDestination (TargetObjects[0].transform.position);
			m_hasDestination = true;
			m_oldPosition = GetComponent<Transform> ().position;
	
		}

		if (Input.GetKeyDown ("r")){

			GameObject[] humans =  GameObject.FindGameObjectsWithTag("Human");

			GetComponent<NavMeshAgent> ().SetDestination (humans[Random.Range (0,humans.Length)].transform.position);
			m_hasDestination = true;
			m_oldPosition = GetComponent<Transform> ().position;
		}

		if (m_hasDestination) {
			ZombieWalk ();
		}

	}
}
