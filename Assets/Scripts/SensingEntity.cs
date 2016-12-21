using UnityEngine;
using System.Collections;

public class SensingEntity : MonoBehaviour {
	
	protected float m_earQueryInterval = 0.5f;
	protected float m_eyeQueryInterval = 0.5f;
	protected GameObject m_nonLocalizedObjectOfInterestCandidate = null;
	protected GameObject m_localizedObjectOfInterestCandidate = null;
	protected GameObject m_objectOfInterest = null;
	protected Vector3 m_positionOfInterest;
	private float m_runnerAlertSqrDistanceThreshold = 256.0f;
	private float m_runnerDetectSqrDistanceThreshold = 64.0f;
	private float m_time = 0.0f;

	public void Init()
	{
		m_time = Random.value; // avoid triggering the senses of all objects at the same time
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
		NavMeshAgent nma = GetComponent<NavMeshAgent>();
		if( nma == null || !nma.enabled || !nma.isOnNavMesh )
		{
			return;
		}
		NavMeshHit hit;
		if( NavMesh.SamplePosition( transform.position, out hit, 3, NavMesh.AllAreas ) )
		{
			nma.SetDestination( targetPosition );
		}
	}

	protected bool reachedPosition()
	{
		NavMeshAgent nma = GetComponent<NavMeshAgent>();
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

		m_time += Time.deltaTime;

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
					// ignore corpses for now
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
					if( Vector3.Dot( direction2D, viewDirection ) > 0.707f )
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
}
