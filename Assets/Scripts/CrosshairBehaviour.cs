using UnityEngine;
using System.Collections;

public class CrosshairBehaviour : MonoBehaviour {

	static GameObject m_s_recentlyFocusedHuman = null;
	float m_cooldown = 0.0f;

	// Use this for initialization
	void Start () {
	
	}

	Color getCrosshairColor()
	{
		float highlightIntensity = 2.0f * m_cooldown;
		return new Color (highlightIntensity, 1.0f - highlightIntensity, 0.0f);
	}

	static public GameObject getRecentlyFocusedHuman()
	{
		return m_s_recentlyFocusedHuman;
	}

	void Update () {
		DebugTint d = GetComponent<DebugTint> ();
		if (m_cooldown > 0.0f)
		{
			m_cooldown -= Time.deltaTime;
			if (m_cooldown <= 0.0f)
			{
				m_s_recentlyFocusedHuman = null;
				m_cooldown = 0.0f;
			}
		}
		if (d != null) {
			Ray ray = Camera.main.GetComponent<Camera> ().ScreenPointToRay (Input.mousePosition);
			RaycastHit hit = new RaycastHit ();
			if (Physics.Raycast (ray, out hit)) {
				if (hit.collider != null && hit.collider.gameObject != null && hit.collider.gameObject.transform != null && hit.collider.gameObject.transform.parent != null) {
					GameObject obj = ZombieBehavior.getRootObject (hit.collider.gameObject.transform.parent.gameObject);
					if (obj.tag == "Human") {
						m_cooldown = 0.5f;
						m_s_recentlyFocusedHuman = obj;
					}
				}
			}
			d.tintColor = getCrosshairColor ();
		}
	}
}
