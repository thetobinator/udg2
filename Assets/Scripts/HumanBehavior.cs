using UnityEngine;
using System.Collections;

public class HumanBehavior : MonoBehaviour {
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
	GameObject m_nonLocalizedTargetCandidate = null;
	GameObject m_localizedTargetCandidate = null;
	GameObject m_dangerObject = null;
	Vector3 m_targetPosition;
	Vector3 m_dangerPosition;
	State m_state;
	float m_earQueryInterval = 0.5f;
	float m_eyeQueryInterval = 0.5f;
	float m_time = 0.0f;
	float m_stateTime = 0.0f;
	float m_runnerAlertSqrDistanceThreshold = 256.0f;
	float m_runnerDetectSqrDistanceThreshold = 64.0f;
	Vector3 m_oldPosition;
	bool m_hasGun;
	GameObject m_gun;
	public Transform handBone = null;
	public RuntimeAnimatorController zombieAnimationController = null;
	
	string opposingFactionTag()
	{
		return gameObject.tag == "Zombie" ? "Human" : "Zombie";
	}
	
	void updateSenses()
	{
		float oldTime = m_time - Time.deltaTime;
		bool updateEars = (int)( oldTime / m_earQueryInterval ) != (int)( m_time / m_earQueryInterval );
		bool updateEyes = (int)( oldTime / m_eyeQueryInterval ) != (int)( m_time / m_eyeQueryInterval );
		
		if( updateEars || updateEyes )
		{
			GameObject[] zombies = GameObject.FindGameObjectsWithTag( opposingFactionTag() );
			GameObject closestHeardZombie = null;
			GameObject closestSeenZombie = null;
			bool targetIsHeardOrSeen = false;
			float closestHeardZombieSqrDistance = float.MaxValue;
			float closestSeenZombieSqrDistance = float.MaxValue;
			
			setLocalizedTargetCandidate( null );
			setNonLocalizedTargetCandidate( null );
			if( m_dangerObject != null && ( m_dangerObject.GetComponent<HealthComponent>() == null || m_dangerObject.GetComponent<HealthComponent>().isDead() ) )
			{
				// target was killed by something else
				setTargetObject( null );
				m_dangerPosition = GetComponent< Transform > ().position;
			}
            //Debug.Log(string.Format("Zombies = {0}", zombies));
            foreach (GameObject zombie in zombies)
                {
                //Debug.Log(string.Format("Zombie= {0}", zombie));
                if (zombie.GetComponent<HealthComponent>() == null || zombie.GetComponent<HealthComponent>().isDead())
                    {
                        // ignore dead zombies for now
                        continue;
                    }
                    Vector3 zombieHeadPosition = GetComponent<Transform>().position;
                    Vector3 zombieViewDirection = GetComponent<Transform>().forward;
                    zombieHeadPosition.y += 1.5f;
                    Vector3 zombieCenter = zombie.GetComponent<Transform>().position;
                    zombieCenter.y += 0.8f;
                    Vector3 direction = zombieCenter - zombieHeadPosition;
                    float sqrDistanceToZombie = direction.sqrMagnitude;

                // Why this bugging out? instance doesn't exist?
               // Debug.Log(String.Format("getObjectSpeed = {0}",MainGameManager.instance.getObjectSpeed(zombie)));
               
                if (updateEars && MainGameManager.instance.getObjectSpeed(zombie) > 1.0f)
                        {
                            // human is running
                            if (zombie == m_dangerObject)
                            {
                                targetIsHeardOrSeen = true;
                            }

                            if (sqrDistanceToZombie < closestHeardZombieSqrDistance)
                            {
                                closestHeardZombie = zombie;
                                closestHeardZombieSqrDistance = sqrDistanceToZombie;
                            }
                        }

                        if (updateEyes)
                        {
                            direction.Normalize();
                            Vector3 direction2D = direction;
                            direction2D.y = 0.0f;
                            direction2D.Normalize();
                            if (Vector3.Dot(direction2D, zombieViewDirection) > 0.707f)
                            {
                                // in azimuth
                                Vector3 rayStart = zombieHeadPosition + 0.5f * direction;
                                Ray ray = new Ray(rayStart, direction);
                                RaycastHit hit = new RaycastHit();
                                if (Physics.Raycast(ray, out hit))
                                {
                                    if ((hit.point - zombieCenter).sqrMagnitude < 0.5f)
                                    {
                                        // ray hit is near zombie -> no obstacle in between
                                        if (zombie == m_dangerObject)
                                        {
                                            targetIsHeardOrSeen = true;
                                        }
                                        if (sqrDistanceToZombie < closestSeenZombieSqrDistance)
                                        {
                                            closestSeenZombie = zombie;
                                            closestSeenZombieSqrDistance = sqrDistanceToZombie;
                                        }
                                    }
                                }
                            }
                        }
            
			}
			
			/*
			if( targetIsHeardOrSeen )
			{
				setLocalizedTargetCandidate( m_dangerObject );
			}
			else*/ if( closestSeenZombie != null )
			{
				setLocalizedTargetCandidate( closestSeenZombie );
			}
			else if( closestHeardZombie != null )
			{
				if( closestHeardZombieSqrDistance < m_runnerDetectSqrDistanceThreshold )
				{
					setLocalizedTargetCandidate( closestHeardZombie );
				}
				else if( closestHeardZombieSqrDistance < m_runnerAlertSqrDistanceThreshold )
				{
					setNonLocalizedTargetCandidate( closestHeardZombie );
				}
			}
		}
	}
	
