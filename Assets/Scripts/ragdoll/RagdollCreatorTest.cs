using UnityEngine;
using System.Collections;
using System.Reflection;
using System;

public class RagdollCreatorTest : MonoBehaviour {
	private bool m_initializedRagdoll = false;
	private GameObject m_ragdoll = null;
	
	// Use this for initialization
	void Start () {
		
		
	}

	string[] skippedPropertyNames = 
	{
		"sleepVelocity",
		"sleepAngularVelocity",
		"useConeFriction"
	};
	 
	  T GetCopyOf<T>(this Component comp, T other) where T : Component
	 {
		 Type type = comp.GetType();
		 if (type != other.GetType()) return null; // type mis-match
		 BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Default | BindingFlags.DeclaredOnly;
		 PropertyInfo[] pinfos = type.GetProperties(flags);
		 foreach (var pinfo in pinfos) {
			 if (pinfo.CanWrite) {
				 try {
					bool skipProperty = false;
					for( int i = 0; i < skippedPropertyNames.Length; ++i )
					{
						if( pinfo.Name == skippedPropertyNames[i] )
						{
							skipProperty = true;
							break;
						}
					}

					if( !skipProperty )
					{
						pinfo.SetValue(comp, pinfo.GetValue(other, null), null);
					}
				 }
				 catch { } // In case of NotImplementedException being thrown. For some reason specifying that exception didn't seem to catch it, so I didn't catch anything specific.
			 }
		 }
		 FieldInfo[] finfos = type.GetFields(flags);
		 foreach (var finfo in finfos) {
			 finfo.SetValue(comp, finfo.GetValue(other));
		 }
		 return comp as T;
	 }
	 
	 enum Bone
	 {
		 Hips,
		 LeftUpLeg,
		 LeftLeg,
		 LeftFoot,
		 RightUpLeg,
		 RightLeg,
		 RightFoot,
		 LeftArm,
		 LeftForeArm,
		 RightArm,
		 RightForeArm,
		 Spine,
		 Head,
		 
		 None
	 }

	// Update is called once per frame
	void Update () {
		if( !m_initializedRagdoll && GetComponent<Animator>() != null) {
			m_ragdoll = MainGameManager.instance.getRagdollTemplate();
			m_initializedRagdoll = true;
			Component[] transforms = GetComponentsInChildren<Transform> ();
			
			string[] boneNames = {
				"Hips",
				"LeftUpLeg",
				"LeftLeg",
				"LeftFoot",
				"RightUpLeg",
				"RightLeg",
				"RightFoot",
				"LeftArm",
				"LeftForeArm",
				"RightArm",
				"RightForeArm",
				"Spine",
				"Head" };
				
			GameObject[] boneObjects = { null, null, null, null, null, null, null, null, null, null, null, null, null };
			Bone[] connections = { Bone.None, Bone.Hips, Bone.LeftUpLeg, Bone.LeftLeg, Bone.Hips, Bone.RightUpLeg, Bone.RightLeg, Bone.Spine, Bone.LeftArm, Bone.Spine, Bone.RightArm, Bone.Hips, Bone.Spine };
			
			Component[] sphereCollidersInRagdollTemplate = m_ragdoll.GetComponentsInChildren<SphereCollider>();
			Component[] boxCollidersInRagdollTemplate = m_ragdoll.GetComponentsInChildren<BoxCollider>();
			Component[] capsuleCollidersInRagdollTemplate = m_ragdoll.GetComponentsInChildren<CapsuleCollider>();
			Component[] rigidBodiesInRagdollTemplate = m_ragdoll.GetComponentsInChildren<Rigidbody>();
			Component[] characterJointsInRagdollTemplate = m_ragdoll.GetComponentsInChildren<CharacterJoint>();
			foreach (Transform t in transforms) {
				// collect game objects that we need reference to for the joint components
				for ( int i = 0; i < boneNames.Length; ++i ) {
					if( t.gameObject.name == boneNames[ i ] ) {
						boneObjects[ i ] = t.gameObject;							
						break;
					}
				}

				// handbone for humans
				if (t.gameObject.name == "RightHand") {
					HumanBehavior hb = GetComponent<HumanBehavior> ();
					if (hb != null) {
						hb.handBone = t;
					}
				}

				// copy different collider components
				foreach (SphereCollider c in sphereCollidersInRagdollTemplate ) {
					if( t.gameObject.name == c.gameObject.name ) {
						GetCopyOf<SphereCollider>(t.gameObject.AddComponent<SphereCollider>(),c);
						break;
					}
				}
				foreach (BoxCollider c in boxCollidersInRagdollTemplate ) {
					if( t.gameObject.name == c.gameObject.name ) {
						GetCopyOf<BoxCollider>(t.gameObject.AddComponent<BoxCollider>(),c);
						break;
					}
				}
				foreach (CapsuleCollider c in capsuleCollidersInRagdollTemplate ) {
					if( t.gameObject.name == c.gameObject.name ) {
						GetCopyOf<CapsuleCollider>(t.gameObject.AddComponent<CapsuleCollider>(),c);
						break;
					}
				}
				// copy rigid bodies
				foreach (Rigidbody c in rigidBodiesInRagdollTemplate ) {
					if( t.gameObject.name == c.gameObject.name ) {
						GetCopyOf<Rigidbody>(t.gameObject.AddComponent<Rigidbody>(),c);
						break;
					}
				}
				// copy joints
				foreach (CharacterJoint c in characterJointsInRagdollTemplate ) {
					if( t.gameObject.name == c.gameObject.name ) {
						CharacterJoint joint = GetCopyOf<CharacterJoint>(t.gameObject.AddComponent<CharacterJoint>(),c);
						joint.connectedBody = null;
						for ( int i = 0; i < boneNames.Length; ++i ) {
							if( t.gameObject.name == boneNames[ i ] ) {
								joint.connectedBody = boneObjects[ (int)connections[ i ] ].GetComponent<Rigidbody>();
								break;
							}
						}
						break;
					}
				}
			}
			
			gameObject.AddComponent<RagdollHelper>();
		}
	}
}
