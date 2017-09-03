using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfDestructor : MonoBehaviour {

	public float lifeTime = 3.0f;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		lifeTime -= Time.deltaTime;
		if (lifeTime <= 0.0f) {
			Destroy (gameObject);
		}
	}
}
