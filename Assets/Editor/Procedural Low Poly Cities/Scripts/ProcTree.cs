using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;

namespace ProceduralCity
{ 
    [ExecuteInEditMode]
    public class ProcTree : MonoBehaviour
    {
        GameObject branch;
        
        public int seed;

        [Range(1, 4)]
        public int palette;

        [HideInInspector]
        public int color;
        [Range(0.5f, 3f)]
        public float radius; //max trunk radius
        [Range(1, 4)]
        public int treeDepth;
        [Range(4, 8)]
        public int numOfSegments;
        [Range(5, 8)]
        public int numOfSides;
        [HideInInspector]
        public float minRadius; // Minimum radius for the tree's smallest branches
        [Range(7f, 30f)]
        public float height;
        [Range(0.3f, 3f)]
        public float crownRadius;

        [HideInInspector]
        public bool coneTree;
        [HideInInspector]
        public int branchPerSegment;

        private float crownProbability;

        private float checksum; // Serialized & Non-Serialized checksums for tree rebuilds only on undo operations, or when parameters change (mesh kept on scene otherwise)
    
        private int originalSeed;

        [HideInInspector]
        public Material[] foliage;

        private Material[] trunkMat;
        [HideInInspector]
        public Material chosenTrunkMat;

        [HideInInspector]
        public List<int> branches;
        private float[] ringShape;

        [HideInInspector]
        public int depth = -1;

        [HideInInspector]
        public Vector3 foliageColliderStart;

        // Use this for initialization
        public void OnEnable()
        {
            //RandomizeSettings();
            originalSeed = seed;
        }
        

        public void RandomizeSettings()
        {
            seed = Random.Range(0, 64000);

            palette = Random.Range(1, 5);
            trunkMat = Resources.LoadAll("TrunkPalette" + palette, typeof(Material)).Cast<Material>().ToArray();
            color = Random.Range(0, trunkMat.Length);

            radius = Random.Range(0.5f, 3f);
            treeDepth = Random.Range(2, 4);
            numOfSegments = Random.Range(4, 8);
            numOfSides = Random.Range(5, 8);
            minRadius = Random.Range(0.1f, 0.4f); // Minimum radius for the tree's smallest branches
            branchPerSegment = Random.Range(0, 3); 
            crownRadius = Mathf.Min(Random.Range(1f, 3f), radius);
            height = Mathf.Max(Random.Range(7f, 30f), radius * 10);
        
            crownProbability = 0.9f;
        
            if (chosenTrunkMat == null)
                chosenTrunkMat = trunkMat[color];
            else
            {
                trunkMat = Resources.LoadAll("TrunkPalette" + palette, typeof(Material)).Cast<Material>().ToArray();
                chosenTrunkMat = trunkMat[Mathf.Min(color, trunkMat.Length - 1)];
            }

            foliage = Resources.LoadAll("FoliagePalette" + palette, typeof(Material)).Cast<Material>().ToArray();

            SetTreeRingShape();
        }

        public void SetTreeRingShape()
        {
            ringShape = new float[numOfSides + 1];
            for (var n = 0; n < numOfSides; n++)
                ringShape[n] = Random.Range(0.75f, 1.2f);
            ringShape[numOfSides] = ringShape[0];
        }
    
        public void Generate(bool cone)
        {
            originalSeed = Random.seed;

            coneTree = cone;
            // Tree parameter checksum
            var newChecksum = (seed & 0xFFFF) + color + radius + treeDepth + numOfSegments + numOfSides +
                                minRadius + height  + branchPerSegment + crownRadius + palette + color; //radiusStep + twisting;

            // Return (do nothing) unless tree parameters change
            if (newChecksum == checksum) return;

            checksum = newChecksum;

            Random.seed = seed;

            GenerateTree(); // Update tree mesh
            
            Random.seed = originalSeed;
        }

