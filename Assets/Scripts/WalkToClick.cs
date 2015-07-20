using UnityEngine;
using System.Collections;

public class WalkToClick : MonoBehaviour {
	public Camera m_camera;
	bool m_hasDestination = false;
	Vector3 m_oldPosition;
	// Use this for initialization
	void Start () {

	}
	// stop the character at a barricade
	void OnCollisionEnter(Collision collision) {
		//Debug.Log (collision.gameObject.tag);
		if(collision.gameObject.tag == "Barricade")
		{
			GetComponent<Animator> ().SetFloat ("speed", 0.2f );
			//m_oldPosition = this.GetComponent<Transform>().position;
			GetComponent<NavMeshAgent>().SetDestination(this.GetComponent<Transform>().position); //this.GetComponent<Transform>().position );

			m_hasDestination = true;
			
		}
	}

	// Update is called once per frame
	void Update () {
		if (Input.GetMouseButtonUp (0)) {
			Ray ray = m_camera.GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
			RaycastHit hit = new RaycastHit();
			if (Physics.Raycast(ray, out hit)) {
				GetComponent<NavMeshAgent>().SetDestination( hit.point );
				m_hasDestination = true;
				m_oldPosition = GetComponent<Transform>().position;
			}
		}

		if( m_hasDestination )
		{
			Vector3 movement = GetComponent<Transform>().position - m_oldPosition;
			m_oldPosition = GetComponent<Transform>().position;
			Vector3 diff = GetComponent<Transform> ().position - GetComponent<NavMeshAgent>().destination;
			if( diff.magnitude > 0.7f )
			{
				GetComponent<Animator> ().SetFloat ("speed", movement.magnitude / Time.deltaTime );
			}
			else
			{
				GetComponent<Animator> ().SetFloat ("speed", 0.0f );
				//print ( "REACHED" );
				m_hasDestination = false;
			}

			//m_oldPosition = GetComponent<Transform> ().position;
			/*
			if( diff.magnitude * 5.0f < 0.1f )
			{
				GetComponent<Animator> ().SetFloat ("speed", 0.0f );
				m_hasDestination = false;
			}
			*/
		}
	}
}
