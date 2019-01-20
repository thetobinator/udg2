using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarBehaviour : MonoBehaviour
{
	public float horsePower;

	Vector3 m_startPosition;
	Quaternion m_startRotation;
	float m_lifeTime = 0.0f;
	float m_initialHorsePower = 0.0f;
	bool m_deccelerate = false;

    // Start is called before the first frame update
    void Start()
    {
		m_startPosition = transform.position;
		m_startRotation = transform.rotation;
		m_initialHorsePower = horsePower;
    }

    // Update is called once per frame
    void Update()
    {
		m_lifeTime += Time.deltaTime;
		if (m_lifeTime > 15.0f) {
			m_lifeTime = 0.0f;
			m_deccelerate = false;
			horsePower = m_initialHorsePower;
			transform.position = m_startPosition;
			transform.rotation = m_startRotation;
		}
		if (m_deccelerate) {
			horsePower = Mathf.Max(horsePower - Time.deltaTime * 5, 0);
		}
		Rigidbody rb = GetComponent<Rigidbody>();
		if( rb.velocity.magnitude < 10.0f )
		{
			Vector3 force = transform.forward;
			force.y = 0.0f;
			force.Normalize();

			force *= horsePower;
			rb.AddForce(force, ForceMode.VelocityChange);
		}
    }


	void OnCollisionEnter(Collision collision)
	{
		GameObject rootColliderObject = ProjectileBehaviour.getRootObject (collision.gameObject);
		HealthComponent colliderHealth = rootColliderObject.GetComponent<HealthComponent>();
	

		if (colliderHealth != null && !colliderHealth.isDead())
		{
			m_deccelerate = true;
			if (rootColliderObject.tag == "Human" )
			{
				HumanBehavior hb = rootColliderObject.GetComponent<HumanBehavior>();
				if (hb != null)
				{
					hb.handleVehicleImpact(collision);
				}
			}
			else if (rootColliderObject.tag == "Zombie")
			{
				ZombieBehavior zb = rootColliderObject.GetComponent<ZombieBehavior>();
				if (zb != null)
				{
					zb.handleVehicleImpact(collision);
				}
			}
		} else if (collision.rigidbody != null) {
			collision.rigidbody.AddForce(GetComponent<Rigidbody>().velocity, ForceMode.VelocityChange);
		}
	}
}
