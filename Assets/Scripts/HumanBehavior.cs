﻿using UnityEngine;
using System.Collections;

public class HumanBehavior : SensingEntity {
	enum State
	{
		WaitForComponents,	// wait until other components have been created (such as Animator by UMA)
		Init,				// initialize (do nothing for initDelay seconds)
		Spawning,			// go from spawn point to idle point
		Idle,				// no zombies sensed, just stand there
		Alerted,			// sensed zombies without exact localization, look around, moaning sounds
		StandAndShoot,		// shoot at localized zombie
		Kick,				// kick to defend self
		RunOff,				// run away from one of the localized zombies
		FleeToSafePosition,	// run to a safe point
		Dead,				// dead, this time really
	};
	
	public float initDelay = 2.0f;
	Vector3 m_targetPosition;
	State m_state;

	float m_stateTime = 0.0f;
	float m_fleeCooldown = 4.0f;

	Vector3 m_oldPosition;
	bool m_hasGun;
	GameObject m_gun;
	public Transform handBone = null;
	
	void updateSpawnBehaviour()
	{
		/*
		updateSenses();
		if( m_localizedObjectOfInterestCandidate != null )
		{
			setObjectOfInterest( m_localizedObjectOfInterestCandidate );
			m_positionOfInterest = m_objectOfInterest.GetComponent< Transform >().position;
			m_state = State.RunOff;
			return;
		}

		m_targetPosition = getTargetPositionForDangerPosition (m_positionOfInterest);
		approachPosition( m_targetPosition );
		if( reachedPosition() )
		*/
		{
			m_state = State.Idle;
		}
	}

	void updateWaitForComponentsBehaviour()
	{
		if( GetComponent<Animator>() == null || ( GetComponent<RagdollCreator>() != null && handBone == null ) )
		{
			return;
		}
		m_hasGun = Random.Range (0, 3) == 0 && handBone != null;
		m_state = State.Init;
		m_targetPosition = transform.position;
		m_positionOfInterest = transform.position;

		// for now, let each human walk to a differnt spawn point after spawning
		GameObject[] spawnPoints = GameObject.FindGameObjectsWithTag("SpawnPoint_Human");
		float maxDiffSqr = 0.0f;
		for( uint i = 0u; i < spawnPoints.Length; ++i )
		{
			Vector3 diff = spawnPoints[ i ].transform.position - transform.position;
			if( diff.sqrMagnitude > maxDiffSqr )
			{
				m_positionOfInterest = spawnPoints[ i ].transform.position;
				maxDiffSqr = diff.sqrMagnitude;
			}
		}		

		if( m_hasGun )
		{
			m_gun = (GameObject)Instantiate (MainGameManager.instance.gun);
			m_gun.transform.parent = handBone.transform;
			m_gun.GetComponent<CannonBehavior> ().enabled = false; // maybe use this script later on
			m_gun.transform.localPosition = new Vector3 (0.0f, 0.0f, 0.0f);
			m_gun.transform.localEulerAngles = new Vector3 (45.0f, 180.0f, 0.0f);
		}	
		else
		{
			m_gun = null;
		}
	}
	
	void updateIdleBehaviour()
	{
		updateSenses ();
		if( m_localizedObjectOfInterestCandidate != null )
		{
			setObjectOfInterest( m_localizedObjectOfInterestCandidate );
			m_positionOfInterest = m_objectOfInterest.GetComponent< Transform >().position;
			m_state = State.RunOff;
		}
		else if( m_nonLocalizedObjectOfInterestCandidate != null )
		{
			m_state = State.Alerted;
		}
	}
	
	void updateAlertBehaviour()
	{
		updateSenses();
		if( m_localizedObjectOfInterestCandidate != null )
		{
			setObjectOfInterest( m_localizedObjectOfInterestCandidate );
			m_positionOfInterest = m_objectOfInterest.GetComponent< Transform >().position;

			const float selfDefenseThreshold = 4.0f;
			const float runOffThresholdSmall = 25.0f;
			const float runOffThresholdLarge = 49.0f;
			float sqrDistance = ( transform.position - m_positionOfInterest ).sqrMagnitude;
			if( sqrDistance < selfDefenseThreshold )
			{
				m_state = State.Kick;
			}
			else if( ( sqrDistance < runOffThresholdSmall && !m_hasGun ) || ( sqrDistance > runOffThresholdLarge && m_hasGun ) )
			{
				m_state = State.RunOff;
			}
			else if( m_hasGun )
			{
				m_state = State.StandAndShoot;
			}
			return;
		} else if (m_nonLocalizedObjectOfInterestCandidate != null || m_stateTime < 4.0f ) {
		// glance left and right (need anim later)
			transform.Rotate( 0.0f, 90.0f * Time.deltaTime * (m_stateTime > 1.0f && m_stateTime <= 3.0f ? 1.0f : -1.0f), 0.0f );
		} else {
			m_state = State.Idle;
		}
	}

