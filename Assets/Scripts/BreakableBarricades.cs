using UnityEngine;
using System.Collections;

// on click the model gets a rigid body, ideally falling apart
public class BreakableBarricades : MonoBehaviour {
	private GameObject thisObject;
	public int hitpoints;
	private Rigidbody hasRigidBody;
	// Use this for initialization


		public Rigidbody rb;
		void Start() {
			rb = GetComponent<Rigidbody>();
		EnableRagdoll ();	
		}
		void EnableRagdoll() {
			rb.isKinematic = false;
			rb.detectCollisions = true;
		}
		void DisableRagdoll() {
			rb.isKinematic = true;
			rb.detectCollisions = false;
		}


		

	void OnTriggerEnter(Collider other) {

		if (other.tag == "Zombie")
		{
			hitpoints+= -2; // remove hit points
			if (hitpoints <= 0){
			hasRigidBody = this.GetComponent<Rigidbody>();
			
			//turn off the rigid body of the main object
			if (hasRigidBody != null) {
			DisableRagdoll();
			}
				// iterate over child objects and give them rigidbody
				foreach (Transform child in transform) {
					hasRigidBody = child.gameObject.GetComponent<Rigidbody>();
					// or do something else with child.gameObject
					if(hasRigidBody == null)
					{
						Rigidbody gameObjectsRigidBody = child.gameObject.AddComponent<Rigidbody> (); // Add the rigidbody.
					gameObjectsRigidBody.mass = 1; // Set the GO's mass to 5 via the Rigidbody.
					}
				}
			}
	}
	}

	void OnTriggerStay(Collider other) {
		if (other.tag == "Zombie")
		{
			hitpoints+= -1; // remove hit points
			if (hitpoints <= 0){
				hasRigidBody = this.GetComponent<Rigidbody>();
				if (hasRigidBody != null) {
					DisableRagdoll();
				}
				//for (int i = 0; i <= 5; i++) {
				//	hasRigidBody = theseObjects [i].GetComponent<Rigidbody>();
				foreach (Transform child in transform) {
					hasRigidBody = child.gameObject.GetComponent<Rigidbody>();
					if(hasRigidBody == null)
					{
						Rigidbody gameObjectsRigidBody = child.gameObject.AddComponent<Rigidbody> (); // Add the rigidbody.
						gameObjectsRigidBody.mass = 1; // Set the GO's mass to 5 via the Rigidbody.
					}
				}
			}
		}


		
	}
	/*void OnMouseDown() {
		//Application.LoadLevel("SomeLevel");
		for (int i = 1; i <= 5; i++) {
			hasRigidBody = theseObjects [i].GetComponent<Rigidbody>();
			if(hasRigidBody == null)
			{
				Rigidbody gameObjectsRigidBody = theseObjects[i].AddComponent<Rigidbody> (); // Add the rigidbody.
			gameObjectsRigidBody.mass = 1; // Set the GO's mass to 5 via the Rigidbody.
			}
		}*/
		


	
		

	// end ClickedRigid.cs
}
