using UnityEngine;
using System.Collections;

public class WalkToClick : MonoBehaviour {
	public Camera m_camera;
	bool m_hasDestination = false;
	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetMouseButtonUp (0)) {
			Ray ray = m_camera.GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
			RaycastHit hit = new RaycastHit();
			if (Physics.Raycast(ray, out hit)) {
				GetComponent<NavMeshAgent>().SetDestination( hit.point );
				m_hasDestination = true;
			}
		}

		if( m_hasDestination )
		{
			Vector3 diff = GetComponent<Transform> ().position - GetComponent<NavMeshAgent>().destination;
			if( diff.magnitude > 0.5f )
			{
				GetComponent<Animator> ().SetFloat ("speed", 1.0f );
			}
			else
			{
				GetComponent<Animator> ().SetFloat ("speed", 0.0f );
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
