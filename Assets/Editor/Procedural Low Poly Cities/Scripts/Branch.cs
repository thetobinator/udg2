using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ProceduralCity
{
    public class Branch : MonoBehaviour
    {
        [HideInInspector]
        public bool coneTree;
        [HideInInspector]
        public Material chosenMat;
        [HideInInspector]
        public float radius;
        [HideInInspector]
        public int treeDepth;
        [HideInInspector]
        public int numOfSegments;
        [HideInInspector]
        public int numOfSides;
        [HideInInspector]
        public float minRadius;
        [HideInInspector]
        public int branchPerSegment;
        [HideInInspector]
        public float crownRadius;
        [HideInInspector]
        public float height;
        [HideInInspector]
        public float crownProbability;
        [HideInInspector]
        public float alpha;
        [HideInInspector]
        public int depth;
        [HideInInspector]
        public Material[] foliage;
        [HideInInspector]
        public List<int> branches;
        public Vector3 branchPos;
        [HideInInspector]
        public bool firstTimeBranching;

        private List<Vector3> vertexList; // Vertex list
        private List<Vector2> uvList; // UV list
        private List<int> triangleList; // Triangle list

        private float[] ringShape;

        GameObject branch;
        GameObject crown;


        public void Generate(Vector3 position, float curRadius)
        {
            foreach (Transform child in transform)
            {
                DestroyImmediate(child.gameObject);
            }
            branchPos = position;
            GenerateBranch(); // Update tree mesh
            CreateBranch(position, 0, 0f, curRadius);
        }

        public void GenerateBranch()
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

            if (branches == null)
                branches = new List<int>();
            else
                branches.Clear();

            for (int i = 0; i < numOfSegments; i++)
            {
                if (i == 0)
                    branches.Add(0);
                else if (i < 3)
                    branches.Add(Random.Range(0f, 1f) < 0.1f ? Random.Range(1, numOfSides) : 0);
                else
                    branches.Add(Random.Range(0f, 1f) < 0.6f ? Random.Range(1, numOfSides) : 0);
            }
        }

        public void CreateBranch(Vector3 position, int segment, float texCoordV, float curRadius)
        {
            var segLen = height / numOfSegments;

            if (segment == numOfSegments || depth > treeDepth || curRadius < minRadius)
            {
                if (coneTree)
                {
                    GameObject crown = new GameObject();
                    crown.name = "crown";
                    crown.transform.parent = this.transform;
                    crown.AddComponent<MeshFilter>();
                    crown.AddComponent<MeshRenderer>();
                    int color = Random.Range(0, foliage.Length);
                    Material foliageMat = foliage[Mathf.Min(color, foliage.Length - 1)];
                    crown.GetComponent<MeshRenderer>().material = foliageMat;
                    crown.AddComponent<Crown>();
                    crown.GetComponent<Crown>().coneTree = true;
                    crown.GetComponent<Crown>().Generate();
                    Vector3 crownPos = new Vector3(position.x, position.y - segLen, position.z);
                    crown.GetComponent<Crown>().CreateConicCrown(crownPos, 0f, curRadius * 2.5f, segLen, height);
                }
                else if (Random.Range(0f, 1f) < crownProbability && segment > 3) //createCrown
                {
                    GameObject crown = new GameObject();
                    crown.name = "crown";
                    crown.transform.parent = this.transform;
                    crown.AddComponent<MeshFilter>();
                    crown.AddComponent<MeshRenderer>();
                    int color = Random.Range(0, foliage.Length);
                    Material foliageMat = foliage[Mathf.Min(color, foliage.Length - 1)];
                    crown.GetComponent<MeshRenderer>().material = foliageMat;
                    crown.AddComponent<Crown>();
                    crown.GetComponent<Crown>().Generate();
                    Vector3 crownPos = new Vector3(position.x - curRadius, position.y - segLen - curRadius, position.z - curRadius);
                    crown.GetComponent<Crown>().CreateCrown(crownPos, 0f, curRadius * 2, crownRadius / curRadius, 1f / crownRadius, 0);
                    //used to position collider
                    if (firstTimeBranching)
                    {
                        this.GetComponentInParent<ProcTree>().foliageColliderStart = new Vector3(position.x - curRadius, position.y - segLen - curRadius, position.z - curRadius);
                        firstTimeBranching = false;
                    }

                }
                else
                {
                    Vector3 capPos = new Vector3(position.x, position.y - segLen, position.z);
                    AddCap(capPos);
                }
                if (vertexList.Count > 1)
                    SetAndClearTreeMesh();
                return;
            }

            if (segment == 0)
            {
                vertexList.Add(position);
                uvList.Add(new Vector2(position.x, position.y));
            }

            var offset = Vector3.zero;
            var texCoord = new Vector2(0f, texCoordV);
            var textureStepU = 1f / numOfSides;
            var angInc = 2f * Mathf.PI * textureStepU;
            var ang = 0f;

            if (!coneTree)
            {
                position.x = Random.Range(position.x - 0.3f, position.x + 0.3f);
                position.z = Random.Range(position.x - 0.3f, position.x + 0.3f);
            }

            for (var n = 0; n <= numOfSides; n++, ang += angInc)
            {
                var r = ringShape[n] * curRadius;
                offset.x = r * Mathf.Cos(ang); // Get X, Z vertex offsets
                offset.z = r * Mathf.Sin(ang);
                vertexList.Add(position + new Quaternion() * offset); // Add Vertex position
                uvList.Add(texCoord); // Add UV coord
                texCoord.x += textureStepU;
            }

            if (segment == 0)
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

            if (depth + 1 < treeDepth && curRadius > minRadius)
            {
                if (!coneTree) //if it is NOT a coneTree, add branches
                {
                    for (int i = 0; i < branches[segment]; i++)
                    {
                        branch = new GameObject();
                        branch.name = "branch";
                        branch.AddComponent<Branch>();
                        branch.transform.parent = this.transform;
                        branch.AddComponent<MeshFilter>();
                        branch.AddComponent<MeshRenderer>();
                        AssignValues();
                        branch.GetComponent<MeshRenderer>().material = chosenMat;
                        branch.GetComponent<Branch>().Generate(new Vector3(position.x, position.y - segLen, position.z), curRadius);
                        float beta = (360f / numOfSides) * (Random.Range(0f, 1f) < 0.5f ? (i + 1) : (numOfSides - i));
                        branch.transform.RotateAround(new Vector3(position.x, position.y - segLen, position.z), new Vector3(1, 0, 0), branch.GetComponent<Branch>().alpha);
                        branch.transform.RotateAround(new Vector3(position.x, position.y - segLen, position.z), new Vector3(0, 1, 0), beta);
                        if (firstTimeBranching)
                        {
                            this.GetComponentInParent<ProcTree>().foliageColliderStart = new Vector3(position.x, position.y - segLen, position.z);
                            firstTimeBranching = false;
                        }
                    }
                }
            }

            Vector3 point = position + new Vector3(0f, segLen, 0f);
            CreateBranch(point, segment + 1, texCoordV, curRadius * 0.8f >= minRadius ? curRadius * 0.8f : minRadius);
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

        public void SetTreeRingShape()
        {
            ringShape = new float[numOfSides + 1];
            for (var n = 0; n < numOfSides; n++)
                ringShape[n] = Random.Range(0.75f, 1.2f);
            ringShape[numOfSides] = ringShape[0];
        }

        public void AssignValues()
        {
            Branch branchInfo = branch.GetComponent<Branch>();

            branchInfo.coneTree = coneTree;
            branchInfo.chosenMat = chosenMat;
            branchInfo.radius = radius * 0.5f;
            branchInfo.treeDepth = treeDepth;
            branchInfo.numOfSegments = Mathf.Min(Random.Range(4, 8), numOfSegments);
            branchInfo.numOfSides = numOfSides;
            branchInfo.minRadius = minRadius;
            branchInfo.branchPerSegment = Mathf.Min(Random.Range(0, 3), branchPerSegment);
            branchInfo.crownRadius = crownRadius;
            branchInfo.height = height * 0.45f;
            branchInfo.crownProbability = crownProbability;
            branchInfo.depth = depth + 1;
            branchInfo.foliage = foliage;
            branchInfo.ringShape = ringShape;
            branchInfo.alpha = Random.Range(30f, 60f);
        }

        public void SetAndClearTreeMesh()
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

    }
}