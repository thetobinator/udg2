using UnityEngine;
using System.Collections;

public class HumanBehavior : MonoBehaviour {
	//public Camera m_camera;
	public GameObject taskObject;
    public string TargetTag;
    public GameObject previousObject;
    



    string[] humansSeekTaglist = new string[] { "Barricade", "Box", "Door", "Furniture", "Human", "Window", "Zombie" };
	
	bool m_hasDestination = false;
	Vector3 m_oldPosition;
	
	//Transform[] hinges = GameObject.FindObjectsOfType (typeof(Transform)) as Transform[];
	
	void Start() {
        //m_camera = Camera.main;
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
        if (collision.gameObject.tag == "Barricade" || collision.gameObject.tag == "Door" || collision.gameObject.tag == "Window" || collision.gameObject.tag == "Human")
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
                taskObject = null;//collision.gameObject;
				m_hasDestination = false;
			}
			
		}
	}

	void GoToTag(string Tag){
		GameObject[] taggedObjects = GameObject.FindGameObjectsWithTag (Tag);
		if (taggedObjects.Length >= 1)
		{
			int RandNum =  Random.Range (0, taggedObjects.Length);
			
			taskObject = taggedObjects [RandNum].gameObject;
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (!m_hasDestination) {
			if (this.tag == "Human"){
				if (!taskObject) {
					//do some tag decisions.
					//string[] Taglist = new string[] {"Barricade","Box","Door","Furniture","Human","Zombie"};
                    TargetTag = humansSeekTaglist[Random.Range(0, humansSeekTaglist.Length)];
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
            //print("HumanBehavior:"  + " Parent: " + this.transform.parent.gameObject + " This Object = " + this.gameObject + " taskObject = " + taskObject +"\n");
          
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