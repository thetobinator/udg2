using UnityEngine;
using System.Collections;

public class ZombieBehavior : MonoBehaviour {
    public Camera m_camera;
	public GameObject taskObject;

	GameObject previousObject;
	bool m_hasDestination = false;
	Vector3 m_oldPosition;
	
	//Transform[] hinges = GameObject.FindObjectsOfType (typeof(Transform)) as Transform[];

    void GoToTag(string Tag)
    {
        GameObject[] taggedObjects = GameObject.FindGameObjectsWithTag(Tag);
        if (taggedObjects.Length >= 1)
        {
           Random.seed = (int) Time.time;
            int RandNum = Random.Range(1, taggedObjects.Length);
         
            taskObject = taggedObjects[RandNum];
            GetComponent<NavMeshAgent>().SetDestination(taskObject.transform.position);
            m_oldPosition = GetComponent<Transform>().position;
            m_hasDestination = true;
        }
    }

    void Start()
    {
        GoToTag("Player");
    }
	
	// stop the character at a barricade
	void OnCollisionEnter(Collision collision) {
    
		//Debug.Log (collision.gameObject.tag);
		if (collision.gameObject.tag == "Human")
		{
			if (this.tag == "Human"){
				m_hasDestination = false;
				
				taskObject = null;
			}
		}
		// if hit a door, barricade or human go somewhere else. or if zombie target the object
        if (collision.gameObject.tag == "Barricade" || collision.gameObject.tag == "Door" || collision.gameObject.tag == "Window" || collision.gameObject.tag == "Human")
		{
			GetComponent<Animator> ().SetFloat ("speed", 0.2f );
			
			if (this.tag == "Human")
			{
				
                taskObject = collision.gameObject;
				m_hasDestination = true;
			}
			else
			{
				
				taskObject = collision.gameObject;
				m_hasDestination = true;
			}
			
		}
	}


	
	// Update is called once per frame
	void Update () {
		m_camera =   Camera.main;
       
		if (Input.GetMouseButtonUp (0)) {
			
			Ray ray = m_camera.GetComponent<Camera> ().ScreenPointToRay (Input.mousePosition);
			RaycastHit hit = new RaycastHit ();
			if (Physics.Raycast (ray, out hit)) {

						GetComponent<NavMeshAgent> ().SetDestination (hit.point);
						
                        if (hit.collider.tag == "Terrain" ) { taskObject = null; }
				m_hasDestination = true;
                m_oldPosition = GetComponent<Transform>().position;
			}
		}

		if (Input.GetKeyDown ("f")){
           // print("f key");
				taskObject =  GameObject.FindWithTag("Player");
                GetComponent<NavMeshAgent>().SetDestination(taskObject.transform.position);
                m_oldPosition = GetComponent<Transform>().position;
                m_hasDestination = true;					
		}

		if (Input.GetKeyDown ("r")){
           // print("r key");
				GoToTag ("Human");
                m_oldPosition = GetComponent<Transform>().position;					
		}

        if (Input.GetKeyDown("b"))
        {
            // print("r key");
            GoToTag("Barricade");
            m_oldPosition = GetComponent<Transform>().position;
        }

        if (Input.GetKeyDown("g"))
        {
            // print("r key");
            GoToTag("Window");
            m_oldPosition = GetComponent<Transform>().position;
        }

		if (taskObject) {
           // print("ZombieBehavior:" + " Parent: " + this.transform.parent.gameObject + " This Object = " + this.gameObject + " taskObject = " + taskObject + "\n");
            GetComponent<NavMeshAgent>().SetDestination(taskObject.transform.position);
           
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
	
	
	//end ZombieBehavior.cs
}