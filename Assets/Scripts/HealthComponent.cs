using UnityEngine;
using System.Collections;

public class HealthComponent : MonoBehaviour {
	public float initialHealth = 100.0f;
	float m_health;

	// Use this for initialization
	void Start () {
		reanimate ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void dealDamage( float damage ) {
		m_health -= damage;
	}

	public bool isDead() {
		return m_health <= 0.0f;
	}

	public void reanimate() {
		m_health = initialHealth;
	}
}