	void approachPosition( Vector3 targetPosition )
	{
        NavMeshHit hit;
        if (NavMesh.SamplePosition(this.transform.position, out hit, 3, NavMesh.AllAreas)){
            GetComponent<NavMeshAgent>().SetDestination(targetPosition);
        }
        }
	
	bool reachedPosition()
	{
		return (GetComponent< NavMeshAgent > ().destination - transform.position).sqrMagnitude < 1.5f;
	}
	
	void updateSpawnBehaviour()
	{
		/*
		updateSenses();
		if( m_localizedTargetCandidate != null )
		{
			setTargetObject( m_localizedTargetCandidate );
			m_dangerPosition = m_dangerObject.GetComponent< Transform >().position;
			m_state = State.RunOff;
			return;
		}

		m_targetPosition = getTargetPositionForDangerPosition (m_dangerPosition);
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
		m_hasGun = Random.Range (0, 6) == 0 && handBone != null;
		m_state = State.Init;
		m_targetPosition = transform.position;
		m_dangerPosition = transform.position;

		// for now, let each human walk to a differnt spawn point after spawning
		GameObject[] spawnPoints = GameObject.FindGameObjectsWithTag("SpawnPoint_Human");
		float maxDiffSqr = 0.0f;
		for( uint i = 0u; i < spawnPoints.Length; ++i )
		{
			Vector3 diff = spawnPoints[ i ].transform.position - transform.position;
			if( diff.sqrMagnitude > maxDiffSqr )
			{
				m_dangerPosition = spawnPoints[ i ].transform.position;
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
		GetComponent<Animator>().SetBool ("walk", false );
		updateSenses ();
		if( m_localizedTargetCandidate != null )
		{
			setTargetObject( m_localizedTargetCandidate );
			m_dangerPosition = m_dangerObject.GetComponent< Transform >().position;
			m_state = State.RunOff;
		}
		else if( m_nonLocalizedTargetCandidate != null )
		{
			m_state = State.Alerted;
		}
	}
	
	void updateAlertBehaviour()
	{
		GetComponent<Animator>().SetBool ("walk", false );
		updateSenses();
		if( m_localizedTargetCandidate != null )
		{
			setTargetObject( m_localizedTargetCandidate );
			m_dangerPosition = m_dangerObject.GetComponent< Transform >().position;

			const float runOffThresholdSmall = 25.0f;
			const float runOffThresholdLarge = 49.0f;
			float sqrDistance = ( transform.position - m_dangerPosition ).sqrMagnitude;
			if( ( sqrDistance < runOffThresholdSmall && !m_hasGun ) || ( sqrDistance > runOffThresholdLarge && m_hasGun ) )
			{
				m_state = State.RunOff;
			}
			else if( m_hasGun )
			{
				m_state = State.StandAndShoot;
			}
			return;
		} else if (m_nonLocalizedTargetCandidate != null || m_stateTime < 4.0f ) {
		// glance left and right (need anim later)
			transform.Rotate( 0.0f, 90.0f * Time.deltaTime * (m_stateTime > 1.0f && m_stateTime <= 3.0f ? 1.0f : -1.0f), 0.0f );
		} else {
			m_state = State.Idle;
		}
	}

	void updateRunOffBehaviour()
	{
		updateSenses ();
		
		if( m_localizedTargetCandidate != null )
		{
			setTargetObject( m_localizedTargetCandidate );
		}
		
		if( m_dangerObject != null )
		{
			m_dangerPosition = m_dangerObject.GetComponent< Transform > ().position;
		}

		m_targetPosition = getTargetPositionForDangerPosition (m_dangerPosition);
		approachPosition( m_targetPosition );
		if (reachedPosition () && m_stateTime > 0.5f ) {
			m_state = State.Alerted;
		} else {
			GetComponent<Animator> ().SetBool ("walk", true);
		}
	}

	void updateStandAndShootBehaviour()
	{
		GetComponent<Animator>().SetBool ("walk", false );
		updateSenses ();
		if( m_localizedTargetCandidate != null )
		{
			setTargetObject( m_localizedTargetCandidate );
			m_dangerPosition = m_dangerObject.GetComponent< Transform >().position;
			
			const float runOffThresholdSmall = 25.0f;
			const float runOffThresholdLarge = 81.0f;
			float sqrDistance = ( transform.position - m_dangerPosition ).sqrMagnitude;
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

			Vector3 look = m_dangerPosition - transform.position;
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
	
	void colorizeObject( GameObject obj, Color color )
	{
		// commented out for now to test zombie target colorization
		/*
		if( obj == null )
		{
			return;
		}
		
		DebugTint debugTint = obj.GetComponent<DebugTint> ();
		if( debugTint != null )
		{
			debugTint.tintColor = color;
		}
		*/
	}
	
