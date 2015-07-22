using UnityEngine;
using System.Collections;

namespace VolumetricLines
{
	/// <summary>
	/// Render a line strip consisting of multiple VolumetricLineBehavior pieces stitched together
	/// 
	/// Based on the Volumetric lines algorithm by Sébastien Hillaire
	/// http://sebastien.hillaire.free.fr/index.php?option=com_content&view=article&id=57&Itemid=74
	/// 
	/// Thread in the Unity3D Forum:
	/// http://forum.unity3d.com/threads/181618-Volumetric-lines
	/// 
	/// Unity3D port by Johannes Unterguggenberger
	/// johannes.unterguggenberger@gmail.com
	/// 
	/// Thanks to Michael Probst for support during development.
	/// 
	/// Thanks for bugfixes and improvements to Unity Forum User "Mistale"
	/// http://forum.unity3d.com/members/102350-Mistale
	/// </summary>
	public class VolumetricMultiLineBehavior : MonoBehaviour 
	{
		public bool m_dynamic;
		public GameObject m_volumetricLinePrefab;
		public Vector3[] m_lineVertices;
		
		private VolumetricLineBehavior[] m_volumetricLines;

		void Start () 
		{
			m_volumetricLines = new VolumetricLineBehavior[m_lineVertices.Length - 1];
			for (int i=0; i < m_lineVertices.Length - 1; ++i)
			{
				var go = GameObject.Instantiate(m_volumetricLinePrefab) as GameObject;
				go.transform.parent = gameObject.transform;
				go.transform.localPosition = Vector3.zero;
				go.transform.localRotation = Quaternion.identity;
				
				var volLine = go.GetComponent<VolumetricLineBehavior>();
				volLine.m_dynamic = false;
				volLine.m_startPos = m_lineVertices[i];
				volLine.m_endPos = m_lineVertices[i+1];
				
				m_volumetricLines[i] = volLine;
			}
		}
		
		void Update () 
		{
			if (m_dynamic)
			{
				for (int i=0; i < m_lineVertices.Length - 1; ++i)
				{
					m_volumetricLines[i].SetStartAndEndPoints(m_lineVertices[i], m_lineVertices[i+1]);
				}
			}
		}
		
		void OnDrawGizmos()
		{
			Gizmos.color = Color.green;
			for (int i=0; i < m_lineVertices.Length - 1; ++i)
			{
				Gizmos.DrawLine(gameObject.transform.TransformPoint(m_lineVertices[i]), gameObject.transform.TransformPoint(m_lineVertices[i+1]));
			}
		}
	}
}