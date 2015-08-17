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
	
	void addRenderComponents( GameObject root ) {
		foreach (Transform child in root.transform)
		{
			var renderer = child.gameObject.GetComponent<Renderer> ();
			if( renderer != null )
			{
				m_renderComponents.Add(renderer);
			}
			else
			{
				// recursion
				addRenderComponents( child.gameObject );
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
		foreach (Renderer renderer in m_renderComponents) {
			// Set specular shader
			renderer.material.SetColor ("_Color", tintColor);
		}
	}
}
