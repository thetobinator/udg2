﻿using UnityEngine;
using System.Collections;

public class DestinationMarkerBehaviour : MonoBehaviour {
	private float m_lifeTime = 1.0f;
	public GameObject trackedObject = null;
	// Use this for initialization
	void Start () {
		transform.localScale = new Vector3( 0.0f, 0.0f, 0.0f );
	}
	
	// Update is called once per frame
	void Update () {
		m_lifeTime -= Time.deltaTime * 2.0f;
		if (trackedObject != null) {
			m_lifeTime = 0.5f;
			transform.position = trackedObject.transform.position;
		}
		float size = Mathf.Sin (Mathf.PI * m_lifeTime);
		transform.localScale = new Vector3 (size, size, size);
		if (m_lifeTime <= 0.0f) {
			Destroy ( gameObject );
		}
	}
}
