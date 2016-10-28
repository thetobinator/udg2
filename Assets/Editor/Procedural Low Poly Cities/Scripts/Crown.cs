using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace ProceduralCity
{
    public class Crown : MonoBehaviour
    {

        [HideInInspector]
        public int numOfSides;
        [HideInInspector]
        public float crownRadius;
        [HideInInspector]
        public bool coneTree;

        private List<Vector3> vertexList;
        private List<Vector2> uvList;
        private List<int> triangleList;

        float[] crownShape;

        public void Generate()
        {
            numOfSides = coneTree ? Random.Range(5, 10) : Random.Range(4, 8);
            crownRadius = transform.parent.GetComponent<Branch>().crownRadius;

            SetCrownRingShape();
            GenerateCrown();
        }

        public void SetCrownRingShape()
        {
            crownShape = new float[numOfSides + 1];
            for (var n = 0; n < numOfSides; n++)
                crownShape[n] = Random.Range(0.75f, 1.2f);
            crownShape[numOfSides] = crownShape[0];
        }

        public void GenerateCrown()
        {
            if (vertexList == null) // Create lists for holding generated vertices
            {
                vertexList = new List<Vector3>();
                uvList = new List<Vector2>();
                triangleList = new List<int>();
            }
            else // Clear lists for holding generated vertices
            {
                vertexList.Clear();
                uvList.Clear();
                triangleList.Clear();
            }
        }

        public void SetAndClearFoliageMesh()
        {
            MeshFilter filter = GetComponent<MeshFilter>();
            var mesh = filter.sharedMesh;
            if (mesh == null)
                mesh = filter.sharedMesh = new Mesh();
            else
                mesh.Clear();

            // Assign vertex data
            mesh.vertices = vertexList.ToArray();
            mesh.uv = uvList.ToArray();
            mesh.triangles = triangleList.ToArray();

            // Update mesh
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
            //mesh.Optimize(); // Do not call this if we are going to change the mesh dynamically!
            vertexList.Clear();
            uvList.Clear();
            triangleList.Clear();
        }

        public void CreateConicCrown(Vector3 point, float texCoordV, float curRadius, float segLen, float height)
        {
            if (point.y < height / 9)
            {
                SetAndClearFoliageMesh();
                return;
            }

            vertexList.Add(point);
            uvList.Add(new Vector2(point.x, point.y));

            var offset = Vector3.zero;
            var texCoord = new Vector2(0f, texCoordV);
            var textureStepU = 1f / numOfSides;
            var angInc = 2f * Mathf.PI * textureStepU;
            var ang = 0f;

            for (var n = 0; n <= numOfSides; n++, ang += angInc)
            {
                var r = crownShape[n] * curRadius;
                offset.x = r * Mathf.Cos(ang); // Get X, Z vertex offsets
                offset.z = r * Mathf.Sin(ang);
                vertexList.Add(point + new Quaternion() * offset); // Add Vertex position
                uvList.Add(texCoord); // Add UV coord
                texCoord.x += textureStepU;
            }

            for (var n = 0; n < numOfSides; n++)
            {
                triangleList.Add(vertexList.Count - (numOfSides + 1) + n + 1);
                triangleList.Add(vertexList.Count - (numOfSides + 1) - 1);
                triangleList.Add(vertexList.Count - (numOfSides + 1) + n);
            }

            vertexList.Add(point + new Vector3(0f, curRadius * 1.2f, 0f));
            uvList.Add(new Vector2(point.x, point.y + curRadius * 1.2f));

            for (var n = 0; n < numOfSides; n++)
            {
                triangleList.Add(vertexList.Count - (numOfSides + 2) + n + 1);
                triangleList.Add(vertexList.Count - (numOfSides + 2) + n);
                triangleList.Add(vertexList.Count - 1);
            }

            point = point - new Vector3(0f, curRadius, 0f);
            CreateConicCrown(point, texCoordV, curRadius * 1.5f, segLen, height);
        }

        public void CreateCrown(Vector3 point, float texCoordV, float curRadius, float increaseF, float decreaseF, int level)
        {
            var segmentLength = crownRadius / 3;

            if (level > 6 || (level > 3 && curRadius <= 1f))
            {
                AddCap(point - new Vector3(0f, segmentLength, 0f));
                SetAndClearFoliageMesh();
                return;
            }
            else
            {
                if (level == 0)
                {
                    vertexList.Add(point);
                    uvList.Add(new Vector2(point.x, point.y));
                }

                var offset = Vector3.zero;
                var texCoord = new Vector2(0f, texCoordV);
                var textureStepU = 1f / numOfSides;
                var angInc = 2f * Mathf.PI * textureStepU;
                var ang = 0f;

                for (var n = 0; n <= numOfSides; n++, ang += angInc)
                {
                    var r = crownShape[n] * curRadius;
                    offset.x = r * Mathf.Cos(ang); // Get X, Z vertex offsets
                    offset.z = r * Mathf.Sin(ang);
                    vertexList.Add(point + new Quaternion() * offset); // Add Vertex position
                    uvList.Add(texCoord); // Add UV coord
                    texCoord.x += textureStepU;
                }

                if (level == 0)
                {
                    for (var n = 0; n < vertexList.Count - 1; n++) // Add cap
                    {
                        triangleList.Add(n + 2);
                        triangleList.Add(0);
                        triangleList.Add(n + 1);
                    }
                }

                else
                {
                    for (var n = 0; n < numOfSides; n++) // Add cap
                    {
                        triangleList.Add(vertexList.Count - 2 * numOfSides - 1 + n);
                        triangleList.Add(vertexList.Count - 2 * numOfSides - 2 + n);
                        triangleList.Add(vertexList.Count - numOfSides - 1 + n);
                        triangleList.Add(vertexList.Count - numOfSides - 1 + n);
                        triangleList.Add(vertexList.Count - numOfSides + n);
                        triangleList.Add(vertexList.Count - 2 * numOfSides - 1 + n);

                    }
                }
                point = point + new Vector3(0f, segmentLength, 0f);
                var radiusFact = level < 3 ? increaseF : decreaseF;

                SetCrownRingShape();
                CreateCrown(point, texCoordV, Mathf.Min(curRadius * radiusFact, crownRadius * 5 / 3), increaseF, decreaseF, level + 1);
            }
        }

        public void AddCap(Vector3 position)
        {
            vertexList.Add(position); // Add central vertex
            uvList.Add(new Vector2(position.x, position.y));
            for (var n = vertexList.Count - numOfSides - 2; n < vertexList.Count - 2; n++) // Add cap
            {
                triangleList.Add(n);
                triangleList.Add(vertexList.Count - 1);
                triangleList.Add(n + 1);
            }
        }

    }
}