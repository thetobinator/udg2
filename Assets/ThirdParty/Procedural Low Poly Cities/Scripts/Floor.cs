using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MeshOperation;

namespace ProceduralCity
{
    public class Floor : MonoBehaviour
    {

        [Range(1, 8)]
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
        [Range(0.1f, 0.4f)]
        public float separatorOffset;
        [Range(0.1f, 0.4f)]
        public float separatorHeight;

        public List<bool> frontWindows;
        public List<bool> backWindows;
        public List<bool> leftWindows;
        public List<bool> rightWindows;

        public Material chosenMat;

        public int floorNumber = 0;

        public float checksum;
        public int sumBalcony;

        private List<Vector3> vertexList; // Vertex list
        private List<Vector2> uvList; // UV list
        private List<int> triangleList; // Triangle list

        private List<Vector3> vertexList2; // Vertex list 2
        private List<Vector2> uvList2; // UV list 2
        private List<int> triangleList2; // Triangle list 2

        GameObject floor;
        GameObject floorAddOns;

        private Building parentInfo;

        public bool floorChanged = false;

        public Mesh thisMesh;

        public void OnEnable()
        {
            floor = new GameObject();
            floor.name = "Floor";
            floor.transform.parent = this.transform;
            floor.AddComponent<MeshFilter>();
            floor.AddComponent<MeshRenderer>();

            floorAddOns = new GameObject();
            floorAddOns.name = "FloorAddOns";
            floorAddOns.transform.parent = this.transform;
            floorAddOns.AddComponent<MeshFilter>();
            floorAddOns.AddComponent<MeshRenderer>();
            Material floorAddOnsMat = Resources.Load("AddOnsMat", typeof(Material)) as Material;
            floorAddOns.GetComponent<MeshRenderer>().material = floorAddOnsMat;
        }

        public void Generate()
        {
            parentInfo = this.transform.parent.GetComponent<Building>();
            if (floor == null)
            {
                floor = new GameObject();
                floor.name = "Floor";
                floor.transform.parent = this.transform;
                floor.transform.localPosition = Vector3.zero;
                floor.transform.localScale = Vector3.one;
                floor.transform.localRotation = new Quaternion(0f, 0f, 0f, 0f);
                floor.AddComponent<MeshFilter>();
                floor.AddComponent<MeshRenderer>();
                
                floorAddOns = new GameObject();
                floorAddOns.name = "FloorAddOns";
                floorAddOns.transform.parent = this.transform;
                floorAddOns.transform.localPosition = Vector3.zero;
                floorAddOns.transform.localScale = Vector3.one;
                floorAddOns.transform.localRotation = new Quaternion(0f, 0f, 0f, 0f);
                floorAddOns.AddComponent<MeshFilter>();
                floorAddOns.AddComponent<MeshRenderer>();
                Material floorAddOnsMat = Resources.Load("AddOnsMat", typeof(Material)) as Material;
                floorAddOns.GetComponent<MeshRenderer>().material = floorAddOnsMat;
                
                this.floorNumber = (parentInfo.transform.childCount) - 1;
            }

            ComputeBalconies();

            //Floor parameter checksum
            var newChecksum = floorHeight + windowBoxWidth + windowHeight + windowWidth +
                            sideWinBoxWidth + sideWindowWidth + sideWindowHeight + separatorOffset + separatorHeight;// + sumBalcony; 

            GenerateFloor(); // Update tree mesh

            if (newChecksum != checksum)
            {
                checksum = newChecksum;
                parentInfo.CreateBuilding(parentInfo.transform.position, 0f, 0);
            }
        }

        public void GenerateFloor()
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

            if (vertexList2 == null) // Create lists for holding generated vertices
            {
                vertexList2 = new List<Vector3>();
                uvList2 = new List<Vector2>();
                triangleList2 = new List<int>();
            }
            else // Clear lists for holding generated vertices
            {
                vertexList2.Clear();
                uvList2.Clear();
                triangleList2.Clear();
            }

        }

