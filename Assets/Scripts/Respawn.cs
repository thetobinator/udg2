using UnityEngine;
using System.Collections;
// Instantiates respawnPrefab at the location 
// of all game objects tagged "Respawn".

public class Respawn : MonoBehaviour {

		public GameObject respawnPrefab;
		public GameObject[] respawns;
		void Start() {
			if (respawns == null)
				respawns = GameObject.FindGameObjectsWithTag("Respawn");
			
			foreach (GameObject respawn in respawns) {
				Instantiate(respawnPrefab, respawn.transform.position, respawn.transform.rotation);
			}
		}
	}

