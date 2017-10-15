﻿using UnityEngine;
using System.Collections;

public class SensingEntity : MonoBehaviour {
	protected enum AnimationFlags
	{
		Walk = 1 << 0,
		Run = 1 << 1,
		Attack = 1 << 2,
		Turn = 1 << 3,
		Eat = 1 << 4,
		Shoot = 1 << 5,
		Kick = 1 << 6,
	}

	public float speedMultiplier = 1.0f;
	protected float m_speedBoost = 1.0f;
	protected uint m_animationFlags = 0u;
	protected float m_earQueryInterval = 0.5f;
	protected float m_eyeQueryInterval = 0.5f;
	protected GameObject m_nonLocalizedObjectOfInterestCandidate = null;
	protected GameObject m_localizedObjectOfInterestCandidate = null;
	protected GameObject m_objectOfInterest = null;
	protected Vector3 m_positionOfInterest;
	private float m_runnerAlertSqrDistanceThreshold = 256.0f;
	private float m_runnerDetectSqrDistanceThreshold = 64.0f;
	private float m_time = 0.0f;
	private uint m_postProcessHumanRagdoll = 0u;

	protected void Start()
	{
		m_time = Random.value; // avoid triggering the senses of all objects at the same time
	}

	protected void Update()
	{
		m_time += Time.deltaTime;
		if (m_postProcessHumanRagdoll > 0u) {
			--m_postProcessHumanRagdoll;
			if (m_postProcessHumanRagdoll == 0u) {
				gameObject.AddComponent<ZombieBehavior> ();
				gameObject.GetComponent<ZombieBehavior> ().initDelay = 8.0f;
				Destroy (this);
			}
		} else if( m_time > 10.0f ) {
			HealthComponent hc = GetComponent<HealthComponent> ();
			RagdollHelper r = GetComponent<RagdollHelper> ();
			if (hc.isDead () && r.ragdolled) {
				if (m_time > 12.0f) {
					Destroy (gameObject);
				} else {
					setCollidersEnabled (false);
				}
			}
		}
	}

	protected string getOpposingFactionTag()
	{
		return gameObject.tag == "Zombie" ? "Human" : "Zombie";
	}

	protected void setNonLocalizedObjectOfInterestCandidate( GameObject obj )
	{
		m_nonLocalizedObjectOfInterestCandidate = obj;
	}

	protected void setLocalizedObjectOfInterestCandidate( GameObject obj )
	{
		m_localizedObjectOfInterestCandidate = obj;
	}

	protected void setObjectOfInterest( GameObject obj )
	{
		m_objectOfInterest = obj;
	}

	protected void approachPosition( Vector3 targetPosition )
	{
		UnityEngine.AI.NavMeshAgent nma = GetComponent<UnityEngine.AI.NavMeshAgent>();
		if( nma == null || !nma.enabled || !nma.isOnNavMesh )
		{
			return;
		}
		UnityEngine.AI.NavMeshHit hit;
		if( UnityEngine.AI.NavMesh.SamplePosition( transform.position, out hit, 3, UnityEngine.AI.NavMesh.AllAreas ) )
		{
			nma.SetDestination( targetPosition );
		}
	}

	protected bool reachedPosition()
	{
		UnityEngine.AI.NavMeshAgent nma = GetComponent<UnityEngine.AI.NavMeshAgent>();
		if( nma == null )
		{
			return false;
		}
		return (nma.destination - transform.position).sqrMagnitude < 1.5f || nma.enabled == false;
	}

	protected void colorizeObject( GameObject obj, Color color )
	{
		if( obj == null )
		{
			return;
		}

		DebugTint debugTint = obj.GetComponent<DebugTint> ();
		if( debugTint != null )
		{
			debugTint.tintColor = color;
		}
	}