        public void ComputeBalconies()
        {
            if (frontWindows == null)
                frontWindows = new List<bool>();
            if (backWindows == null)
                backWindows = new List<bool>();
            if (leftWindows == null)
                leftWindows = new List<bool>();
            if (rightWindows == null)
                rightWindows = new List<bool>();

            float winBox = Mathf.Min(this.windowBoxWidth, this.transform.parent.GetComponent<Building>().width);
            int winNumber = (int)Mathf.Floor(this.transform.parent.GetComponent<Building>().width / winBox);
            int length = frontWindows.Count;
            if (length < winNumber)
            {
                for (int i = length; i < winNumber; i++)
                {
                    frontWindows.Add(Random.Range(0f, 1f) < 0.2f ? true : false); //balconyProbability = 0.2f
                    backWindows.Add(Random.Range(0f, 1f) < 0.2f ? true : false); //balconyProbability = 0.2f
                }
            }
            else
            {
                //remove elements not needed from list
                for (int i = winNumber; i < length; i++)
                {
                    frontWindows.RemoveAt(frontWindows.Count - 1);
                    backWindows.RemoveAt(backWindows.Count - 1);
                }
            }

            float sideWinBox = Mathf.Min(this.sideWinBoxWidth, this.transform.parent.GetComponent<Building>().depth);
            int sideWinNumber = (int)Mathf.Floor(this.transform.parent.GetComponent<Building>().depth / sideWinBox);
            length = leftWindows.Count;
            if (length < sideWinNumber)
            {
                for (int i = length; i < sideWinNumber; i++)
                {
                    leftWindows.Add(Random.Range(0f, 1f) < 0.2f ? true : false); //balconyProbability = 0.2f
                    rightWindows.Add(Random.Range(0f, 1f) < 0.2f ? true : false); //balconyProbability = 0.2f
                }
            }
            else
            {
                //remove elements not needed from list
                for (int i = sideWinNumber; i < length; i++)
                {
                    leftWindows.RemoveAt(leftWindows.Count - 1);
                    rightWindows.RemoveAt(rightWindows.Count - 1);
                }
            }
        }

        public Vector3 CreateFloor(Vector3 position)
        {
            GenerateFloor();
            floor = this.transform.FindChild("Floor").gameObject;
            floorAddOns = this.transform.FindChild("FloorAddOns").gameObject;
            parentInfo = this.transform.parent.GetComponent<Building>();

            floor.GetComponent<MeshRenderer>().material = this.transform.parent.GetComponent<Building>().chosenMat;
            Vector3 point = position;

            MeshOps.CreateParall(point, parentInfo.width, this.floorHeight, parentInfo.depth, vertexList, triangleList, uvList);
            CreateWindows(point, position.y, this.floorNumber, parentInfo.width, parentInfo.depth);
            point = new Vector3(point.x, position.y + this.floorHeight, point.z);

            if (parentInfo.separatorList[this.floorNumber])
            {
                //create separator
                MeshOps.CreateParall(point, parentInfo.width + this.separatorOffset * 2, this.separatorHeight, parentInfo.depth + this.separatorOffset * 2, vertexList2, triangleList2, uvList2);
                point = new Vector3(position.x, point.y + this.separatorHeight, position.z);
            }
            if (this.floorNumber == this.transform.parent.GetComponent<Building>().floors - 1)
            {
                //CreateRoof if needed
                if (parentInfo.hasRoof)
                    CreateRoof(point);
            }

            SetAndClearFloorMesh();
            SetAndClearFloorAddOnsMesh();

            if (floor.GetComponent<BoxCollider>() != null)
                DestroyImmediate(floor.gameObject.GetComponent<BoxCollider>());
            if (floorAddOns.GetComponent<BoxCollider>() != null)
                DestroyImmediate(floorAddOns.gameObject.GetComponent<BoxCollider>());
            floor.AddComponent<BoxCollider>();
            floorAddOns.AddComponent<BoxCollider>();
            return point;
        }

