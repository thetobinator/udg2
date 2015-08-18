using UnityEngine;
using System.Collections;

public class ZombieBehavior : MonoBehaviour {
	enum State
	{
		Spawning,			// go from spawn point to idle point
		Idle,				// no humans sensed, just stand there
		Alerted,			// sensed humans without exact localization, look around, moaning sounds
		ApproachTarget,		// a human is localized, approach target
		TargetInRange,		// the target can be attacked
		Attack,				// attacking the target
		EatFlesh,			// eat dead target
		Stunned,			// stunned, target has some time to escape
		Undead,				// down, appears to be dead but will resurrect
		Resurrecting,		// getting up from the dead again
		Dead,				// dead, this time really
		Dissolve,			// dead for some time, dissolve or disappear in some other way (maybe move slowly under the floor)
		Remove				// fully dissolved/under the floor, game object will be removed
	};

	GameObject m_nonLocalizedTargetCandidate = null;
	GameObject m_localizedTargetCandidate = null;
	GameObject m_targetObject = null;
	Vector3 m_targetPosition;
	State m_state;
	float m_earQueryFrequency;
	//List<
	
	void updateSenses()
	{
		// :TODO: :TO: implement this first, add some debug visualization of sensed humans (maybe by turning them red in the shader)
		// basic approach: query surrounding humans (use some sort of grid) with a certain frequency and check if they can be seen, heard or smelled, TBD
		// fill m_nonLocalizedTargetCandidate and m_localizedTargetCandidate accordingly

		// DUMMY, tint closest human red, everyone else white
		GameObject[] humans = GameObject.FindGameObjectsWithTag ("Human");
		GameObject closestHuman = null;
		float closestHumanSqrDistance = float.MaxValue;


		
		foreach (GameObject human in humans) {
			DebugTint debugTint = human.GetComponent<DebugTint>();

			if( debugTint != null )
			{
				if( MainGameManager.instance.getObjectSpeed ( human ) > 1.0f )
				{
					debugTint.tintColor = Color.blue;
				}
				else
				{
					debugTint.tintColor = Color.white;
				}
				float sqrDistanceToHuman = ( human.GetComponent<Transform>().position - GetComponent<Transform>().position ).sqrMagnitude;
				if( sqrDistanceToHuman < closestHumanSqrDistance )
				{
					closestHumanSqrDistance = sqrDistanceToHuman;
					closestHuman = human;
				}
			}
		}

		if (closestHuman != null) {
			closestHuman.GetComponent<DebugTint>().tintColor = Color.red;
		}
	}

	void approachPosition( Vector3 targetPosition )
	{
		
	}

	bool reachedPosition( Vector3 targetPosition )
	{
		return false;
	}

	void updateSpawnBehaviour()
	{
		updateSenses();
		if( m_localizedTargetCandidate != null )
		{
			m_targetObject = m_localizedTargetCandidate;
			m_state = State.ApproachTarget;
			return;
		}

		approachPosition( m_targetPosition );
		if( reachedPosition( m_targetPosition ) )
		{
			m_state = State.Idle;
		}
	}
		
	void updateIdleBehaviour()
	{
		updateSenses ();
		if( m_localizedTargetCandidate != null )
		{
			m_targetObject = m_localizedTargetCandidate;
			m_state = State.ApproachTarget;
			return;
		}
		else if( m_nonLocalizedTargetCandidate != null )
		{
			m_state = State.Alerted;
			return;
		}
	}
		
	void updateState()
	{
		switch( m_state )
		{
			case State.Spawning:
				updateSpawnBehaviour();
				break;
				
			case State.Idle:
				updateIdleBehaviour();
				break;

			case State.Alerted:
			case State.ApproachTarget:
			case State.TargetInRange:
			case State.Attack:
			case State.EatFlesh:
			case State.Stunned:
			case State.Undead:
			case State.Resurrecting:
			case State.Dead:
			case State.Dissolve:
			case State.Remove:
				break;
		}
	}

	public Camera m_camera;
	public GameObject taskObject;
	GameObject previousObject;
	bool m_hasDestination = false;
	Vector3 m_oldPosition;

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
            m_hasDestination = true;
        }
    }

    void Start()
    {
        GoToTag("Player");

		m_state = State.Spawning;
    }
	
	// stop the character at a barricade
	void OnCollisionEnter(Collision collision) {
    
		//Debug.Log (collision.gameObject.tag);
		if (collision.gameObject.tag == "Human")
		{
			if (this.tag == "Human"){
				m_hasDestination = false;
				
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
				m_hasDestination = true;
			}
			else
			{
				taskObject = collision.gameObject;
				m_hasDestination = true;
			}
			
		}
	}


	
	// Update is called once per frame
	void Update ()
	{
		updateState();

		//
		//
		// :TODO: :TO: control animation controller parameters and nav mesh agent parameters based on the state
		//
		//

		m_camera =   Camera.main;
       
		if (Input.GetMouseButtonUp (0)) {
			
			Ray ray = m_camera.GetComponent<Camera> ().ScreenPointToRay (Input.mousePosition);
			RaycastHit hit = new RaycastHit ();
			if (Physics.Raycast (ray, out hit)) {

						GetComponent<NavMeshAgent> ().SetDestination (hit.point);
						
                        //if (hit.collider.tag == "Terrain" ) { taskObject = null; }
				m_hasDestination = true;
                m_oldPosition = GetComponent<Transform>().position;
			}
		}

		if (Input.GetKeyDown ("f")){
           // print("f key");
				taskObject =  GameObject.FindWithTag("Player");
                GetComponent<NavMeshAgent>().SetDestination(taskObject.transform.position);
                m_oldPosition = GetComponent<Transform>().position;
                m_hasDestination = true;					
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
           
            m_hasDestination = true;
		}

		if (m_hasDestination) {
            
			Vector3 movement = GetComponent<Transform> ().position - m_oldPosition;
			m_oldPosition = GetComponent<Transform> ().position;
			Vector3 diff = GetComponent<Transform> ().position - GetComponent<NavMeshAgent> ().destination;
			if (GetComponent<Animator> ()) {
				if (diff.magnitude > 0.7f) {
					GetComponent<Animator> ().SetFloat ("speed", movement.magnitude / Time.deltaTime);
				} else {
					GetComponent<Animator> ().SetFloat ("speed", 0.0f);
					//print ( "REACHED" );
					m_hasDestination = false;
					previousObject = taskObject;
					taskObject = null;
				}
			} else {
				this.transform.Translate (Vector3.forward * Time.deltaTime);
			}
			
		}
	}// end update
	

}