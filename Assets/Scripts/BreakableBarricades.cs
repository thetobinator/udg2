﻿using UnityEngine;
using System.Collections;

// on click the model gets a rigid body, ideally falling apart
public class BreakableBarricades : MonoBehaviour {
	private GameObject thisObject;
	public int hitpoints;
	private Rigidbody hasRigidBody;
	private Vector3 childscale;
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
				// iterate over child objects and give them rigidbody
				foreach (Transform child in transform) {
					hasRigidBody = child.gameObject.GetComponent<Rigidbody>();
					// break the object apart
					if(hasRigidBody == null)
					{
						Vector3 childscale = child.gameObject.transform.localScale;
						
						childscale.x = childscale.x * 0.75F;
						childscale.y = childscale.y * 0.75F;
						childscale.z = childscale.z * 0.75F;
						child.gameObject.transform.localScale = new Vector3(childscale.x, childscale.y, childscale.z);
						Rigidbody gameObjectsRigidBody = child.gameObject.AddComponent<Rigidbody> (); // Add the rigidbody.
						gameObjectsRigidBody.mass = 1; // Set the GO's mass to 5 via the Rigidbody.

					}
				}
			}
		}


		
	}
	/*void OnMouseDown() {
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
