using UnityEngine;
using System.Collections;

public class ZombieBehavior : MonoBehaviour {
	enum State
	{
		WaitForComponents,	// wait until other components have been created (such as Animator by UMA)
		Init,				// initialize (do nothing for initDelay seconds)
		Spawning,			// go from spawn point to idle point
		Idle,				// no humans sensed, just stand there
		Alerted,			// sensed humans without exact localization, look around, moaning sounds
		ApproachTarget,		// a human is localized, approach target
		TargetInRange,		// the target can be attacked
		Attack,				// attacking the target
		EatFlesh,			// eat dead target
		Stunned,			// stunned, target has some time to escape
		Dead,				// dead, this time really
	};
    
	public float initDelay = 0.0f;
	GameObject m_nonLocalizedTargetCandidate = null;
	GameObject m_localizedTargetCandidate = null;
	GameObject m_targetObject = null;
	Vector3 m_targetPosition;
	State m_state;
	float m_earQueryInterval = 0.5f;
	float m_eyeQueryInterval = 0.5f;
	float m_time = 0.0f;
	float m_stateTime = 0.0f;
	float m_runnerAlertSqrDistanceThreshold = 256.0f;
	float m_runnerDetectSqrDistanceThreshold = 64.0f;
	public GameObject taskObject;
	GameObject previousObject;
	bool m_hasPlayerTask = false;
	Vector3 m_oldPosition;


    

public bool hasPlayerTask()
	{
		return m_hasPlayerTask;
	}

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
			GameObject[] humans = GameObject.FindGameObjectsWithTag( opposingFactionTag() );

			GameObject closestHeardHuman = null;
			GameObject closestSeenHuman = null;
			bool targetIsHeardOrSeen = false;
			float closestHeardHumanSqrDistance = float.MaxValue;
			float closestSeenHumanSqrDistance = float.MaxValue;

			setLocalizedTargetCandidate( null );
			setNonLocalizedTargetCandidate( null );
			if( m_targetObject != null && ( m_targetObject.GetComponent<HealthComponent>() == null || m_targetObject.GetComponent<HealthComponent>().isDead() ) )
			{
				// target was killed by something else
				setTargetObject( null );
				m_targetPosition = GetComponent< Transform > ().position;
			}
           // Debug.Log(string.Format("Humans = {0}", humans));
            if (humans != null)
            {
                foreach (GameObject human in humans)
                {
                    if (human.GetComponent<HealthComponent>() == null || human.GetComponent<HealthComponent>().isDead())
                    {
                        // ignore dead humans for now
                        continue;
                    }
                    Vector3 zombieHeadPosition = GetComponent<Transform>().position;
                    Vector3 zombieViewDirection = GetComponent<Transform>().forward;
                    zombieHeadPosition.y += 1.5f;
                    Vector3 humanCenter = human.GetComponent<Transform>().position;
                    humanCenter.y += 0.8f;
                    Vector3 direction = humanCenter - zombieHeadPosition;
                    float sqrDistanceToHuman = direction.sqrMagnitude;

                    if (updateEars && MainGameManager.instance.getObjectSpeed(human) > 1.0f)
                    {
                        // human is running
                        if (human == m_targetObject)
                        {
                            targetIsHeardOrSeen = true;
                        }

                        if (sqrDistanceToHuman < closestHeardHumanSqrDistance)
                        {
                            closestHeardHuman = human;
                            closestHeardHumanSqrDistance = sqrDistanceToHuman;
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
                                if ((hit.point - humanCenter).sqrMagnitude < 0.5f)
                                {
                                    // ray hit is near human -> no obstacle in between
                                    if (human == m_targetObject)
                                    {
                                        targetIsHeardOrSeen = true;
                                    }
                                    if (sqrDistanceToHuman < closestSeenHumanSqrDistance)
                                    {
                                        closestSeenHuman = human;
                                        closestSeenHumanSqrDistance = sqrDistanceToHuman;
                                    }
                                }
                            }
                        }
                    }
                }
           
