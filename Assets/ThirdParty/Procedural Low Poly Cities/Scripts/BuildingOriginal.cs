using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ProceduralCity
{ 
    [ExecuteInEditMode]
    public class BuildingOriginal : MonoBehaviour {

        private GameObject buildingFloor;

        [Range(1,3)]
        public int palette;
        [Range(0, 5)]
        public int color;
        [Range(1, 8)]
        public int floors;
        [Range(1f, 8f)]
        public float width;
        [Range(1f, 7f)]
        public float depth;
        [Range(1f, 2f)]
        public float floorHeight;
        [Range(0.1f, 3f)]
        public float windowBoxWidth;
        [Range(0.3f, 1.5f)]
        public float windowHeight;
        [Range(0.3f, 1.5f)]
        public float windowWidth;
        [Range(0.1f, 3f)]
        public float sideWinBoxWidth;
        [Range(0.3f, 1.3f)]
        public float sideWindowWidth;
        [Range(0.3f, 1.5f)]
        public float sideWindowHeight;
        [Range(0.5f, 1f)]
        public float doorWidth;
        [Range(0.5f, 1.2f)]
        public float doorHeight;
        [Range(0.1f, 0.4f)]
        public float separatorOffset;
        [Range(0.1f, 0.4f)]
        public float separatorHeight;
        [Range(0.8f, 1.5f)]
        public float roofHeight;
        [Range(1f, 10f)]
        public float roofLowWidth;
        [Range(0f, 9f)]
        public float roofUpWidth;
        [Range(0.4f, 10f)]
        public float roofDepth;
   
        [HideInInspector]
		public float roofProbability;
        [HideInInspector]
		public float separatorProb;

        public bool hasRoof;
        public List<bool> separatorList;
    
        [HideInInspector]
        public Dictionary<string, Dictionary<string, List<bool>>> balconyDic;
        private int numOfWin;
        private int numOfSideWin;
        [HideInInspector]
        public int sumBalcony;

        [HideInInspector]
        public int roofType;

        [HideInInspector]
        public int seed;

        private Material[] wallsMat;
        [HideInInspector]
        public Material chosenMat;

        private float checksum;
        private float floorsChecksum;
    
        // Use this for initialization
        public void OnEnable()
        {
			//RandomizeSettings();
        }
        
        public void RandomizeSettings()
        {
			roofProbability = Random.Range (0f, 1f);
			separatorProb = Random.Range(0f, 1f);

			var children = transform.childCount;

            for (int index = 0; index < children; index++)
            {
                DestroyImmediate(this.transform.GetChild(0).gameObject);
            }

            seed = Random.Range(0, 65536);

            palette = Random.Range(1, 4);
            wallsMat = Resources.LoadAll("Palette" + palette, typeof(Material)).Cast<Material>().ToArray(); 
            color = Random.Range(0, wallsMat.Length-2);

            floors = Random.Range(1, 8);
            width = Random.Range(1f, 8f);
            depth = Random.Range(1f, 7f);
            floorHeight = Mathf.Min(Random.Range(1f, 2f), width);
            windowBoxWidth = Mathf.Max(Random.Range(0.1f, 3f), width/5);
            windowHeight = Random.Range(0.3f, 1.5f);
            windowWidth = Mathf.Max(Random.Range(0.3f, 1.5f), windowBoxWidth/3);
            sideWinBoxWidth = Mathf.Max(Random.Range(0.1f, 3f), depth/5);
            sideWindowWidth = Mathf.Max(Random.Range(0.3f, 1.3f), sideWinBoxWidth/3);
            sideWindowHeight = Random.Range(0.3f, 1.5f);
            doorWidth = Mathf.Max(Random.Range(0.5f, 1f), width/5);
            doorHeight = Mathf.Max(Random.Range(0.5f, 1.2f), floorHeight * 6 / 7);
            separatorProb = Random.Range(0f, 1f);
            separatorOffset = Mathf.Min(Random.Range(1f, 10f), Mathf.Min(width, depth) /12);
            separatorHeight = Mathf.Min(Random.Range(0.1f, 0.4f), floorHeight * 1/6);
            roofProbability = Random.Range(0f, 1f);
            roofHeight = Mathf.Min(Random.Range(0.8f, 1.5f), floorHeight * 7/6);
            roofLowWidth = Mathf.Min(Random.Range(1f, 10f), width * 7/6);
            roofUpWidth = Random.Range(0f, 9f);
            roofDepth = Mathf.Min(Random.Range(0.4f, 10f), depth * 7/6);

            if (separatorList == null)
                separatorList = new List<bool>();
            else
                separatorList.Clear();

            for (int i = 0; i < floors; i++)
                separatorList.Add((Random.value < separatorProb) ? true : false);

            hasRoof = Random.value < roofProbability ? true : false;
            if (Random.Range(0f, 1f) <= 0.5f)
                roofType = 1;
            else
                roofType = 2;

            if (chosenMat == null)
                chosenMat = wallsMat[color];
            else
            {
                wallsMat = Resources.LoadAll("Palette" + palette, typeof(Material)).Cast<Material>().ToArray();
                chosenMat = wallsMat[Mathf.Min(color, wallsMat.Length - 1)];
            }
        }

        // Update is called once per frame
        public void Generate()
        {
            float sumSep = 0f;
            for (int i = 0; i < separatorList.Count(); i++)
                sumSep += separatorList[i] ? 1 : 0;

            floorsChecksum = 0f;
            for (int i = 0; i < transform.childCount; i++)
                floorsChecksum += this.transform.GetChild(i).gameObject.GetComponent<Floor>().checksum;

            // Building parameter checksum
            var newChecksum = (seed & 0xFFFF) + palette + color + floors + width + depth + floorHeight + separatorOffset + separatorHeight +
                            windowBoxWidth + windowHeight + windowWidth + sideWinBoxWidth + sideWindowWidth + sideWindowHeight +
                            doorWidth + doorHeight + roofHeight + roofLowWidth + roofUpWidth + roofDepth +
                            (hasRoof ? 1 : 0) + sumSep + floorsChecksum; //sumBalcony + RoofProbability + SeparatorProb;

            // Return (do nothing) unless tree parameters change
            if (newChecksum == checksum) return;

            checksum = newChecksum;

            GenerateBuilding(); // Update tree mesh

            if (this.gameObject.GetComponent<BoxCollider>() == null)
                this.gameObject.AddComponent<BoxCollider>();

            Vector3 size = Vector3.zero;
            foreach (Transform child in this.transform)
            {
                float ySize = 0;
                foreach (Transform grandchild in child)
                {

                    size.x = grandchild.gameObject.GetComponent<BoxCollider>().size.x > size.x ? grandchild.gameObject.GetComponent<BoxCollider>().size.x : size.x;
                    ySize = grandchild.gameObject.GetComponent<BoxCollider>().size.y > ySize ? grandchild.gameObject.GetComponent<BoxCollider>().size.y : ySize;
                    size.z = grandchild.gameObject.GetComponent<BoxCollider>().size.z > size.z ? grandchild.gameObject.GetComponent<BoxCollider>().size.z : size.z;
                    DestroyImmediate(grandchild.gameObject.GetComponent<BoxCollider>());
                }
                size.y += ySize;
            }
            size = size + new Vector3(0.2f, 0.2f, 0.2f);
            this.gameObject.GetComponent<BoxCollider>().center = new Vector3(0f, size.y/2, 0f);
            this.gameObject.GetComponent<BoxCollider>().size = size;
            
        }

        public void GenerateBuilding()
        {
            var children = transform.childCount;

            if (children > floors)
            { 
                List<GameObject> childrenToDelete = new List<GameObject>();
                for (int i = floors; i < children; i++)
                    childrenToDelete.Add(this.transform.GetChild(i).gameObject);
                for (int i = 0; i < childrenToDelete.Count; i++)
                    DestroyImmediate(childrenToDelete[i]);
                childrenToDelete.Clear();
            }
            else
            {
                for (int i = children; i <= floors - 1; i++)
                {
                    buildingFloor = new GameObject();
                    buildingFloor.name = string.Format("Floor" + i);
                    buildingFloor.transform.parent = this.transform;

//CHECK
                    buildingFloor.transform.localPosition = Vector3.zero;
                    buildingFloor.transform.localScale = Vector3.one;
                    buildingFloor.transform.localRotation = new Quaternion(0f, 0f, 0f, 0f);
//END CHECK
                    buildingFloor.AddComponent<Floor>();
                    AssignValues(buildingFloor.GetComponent<Floor>());

                    buildingFloor.GetComponent<Floor>().Generate();
                    buildingFloor.GetComponent<Floor>().floorNumber = i;
                }
            }

            for (int i = 0; i < floors; i++)
            {
                buildingFloor = this.transform.GetChild(i).gameObject;

                if (buildingFloor != null && !buildingFloor.GetComponent<Floor>().floorChanged)
                    AssignValues(buildingFloor.GetComponent<Floor>());

                buildingFloor.GetComponent<Floor>().ComputeBalconies();
            }

            if (chosenMat == null)
                chosenMat = wallsMat[color];
            else
            {
                wallsMat = Resources.LoadAll("Palette" + palette, typeof(Material)).Cast<Material>().ToArray();
                chosenMat = wallsMat[Mathf.Min(color, wallsMat.Length - 1)];
            }

            if (separatorList.Count < floors)
                for (int i = separatorList.Count; i < floors; i++)
                    separatorList.Add((Random.value < separatorProb) ? true : false);
            else
            {
                var count = separatorList.Count;
                for (int i = floors; i < count; i++)
                    separatorList.RemoveAt(separatorList.Count -1);
            }
            var buildingPos = this.transform.position;
            var buildingRot = this.transform.rotation;
            CreateBuilding(Vector3.zero, 0f, 0);
            this.transform.position = buildingPos;
            this.transform.rotation = buildingRot;
        }

        public void AssignValues(Floor floorScript)
        {
            floorScript.floorHeight = floorHeight;
            floorScript.windowBoxWidth = windowBoxWidth;
            floorScript.windowHeight = windowHeight;
            floorScript.windowWidth = windowWidth;
            floorScript.sideWinBoxWidth = sideWinBoxWidth;
            floorScript.sideWindowWidth = sideWindowWidth;
            floorScript.sideWindowHeight = sideWindowHeight;
            floorScript.separatorOffset = separatorOffset;
            floorScript.separatorHeight = separatorHeight;
            floorScript.checksum = floorHeight + windowBoxWidth + windowHeight + windowWidth +
                            sideWinBoxWidth + sideWindowWidth + sideWindowHeight + separatorOffset + separatorHeight;
        }

        public void CreateBuilding(Vector3 position, float texCoord, int floor)
        {
            Floor floorScript = this.transform.GetChild(floor).GetComponent<Floor>();

            if (floor == floors - 1)
            {
                floorScript.CreateFloor(position);
                return;
            }
            else
            {
                Vector3 point = floorScript.CreateFloor(position);
                CreateBuilding(point, point.y, floor + 1);
            }
        }

        public Bounds GetASingleMesh()
        {
            Mesh mesh = new Mesh();
            int childCount = this.transform.childCount;
            CombineInstance[] combine = new CombineInstance[childCount];

            int i = 0;
            foreach (Transform child in this.transform)
            {
                combine[i].mesh = child.GetComponent<Floor>().thisMesh;
                combine[i].transform = child.GetComponent<Floor>().transform.localToWorldMatrix;
                i++;
            }
            mesh.CombineMeshes(combine);
            return mesh.bounds;
        }

    }
}
