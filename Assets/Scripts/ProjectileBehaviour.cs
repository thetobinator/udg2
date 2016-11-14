using UnityEngine;
using System.Collections;

public class ProjectileBehaviour : MonoBehaviour {

	public float maxLifetime = 2.0f;
	public float speed = 20.0f;

	Vector3 m_spawnPosition;
	float m_lifeTime;
	Rigidbody m_impactTarget = null; 

	// Use this for initialization
	void Start () {
		m_lifeTime = maxLifetime;
		m_spawnPosition = transform.position;
        // play shot sound
        GameObject go = GameObject.Find("Colt45_Player");
        if (go != null)
        {
            PlayAudioSource sndscript = go.GetComponent<PlayAudioSource>();
            if (sndscript)
            {
                PlayAudioSource mshStart = GameObject.Find("Colt45_Player").GetComponent<PlayAudioSource>();
                mshStart.StartCoroutine("PlaySound");
            }
        }
    }
	
	// Update is called once per frame
	void Update () {
		transform.position = transform.position + transform.up * speed * Time.deltaTime;
		m_lifeTime -= Time.deltaTime;
		if (m_lifeTime < 0.0f) {
			Destroy( gameObject );
		}
		else if (m_impactTarget != null) {
			Vector3 impact = transform.position;
			impact -= m_spawnPosition;
			impact.Normalize ();
			impact *= 2.0f;
			m_impactTarget.AddForce(impact,ForceMode.VelocityChange);
		}
	}

	static public GameObject getRootObject( GameObject obj )
	{
		GameObject root = obj; 
		for( ; root.transform.parent != null; root = root.transform.parent.gameObject ){}
		return root;
	}

    //Returns the parent object discovered with the appropriate tag.
    static public GameObject getChildRootObject(GameObject obj)
    {
        GameObject rootChild = obj;
        GameObject root = obj;
        for (; rootChild.transform.parent != null; rootChild = rootChild.transform.parent.gameObject) {
            if (rootChild.tag == "Zombie" || rootChild.tag == "Human"){           
                return rootChild;
            }
        }
        return root;
    }


    void OnCollisionEnter(Collision collision) {
		if (m_impactTarget == null) {
            // :BILL: rootColliderObject doesn't play well with nesting child to runtime parents. 10 17 2018
            //note we aren't nested in a parent now, betcause SetParent position error.
            GameObject rootColliderObject = getRootObject (collision.gameObject);
            if (rootColliderObject.tag == "Human")
            {
                HumanBehavior hb = rootColliderObject.GetComponent<HumanBehavior>();
                if (hb != null)
                {
                    hb.handleBulletImpact(collision);
                }
            }
            else if (rootColliderObject.tag == "Zombie")
            {
                ZombieBehavior zb = rootColliderObject.GetComponent<ZombieBehavior>();
                if (zb != null)
                {
                    zb.handleBulletImpact(collision);
                }
            }

            if (rootColliderObject.GetComponent<HealthComponent>() != null)
            {
                m_impactTarget = collision.rigidbody;
                m_lifeTime = 0.25f;
            }
/*
            GameObject childColliderObject = getChildRootObject(collision.gameObject);
          
			if (childColliderObject.tag == "Human") {
				HumanBehavior hb = childColliderObject.GetComponent<HumanBehavior> ();
				if (hb != null) {
					hb.handleBulletImpact (collision);
				}
			} else if (childColliderObject.tag == "Zombie") {
				ZombieBehavior zb = childColliderObject.GetComponent<ZombieBehavior> ();
				if (zb != null) {
					zb.handleBulletImpact (collision);
				}
			}

			if (childColliderObject.GetComponent<HealthComponent> () != null ) {
				m_impactTarget = collision.rigidbody;
				m_lifeTime = 0.25f;
			}
 */
		}

		if (m_impactTarget == null) {
			Destroy (gameObject);
		}
	}
}
