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
		EatFlesh,			// eat dead target
		Hit,				// hit, target has some time to escape
		Dead,				// dead, this time really
	};
    
	public float initDelay = 0.0f;
	public float speedMultiplier = 1.0f;
	State m_state;
	float m_earQueryInterval = 0.5f;
	float m_eyeQueryInterval = 0.5f;
	float m_time = 0.0f;
	float m_stateTime = 0.0f;
	public GameObject taskObject;
	GameObject previousObject;
	bool m_hasPlayerTask = false;
	Vector3 m_oldPosition;
    public bool hasPlayerTask()
	{
		return m_hasPlayerTask;
	}

	void approachPosition( Vector3 targetPosition )
	{
        if (this.GetComponent<NavMeshAgent>().isOnNavMesh) { 
           GetComponent<NavMeshAgent>().SetDestination(targetPosition);
        }

    }

    bool reachedPosition()
    {
        NavMeshAgent nma = GetComponent<NavMeshAgent>();
        if (!nma) { return false; }
		return (nma.destination - transform.position).sqrMagnitude < 1.5f || nma.enabled == false;
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
		updateSenses();
		if (m_localizedObjectOfInterestCandidate != null) {
			setObjectOfInterest (m_localizedObjectOfInterestCandidate);
			m_positionOfInterest = m_objectOfInterest.GetComponent< Transform > ().position;
			m_state = State.ApproachTarget;
			return;
		} else if (m_nonLocalizedObjectOfInterestCandidate != null || m_stateTime < 4.0f ) {
			// glance left and right (need anim later)
			transform.Rotate( 0.0f, 90.0f * Time.deltaTime * (m_stateTime > 1.0f && m_stateTime <= 3.0f ? 1.0f : -1.0f), 0.0f );
		} else {
			m_state = State.Idle;
		}
	}

	void updateHitBehaviour()
	{
		if (m_stateTime > 4.0f) {
			GetComponent<NavMeshAgent> ().enabled = true;
			GetComponent<Animator> ().enabled = true;
			m_state = State.Idle;
		}
		else if (m_stateTime > 2.0f) {
			GetComponent<RagdollHelper> ().ragdolled = false;
		}
	}

	static public bool turnIntoRagdoll( GameObject obj )
	{
		NavMeshAgent n = obj.GetComponent<NavMeshAgent>();
		Animator a = obj.GetComponent<Animator>();
		HumanBehavior h = obj.GetComponent<HumanBehavior>();
		ZombieBehavior z = obj.GetComponent<ZombieBehavior>();
		RagdollHelper r = obj.GetComponent<RagdollHelper> ();
        HealthComponent hc = obj.GetComponent<HealthComponent>();

		bool result = false;
		if (n != null && a != null && r != null) {
			r.ragdolled=true;
			
			if (h != null) {
				h.dropWeapon();
				a.runtimeAnimatorController = h.zombieAnimationController; // use zombie animation controller after resurrection
				Destroy (h);
                obj.AddComponent<ZombieBehavior>();
                obj.GetComponent<ZombieBehavior>().initDelay = 8.0f;
                result = true;
			} else if (z != null) {
                if (obj.GetComponent<HealthComponent>().isDead()) {                    
                    Destroy(z);   
                }
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
                m_state = State.EatFlesh;
                updateEatFleshBehaviour();
             
                bool hasHead = false;
                GameObject eatTarget = null;
                foreach (Transform child in GetComponentsInChildren<Transform>())
                {
                    
                    if (child.name == "Head")
                    {
                        hasHead = true;
                        eatTarget = child.gameObject;                         
                    }
                }

                if (hasHead)
                {
                    m_objectOfInterest = eatTarget;
                    m_state = State.ApproachTarget;
                  
                }

              //  m_objectOfInterest = null;

                return false;
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
		
		NavMeshAgent n = GetComponent<NavMeshAgent>();
		Animator a = GetComponent<Animator>();
		RagdollHelper r = GetComponent<RagdollHelper> ();
			
		if (n != null && a != null && r != null) {			
			n.enabled = true;
			a.enabled = true;
			n.SetDestination (this.transform.position);
		}
	}

	void updateApproachBehaviour()
	{
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
		GetComponent<Animator> ().SetBool ("attack", true);
		m_state = State.Attack;
	}

	void updateAttackBehaviour()
	{
		GetComponent<Animator> ().SetBool ("attack", false);
		if (m_stateTime > 0.1f) {
			if (m_objectOfInterest != null) {
				if (dealDamage (m_objectOfInterest, 33.3f)) {
					setObjectOfInterest (null);
					setLocalizedObjectOfInterestCandidate( null );
					setNonLocalizedObjectOfInterestCandidate( null );
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
		if (m_nonLocalizedObjectOfInterestCandidate != null
		    || m_localizedObjectOfInterestCandidate != null
		    || m_stateTime > 2.0f ) {
			GetComponent<Animator>().SetBool ("eat", false );
			m_state = State.ApproachTarget;
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

			case State.Hit:
				updateHitBehaviour();
				break;

			case State.Dead:
				break;
		}

		//colorizeObject( m_nonLocalizedObjectOfInterestCandidate, Color.blue );
		//colorizeObject( m_localizedObjectOfInterestCandidate, Color.green );
		//colorizeObject( m_objectOfInterest, Color.red );

		if( m_state != oldState )
		{
			m_stateTime = 0.0f;
		}
      
		Animator animatorComponent = GetComponent<Animator> ();
		if (animatorComponent != null ) {
			animatorComponent.SetBool ("walk", !reachedPosition ());
			animatorComponent.SetFloat ("speedMultiplier", speedMultiplier);
		}
        NavMeshAgent nma = GetComponent<NavMeshAgent>();
        if (!nma) { return; }
		GetComponent<NavMeshAgent> ().speed = 1.2f * speedMultiplier;//m_hasPlayerTask ? 1.2f : 1.2f;
        


    }

    //Transform[] hinges = GameObject.FindObjectsOfType (typeof(Transform)) as Transform[];


    
    public void handleBulletImpact( Collision collision )
	{
		HealthComponent h = GetComponent<HealthComponent>();
		if( h != null && h.enabled ){
			h.dealDamage( 25.0f );
			ZombieBehavior.turnIntoRagdoll( gameObject );
			if( !h.isDead() ) {
				m_state = State.Hit;
				m_stateTime = 0.0f;
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
        GameObject colliderRootObject = getRootObject(hit.collider.gameObject.transform.parent.gameObject);
        if (colliderRootObject.tag == getOpposingFactionTag ()) {
			setObjectOfInterest (colliderRootObject);
			m_positionOfInterest = m_objectOfInterest.GetComponent< Transform > ().position;
		} else {
			setObjectOfInterest (null);
			m_positionOfInterest = hit.point;
		}
        	
		m_state = State.ApproachTarget;
		m_hasPlayerTask = true;
		GetComponent<Animator> ().SetBool ("attack", false);
		GetComponent<Animator> ().SetBool ("eat", false);
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
     
        updateState();
    
        if (GetComponent<NavMeshAgent>().enabled)
        {
           // zombieKeyboardInput(); // passed control to a keyboard script? Probably A good idea
            Vector3 movement = GetComponent<Transform>().position - m_oldPosition;
            m_oldPosition = GetComponent<Transform>().position;
            Vector3 diff = GetComponent<Transform>().position - GetComponent<NavMeshAgent>().destination;
    
			if (GetComponent<Animator> ()) {
				if (diff.magnitude > 0.7f) {
					GetComponent<Animator> ().SetFloat ("speed", movement.magnitude / Time.deltaTime);
				} else {
					GetComponent<Animator> ().SetFloat ("speed", 0.0f);
					//print ( "REACHED" );
					m_hasPlayerTask = false;
					previousObject = taskObject;
					taskObject = null;
				}
			} else {
				this.transform.Translate (Vector3.forward * Time.deltaTime);
			}
        }

    }// end update

}//endzombiebehavior