using UnityEngine;
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
		RunOff,				// run away from one of the localized zombies
		Dead,				// dead, this time really
	};
	
	public float initDelay = 2.0f;
	Vector3 m_targetPosition;
	State m_state;

	float m_stateTime = 0.0f;

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
		if( GetComponent<Animator>() == null || ( GetComponent<RagdollCreatorTest>() != null && handBone == null ) )
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

			const float runOffThresholdSmall = 25.0f;
			const float runOffThresholdLarge = 49.0f;
			float sqrDistance = ( transform.position - m_positionOfInterest ).sqrMagnitude;
			if( ( sqrDistance < runOffThresholdSmall && !m_hasGun ) || ( sqrDistance > runOffThresholdLarge && m_hasGun ) )
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
		
		if( m_localizedObjectOfInterestCandidate != null )
		{
			setObjectOfInterest( m_localizedObjectOfInterestCandidate );
		}
		
		if( m_objectOfInterest != null )
		{
			m_positionOfInterest = m_objectOfInterest.GetComponent< Transform > ().position;
		}

		m_targetPosition = getTargetPositionForDangerPosition (m_positionOfInterest);
		approachPosition( m_targetPosition );
		if (reachedPosition () && m_stateTime > 0.5f ) {
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
			
			const float runOffThresholdSmall = 25.0f;
			const float runOffThresholdLarge = 81.0f;
			float sqrDistance = ( transform.position - m_positionOfInterest ).sqrMagnitude;
			if( ( /*sqrDistance < runOffThresholdSmall ||*/ sqrDistance > runOffThresholdLarge ) && m_stateTime > 1.0f )
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

		return GetComponent<NavMeshAgent>().destination;
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
			
		case State.RunOff:
			updateRunOffBehaviour();
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

		updateAnimationState ();
    }

	public void dropWeapon()
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
		}
	}

	void Start()
	{
		m_state = State.WaitForComponents;
	}

	void Update ()
	{
		updateState();



		GetComponent<NavMeshAgent>().speed = 1.5f;

		Vector3 movement = GetComponent<Transform> ().position - m_oldPosition;
		m_oldPosition = GetComponent<Transform> ().position;
		Vector3 diff = GetComponent<Transform> ().position - GetComponent<NavMeshAgent> ().destination;
		if (GetComponent<Animator> ()) {
			if (diff.magnitude > 0.7f) {
				GetComponent<Animator> ().SetFloat ("speed", movement.magnitude / Time.deltaTime);
			} else {
				GetComponent<Animator> ().SetFloat ("speed", 0.0f);
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
