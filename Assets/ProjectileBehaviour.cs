using UnityEngine;
using System.Collections;

public class ProjectileBehaviour : MonoBehaviour {

	public float maxLifetime = 2.0f;
	public float speed = 20.0f;

	float m_lifeTime;

	// Use this for initialization
	void Start () {
		m_lifeTime = maxLifetime;
	}
	
	// Update is called once per frame
	void Update () {
		transform.position = transform.position + transform.up * speed * Time.deltaTime;
		m_lifeTime -= Time.deltaTime;
		if (m_lifeTime < 0.0f) {
			Destroy( gameObject );
		}
	}

	GameObject getRootObject( GameObject obj )
	{
		GameObject root = obj; 
		for( ; root.transform.parent != null; root = root.transform.parent.gameObject ){}
		return root;
	}

	void OnCollisionEnter(Collision collision) {
		GameObject rootColliderObject = getRootObject (collision.gameObject);
		if (rootColliderObject.tag == "Human") {
			rootColliderObject.GetComponent<HumanBehavior> ().handleBulletImpact (collision);
		} else if (rootColliderObject.tag == "Zombie") {
			rootColliderObject.GetComponent<ZombieBehavior> ().handleBulletImpact (collision);
		}

		Destroy (gameObject);
	}
}
