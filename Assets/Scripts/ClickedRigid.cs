using UnityEngine;
using System.Collections;

// on click the model gets a rigid body, ideally falling apart
public class ClickedRigid : MonoBehaviour {
	public GameObject[] gameObjects = new GameObject[6];
	public Rigidbody hasRigidBody;
	// Use this for initialization

	void OnTriggerEnter(Collider other) {


	}

	void OnMouseDown() {
		//Application.LoadLevel("SomeLevel");
		for (int i = 1; i <= 5; i++) {
			 hasRigidBody = gameObjects [i].GetComponent<Rigidbody>();
			if(hasRigidBody == null)
			{
			Rigidbody gameObjectsRigidBody = gameObjects[i].AddComponent<Rigidbody> (); // Add the rigidbody.
			gameObjectsRigidBody.mass = 1; // Set the GO's mass to 5 via the Rigidbody.
			}
		}
		
	}

	void OnCollisionEnter(Collision collision) {
	
	
		
	}
	// end ClickedRigid.cs
}
