using UnityEngine;
using System.Collections;

public class DisableConstraints : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
    void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.tag == "Zombie") { 
      Rigidbody RB =   this.GetComponent<Rigidbody>() as Rigidbody;
      RB.constraints = RigidbodyConstraints.None;
        }
    }
}