	protected void updateSenses()
	{
		float oldTime = m_time - Time.deltaTime;
		bool updateEars = (int)( oldTime / m_earQueryInterval ) != (int)( m_time / m_earQueryInterval );
		bool updateEyes = (int)( oldTime / m_eyeQueryInterval ) != (int)( m_time / m_eyeQueryInterval );

		if( updateEars || updateEyes )
		{
			GameObject[] objectsOfOpposingFaction = GameObject.FindGameObjectsWithTag( getOpposingFactionTag() );
			GameObject closestHeardObjectOfOpposingFaction = null;
			GameObject closestSeenObjectOfOpposingFaction = null;
			bool objectOfInterestIsHeardOrSeen = false;
			float closestHeardObjectOfOpposingFactionSqrDistance = float.MaxValue;
			float closestSeenObjectOfOpposingFactionSqrDistance = float.MaxValue;

			setLocalizedObjectOfInterestCandidate( null );
			setNonLocalizedObjectOfInterestCandidate( null );
			if( m_objectOfInterest != null && ( m_objectOfInterest.GetComponent<HealthComponent>() == null || m_objectOfInterest.GetComponent<HealthComponent>().isDead() ) )
			{
				// target was killed by something else
				setObjectOfInterest( null );
				m_positionOfInterest = transform.position;
			}

			foreach (GameObject enemyObject in objectsOfOpposingFaction)
			{
				if (enemyObject.GetComponent<HealthComponent>() == null || enemyObject.GetComponent<HealthComponent>().isDead())
				{
					// ignore corpses
					continue;
				}
				if (enemyObject.GetComponent<RagdollHelper> () != null && enemyObject.GetComponent<RagdollHelper> ().ragdolled)
				{
					// ignore ragdolls
					continue;
				}
				Vector3 headPosition = transform.position;
				Vector3 viewDirection = transform.forward;
				headPosition.y += 1.5f;
				Vector3 enemyObjectCenter = enemyObject.transform.position;
				enemyObjectCenter.y += 0.8f;
				Vector3 direction = enemyObjectCenter - headPosition;
				float sqrDistanceToEnemyObject = direction.sqrMagnitude;

				if( updateEars && MainGameManager.instance.getObjectSpeed(enemyObject) > 1.0f )
				{
					// object of interest is running -> loud
					if( enemyObject == m_objectOfInterest )
					{
						objectOfInterestIsHeardOrSeen = true;
					}

					if( sqrDistanceToEnemyObject < closestHeardObjectOfOpposingFactionSqrDistance )
					{
						closestHeardObjectOfOpposingFaction = enemyObject;
						closestHeardObjectOfOpposingFactionSqrDistance = sqrDistanceToEnemyObject;
					}
				}

				if( updateEyes )
				{
					direction.Normalize();
					Vector3 direction2D = direction;
					direction2D.y = 0.0f;
					direction2D.Normalize();
					HealthComponent hc = GetComponent<HealthComponent> ();
					float relevantDotProduct = 0.707f;
					if (hc != null && hc.initialHealth != hc.getCurrentHealth()) {
						relevantDotProduct = -1.0f;
						// give hurt victims chance to react
					}
					if( Vector3.Dot( direction2D, viewDirection ) > relevantDotProduct )
					{
						// in azimuth
						Vector3 rayStart = headPosition + 0.5f * direction;
						Ray ray = new Ray(rayStart, direction);
						RaycastHit hit = new RaycastHit();
						if( Physics.Raycast( ray, out hit ) )
						{
							if( ( hit.point - enemyObjectCenter ).sqrMagnitude < 0.5f )
							{
								// ray hit is near object of interest -> no obstacle in between
								if( enemyObject == m_objectOfInterest )
								{
									objectOfInterestIsHeardOrSeen = true;
								}
								if( sqrDistanceToEnemyObject < closestSeenObjectOfOpposingFactionSqrDistance )
								{
									closestSeenObjectOfOpposingFaction = enemyObject;
									closestSeenObjectOfOpposingFactionSqrDistance = sqrDistanceToEnemyObject;
								}
							}
						}
					}
				}

			}

			if( closestSeenObjectOfOpposingFaction != null )
			{
				setLocalizedObjectOfInterestCandidate( closestSeenObjectOfOpposingFaction );
			}
			else if( closestHeardObjectOfOpposingFaction != null )
			{
				if( closestHeardObjectOfOpposingFactionSqrDistance < m_runnerDetectSqrDistanceThreshold )
				{
					setLocalizedObjectOfInterestCandidate( closestHeardObjectOfOpposingFaction );
				}
				else if( closestHeardObjectOfOpposingFactionSqrDistance < m_runnerAlertSqrDistanceThreshold )
				{
					setNonLocalizedObjectOfInterestCandidate( closestHeardObjectOfOpposingFaction );
				}
			}
		}
	}

