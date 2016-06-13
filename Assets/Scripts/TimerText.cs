using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TimerText : MonoBehaviour {

	public Text timerText;
	private float startTime;

	// Use this for initialization
	void Start () {
		startTime = Time.time;

	}
	
	// Update is called once per frame
	void Update () {
		float timePassed = Time.time - startTime;
		string minutes = ((int)timePassed / 60).ToString ();
		string seconds = (timePassed % 60).ToString ("f2");
		timerText.text = minutes + ":" + seconds;
	}
}
