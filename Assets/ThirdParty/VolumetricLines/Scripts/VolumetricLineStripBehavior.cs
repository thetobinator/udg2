using UnityEngine;
using System.Collections;

namespace VolumetricLines
{
	/// <summary>
	/// Render a line strip of volumetric lines
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
	public class VolumetricLineStripBehavior : MonoBehaviour 
	{
		/// <summary>
		/// Set to true to change the material's color to the color specified with "Line Color"
		/// </summary>
		public bool m_setMaterialColor;
		
		/// <summary>
		/// The material is set to this color in Start() if "Set Material Color" is set to true
		/// </summary>
		public Color m_lineColor;

		/// <summary>
		/// The vertices of the line
		/// </summary>
		public Vector3[] m_lineVertices;

		void Start () 
		{
			if (m_lineVertices.Length < 3)
			{
				Debug.LogError("Add at least 3 vertices to the VolumetricLineStrip");
				return;
			}

			// fill vertex positions, and indices
			// 2 for each position, + 2 for the start, + 2 for the end
			Vector3[] vertexPositions = new Vector3[m_lineVertices.Length * 2 + 4];
			// there are #vertices - 2 faces, and 3 indices each
			int[] indices = new int[(m_lineVertices.Length * 2 + 2) * 3];
			int v = 0;
			int x = 0;
			vertexPositions[v++] = m_lineVertices[0];
			vertexPositions[v++] = m_lineVertices[0];
			for (int i=0; i < m_lineVertices.Length; ++i)
			{
				vertexPositions[v++] = m_lineVertices[i];
				vertexPositions[v++] = m_lineVertices[i];
				indices[x++] = v - 2;
				indices[x++] = v - 3;
				indices[x++] = v - 4;
				indices[x++] = v - 1;
				indices[x++] = v - 2;
				indices[x++] = v - 3;
			}
			vertexPositions[v++] = m_lineVertices[m_lineVertices.Length - 1];
			vertexPositions[v++] = m_lineVertices[m_lineVertices.Length - 1];
			indices[x++] = v - 2;
			indices[x++] = v - 3;
			indices[x++] = v - 4;
			indices[x++] = v - 1;
			indices[x++] = v - 2;
			indices[x++] = v - 3;

			// fill texture coordinates and vertex offsets
			Vector2[] texCoords		  = new Vector2[vertexPositions.Length];
			Vector2[] vertexOffsets	  = new Vector2[vertexPositions.Length];
			int t = 0;
			int o = 0;
			texCoords[t++] = new Vector2(1.0f, 0.0f);
			texCoords[t++] = new Vector2(1.0f, 1.0f);
			texCoords[t++] = new Vector2(0.5f, 0.0f);
			texCoords[t++] = new Vector2(0.5f, 1.0f);
			vertexOffsets[o++] = new Vector2(1.0f,	-1.0f);
			vertexOffsets[o++] = new Vector2(1.0f,	 1.0f);
			vertexOffsets[o++] = new Vector2(0.0f,	-1.0f);
			vertexOffsets[o++] = new Vector2(0.0f,	 1.0f);
			for (int i=1; i < m_lineVertices.Length - 1; ++i)
			{
				if ((i & 0x1) == 0x1)
				{
					texCoords[t++] = new Vector2(0.5f, 0.0f);
					texCoords[t++] = new Vector2(0.5f, 1.0f);
				}
				else 
				{
					texCoords[t++] = new Vector2(0.5f, 0.0f);
					texCoords[t++] = new Vector2(0.5f, 1.0f);
				}
				vertexOffsets[o++] = new Vector2(0.0f,	 1.0f);
				vertexOffsets[o++] = new Vector2(0.0f,	-1.0f);
			}
			texCoords[t++] = new Vector2(0.5f, 0.0f);
			texCoords[t++] = new Vector2(0.5f, 1.0f);
			texCoords[t++] = new Vector2(0.0f, 0.0f);
			texCoords[t++] = new Vector2(0.0f, 1.0f);
			vertexOffsets[o++] = new Vector2(0.0f,	 1.0f);
			vertexOffsets[o++] = new Vector2(0.0f,	-1.0f);
			vertexOffsets[o++] = new Vector2(1.0f,	 1.0f);
			vertexOffsets[o++] = new Vector2(1.0f,	-1.0f);


			// fill previous and next positions
			Vector3[] prevPositions = new Vector3[vertexPositions.Length];
			Vector4[] nextPositions = new Vector4[vertexPositions.Length];
			int p = 0;
			int n = 0;
			prevPositions[p++] = m_lineVertices[1];
			prevPositions[p++] = m_lineVertices[1];
			prevPositions[p++] = m_lineVertices[1];
			prevPositions[p++] = m_lineVertices[1];
			nextPositions[n++] = m_lineVertices[1];
			nextPositions[n++] = m_lineVertices[1];
			nextPositions[n++] = m_lineVertices[1];
			nextPositions[n++] = m_lineVertices[1];
			for (int i=1; i < m_lineVertices.Length - 1; ++i)
			{
				prevPositions[p++] = m_lineVertices[i-1];
				prevPositions[p++] = m_lineVertices[i-1];
				nextPositions[n++] = m_lineVertices[i+1];
				nextPositions[n++] = m_lineVertices[i+1];
			}
			prevPositions[p++] = m_lineVertices[m_lineVertices.Length - 2];
			prevPositions[p++] = m_lineVertices[m_lineVertices.Length - 2];
			prevPositions[p++] = m_lineVertices[m_lineVertices.Length - 2];
			prevPositions[p++] = m_lineVertices[m_lineVertices.Length - 2];
			nextPositions[n++] = m_lineVertices[m_lineVertices.Length - 2];
			nextPositions[n++] = m_lineVertices[m_lineVertices.Length - 2];
			nextPositions[n++] = m_lineVertices[m_lineVertices.Length - 2];
			nextPositions[n++] = m_lineVertices[m_lineVertices.Length - 2];

			// Need to set vertices before assigning new Mesh to the MeshFilter's mesh property
			Mesh mesh = new Mesh();
			mesh.vertices = vertexPositions;
			mesh.normals = prevPositions;
			mesh.tangents = nextPositions;
			mesh.uv = texCoords;
			mesh.uv2 = vertexOffsets;
			mesh.SetIndices(indices, MeshTopology.Triangles, 0);
			GetComponent<MeshFilter>().mesh = mesh;
			// Need to duplicate the material, otherwise multiple volume lines would interfere
			GetComponent<Renderer>().material = GetComponent<Renderer>().material;
			if (m_setMaterialColor)
			{
				GetComponent<Renderer>().sharedMaterial.color = m_lineColor;
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