	protected void verifyObjectOfInterest()
	{
		if (m_objectOfInterest != null ) {
			HealthComponent h = m_objectOfInterest.GetComponent<HealthComponent> ();
			if( h == null || h.wasKilledBy( gameObject ) )
			{
				return;
			}
			if (m_objectOfInterest.tag != getOpposingFactionTag () || h.isDead()) {
				m_objectOfInterest = null;
				m_positionOfInterest = transform.position;
			}
		}
	}

	public bool turnIntoRagdoll()
	{
		UnityEngine.AI.NavMeshAgent n = GetComponent<UnityEngine.AI.NavMeshAgent>();
		Animator a = GetComponent<Animator>();
		HumanBehavior h = GetComponent<HumanBehavior>();
		ZombieBehavior z = GetComponent<ZombieBehavior>();
		RagdollHelper r = GetComponent<RagdollHelper> ();
		HealthComponent hc = GetComponent<HealthComponent>();

		setCollidersEnabled (true);

		bool result = false;
		if (n != null && a != null && r != null && a.enabled) {
			r.ragdolled = true;
			m_time = 0.0f;
			if (h != null) {
				h.die ();
				if (!hc.wasKilledBy (null)) {
					m_postProcessHumanRagdoll = 1u;
				}
				result = true;
			} else if (z != null) {
				if (hc.isDead()) {
					z.die ();
				}
				result = true;
			}
			n.enabled = false;
		}
		return result;
	}

	protected void updateAnimationState()
	{
		bool isWalking = (m_animationFlags & (uint)AnimationFlags.Walk) != 0u;
		float animationSpeedMultiplier = m_speedBoost * (isWalking ? 2.5f : speedMultiplier * 0.6f);
		Animator animatorComponent = GetComponent<Animator> ();
		if (animatorComponent != null && animatorComponent.enabled && animatorComponent.runtimeAnimatorController != null) {
			animatorComponent.SetBool ("walk", isWalking );
			animatorComponent.SetBool ("run", (m_animationFlags & (uint)AnimationFlags.Run) != 0u );
			animatorComponent.SetBool ("attack", (m_animationFlags & (uint)AnimationFlags.Attack) != 0u);
			animatorComponent.SetBool ("turn", (m_animationFlags & (uint)AnimationFlags.Turn) != 0u);
			animatorComponent.SetBool ("eat", (m_animationFlags & (uint)AnimationFlags.Eat) != 0u);
			animatorComponent.SetBool ("shoot", (m_animationFlags & (uint)AnimationFlags.Shoot) != 0u);
			animatorComponent.SetBool ("kick", (m_animationFlags & (uint)AnimationFlags.Kick) != 0u);
			animatorComponent.SetBool ("zombie", gameObject.tag == "Zombie");
			animatorComponent.SetFloat ("speedMultiplier", animationSpeedMultiplier);
		}
	}

	protected void setCollidersEnabled(bool collidersEnabled){
		foreach (CapsuleCollider child in GetComponentsInChildren<CapsuleCollider>()) {
			child.enabled = collidersEnabled;
		}
		foreach (SphereCollider child in GetComponentsInChildren<SphereCollider>()) {
			child.enabled = collidersEnabled;
		}
		foreach (BoxCollider child in GetComponentsInChildren<BoxCollider>()) {
			child.enabled = collidersEnabled;
		}
		/*
		foreach (Rigidbody child in GetComponentsInChildren<Rigidbody>()) {
			child.velocity = new Vector3(0.0f, 0.0f, 0.0f);
			child.angularVelocity = new Vector3(0.0f, 0.0f, 0.0f);
			child.Sleep ();
		}
		*/
	}
}