	void setNonLocalizedTargetCandidate( GameObject obj )
	{
		colorizeObject( m_nonLocalizedTargetCandidate, Color.white );
		m_nonLocalizedTargetCandidate = obj;
	}
	
	void setLocalizedTargetCandidate( GameObject obj )
	{
		colorizeObject( m_localizedTargetCandidate, Color.white );
		m_localizedTargetCandidate = obj;
	}
	
	void setTargetObject( GameObject obj )
	{
		colorizeObject( m_dangerObject, Color.white );
		m_dangerObject = obj;
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
		m_time += Time.deltaTime;
		m_stateTime += Time.deltaTime;
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


		
		//colorizeObject( m_nonLocalizedTargetCandidate, Color.blue );
		//colorizeObject( m_localizedTargetCandidate, Color.green );
		//colorizeObject( m_dangerObject, Color.red );
		
		if( m_state != oldState )
		{
			m_stateTime = 0.0f;
		}

		Animator animatorComponent = GetComponent<Animator> ();
		if (animatorComponent != null) {
			animatorComponent.SetBool ("shoot", m_state == State.StandAndShoot);
		}
    }

	public void dropWeapon()
	{
		if( m_gun != null )
		{
			m_gun.transform.parent = null;
			m_gun.GetComponent<CapsuleCollider>().enabled = true;
			m_gun.GetComponent<Rigidbody>().useGravity = true;
		}
	}

	public void handleBulletImpact( Collision collision )
	{
        //Debug.Log(collision.gameObject.name);

        HealthComponent h = GetComponent<HealthComponent>();
		if( h != null && h.enabled ){
			h.dealDamage( 25.0f );
			if( h.isDead() ){
				ZombieBehavior.turnIntoRagdoll( gameObject );
			}
		}
	}

	void Start()
	{
		m_state = State.WaitForComponents;
	}

	// Update is called once per frame
	void Update ()
	{
		updateState();

		/*
		if (m_hasPlayerTask) {
            */

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
       
        
		//}
	}// end update
	
	
}
