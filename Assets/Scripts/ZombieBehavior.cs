using UnityEngine;
using System.Collections;

public class ZombieBehavior : SensingEntity {
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
		DealDamage,			// dealing damage to target
		EatFlesh,			// eat dead target
		Hit,				// hit, target has some time to escape
		Dead,				// dead, this time really
	};
    
	public float initDelay = 0.0f;
	State m_state;
	float m_stateTime = 0.0f;
	Vector3 m_forwardDirectionBeforeLookingAround;
	Vector3 m_rightDirectionBeforeLookingAround;
	public GameObject taskObject;
	GameObject previousObject;
	bool m_hasPlayerTask = false;
	Vector3 m_oldPosition;
	GameObject m_victimHead = null;

    public bool hasPlayerTask()
	{
		return m_hasPlayerTask;
	}

	void updateSpawnBehaviour()
	{
		updateSenses();
		/*
		if( m_localizedObjectOfInterestCandidate != null )
		{
			setObjectOfInterest( m_localizedObjectOfInterestCandidate );
			m_positionOfInterest = m_objectOfInterest.GetComponent< Transform >().position;
			m_state = State.ApproachTarget;
			return;
		}

		approachPosition( m_positionOfInterest );
		if( reachedPosition() )
		*/
		{
			m_state = State.Idle;
			m_positionOfInterest = transform.position;
		}
	}

	void updateWaitForComponentsBehavior()
	{
		if( GetComponent<Animator>() != null )
		{
			m_state = State.Idle;
			m_positionOfInterest = transform.position;
           
			// experimental: zombies go to player
/*
			GameObject player = GameObject.FindGameObjectWithTag ("Player");
			if (player != null && initDelay == 0.0f ) {
				setObjectOfInterest (null);
				m_positionOfInterest = player.transform.position;		
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
		if( m_localizedObjectOfInterestCandidate != null )
		{
			setObjectOfInterest( m_localizedObjectOfInterestCandidate );
			m_positionOfInterest = m_objectOfInterest.GetComponent< Transform >().position;
			m_state = State.ApproachTarget;
		}
		else if( m_nonLocalizedObjectOfInterestCandidate != null )
		{
			m_state = State.Alerted;
		}
	}

	void updateAlertBehaviour()
	{
		updateSenses();/*
		if (m_localizedObjectOfInterestCandidate != null) {
			setObjectOfInterest (m_localizedObjectOfInterestCandidate);
			m_positionOfInterest = m_objectOfInterest.GetComponent< Transform > ().position;
			m_state = State.ApproachTarget;
			return;
		} else*/ if (m_nonLocalizedObjectOfInterestCandidate != null || m_stateTime < 3.5f ) {
			// glance left and right (need anim later)
			//transform.Rotate( 0.0f, 90.0f * Time.deltaTime * (m_stateTime > 1.0f && m_stateTime <= 3.0f ? 1.0f : -1.0f), 0.0f );
			if( m_stateTime == 0.0f )
			{
				m_forwardDirectionBeforeLookingAround = transform.forward;
				m_rightDirectionBeforeLookingAround = transform.right;
			}
			else
			{
				Quaternion originalRotation = Quaternion.FromToRotation (Vector3.forward, m_forwardDirectionBeforeLookingAround);
				Quaternion glanceRightRotation = Quaternion.FromToRotation (Vector3.forward, m_rightDirectionBeforeLookingAround);
				Quaternion glanceLeftRotation = Quaternion.FromToRotation (-Vector3.forward, m_rightDirectionBeforeLookingAround);
				float lerpFactor = 2.0f;
				if( m_stateTime >= 0.5f && m_stateTime < 1.5f )
				{
					lerpFactor = (m_stateTime - 0.5f ) * 4.0f;
					transform.rotation = Quaternion.Lerp (originalRotation, glanceRightRotation, Mathf.Sin(Mathf.Clamp01(lerpFactor)*Mathf.PI*0.5f));
				}
				else if( m_stateTime >= 1.5f && m_stateTime < 2.0f )
				{
					lerpFactor = (m_stateTime - 1.5f ) * 2.0f;
					transform.rotation = Quaternion.Lerp (glanceRightRotation, originalRotation, Mathf.Sin(Mathf.Clamp01(lerpFactor)*Mathf.PI*0.5f));
				}
				else if( m_stateTime >= 2.0f && m_stateTime < 3.0f )
				{
					lerpFactor = (m_stateTime - 2.0f ) * 2.0f;
					transform.rotation = Quaternion.Lerp (originalRotation, glanceLeftRotation, Mathf.Sin(Mathf.Clamp01(lerpFactor)*Mathf.PI*0.5f));
				}
				else if( m_stateTime >= 3.0f )
				{
					lerpFactor = (m_stateTime - 3.0f ) * 4.0f;
					transform.rotation = Quaternion.Lerp (glanceLeftRotation, originalRotation, Mathf.Sin(Mathf.Clamp01(lerpFactor)*Mathf.PI*0.5f));
				}

				if( lerpFactor < 0.5f )
				{
					m_animationFlags |= (uint)AnimationFlags.Turn;					
				}
			}			
		} else {
			m_state = State.Idle;
		}
	}

	void updateHitBehaviour()
	{
		if (m_stateTime > 4.0f) {
			GetComponent<UnityEngine.AI.NavMeshAgent> ().enabled = true;
			m_state = State.Idle;
		}
		else if (m_stateTime > 2.0f) {
			GetComponent<RagdollHelper> ().ragdolled = false;
		}
	}

	bool dealDamage( GameObject human, float damage )
	{
		HealthComponent health = human.GetComponent<HealthComponent> ();
		HumanBehavior h = human.GetComponent<HumanBehavior> ();

		if( health == null )
		{
			return true;
		}

		health.dealDamage( damage, gameObject );

		if( health.isDead() )
		{
			if( h != null && h.turnIntoRagdoll() )
			{
				m_victimHead = human;
             
                foreach (Transform child in human.GetComponentsInChildren<Transform>())
                {
                    if (child.name == "Head")
                    {
						m_victimHead = child.gameObject;
						break;
                    }
                }
            }
		}

		return health.isDead ();
	}

	void reanimate()
	{
		HealthComponent health = GetComponent<HealthComponent> ();
		health.reanimate ();
		tag = "Zombie";
      
        this.gameObject.name = "Zombie" + (GameObject.FindGameObjectsWithTag("Zombie").Length).ToString();
        //this.gameObject.transform.parent = GameObject.Find("ZombieParent").transform;
        speedMultiplier = Random.Range (0.5f, 2.5f);
		
		UnityEngine.AI.NavMeshAgent n = GetComponent<UnityEngine.AI.NavMeshAgent>();
		Animator a = GetComponent<Animator>();
		RagdollHelper r = GetComponent<RagdollHelper> ();
			
		if (n != null && a != null && r != null) {			
			n.enabled = true;
			n.SetDestination (this.transform.position);
		}
	}

	void updateApproachBehaviour()
	{
		verifyObjectOfInterest ();
		if( m_hasPlayerTask )
		{
			setLocalizedObjectOfInterestCandidate (null);
			setNonLocalizedObjectOfInterestCandidate (null);
		}
		else
		{
			updateSenses ();
			return;// do not approach anybody if not given a command
		}

		if( m_localizedObjectOfInterestCandidate != null && !m_hasPlayerTask )
		{
			setObjectOfInterest( m_localizedObjectOfInterestCandidate );
		}

		if( m_objectOfInterest != null )
		{
			m_positionOfInterest = m_objectOfInterest.GetComponent< Transform > ().position;
        }
        approachPosition(m_positionOfInterest);
        if ( reachedPosition() )
		{
			if( m_objectOfInterest != null && m_positionOfInterest == m_objectOfInterest.GetComponent< Transform >().position )
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
		m_animationFlags |= (uint)AnimationFlags.Attack;
		m_state = State.Attack;
	}

	void updateAttackBehaviour()
	{
		if( m_stateTime >= 0.5f )
		{
			m_state = State.DealDamage;
		}
	}

	void updateDealDamageBehaviour()
	{
		if (m_objectOfInterest != null)
		{
			if (dealDamage (m_objectOfInterest, 33.3f))
			{
				setObjectOfInterest (null);
				setLocalizedObjectOfInterestCandidate( null );
				setNonLocalizedObjectOfInterestCandidate( null );
				m_hasPlayerTask = false;
				m_state = State.EatFlesh;
				return;
			}
		}
		m_state = State.ApproachTarget;
	}

	void updateEatFleshBehaviour()
	{
		if (m_stateTime == 0.0f) {
			m_animationFlags |= (uint)AnimationFlags.Eat;
		}

		if (m_victimHead != null && m_stateTime < 3.0f) {
			Vector3 offset = m_victimHead.transform.position;
			offset -= transform.position;
			offset.y = 0.0f;
			offset.Normalize ();
			transform.rotation = Quaternion.FromToRotation (Vector3.forward, offset);
			offset *= 0.25f;
			Vector3 finalEatingPosition = m_victimHead.transform.position - offset;
			finalEatingPosition.y = transform.position.y;
			Vector3 delta = finalEatingPosition - transform.position;
			if (delta.sqrMagnitude < 9.0) {
				transform.position = transform.position + 0.1f * delta;
			}
			if ((int)(m_stateTime + 0.5f) != (int)(m_stateTime + 0.5f + Time.deltaTime)) {
				Instantiate( MainGameManager.instance.bloodParticles, m_victimHead.transform.position, Quaternion.LookRotation(new Vector3(0,1,0), new Vector3(0,0,1)));
			}
			// disable collision while eating
			setCollidersEnabled(false);
		}

		approachPosition (transform.position);

		if (m_stateTime > 6.8f ) { // full playback of animation
			// enable collision again
			setCollidersEnabled(true);
			m_state = State.ApproachTarget;
		}
	}


	/*
	bool isInViewFrustum()
	{
		Bounds bounds = GetComponent<Collider> ().bounds;
		bounds.Expand (new Vector3 (2.0f, 2.0f, 2.0f));
		Plane[] planes = GeometryUtility.CalculateFrustumPlanes( Camera.main );
		return GeometryUtility.TestPlanesAABB( planes, bounds );
	}
	*/
		
	void updateState()
	{
		m_animationFlags = 0u;

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

			case State.DealDamage:
				updateDealDamageBehaviour();
				break;

			case State.EatFlesh:
				updateEatFleshBehaviour();
				break;

			case State.Hit:
				updateHitBehaviour();
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


		if( !reachedPosition() && m_animationFlags == 0u )
		{
			m_animationFlags |= (uint)AnimationFlags.Run;
		}

		updateAnimationState ();
		
        UnityEngine.AI.NavMeshAgent nma = GetComponent<UnityEngine.AI.NavMeshAgent>();
        if (!nma) { return; }
		GetComponent<UnityEngine.AI.NavMeshAgent> ().speed = 1.2f * speedMultiplier;//m_hasPlayerTask ? 1.2f : 1.2f;
    }

	public void die()
	{
		m_state = State.Dead;
	}

	void dealSomeDamageAndTurnIntoRagdoll()
	{
		HealthComponent h = GetComponent<HealthComponent>();
		if( h != null && h.enabled ){
			h.dealDamage( 25.0f, null );
			turnIntoRagdoll();
			if( !h.isDead() ) {
				m_state = State.Hit;
				m_stateTime = 0.0f;
			}
		}
	}

    public void handleBulletImpact( Collision collision )
	{
		dealSomeDamageAndTurnIntoRagdoll ();
		Instantiate( MainGameManager.instance.bloodParticles, collision.transform.position, collision.transform.rotation);
	}

	public void handleKicked( GameObject kicker )
	{
		Vector3 impact = transform.position;
		impact -= kicker.transform.position;
		impact.Normalize ();
		impact *= 2.0f;
		Component[] rigidBodies = GetComponentsInChildren<Rigidbody> ();
		foreach (Rigidbody r in rigidBodies) {
			r.AddForce(impact,ForceMode.VelocityChange);
		}

		dealSomeDamageAndTurnIntoRagdoll ();
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

    static public GameObject getRootObject(GameObject obj)
    {
        GameObject root = obj;
        for (; root.transform.parent != null; root = root.transform.parent.gameObject) { }
        return root;
    }
    //Returns the parent object discovered with the appropriate tag.
   static public GameObject getChildRootObject(GameObject obj)
    {
        GameObject rootChild = obj;
        GameObject root = obj;
        string names = obj.name;
        for (; rootChild.transform.parent != null; rootChild = rootChild.transform.parent.gameObject)
        {
            names = rootChild.name;
            if (rootChild.tag == "Zombie" || rootChild.tag == "Human")
            {
               // Debug.Log(names);
                return rootChild;
            }
        }
        return root;
    }

    public void setTargetFromRaycastHit( RaycastHit hit )
	{
        // getChildRootObject for Humans/Zombies contained in the HumanParent/ZombieParent
        //GameObject colliderRootObject = getChildRootObject(hit.collider.gameObject.transform.parent.gameObject);
		GameObject colliderRootObject = CrosshairBehaviour.getRecentlyFocusedHuman() != null ? CrosshairBehaviour.getRecentlyFocusedHuman() : getRootObject(hit.collider.gameObject.transform.parent.gameObject);
        if (colliderRootObject.tag == getOpposingFactionTag ()) {
			setObjectOfInterest (colliderRootObject);
			m_positionOfInterest = m_objectOfInterest.GetComponent< Transform > ().position;
		} else {
			setObjectOfInterest (null);
			m_positionOfInterest = hit.point;
		}
        	
		m_state = State.ApproachTarget;
		m_hasPlayerTask = true;
	}

   public void GoToTag(string Tag)
    {
        m_hasPlayerTask = false;
        m_state = State.Idle;
        GameObject[] taggedObjects = GameObject.FindGameObjectsWithTag(Tag);
        if (taggedObjects.Length >= 1)
        {
            Random.InitState(Random.Range(0,(int)Time.time)+(int)Time.time);
            int RandNum = Random.Range(0, taggedObjects.Length);
            taskObject = taggedObjects[RandNum];

            if (taskObject)
            {               
                setObjectOfInterest(taskObject);
                m_oldPosition = GetComponent<Transform>().position;
                m_state = State.ApproachTarget;
                m_hasPlayerTask = true;             
            }
        }
        else
        {
            // play no target sound
            GameObject ntp = GameObject.Find("NoTarget_Player");
            if (ntp != null)
            {
                NoTarget_Sound nts = ntp.GetComponent<NoTarget_Sound>();
                if (nts)
                {
                    NoTarget_Sound ntsStart = GameObject.Find("NoTarget_Player").GetComponent<NoTarget_Sound>();
                    ntsStart.StartCoroutine("PlaySound");
                }
            }
        }
    }
   
    void Start()
    {
		base.Start ();
        if (initDelay > 0.0f)
        {
            m_state = State.Init;
        }
        else
        {
            m_state = State.WaitForComponents;
        }

    }

    void Update()
    {
		base.Update ();
        updateState();
    
        if (GetComponent<UnityEngine.AI.NavMeshAgent>().enabled)
        {
           // zombieKeyboardInput(); // passed control to a keyboard script? Probably A good idea
            Vector3 movement = GetComponent<Transform>().position - m_oldPosition;
            m_oldPosition = GetComponent<Transform>().position;
            Vector3 diff = GetComponent<Transform>().position - GetComponent<UnityEngine.AI.NavMeshAgent>().destination;
    
			if (GetComponent<Animator> ()) {
				if (diff.magnitude > 0.7f) {
					//GetComponent<Animator> ().SetFloat ("speed", movement.magnitude / Time.deltaTime);
				} else {
					//GetComponent<Animator> ().SetFloat ("speed", 0.0f);
					//print ( "REACHED" );
					// :TO: this needs to be refactored
					//m_hasPlayerTask = false;
					//previousObject = taskObject;
					//taskObject = null;
				}
			} else {
				this.transform.Translate (Vector3.forward * Time.deltaTime);
			}
        }

    }// end update

}//endzombiebehavior