        public void SetAndClearFloorMesh()
        {
            MeshFilter filter = floor.GetComponent<MeshFilter>();
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

        public void SetAndClearFloorAddOnsMesh()
        {
            MeshFilter filter = floorAddOns.GetComponent<MeshFilter>();
            var mesh = filter.sharedMesh;
            if (mesh == null)
                mesh = filter.sharedMesh = new Mesh();
            else
                mesh.Clear();

            // Assign vertex data
            mesh.vertices = vertexList2.ToArray();
            mesh.uv = uvList2.ToArray();
            mesh.triangles = triangleList2.ToArray();

            // Update mesh
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
            //mesh.Optimize(); // Do not call this if we are going to change the mesh dynamically!
            vertexList2.Clear();
            uvList2.Clear();
            triangleList2.Clear();

        }
        
        public void CreateRoof(Vector3 position)
        {
            float rLowWidth;
            if ((parentInfo.roofLowWidth - parentInfo.width) < 0)
                rLowWidth = parentInfo.width;
            else
                rLowWidth = parentInfo.roofLowWidth;

            float rUpWidth = Mathf.Min(rLowWidth, parentInfo.roofUpWidth);

            float rDepth;
            if ((parentInfo.roofDepth - parentInfo.depth) < 0)
                rDepth = parentInfo.depth;
            else
                rDepth = parentInfo.roofDepth;

            var offsetLowX = (rLowWidth - parentInfo.width) / 2;
            var offsetUpX = (parentInfo.width - rUpWidth) / 2;
            var offsetZ = (rDepth - parentInfo.depth) / 2;

            //build classic roof
            if (floorNumber <= 3)
            {
                //front
                vertexList.Add(new Vector3(position.x - parentInfo.width/2 + offsetUpX, position.y + parentInfo.roofHeight, position.z)); //top left
                vertexList.Add(new Vector3(position.x + parentInfo.width/2 - offsetUpX, position.y + parentInfo.roofHeight, position.z)); //top right
                vertexList.Add(new Vector3(position.x - parentInfo.width/2 - offsetLowX, position.y, position.z  - parentInfo.depth/2 - offsetZ)); //bot left 
                vertexList.Add(new Vector3(position.x + parentInfo.width/2 + offsetLowX, position.y, position.z - parentInfo.depth/2 - offsetZ)); //bot right

                MeshOps.AddQuad(vertexList, triangleList);

                uvList.Add(new Vector2(position.x - parentInfo.width / 2 + offsetUpX, position.y + parentInfo.roofHeight)); //top left
                uvList.Add(new Vector2(position.x + parentInfo.width / 2 - offsetUpX, position.y + parentInfo.roofHeight)); //top right
                uvList.Add(new Vector2(position.x - parentInfo.width / 2 - offsetLowX, position.y)); //bot left
                uvList.Add(new Vector2(position.x + parentInfo.width / 2 + offsetLowX, position.y)); //bot right

                //back
                vertexList.Add(new Vector3(position.x + parentInfo.width/2 - offsetUpX, position.y + parentInfo.roofHeight, position.z)); //top right
                vertexList.Add(new Vector3(position.x - parentInfo.width/2 + offsetUpX, position.y + parentInfo.roofHeight, position.z)); //top left
                vertexList.Add(new Vector3(position.x + parentInfo.width/2 + offsetLowX, position.y, position.z + parentInfo.depth/2 + offsetZ)); //bot right
                vertexList.Add(new Vector3(position.x - parentInfo.width/2 - offsetLowX, position.y, position.z + parentInfo.depth/2 + offsetZ)); //bot left 

                MeshOps.AddQuad(vertexList, triangleList);

                uvList.Add(new Vector2(position.x + parentInfo.width / 2 - offsetUpX, position.y + parentInfo.roofHeight)); //top right
                uvList.Add(new Vector2(position.x - parentInfo.width / 2 + offsetUpX, position.y + parentInfo.roofHeight)); //top left
                uvList.Add(new Vector2(position.x + parentInfo.width / 2 + offsetLowX, position.y)); //bot right
                uvList.Add(new Vector2(position.x - parentInfo.width/2 - offsetLowX, position.y)); //bot left

                //left side
                vertexList.Add(new Vector3(position.x - parentInfo.width/2 + offsetUpX, position.y + parentInfo.roofHeight, position.z)); //top
                vertexList.Add(new Vector3(position.x - parentInfo.width/2 - offsetLowX, position.y, position.z - parentInfo.depth/2 - offsetZ)); //bot right
                vertexList.Add(new Vector3(position.x - parentInfo.width/2- offsetLowX, position.y, position.z + parentInfo.depth/2 + offsetZ)); //bot left 

                MeshOps.AddTriangle(vertexList, triangleList);

                uvList.Add(new Vector2(position.x - parentInfo.width / 2 + offsetUpX, position.y + parentInfo.roofHeight)); //top right
                uvList.Add(new Vector2(position.x - parentInfo.width / 2 - offsetLowX, position.y)); //top left
                uvList.Add(new Vector2(position.x - parentInfo.width / 2 - offsetLowX, position.y)); //bot left

                //right side
                vertexList.Add(new Vector3(position.x + parentInfo.width/2 - offsetUpX, position.y + parentInfo.roofHeight, position.z)); //top right
                vertexList.Add(new Vector3(position.x + parentInfo.width/2 + offsetLowX, position.y, position.z + parentInfo.depth/2 + offsetZ)); //bot right
                vertexList.Add(new Vector3(position.x + parentInfo.width/2 + offsetLowX, position.y, position.z - parentInfo.depth/2 - offsetZ)); //bot left 

                MeshOps.AddTriangle(vertexList, triangleList);

                uvList.Add(new Vector2(position.x + parentInfo.width/2 - offsetUpX, position.y + parentInfo.roofHeight)); //top right
                uvList.Add(new Vector2(position.x + parentInfo.width/2 + offsetLowX, position.y)); //top left
                uvList.Add(new Vector2(position.x + parentInfo.width/2- offsetLowX, position.y)); //bot left

                //bottom
                vertexList.Add(new Vector3(position.x - parentInfo.width/2 - offsetLowX, position.y, position.z - parentInfo.depth/2 - offsetZ)); //top left
                vertexList.Add(new Vector3(position.x + parentInfo.width/2 + offsetLowX, position.y, position.z - parentInfo.depth/2 - offsetZ)); //top right
                vertexList.Add(new Vector3(position.x - parentInfo.width/2 - offsetLowX, position.y, position.z + parentInfo.depth/2 + offsetZ)); //bot left 
                vertexList.Add(new Vector3(position.x + parentInfo.width/2 + offsetLowX, position.y, position.z + parentInfo.depth/2 + offsetZ)); //bot right

                MeshOps.AddQuad(vertexList, triangleList);

                uvList.Add(new Vector2(position.x - parentInfo.width / 2 - offsetLowX, position.y)); //top left
                uvList.Add(new Vector2(position.x + parentInfo.width / 2 + offsetLowX, position.y)); //top right
                uvList.Add(new Vector2(position.x - parentInfo.width / 2 - offsetLowX, position.y)); //bot left
                uvList.Add(new Vector2(position.x + parentInfo.width / 2 + offsetLowX, position.y)); //bot right
            }
            //build skyscraper roof
            else
            {
                Vector3 point;

                //square empty in the middle
                if (parentInfo.roofType == 1)
                {
                    if (offsetZ <= 0.1f)
                    {
                        rDepth = rDepth - offsetZ * 2 + 0.4f;
                        offsetZ = 0.2f;
                    }
                    if (offsetLowX <= 0.1f)
                    {
                        rLowWidth = rLowWidth - offsetLowX * 2 + 0.4f;
                        offsetLowX = 0.2f;
                    }
                    
                    point = new Vector3(position.x, position.y, position.z - parentInfo.depth/2);
                    MeshOps.CreateParall(point, Mathf.Min(rLowWidth, parentInfo.width * 11/10), 0.4f, offsetZ * 2, vertexList, triangleList, uvList); //front
                    
                    point = new Vector3(position.x, position.y, position.z + parentInfo.depth/2);
                    MeshOps.CreateParall(point, Mathf.Min(rLowWidth, parentInfo.width * 11 / 10), 0.4f, offsetZ * 2, vertexList, triangleList, uvList); //back

                    point = new Vector3(position.x - parentInfo.width/2, position.y, position.z);
                    MeshOps.CreateParall(point, 2 * offsetLowX, 0.4f, Mathf.Min(rDepth, parentInfo.depth * 11/10), vertexList, triangleList, uvList); //left

                    point = new Vector3(position.x + parentInfo.width/2, position.y, position.z);
                    MeshOps.CreateParall(point, 2 * offsetLowX, 0.4f, Mathf.Min(rDepth, parentInfo.depth * 11 / 10), vertexList, triangleList, uvList);
                }
                //single big square
                else
                {
                    MeshOps.CreateParall(position, rLowWidth, 0.4f, rDepth, vertexList2, triangleList2, uvList2);
                }
            }
        }

        public void CreateWindows(Vector3 position, float textCoord, int floorNumber, float width, float depth)
        {
            Vector3 point;

            if (this.floorNumber == parentInfo.floors)
                return;

            if (this.floorNumber == 0) //create door
            {
                point = new Vector3(position.x, position.y, position.z - depth/2 - 0.01f);
                MeshOps.CreateParall(point, parentInfo.doorWidth, parentInfo.doorHeight, 0.3f, vertexList2, triangleList2, uvList2);
                return;
            }

            //create windows
            //front and back windows
            float winBox = Mathf.Min(this.windowBoxWidth, width);
            int winNumber = (int)Mathf.Floor(width / winBox);
            float winWidth = Mathf.Min(winBox - (0.1f * 2), this.windowWidth);
            float winHeight = Mathf.Min(this.windowHeight, this.floorHeight - 0.2f);
            float winDepth = 0.03f;
            float offset = width / winNumber;

            for (int i = 0; i < winNumber; i++)
            {
                //CreateBalcony
                if (this.frontWindows[i])
                {

                    var balconyWidth = winWidth + 0.3f;
                    point = new Vector3(position.x - width/2 + offset * i + (offset/2), position.y + 0.05f, position.z - depth/2 - 0.4f);
                    MeshOps.CreateParall(point, balconyWidth, 0.4f, 0.1f, vertexList2, triangleList2, uvList2); //front
                    var newPoint = new Vector3(point.x, point.y, point.z + 0.2f);
                    MeshOps.CreateParall(newPoint, balconyWidth, 0.1f, 0.5f, vertexList2, triangleList2, uvList2); //bottom

                    newPoint = new Vector3(point.x - balconyWidth / 2, point.y, point.z + 0.2f);
                    MeshOps.CreateParall(newPoint, 0.1f, 0.4f, 0.5f, vertexList2, triangleList2, uvList2); //left
                    newPoint = new Vector3(point.x + balconyWidth/2, point.y, point.z + 0.2f);
                    MeshOps.CreateParall(newPoint, 0.1f, 0.4f, 0.5f, vertexList2, triangleList2, uvList2); //right

                    point = new Vector3(point.x, position.y + 0.05f, position.z - depth/2 - 0.01f);
                    MeshOps.CreateParall(point, winWidth, this.floorHeight - (this.floorHeight - winHeight) / 2 - 0.05f, winDepth, vertexList2, triangleList2, uvList2); //window

                }
                else
                {
                    point = new Vector3(position.x - width/2 + offset * i + (offset / 2), position.y + (this.floorHeight - winHeight) / 2, position.z - depth/2 - 0.01f);
                    MeshOps.CreateParall(point, winWidth, winHeight, winDepth, vertexList2, triangleList2, uvList2);
                }
                
                if (this.backWindows[i])
                {
                    var balconyWidth = winWidth + 0.3f;
                    point = new Vector3(position.x - width/2 + offset * i + (offset / 2), position.y + 0.05f, position.z + depth/2);
                    var newPoint = new Vector3(point.x, point.y, point.z + 0.4f);
                    MeshOps.CreateParall(newPoint, balconyWidth, 0.4f, 0.1f, vertexList2, triangleList2, uvList2); //front
                    newPoint = new Vector3(point.x, point.y, point.z + 0.2f);
                    MeshOps.CreateParall(newPoint, balconyWidth, 0.1f, 0.5f, vertexList2, triangleList2, uvList2); //bottom
                    newPoint = new Vector3(point.x - balconyWidth/2, point.y, point.z + 0.2f);
                    MeshOps.CreateParall(newPoint, 0.1f, 0.4f, 0.5f, vertexList2, triangleList2, uvList2); //left side
                    newPoint = new Vector3(point.x + balconyWidth / 2, point.y, point.z + 0.2f);
                    MeshOps.CreateParall(newPoint, 0.1f, 0.4f, 0.5f, vertexList2, triangleList2, uvList2); //right side

                    point = new Vector3(point.x, position.y + 0.05f, position.z + depth / 2 + 0.01f);
                    MeshOps.CreateParall(point, winWidth, this.floorHeight - (this.floorHeight - winHeight) / 2 - 0.05f, winDepth, vertexList2, triangleList2, uvList2); //window

                }
                else
                {
                    point = new Vector3(position.x - width / 2 + offset * i + (offset / 2), position.y + (this.floorHeight - winHeight) / 2, position.z + depth / 2 + 0.01f);
                    MeshOps.CreateParall(point, winWidth, winHeight, winDepth, vertexList2, triangleList2, uvList2);
                }
            }
            
            //side windows
            float sideWinBox = Mathf.Min(this.sideWinBoxWidth, depth);
            int sideWinNumber = (int)Mathf.Floor(depth / sideWinBox);
            winWidth = Mathf.Min(sideWinBox - (0.1f * 2), this.sideWindowWidth);
            winHeight = Mathf.Min(this.sideWindowHeight, this.floorHeight - 0.2f);
            winDepth = 0.03f;
            offset = depth / sideWinNumber;

            for (int i = 0; i < sideWinNumber; i++)
            {
                //CreateBalcony
                if (this.leftWindows[i])
                {
                    var balconyWidth = winWidth + 0.3f;
                    point = new Vector3(position.x - width/2, position.y + 0.05f, position.z - depth/2 + offset * i + (offset / 2));

                    var newPoint = new Vector3(point.x - 0.4f, point.y, point.z);
                    MeshOps.CreateParall(newPoint, 0.1f, 0.4f, balconyWidth, vertexList2, triangleList2, uvList2); //front
                    newPoint = new Vector3(point.x - 0.2f, point.y, point.z);
                    MeshOps.CreateParall(newPoint, 0.5f, 0.1f, balconyWidth, vertexList2, triangleList2, uvList2); //bottom
                    newPoint = new Vector3(point.x - 0.2f, point.y, point.z + balconyWidth / 2);
                    MeshOps.CreateParall(newPoint, 0.5f, 0.4f, 0.1f, vertexList2, triangleList2, uvList2); //left
                    newPoint = new Vector3(point.x - 0.2f, point.y, point.z - balconyWidth / 2);
                    MeshOps.CreateParall(newPoint, 0.5f, 0.4f, 0.1f, vertexList2, triangleList2, uvList2); //right

                    point = new Vector3(point.x - 0.01f, position.y + 0.05f, point.z);
                    MeshOps.CreateParall(point, winDepth, this.floorHeight - (this.floorHeight - winHeight) / 2 - 0.05f, winWidth, vertexList2, triangleList2, uvList2); //window

                }
                else
                {
                    point = new Vector3(position.x - width / 2 - 0.01f, position.y + (this.floorHeight - winHeight) / 2, position.z - depth / 2 + offset * i + (offset / 2));
                    MeshOps.CreateParall(point, winDepth, winHeight, winWidth, vertexList2, triangleList2, uvList2);
                }

                if (this.rightWindows[i])
                {
                    var balconyWidth = winWidth + 0.3f;
                    point = new Vector3(position.x + width / 2, position.y + 0.05f, position.z - depth / 2 + offset * i + (offset / 2));

                    var newPoint = new Vector3(point.x + 0.4f, point.y, point.z);
                    MeshOps.CreateParall(newPoint, 0.1f, 0.4f, balconyWidth, vertexList2, triangleList2, uvList2); //front
                    newPoint = new Vector3(point.x + 0.2f, point.y, point.z);
                    MeshOps.CreateParall(newPoint, 0.5f, 0.1f, balconyWidth, vertexList2, triangleList2, uvList2); //bottom
                    newPoint = new Vector3(point.x + 0.2f, point.y, point.z - balconyWidth / 2);
                    MeshOps.CreateParall(newPoint, 0.5f, 0.4f, 0.1f, vertexList2, triangleList2, uvList2); //left
                    newPoint = new Vector3(point.x + 0.2f, point.y, point.z + balconyWidth / 2);
                    MeshOps.CreateParall(newPoint, 0.5f, 0.4f, 0.1f, vertexList2, triangleList2, uvList2); //right

                    point = new Vector3(point.x + 0.01f, position.y + 0.05f, point.z);
                    MeshOps.CreateParall(point, winDepth, this.floorHeight - (this.floorHeight - winHeight) / 2 - 0.05f, winWidth, vertexList2, triangleList2, uvList2); //window
                }
                else
                {
                    point = new Vector3(position.x + width / 2 + 0.01f, position.y + (this.floorHeight - winHeight) / 2, position.z - depth / 2 + offset * i + (offset / 2));
                    MeshOps.CreateParall(point, winDepth, winHeight, winWidth, vertexList2, triangleList2, uvList2);
                }
            }
        }
    }
}
