using UnityEngine;
using System.Collections;

public class HealthComponent : MonoBehaviour {
	public float initialHealth = 100.0f;
    
	public float current_health;

	// Use this for initialization
	void Start () {
		reanimate ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void dealDamage( float damage ) {
        current_health -= damage;
	}

	public bool isDead() {
		return current_health <= 0.0f;
	}

	public void reanimate() {
        current_health = initialHealth;
	}
}
