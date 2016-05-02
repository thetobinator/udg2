using UnityEngine;
using System.Collections;

public class DestinationMarkerBehaviour : MonoBehaviour {
	private float m_lifeTime = 1.0f;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		m_lifeTime -= Time.deltaTime * 2.0f;
		float size = Mathf.Sin (Mathf.PI * m_lifeTime);
		transform.localScale = new Vector3 (size, size, size);
		if (m_lifeTime <= 0.0f) {
			Destroy ( gameObject );
		}
	}
}
