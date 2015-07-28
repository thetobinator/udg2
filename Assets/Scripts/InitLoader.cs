using UnityEngine;
using System.Collections;


	public class InitLoader : MonoBehaviour 
	{
        public GameObject mainGameInitObject;
		void Awake ()   
		{
            if (mainGameInitObject == null) {
                mainGameInitObject  = (GameObject)Resources.Load("Prefabs/Management/TestingInitObject", typeof(GameObject)) as GameObject; 
            }
            //if (mainGameInitObject.GetComponentinstance == null)
                Instantiate(mainGameInitObject);
		}
    
	}
