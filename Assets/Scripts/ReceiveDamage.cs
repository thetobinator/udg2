using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ReceiveDamage : MonoBehaviour {
	public GameObject m_ragdoll;
	public AudioClip Hit_Sound;
	//public Texture2D Splash_Screem;
	//public Texture2D Button_Art;
	public float m_Healt = 100f;
	protected float m_alpha = 0.0f;
	public Color Splash_Color;
	protected bool show_Deat = false;
	public bool Show_Splash = true;
	private bool Alredy_Death = false;
	public int Respawn_Time;
	//Shake
	private static Vector3 originPosition;
	private static Quaternion originRotation;
	
	//private static float shakeDecay = 0.005f;
	private static float shakeIntensity;
	private static Vector3 childscale;



	/// <summary>
	/// Where recive information of damage and others
	/// </summary>
	/// <param name="DirectionAttack"> direction from atakker</param>
	/// <param name="from"> name of atakker</param>
	public void Damage_Recive(float damage ,string from){
		Debug.Log ("hit!");
		m_Healt -= damage;
		m_alpha = 2.0f;
		if (m_Healt > 0.0f)
		{
			//StartCoroutine(Shake(this.transform));
			AudioSource.PlayClipAtPoint(Hit_Sound, this.transform.position, 1.0f);
			//Destroy (this.gameObject);
			
		}
		else if( m_Healt <= 0.0f)
		{
			m_Healt = 0.0f;
			if (!Alredy_Death)
			{
				Death(from);
				Alredy_Death = true;
			}
		}
	}

	void Death(string Killer)
	{
		Debug.Log ("Dead" + this.name);
		//GameObject DeatBody = Instantiate (m_ragdoll, Camera.main.transform.position, Camera.main.transform.rotation) as GameObject;
		//bl_KillCam killcam = DeatBody.GetComponent<bl_KillCam> ();
		//killcam.Send_Target (Killer, Respawn_Time);
		//Destroy (gameObject);
		Vector3 childscale = gameObject.transform.localScale;
		
		childscale.x = childscale.x * 0.05F;
		childscale.y = childscale.y * 0.05F;
		childscale.z = childscale.z * 0.05F;
		gameObject.transform.localScale = new Vector3(childscale.x, childscale.y, childscale.z);
	}
	
	//end ReceiveDamage.cs

}
