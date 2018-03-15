using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using System.IO;
public class Title_Menu_Behavior : MonoBehaviour {
	public string Level1 = "sandbox_tobi";

	public GameObject defualtSelectedMain;

	public void PlayGame() {
		
		SceneManager.LoadScene(Level1);

//		uiEventSystem.firstSelectedGameObject = defualtSelectedMain;
	}
}
