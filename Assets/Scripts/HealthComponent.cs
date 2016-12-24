using UnityEngine;
using System.Collections;

public class HealthComponent : MonoBehaviour {
	public float initialHealth = 100.0f;   
	public float current_health;

    //breakable barricades code.
    private GameObject thisObject;
    public Rigidbody rb;

    public float radius = 15.0F;
    public float power = 110.0F;
	private Rigidbody hasRigidBody;
	private Vector3 childscale;
	private GameObject m_killer = null;
	
        void Awake()
        {
            rb = this.GetComponent<Rigidbody>();
        }


		void EnableRagdoll() {
			rb.isKinematic = false;
			rb.detectCollisions = true;
		}
		void DisableRagdoll() {
			rb.isKinematic = true;
			rb.detectCollisions = false;
		}

    // Use this for initialization
    void Start()
    {
        rb = this.GetComponent<Rigidbody>();
        if (rb != null) { EnableRagdoll(); }
        reanimate();
    }

	public void dealDamage( float damage, GameObject damageDealer ) {
        current_health -= damage;
		if (isDead ()) {
			m_killer = damageDealer;
		}
	}

	public bool isDead() {
		return current_health <= 0.0f;
	}

	public bool wasKilledBy( GameObject obj )
	{
		return isDead () && m_killer == obj;
	}

	public void reanimate() {
        current_health = initialHealth;
		m_killer = null;
	}

    public void explodeInstantly()
    {
        current_health = 0;
       this.tag = "Untagged";
      
            if (this.GetComponent<Rigidbody>() != null)
            {
                DisableRagdoll();
            }
            // iterate over child objects and give them rigidbody
            foreach (Transform child in transform)
            {
                child.gameObject.tag = "Wood";
                hasRigidBody = child.gameObject.GetComponent<Rigidbody>();
                // break the object apart
                if (hasRigidBody == null)
                {
                    Vector3 childscale = child.gameObject.transform.localScale;
                    childscale.x = childscale.x * 0.45F;
                    childscale.y = childscale.y * 0.45F;
                    childscale.z = childscale.z * 0.75F;
                    child.gameObject.transform.localScale = new Vector3(childscale.x, childscale.y, childscale.z);
                    Rigidbody gameObjectsRigidBody = child.gameObject.AddComponent<Rigidbody>(); // Add the rigidbody.
                    gameObjectsRigidBody.mass = 1; // Set the GO's mass to 5 via the Rigidbody.
                                                   // Rigidbody rb = hit.GetComponent<Rigidbody>();

                    if (gameObjectsRigidBody != null)
                    {
                        gameObjectsRigidBody.AddExplosionForce(power, child.gameObject.transform.position, radius, 3.0F);
                    }
                }
            }
        }
   
}
