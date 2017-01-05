using UnityEngine;
using System.Collections;

public class CrosshairBehaviour : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		DebugTint d = GetComponent<DebugTint> ();
		if (d != null) {
			d.tintColor = Color.green;
			Ray ray = Camera.main.GetComponent<Camera> ().ScreenPointToRay (Input.mousePosition);
			RaycastHit hit = new RaycastHit ();
			if (Physics.Raycast (ray, out hit)) {
				GameObject obj = ZombieBehavior.getRootObject( hit.collider.gameObject.transform.parent.gameObject );
				if (obj.tag == "Human") {
					d.tintColor = Color.red;
				}
			}
		}
	}
}
