using UnityEngine;
using System.Collections;


	public class InitLoader : MonoBehaviour 
	{
        public GameObject mainGameInitObject;
		void Awake ()   
		{
            //if (mainGameInitObject.GetComponentinstance == null)
                Instantiate(mainGameInitObject);
		}
    
	}