	void updateRunOffBehaviour()
	{
		updateSenses ();

		m_fleeCooldown -= Time.deltaTime;
		
		if( m_localizedObjectOfInterestCandidate != null )
		{
			setObjectOfInterest( m_localizedObjectOfInterestCandidate );
		}
		
		if( m_objectOfInterest != null )
		{
			m_positionOfInterest = m_objectOfInterest.GetComponent< Transform > ().position;
			const float selfDefenseThreshold = 4.0f;
			float sqrDistance = ( transform.position - m_positionOfInterest ).sqrMagnitude;
			if (sqrDistance < selfDefenseThreshold) {
				m_state = State.Kick;
			}
		}

		m_targetPosition = getTargetPositionForDangerPosition (m_positionOfInterest);
		approachPosition( m_targetPosition );
		if (reachedPosition () && m_stateTime > 0.5f) {
			m_state = State.Alerted;
		} else if ( m_fleeCooldown <= 0.0f) {
			m_targetPosition = getSafePosition ();
			m_fleeCooldown = 4.0f;
			m_state = State.FleeToSafePosition;
		} else {
			m_animationFlags |= (uint)AnimationFlags.Walk;
		}
	}

	private Vector3 getSafePosition()
	{
		GameObject[] safePoints = GameObject.FindGameObjectsWithTag("SafePoint_Human");
		int nearestSafepointIndex = -1;

		if (safePoints.Length == 0) {
			print ("No safe points found! Cannot flee");
		} else if (safePoints.Length == 1) {
			return safePoints [0].transform.position;
		}

		float minSqrDistance = -1.0f;
		for (int i = 0; i < safePoints.Length; ++i) {
			float sqrDistance = (transform.position - safePoints[i].transform.position).sqrMagnitude;
			if (minSqrDistance == -1.0f || sqrDistance < minSqrDistance) {
				nearestSafepointIndex = i;
				minSqrDistance = sqrDistance;
			}
		}

		// pick a random safe point that is not the nearest
		for (;;) {
			int rnd = Random.Range (0, safePoints.Length - 1);
			if (rnd != nearestSafepointIndex) {
				return safePoints [rnd].transform.position;
			}
		}

		//return transform.position;
	}

	void updateFleeToSafePosition()
	{
		// no roundhouse kicks in this mode, assume the target position equals a safe position
		approachPosition( m_targetPosition );
		if (reachedPosition() || m_stateTime > 4.0f) {
			m_targetPosition = transform.position;
			approachPosition (m_targetPosition);
			m_state = State.Alerted;
		} else {
			m_animationFlags |= (uint)AnimationFlags.Walk;
		}
	}

	void updateStandAndShootBehaviour()
	{
		m_animationFlags |= (uint)AnimationFlags.Shoot;
		updateSenses ();
		if( m_localizedObjectOfInterestCandidate != null )
		{
			setObjectOfInterest( m_localizedObjectOfInterestCandidate );
			m_positionOfInterest = m_objectOfInterest.GetComponent< Transform >().position;
			
			const float selfDefenseThreshold = 4.0f;
			const float runOffThresholdLarge = 81.0f;
			float sqrDistance = ( transform.position - m_positionOfInterest ).sqrMagnitude;
			if (sqrDistance < selfDefenseThreshold) {
				m_state = State.Kick;
			}
			else if( sqrDistance > runOffThresholdLarge && m_stateTime > 1.0f )
			{
				m_state = State.RunOff;
			}
			else if( (int)( m_stateTime / 0.5f ) != (int)( ( m_stateTime - Time.deltaTime ) / 0.5f ) )
			{
				Quaternion bulletRotation = new Quaternion();
				Vector3 forward = transform.forward + transform.right * Random.Range( -0.15f, 0.15f );
				forward.Normalize();
				bulletRotation.SetLookRotation( -transform.up, forward );
				Instantiate( MainGameManager.instance.bullet, transform.position + transform.forward + transform.up * 1.1f, bulletRotation);
			}

			Vector3 look = m_positionOfInterest - transform.position;
			look.Normalize();
			Quaternion newRotation = new Quaternion();
			newRotation.SetLookRotation( look, transform.up );
			transform.rotation = newRotation;
			return;
		}

		if (m_stateTime > 2.0f) {
			m_state = State.Alerted;
		}		
	}