			/*
			if( targetIsHeardOrSeen )
			{
				setLocalizedTargetCandidate( m_targetObject );
			}
			else*/ if( closestSeenHuman != null )
			{
				setLocalizedTargetCandidate( closestSeenHuman );
			}
			else if( closestHeardHuman != null )
			{
				if( closestHeardHumanSqrDistance < m_runnerDetectSqrDistanceThreshold )
				{
					setLocalizedTargetCandidate( closestHeardHuman );
				}
				else if( closestHeardHumanSqrDistance < m_runnerAlertSqrDistanceThreshold )
				{
					setNonLocalizedTargetCandidate( closestHeardHuman );
				}
			}
		}
        }// end if humans not null
    }

	void approachPosition( Vector3 targetPosition )
	{
        if (this.GetComponent<NavMeshAgent>()) { 
            GetComponent<NavMeshAgent>().SetDestination(targetPosition);
        }

    }
    
	bool reachedPosition()
	{
		NavMeshAgent nma = GetComponent<NavMeshAgent> ();
		return (nma.destination - transform.position).sqrMagnitude < 1.5f || nma.enabled == false;
	}

	void updateSpawnBehaviour()
	{
		updateSenses();
		/*
		if( m_localizedTargetCandidate != null )
		{
			setTargetObject( m_localizedTargetCandidate );
			m_targetPosition = m_targetObject.GetComponent< Transform >().position;
			m_state = State.ApproachTarget;
			return;
		}

		approachPosition( m_targetPosition );
		if( reachedPosition() )
		*/
		{
			m_state = State.Idle;
			m_targetPosition = transform.position;
		}
	}

	void updateWaitForComponentsBehavior()
	{
		if( GetComponent<Animator>() != null )
		{
			m_state = State.Idle;
			m_targetPosition = transform.position;

			// experimental: zombies go to player
/*
			GameObject player = GameObject.FindGameObjectWithTag ("Player");
			if (player != null && initDelay == 0.0f ) {
				setTargetObject (null);
				m_targetPosition = player.transform.position;		
				m_state = State.ApproachTarget;
				m_hasPlayerTask = true;

				GetComponent<Animator> ().SetBool ("attack", false);
				GetComponent<Animator> ().SetBool ("eat", false);
			}
			*/
		}
	}

	void updateInitBehaviour()
	{
		if (initDelay == 0.0f) {
			m_state = State.Spawning;
			return;
		}


		if (m_stateTime >= initDelay / 2 ) {
			RagdollHelper r = GetComponent<RagdollHelper> ();
			if (r != null) {
				r.ragdolled = false;
			}
		}

		if( m_stateTime >= initDelay )
		{
			reanimate();
			m_state = State.Spawning;
		}
	}
		
	void updateIdleBehaviour()
	{
		updateSenses ();
		if( m_localizedTargetCandidate != null )
		{
			setTargetObject( m_localizedTargetCandidate );
			m_targetPosition = m_targetObject.GetComponent< Transform >().position;
			m_state = State.ApproachTarget;
		}
		else if( m_nonLocalizedTargetCandidate != null )
		{
			m_state = State.Alerted;
		}
	}

	void updateAlertBehaviour()
	{
		updateSenses();
		if (m_localizedTargetCandidate != null) {
			setTargetObject (m_localizedTargetCandidate);
			m_targetPosition = m_targetObject.GetComponent< Transform > ().position;
			m_state = State.ApproachTarget;
			return;
		} else if (m_nonLocalizedTargetCandidate != null || m_stateTime < 4.0f ) {
			// glance left and right (need anim later)
			transform.Rotate( 0.0f, 90.0f * Time.deltaTime * (m_stateTime > 1.0f && m_stateTime <= 3.0f ? 1.0f : -1.0f), 0.0f );
		} else {
			m_state = State.Idle;
		}
	}

	static public bool turnIntoRagdoll( GameObject obj )
	{
		NavMeshAgent n = obj.GetComponent<NavMeshAgent>();
		Animator a = obj.GetComponent<Animator>();
		HumanBehavior h = obj.GetComponent<HumanBehavior>();
		ZombieBehavior z = obj.GetComponent<ZombieBehavior>();
		RagdollHelper r = obj.GetComponent<RagdollHelper> ();

		bool result = false;
		if (n != null && a != null && r != null) {
			r.ragdolled=true;
			
			if (h != null) {
				h.dropWeapon();
				a.runtimeAnimatorController = h.zombieAnimationController; // use zombie animation controller after resurrection
				Destroy (h);
				result = true;
			} else if (z != null) {
				Destroy (z);
				result = true;
			}
			n.enabled = false;
		}

		return result;
	}

	bool dealDamage( GameObject human, float damage )
	{
		HealthComponent health = human.GetComponent<HealthComponent> ();

		if( health == null )
		{
			return true;
		}

		health.dealDamage( damage );

		if( health.isDead() )
		{
			if( turnIntoRagdoll( human ) )
			{
				ZombieBehavior z = human.AddComponent<ZombieBehavior>() as ZombieBehavior;
				z.initDelay = 8.0f;
			}

		}

		return health.isDead ();
	}

	void reanimate()
	{
		HealthComponent health = GetComponent<HealthComponent> ();
		health.reanimate ();
		tag = "Zombie";
		
		NavMeshAgent n = GetComponent<NavMeshAgent>();
		Animator a = GetComponent<Animator>();
		RagdollHelper r = GetComponent<RagdollHelper> ();
			
		if (n != null && a != null && r != null) {			
			n.enabled = true;
			a.enabled = true;
			n.SetDestination (transform.position);
		}
	}

	void updateApproachBehaviour()
	{
		if( m_hasPlayerTask )
		{
			setLocalizedTargetCandidate (null);
			setNonLocalizedTargetCandidate (null);
		}
		else
		{
			updateSenses ();
			return;// do not approach anybody if not given a command
		}

		if( m_localizedTargetCandidate != null && !m_hasPlayerTask )
		{
			setTargetObject( m_localizedTargetCandidate );
		}

		if( m_targetObject != null )
		{
			m_targetPosition = m_targetObject.GetComponent< Transform > ().position;
		}

		approachPosition( m_targetPosition );
		if( reachedPosition() )
		{
			if( m_targetObject != null && m_targetPosition == m_targetObject.GetComponent< Transform >().position )
			{			
				m_state = State.TargetInRange;
			}
			else
			{
				m_hasPlayerTask = false;
				m_state = State.Alerted;
			}
		}
	}

	void updateTargetInRangeBehaviour()
	{
		GetComponent<Animator> ().SetBool ("attack", true);
		m_state = State.Attack;
	}

	void updateAttackBehaviour()
	{
		GetComponent<Animator> ().SetBool ("attack", false);
		if (m_stateTime > 0.5f) {
			if (m_targetObject != null) {
				if (dealDamage (m_targetObject, 50.0f)) {
					setTargetObject (null);
					setLocalizedTargetCandidate( null );
					setNonLocalizedTargetCandidate( null );
					GetComponent<Animator>().SetBool ("eat", true );
					m_hasPlayerTask = false;
					m_state = State.EatFlesh;
					return;
				}
			}
			m_state = State.ApproachTarget;
		}
	}

	void updateEatFleshBehaviour()
	{
		updateSenses ();
		if (m_nonLocalizedTargetCandidate != null
		    || m_localizedTargetCandidate != null
		    || m_stateTime > 2.0f ) {
			GetComponent<Animator>().SetBool ("eat", false );
			m_state = State.Alerted;
		}
	}

	void colorizeObject( GameObject obj, Color color )
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

	void setNonLocalizedTargetCandidate( GameObject obj )
	{
		//colorizeObject( m_nonLocalizedTargetCandidate, Color.white );
		m_nonLocalizedTargetCandidate = obj;
	}

	void setLocalizedTargetCandidate( GameObject obj )
	{
		//colorizeObject( m_localizedTargetCandidate, Color.white );
		m_localizedTargetCandidate = obj;
	}

	void setTargetObject( GameObject obj )
	{
		//colorizeObject( m_targetObject, Color.white );
		m_targetObject = obj;
	}

	bool isInViewFrustum()
	{
		Bounds bounds = GetComponent<Collider> ().bounds;
		bounds.Expand (new Vector3 (2.0f, 2.0f, 2.0f));
		Plane[] planes = GeometryUtility.CalculateFrustumPlanes( Camera.main );
		return GeometryUtility.TestPlanesAABB( planes, bounds );
	}
		
	void updateState()
	{
		m_time += Time.deltaTime;
		m_stateTime += Time.deltaTime;
		State oldState = m_state;

		switch( m_state )
		{
			case State.WaitForComponents:
				updateWaitForComponentsBehavior ();
				break;

			case State.Init:
				updateInitBehaviour ();
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

			case State.ApproachTarget:
				updateApproachBehaviour();
				break;

			case State.TargetInRange:
				updateTargetInRangeBehaviour();
				break;

			case State.Attack:
				updateAttackBehaviour();
				break;

			case State.EatFlesh:
				updateEatFleshBehaviour();
				break;

			case State.Stunned:
				break;

			case State.Dead:
				break;
		}

		//colorizeObject( m_nonLocalizedTargetCandidate, Color.blue );
		//colorizeObject( m_localizedTargetCandidate, Color.green );
		//colorizeObject( m_targetObject, Color.red );

		if( m_state != oldState )
		{
			m_stateTime = 0.0f;
		}
      
		Animator animatorComponent = GetComponent<Animator> ();
		if (animatorComponent != null) {
			animatorComponent.SetBool ("walk", !reachedPosition ());
		}

		GetComponent<NavMeshAgent>().speed = m_hasPlayerTask ? 1.2f : 1.2f;
        
    }

	//Transform[] hinges = GameObject.FindObjectsOfType (typeof(Transform)) as Transform[];

    void GoToTag(string Tag)
    {
        GameObject[] taggedObjects = GameObject.FindGameObjectsWithTag(Tag);
        if (taggedObjects.Length >= 1)
        {
           Random.seed = (int) Time.time;
            int RandNum = Random.Range(0, taggedObjects.Length-1);
         
            taskObject = taggedObjects[RandNum];
            GetComponent<NavMeshAgent>().SetDestination(taskObject.transform.position);
            m_oldPosition = GetComponent<Transform>().position;
			m_hasPlayerTask = true;
        }
    }

    void Start()
    {
		if (initDelay > 0.0f) {
			m_state = State.Init;
		} else {
			m_state = State.WaitForComponents;
		}
    }

	public void handleBulletImpact( Collision collision )
	{
		HealthComponent h = GetComponent<HealthComponent>();
		if( h != null && h.enabled ){
			h.dealDamage( 25.0f );
			if( h.isDead() ){
				ZombieBehavior.turnIntoRagdoll( gameObject );
			}
		}
	}

	/*
	// stop the character at a barricade
	void OnCollisionEnter(Collision collision) {
    
		//Debug.Log (collision.gameObject.tag);
		if (collision.gameObject.tag == "Human")
		{
			if (this.tag == "Human"){
				m_hasPlayerTask = false;
				
				taskObject = null;
			}
		}
		// if hit a door, barricade or human go somewhere else. or if zombie target the object
        if (collision.gameObject.tag == "Breakable" || collision.gameObject.tag == "Barricade" || collision.gameObject.tag == "Door" || collision.gameObject.tag == "Window" || collision.gameObject.tag == "Human")
		{
			GetComponent<Animator> ().SetFloat ("speed", 0.2f );
			
			if (this.tag == "Human")
			{
                //attack the human
                taskObject = collision.gameObject;
				m_hasPlayerTask = true;
			}
			else
			{
				taskObject = collision.gameObject;
				m_hasPlayerTask = true;
			}
			
		}
	}
*/

	public void setTargetFromRaycastHit( RaycastHit hit )
	{
		GameObject colliderRootObject = ProjectileBehaviour.getRootObject (hit.collider.gameObject);
		if (colliderRootObject.tag == opposingFactionTag ()) {
			setTargetObject (colliderRootObject);
			m_targetPosition = m_targetObject.GetComponent< Transform > ().position;
		} else {
			setTargetObject (null);
			m_targetPosition = hit.point;
		}
	
		m_state = State.ApproachTarget;
		m_hasPlayerTask = true;
		GetComponent<Animator> ().SetBool ("attack", false);
		GetComponent<Animator> ().SetBool ("eat", false);
	}

	// Update is called once per frame
	void Update ()
	{
		/* @Bill: to test the input again,
		 * comment out the call to updateState and instead
		 * uncomment everything that is commented below
		*/

		updateState();

		/*
	*if (Input.GetKeyDown ("f")){
           // print("f key");
				taskObject =  GameObject.FindWithTag("Player");
                GetComponent<NavMeshAgent>().SetDestination(taskObject.transform.position);
                m_oldPosition = GetComponent<Transform>().position;
                m_hasPlayerTask = true;					
		}

		if (Input.GetKeyDown ("r")){
           // print("r key");
				GoToTag ("Human");
                m_oldPosition = GetComponent<Transform>().position;					
		}

        if (Input.GetKeyDown("b"))
        {
            
            GoToTag("Breakable");
            m_oldPosition = GetComponent<Transform>().position;
        }

        if (Input.GetKeyDown("g"))
        {
           
            GoToTag("Window");
            m_oldPosition = GetComponent<Transform>().position;
        }

		if (taskObject) {
           // print("ZombieBehavior:" + " Parent: " + this.transform.parent.gameObject + " This Object = " + this.gameObject + " taskObject = " + taskObject + "\n");
            GetComponent<NavMeshAgent>().SetDestination(taskObject.transform.position);
           
            m_hasPlayerTask = true;
		}


		if (m_hasPlayerTask) {
            */
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
	

}//endzombiebehavior