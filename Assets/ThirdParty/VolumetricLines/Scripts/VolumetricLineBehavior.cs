using UnityEngine;
using System.Collections;

namespace VolumetricLines
{
	/// <summary>
	/// Render a single volumetric line
	/// 
	/// Based on the Volumetric lines algorithm by SÃ©bastien Hillaire
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
	[RequireComponent(typeof(MeshFilter))]
	[RequireComponent(typeof(Renderer))]
	public class VolumetricLineBehavior : MonoBehaviour 
	{
		/// <summary>
		/// Dynamic means that start and end positions can be modified during runtime.
		/// This comes with a performance penalty.
		/// Use
		/// </summary>
		public bool m_dynamic;
		
		/// <summary>
		/// The start position relative to the GameObject's origin
		/// </summary>
		public Vector3 m_startPos;
		
		/// <summary>
		/// The end position relative to the GameObject's origin
		/// </summary>
		public Vector3 m_endPos = new Vector3(0f, 0f, 100f);

		/// <summary>
		/// Set to true to change the material's color to the color specified with "Line Color"
		/// </summary>
		public bool m_setMaterialColor;

		/// <summary>
		/// The material is set to this color in Start() if "Set Material Color" is set to true
		/// </summary>
		public Color m_lineColor;
		

		private static readonly Vector2[] m_vline_texCoords = {
			new Vector2(1.0f, 1.0f),
			new Vector2(1.0f, 0.0f),
			new Vector2(0.5f, 1.0f),
			new Vector2(0.5f, 0.0f),
			new Vector2(0.5f, 0.0f),
			new Vector2(0.5f, 1.0f),
			new Vector2(0.0f, 0.0f),
			new Vector2(0.0f, 1.0f),
		};


		private static readonly Vector2[] m_vline_vertexOffsets = {
			 new Vector2(1.0f,	 1.0f),
			 new Vector2(1.0f,	-1.0f),
			 new Vector2(0.0f,	 1.0f),
			 new Vector2(0.0f,	-1.0f),
			 new Vector2(0.0f,	 1.0f),
			 new Vector2(0.0f,	-1.0f),
			 new Vector2(1.0f,	 1.0f),
			 new Vector2(1.0f,	-1.0f)
		};

		private static readonly int[] m_vline_indices =
		{
			2, 1, 0,
			3, 1, 2,
			4, 3, 2,
			5, 4, 2,
			4, 5, 6,
			6, 5, 7
		};
		
		/// <summary>
		/// Sets the start and end points - updates the data of the Mesh.
		/// </summary>
		public void SetStartAndEndPoints(Vector3 startPoint, Vector3 endPoint)
		{
			Vector3[] vertexPositions = {
				startPoint,
				startPoint,
				startPoint,
				startPoint,
				endPoint,
				endPoint,
				endPoint,
				endPoint,
			};
			
			Vector3[] other = {
				endPoint,
				endPoint,
				endPoint,
				endPoint,
				startPoint,
				startPoint,
				startPoint,
				startPoint,
			};
			
			var mesh = GetComponent<MeshFilter>().sharedMesh;
			mesh.vertices = vertexPositions;
			mesh.normals = other;
		}
		
		// Vertex data is updated only in Start() unless m_dynamic is set to true
		void Start () 
		{
			Vector3[] vertexPositions = {
				m_startPos,
				m_startPos,
				m_startPos,
				m_startPos,
				m_endPos,
				m_endPos,
				m_endPos,
				m_endPos,
			};
			
			Vector3[] other = {
				m_endPos,
				m_endPos,
				m_endPos,
				m_endPos,
				m_startPos,
				m_startPos,
				m_startPos,
				m_startPos,
			};
			
			// Need to set vertices before assigning new Mesh to the MeshFilter's mesh property
			Mesh mesh = new Mesh();
			mesh.vertices = vertexPositions;
			mesh.normals = other;
			mesh.uv = m_vline_texCoords;
			mesh.uv2 = m_vline_vertexOffsets;
			mesh.SetIndices(m_vline_indices, MeshTopology.Triangles, 0);
			GetComponent<MeshFilter>().mesh = mesh;
			// Need to duplicate the material, otherwise multiple volume lines would interfere
			GetComponent<Renderer>().material = GetComponent<Renderer>().material;
			if (m_setMaterialColor)
			{
				GetComponent<Renderer>().sharedMaterial.color = m_lineColor;
			}
		}
		
		// Vertex data is updated if m_dynamic is set to true, this implies a performance penalty
		// compared to uploading the vertex data to GPU only once in the Start() method.
		// Delete the Update() method for maximum performance
		void Update () 
		{
			if (m_dynamic)
			{
				SetStartAndEndPoints(m_startPos, m_endPos);
				if (m_setMaterialColor)
				{
					GetComponent<Renderer>().sharedMaterial.color = m_lineColor;
				}
			}
		}

		void OnDrawGizmos()
		{
			Gizmos.color = Color.green;
			Gizmos.DrawLine(gameObject.transform.TransformPoint(m_startPos), gameObject.transform.TransformPoint(m_endPos));
		}
	}
}