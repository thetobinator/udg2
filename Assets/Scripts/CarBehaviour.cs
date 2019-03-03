using System.Collections;
using System.Collections.Generic;
using UnityEngine;


using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class AxleInfo {
	public WheelCollider leftWheel;
	public WheelCollider rightWheel;
	public bool motor;
	public bool steering;
}

public class CarBehaviour : MonoBehaviour {
	public List<AxleInfo> axleInfos; 
	public float maxMotorTorque;
	public float maxSteeringAngle;
	Vector3 m_startPosition;
	Quaternion m_startRotation;
	float m_lifeTime = 0.0f;
	float m_steering = 0.0f;
	float m_travelDirection = 1.0f;

	// finds the corresponding visual wheel
	// correctly applies the transform
	public void ApplyLocalPositionToVisuals(WheelCollider collider)
	{
		if (collider.transform.childCount == 0) {
			return;
		}

		Transform visualWheel = collider.transform.GetChild(0);

		Vector3 position;
		Quaternion rotation;
		collider.GetWorldPose(out position, out rotation);

		visualWheel.transform.position = position;
		visualWheel.transform.rotation = rotation;
	}

	void Start()
	{
		m_startPosition = transform.position;
		m_startRotation = transform.rotation;
	}

	public void FixedUpdate()
	{
		float motor = maxMotorTorque * m_travelDirection;//Input.GetAxis("Vertical");
		float steering = m_steering;//maxSteeringAngle * Input.GetAxis("Horizontal");

		foreach (AxleInfo axleInfo in axleInfos) {
			if (axleInfo.steering) {
				axleInfo.leftWheel.steerAngle = steering;
				axleInfo.rightWheel.steerAngle = steering;
			}
			if (axleInfo.motor) {
				axleInfo.leftWheel.motorTorque = motor;
				axleInfo.rightWheel.motorTorque = motor;
			}
			ApplyLocalPositionToVisuals(axleInfo.leftWheel);
			ApplyLocalPositionToVisuals(axleInfo.rightWheel);
		}

		m_lifeTime += Time.deltaTime;
		if (m_lifeTime > 15.0f) {
			m_lifeTime = 0.0f;
			m_travelDirection = 1.0f;
			m_steering = 0.0f;
			transform.position = m_startPosition;
			transform.rotation = m_startRotation;
		}
	}

	void OnCollisionEnter(Collision collision)
	{
		GameObject rootColliderObject = ProjectileBehaviour.getRootObject (collision.gameObject);
		HealthComponent colliderHealth = rootColliderObject.GetComponent<HealthComponent>();

		if (colliderHealth != null && !colliderHealth.isDead() && m_travelDirection > 0.0f)
		{
			Vector3 force = (transform.position - m_startPosition);
			force.Normalize();
			force.Scale(new Vector3(8.0f,2.0f,8.0f));

			if (rootColliderObject.tag == "Human" )
			{
				HumanBehavior hb = rootColliderObject.GetComponent<HumanBehavior>();
				if (hb != null)
				{
					hb.handleVehicleImpact(collision, force);
				}
				m_steering = maxSteeringAngle;
				m_travelDirection = -1.0f;
			}
			else if (rootColliderObject.tag == "Zombie")
			{
				ZombieBehavior zb = rootColliderObject.GetComponent<ZombieBehavior>();
				if (zb != null)
				{
					zb.handleVehicleImpact(collision, force);
				}
				m_steering = maxSteeringAngle;
				m_travelDirection = -1.0f;
			}
		}
	}
}

