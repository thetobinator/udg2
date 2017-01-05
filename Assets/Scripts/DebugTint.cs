using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DebugTint : MonoBehaviour {
	public Color tintColor = Color.white;
	List<Renderer> m_renderComponents = new List<Renderer>();
	
	// Use this for initialization
	void Start () {
		addRenderComponents (this.gameObject);
	}

	bool addRenderComponent( GameObject obj ){
		var renderer = obj.GetComponent<Renderer> ();
		if (renderer != null) {
			m_renderComponents.Add (renderer);
			return true;
		}
		return false;
	}
	
	void addRenderComponents( GameObject root ) {
		if (addRenderComponent (gameObject)) {
			return;
		}

		foreach (Transform child in root.transform)
		{
			if( !addRenderComponent( child.gameObject ) )
			{
				// recursion
				addRenderComponents( child.gameObject );
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
		foreach (Renderer renderer in m_renderComponents) {
			renderer.material.SetColor ("_Color", tintColor);
		}
	}
}
