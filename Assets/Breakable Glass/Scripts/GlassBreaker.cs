using UnityEngine;
using System.Collections;

public class GlassBreaker : MonoBehaviour {
	Vector3 vel;
	BreakGlass g;
	void FixedUpdate () {
		vel = GetComponent<Rigidbody>().velocity;
	}
	
	void OnCollisionEnter (Collision col) {
		if(col.gameObject.GetComponent<BreakGlass>()!=null){
			g = col.gameObject.GetComponent<BreakGlass>();
			GetComponent<Rigidbody>().velocity = vel * g.SlowdownCoefficient;
			
			if(g.BreakByVelocity){
				if(col.relativeVelocity.magnitude >= g.BreakVelocity){
					col.gameObject.GetComponent<BreakGlass>().BreakIt();
					return;	
				}
			}
			
			if(g.BreakByImpulse){
				if(col.relativeVelocity.magnitude * GetComponent<Rigidbody>().mass >= g.BreakImpulse){
					col.gameObject.GetComponent<BreakGlass>().BreakIt();
					return;	
				}
			}
			
		}
	}
}
