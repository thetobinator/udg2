using UnityEngine;
using System.Collections;

public class RigidOnTouch : MonoBehaviour {
    public int hitPoints;
    void Start() {  }
   public void TakeDamage()
    {

        if (hitPoints >= 1)
        {
            hitPoints += -1;
        }
        else
        {
             Rigidbody gameObjectsRigidBody = this.gameObject.GetComponent<Rigidbody>();
             if (gameObjectsRigidBody == null)
             {
              this.gameObject.AddComponent<Rigidbody>(); // Add the rigidbody.
              gameObjectsRigidBody = this.gameObject.GetComponent<Rigidbody>();
             }
            gameObjectsRigidBody.mass = 5; // Set the GO's mass to 5 via the Rigidbody.
            gameObjectsRigidBody.AddExplosionForce(1.0f, this.gameObject.transform.position, 1.0f, 3.0F);
            this.gameObject.tag = "Wood";
            gameObjectsRigidBody.isKinematic = false ;
            //gameObjectsRigidBody.constraints = RigidbodyConstraints.None;
          //BoxCollider myBoxCollider =  this.gameObject.GetComponent<BoxCollider>();
         // myBoxCollider.enabled = false;
            // iterate over child objects and give them rigidbody
            foreach (Transform child in transform)
            {
                Rigidbody hasRigidBody = child.transform.GetComponent<Rigidbody>();
                
                // break the object apart
                if (hasRigidBody == null)
                {
                    print("Adding Rigid Body: " + this + "\n to " + child.gameObject.name);
                    //Rigidbody childBody = child.gameObject.AddComponent<Rigidbody>() as Rigidbody;
                     hasRigidBody = child.gameObject.GetComponent<Rigidbody>();
                }
                    Vector3 childscale = child.gameObject.transform.localScale;
                    hasRigidBody.mass = 5; // Set the GO's mass to 5 via the Rigidbody.
                    childscale.x = childscale.x * 0.95F;
                    childscale.y = childscale.y * 0.95F;
                    childscale.z = childscale.z * 0.95F;
                    child.gameObject.transform.localScale = new Vector3(childscale.x, childscale.y, childscale.z);
                    hasRigidBody.AddExplosionForce(1.0f, this.gameObject.transform.position, 1.0f, 3.0F);
                    hasRigidBody.isKinematic = false;
                    hasRigidBody.constraints = RigidbodyConstraints.None;
                    child.gameObject.transform.tag = "Wood";
                
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Zombie"){
        Rigidbody rb = this.GetComponent<Rigidbody>();
        string myTag = this.gameObject.tag;
        if (rb == null && myTag != "Wood" )
        {         
            TakeDamage();
        }
        else
        {
            rb.isKinematic = false;
            TakeDamage();
        }
        }
    }

    void OnTriggerStay(Collider other) {
        if (other.gameObject.tag == "Zombie")
        {
            Rigidbody rb = this.GetComponent<Rigidbody>();
            string myTag = this.gameObject.tag;
             if (rb == null && myTag != "Wood" )
            {
                TakeDamage();
            }
        }
    }

}