	void updateKickBehaviour()
	{
		m_fleeCooldown -= Time.deltaTime;
		setCollidersEnabled (false);
		if (m_stateTime < 0.7f) {
			m_animationFlags |= (uint)AnimationFlags.Walk;
			approachPosition (m_positionOfInterest);
		} else if (m_stateTime < 1.2f) {
			m_animationFlags |= (uint)AnimationFlags.Kick;
		} else if (m_localizedObjectOfInterestCandidate != null) {
			if (Random.Range (0, 1) == 0) {
				m_localizedObjectOfInterestCandidate.GetComponent<ZombieBehavior> ().handleKicked (gameObject);
			}
			m_localizedObjectOfInterestCandidate = null;
		} else {
			setCollidersEnabled (true);
			m_state = State.Alerted;
		}
	}

	Vector3 getTargetPositionForDangerPosition( Vector3 dangerPosition )
	{
		Vector3 currentPosition = GetComponent<Transform> ().position;
		Vector3 oppositeDirection = currentPosition - dangerPosition;

		float sqrDistance = oppositeDirection.sqrMagnitude;
		oppositeDirection.Normalize ();
		if ( sqrDistance < 25.0f || ( sqrDistance > 49.0f && m_hasGun ) ) {

			oppositeDirection.Scale (new Vector3 (5.0f, 0.0f, 5.0f));
			return oppositeDirection + dangerPosition;
		}

		return GetComponent<UnityEngine.AI.NavMeshAgent>().destination;
	}
	
	void updateState()
	{
		m_animationFlags = 0u;

		State oldState = m_state;

		switch( m_state )
		{
		case State.WaitForComponents:
			updateWaitForComponentsBehaviour ();
			break;

		case State.Init:
			m_state = State.Spawning;
			break;
			
		case State.Spawning:
			updateSpawnBehaviour();
			break;
			
		case State.Idle:
			updateIdleBehaviour();
			break;
			
		case State.Alerted:
			updateAlertBehaviour();
			break;

		case State.StandAndShoot:
			updateStandAndShootBehaviour();
			break;

		case State.Kick:
			updateKickBehaviour();
			break;
			
		case State.RunOff:
			updateRunOffBehaviour();
			break;

		case State.FleeToSafePosition:
			updateFleeToSafePosition();
			break;
			
		case State.Dead:
			break;
		}
		
		if( m_state != oldState )
		{
			m_stateTime = 0.0f;
		}
		else
		{
			m_stateTime += Time.deltaTime;	
		}

		//GetComponent<ShowTextMeshInEditor> ().gameText = m_state.ToString();
		//GetComponent<TextMesh> ().alignment = TextAlignment.Center;
		//GetComponent<TextMesh> ().anchor = TextAnchor.UpperCenter;
		//GetComponent<TextMesh> ().characterSize = 0.1f;

		m_speedBoost = m_state == State.FleeToSafePosition ? 1.5f : 1.0f;
		updateAnimationState ();
    }

	void dropWeapon()
	{
		if( m_gun != null )
		{
			m_gun.transform.parent = null;
			m_gun.GetComponent<BoxCollider>().enabled = true;
			m_gun.GetComponent<Rigidbody>().useGravity = true;
		}
	}

	public void handleBulletImpact( Collision collision )
	{
        HealthComponent h = GetComponent<HealthComponent>();
		if( h != null && h.enabled ){
			h.dealDamage( 25.0f, null );
			if( h.isDead() ){
				turnIntoRagdoll();
			}
			Instantiate( MainGameManager.instance.bloodParticles, collision.transform.position, collision.transform.rotation);
		}
	}

	public void die()
	{
		dropWeapon ();
		m_state = State.Dead;
	}

	void Start()
	{
		base.Start ();
		m_state = State.WaitForComponents;
	}

	void Update ()
	{
		base.Update ();
		updateState();

		GetComponent<UnityEngine.AI.NavMeshAgent>().speed = 1.5f * m_speedBoost;

		Vector3 movement = GetComponent<Transform> ().position - m_oldPosition;
		m_oldPosition = GetComponent<Transform> ().position;
		Vector3 diff = GetComponent<Transform> ().position - GetComponent<UnityEngine.AI.NavMeshAgent> ().destination;
		if (GetComponent<Animator> ()) {
			if (diff.magnitude > 0.7f) {
				//GetComponent<Animator> ().SetFloat ("speed", movement.magnitude / Time.deltaTime);
			} else {
				//GetComponent<Animator> ().SetFloat ("speed", 0.0f);
				//print ( "REACHED" );
				//m_hasPlayerTask = false;
				//previousObject = taskObject;
				//taskObject = null;
			}
		} else {
			this.transform.Translate (Vector3.forward * Time.deltaTime);
		}
	}
	
	
}