        public void GenerateTree() 
        {
            foreach (Transform child in transform)
                DestroyImmediate(child.gameObject);

            branch = new GameObject();
            branch.name = "branch";
            branch.AddComponent<Branch>();
            branch.transform.parent = this.transform;
            branch.AddComponent<MeshFilter>();
            branch.AddComponent<MeshRenderer>();
            
            if (chosenTrunkMat == null)
                chosenTrunkMat = trunkMat[color];
            else
            {
                trunkMat = Resources.LoadAll("TrunkPalette" + palette, typeof(Material)).Cast<Material>().ToArray();
                chosenTrunkMat = trunkMat[Mathf.Min(color, trunkMat.Length - 1)];
            }

            foliage = Resources.LoadAll("FoliagePalette" + palette, typeof(Material)).Cast<Material>().ToArray();

            AssignValues();
            branch.GetComponent<MeshRenderer>().material = chosenTrunkMat;
            branch.GetComponent<Branch>().Generate(Vector3.zero, branch.GetComponent<Branch>().radius);
            branch.transform.localPosition = Vector3.zero;
            branch.transform.localScale = Vector3.one;

            //Add trunk  and foliage collider
            Vector3 branchPos = branch.transform.position;
            Vector3 branchLocPos = branch.transform.localPosition;
            if (!coneTree)
            {
                branch.AddComponent<BoxCollider>();

                Vector3 trunkColliderSize = new Vector3(radius * 2, (branch.GetComponent<MeshFilter>().sharedMesh.bounds.max.y - branchPos.y), radius * 2);
                branch.GetComponent<BoxCollider>().size = trunkColliderSize;
                branch.GetComponent<BoxCollider>().center = branchLocPos + new Vector3(0, trunkColliderSize.y / 2, 0);
            }

            Bounds wholeMeshBounds = GetASingleMesh(branch);
            branch.AddComponent<CapsuleCollider>();
            branch.GetComponent<CapsuleCollider>().radius = Mathf.Max(branchPos.x - wholeMeshBounds.min.x, wholeMeshBounds.max.x - branchPos.x, 
                                                                        branchPos.z - wholeMeshBounds.min.z, wholeMeshBounds.max.z - branchPos.z);
            branch.GetComponent<CapsuleCollider>().height = coneTree? wholeMeshBounds.max.y : wholeMeshBounds.max.y - foliageColliderStart.y;
            branch.GetComponent<CapsuleCollider>().center = branchLocPos + (coneTree? new Vector3(0, wholeMeshBounds.max.y/2, 0) : new Vector3(0, wholeMeshBounds.max.y -(wholeMeshBounds.max.y - foliageColliderStart.y)/2, 0));
            
            originalSeed = seed;
        }

        public void AssignValues()
        {
            Branch branchInfo = branch.GetComponent<Branch>();
            branchInfo.chosenMat = chosenTrunkMat;
            branchInfo.radius = radius;
            branchInfo.treeDepth = treeDepth;
            branchInfo.numOfSegments = numOfSegments;
            branchInfo.numOfSides = numOfSides;
            branchInfo.minRadius = minRadius; // Minimum radius for the tree's smallest branches
            branchInfo.branchPerSegment = branchPerSegment;
            branchInfo.crownRadius = crownRadius;
            branchInfo.height = height;
            branchInfo.crownProbability = crownProbability;
            branchInfo.foliage = foliage;
            branchInfo.coneTree = coneTree;
            branchInfo.depth = depth + 1;
            branchInfo.SetTreeRingShape();
            branchInfo.firstTimeBranching = true;
        }

        public Bounds GetASingleMesh(GameObject branch)
        {
            MeshFilter[] meshFilters = branch.GetComponentsInChildren<MeshFilter>();
            CombineInstance[] combine = new CombineInstance[meshFilters.Length];
            int i = 0;
            while (i < meshFilters.Length)
            {
                combine[i].mesh = meshFilters[i].sharedMesh;
                combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
                //meshFilters[i].gameObject.SetActive(false);
                i++;
            }
            Mesh mesh = new Mesh();
            mesh.CombineMeshes(combine);
            return mesh.bounds;
        }

    }
}
