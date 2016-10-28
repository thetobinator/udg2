using UnityEngine;
using System.Collections.Generic;

namespace ProceduralCity
{ 
    public class GridCreator : MonoBehaviour
    {
        public List<GameObject> housePrefab = new List<GameObject>();
        public List<GameObject> skyscraperPrefab = new List<GameObject>();
        public List<GameObject> conetreePrefab = new List<GameObject>();
        public List<GameObject> woodtreePrefab = new List<GameObject>();

        public enum CellType
        {
            Empty,
            Street,
            SkyScraper,
            Residential,
            Forest,
            ConeForest,
            Woods
        };

        public int width = 40;
        public int height = 40;
        public int brushSize = 2;
        public Vector3 startingPoint;
        public Vector3 endingPoint;
        public float globalScale = 1f;

        public CellType[,] worldMap;
        public List<List<Vector3>> streets;
        public bool showGridAlways = false;
        public bool showGrid = true;
        public bool mouseOverGrid = false;
        public bool createFloor = true;
        public bool firstTimeBuild = true;
        public bool paintStreets = false;
        public bool residential = false;
        public int paintForestArea = -1;
        public List<bool> selectedStreet;
        public Color streetColor = Color.blue;
        public Color skyScraperZoneColor = new Color(2f/255f, 10f/255f, 34f/255f, 1f);
        public Color residentialColor = new Color(94f / 255f, 135f / 255f, 1f, 1f);
        public Color forestColor = Color.green;
        public Color coneForestColor = new Color(36f/255f, 68f/255f, 43f/255f, 1f);
        public Color woodsColor = new Color(37f/255f, 199f/255f, 72f/255f, 1f);
        public Color floorCellColor = Color.clear;
        public Color brushColor = Color.magenta;

        public Vector3 mouseWorldPosition = new Vector3(0, 0, 0);
    
        public void GenerateMaps(bool firstTime)
        {
            if (firstTime)
            {
                worldMap = new CellType[width, height];
                worldMap = InitialiseMap(worldMap);
                streets = new List<List<Vector3>>();
                selectedStreet = new List<bool>();
                //intersectionPoints = new List<Vector3>();
            }
            else
            {
                worldMap = UpdateMap();
            }
        }

        CellType[,] InitialiseMap(CellType[,] map)
        {
            for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++)
                    map[x, y] = CellType.Empty;

            return map;
        }

        public CellType[,] UpdateMap()
        {
            CellType[,] newMap = new CellType[width, height];
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    if (i >= worldMap.GetLength(0) || j >= worldMap.GetLength(1))
                        newMap[i, j] = CellType.Empty;
                    else
                        newMap[i, j] = worldMap[i, j];
                }
            }
            return newMap; 
        }

        void OnDrawGizmosSelected()
        {
            if (!showGridAlways)
            {
                DrawGrid();
                DrawBrush();
            }
        }

        void OnDrawGizmos()
        {
            if (showGridAlways)
            {
                DrawGrid();
                DrawBrush();
            }
        }

        void DrawGrid()
        {
            if (!showGrid)
                return;
        
            Vector3 pos = new Vector3(transform.position.x, (transform.position.y + globalScale), transform.position.z);
            startingPoint = pos;
            endingPoint = new Vector3(pos.x + width * globalScale, pos.y, pos.z + height * globalScale);
            //draw grid
            int step = 0;

            for (float z = 0; z <= (float)height * globalScale; z += 1 * globalScale)
            {
                if (step == 0)
                {
                    Gizmos.color = Color.white;
                    step = 1;
                }
                else
                {
                    Gizmos.color = Color.grey;
                    step = 0;
                }
                Gizmos.DrawLine(new Vector3(pos.x, pos.y, pos.z + z),
                    new Vector3(pos.x + width * globalScale, pos.y, pos.z + z));
            }

            step = 0;
            for (float x = 0; x <= (float)width * globalScale; x += 1 * globalScale)
            {
                if (step == 0)
                {
                    Gizmos.color = Color.white;
                    step = 1;
                }
                else
                {
                    Gizmos.color = Color.grey;
                    step = 0;
                }

                Gizmos.DrawLine(new Vector3(pos.x + x, pos.y, pos.z), new Vector3(pos.x + x, pos.y, pos.z + height * globalScale));
            }

            //draw cells
            for (int y = 0; y < worldMap.GetLength(1); y++)
            {
                for (int x = 0; x < worldMap.GetLength(0); x++)
                {
                    //draw block cells
                    if (worldMap[x, y] == CellType.Street)
                    {
                        Gizmos.color = streetColor;
                        Gizmos.DrawCube(new Vector3((pos.x + (x + 0.5f) * globalScale), pos.y, (pos.z + (y + 0.5f) * globalScale)), new Vector3(1 * globalScale, 0.05f, 1 * globalScale));
                        //Gizmos.DrawCube()
                    }

                    if (worldMap[x, y] == CellType.Residential)
                    {
                        Gizmos.color = residentialColor;
                        Gizmos.DrawCube(new Vector3((pos.x + (x + 0.5f) * globalScale), pos.y, (pos.z + (y + 0.5f) * globalScale)), new Vector3(1 * globalScale, 0.05f, 1 * globalScale));
                        //Gizmos.DrawCube()
                    }

                    if (worldMap[x, y] == CellType.SkyScraper)
                    {
                        Gizmos.color = skyScraperZoneColor;
                        Gizmos.DrawCube(new Vector3((pos.x + (x + 0.5f) * globalScale), pos.y, (pos.z + (y + 0.5f) * globalScale)), new Vector3(1 * globalScale, 0.05f, 1 * globalScale));
                        //Gizmos.DrawCube()
                    }

                    if (worldMap[x, y] == CellType.Forest)
                    {
                        Gizmos.color = forestColor;
                        Gizmos.DrawCube(new Vector3((pos.x + (x + 0.5f) * globalScale), pos.y, (pos.z + (y + 0.5f) * globalScale)), new Vector3(1 * globalScale, 0.05f, 1 * globalScale));
                        //Gizmos.DrawCube()
                    }

                    if (worldMap[x, y] == CellType.ConeForest)
                    {
                        Gizmos.color = coneForestColor;
                        Gizmos.DrawCube(new Vector3((pos.x + (x + 0.5f) * globalScale), pos.y, (pos.z + (y + 0.5f) * globalScale)), new Vector3(1 * globalScale, 0.05f, 1 * globalScale));
                        //Gizmos.DrawCube()
                    }

                    if (worldMap[x, y] == CellType.Woods)
                    {
                        Gizmos.color = woodsColor;
                        Gizmos.DrawCube(new Vector3((pos.x + (x + 0.5f) * globalScale), pos.y, (pos.z + (y + 0.5f) * globalScale)), new Vector3(1 * globalScale, 0.05f, 1 * globalScale));
                        //Gizmos.DrawCube()
                    }
                    
                    //draw floor
                    if (worldMap[x, y] == CellType.Empty && createFloor)
                    {
                        Gizmos.color = floorCellColor;
                        Gizmos.DrawCube(new Vector3((pos.x + (x + 0.5f) * globalScale), pos.y, (pos.z + (y + 0.5f) * globalScale)), new Vector3(1 * globalScale, 0.05f, 1 * globalScale));
                    }
                }
            }

            //draw streets
            for (int i = 0; i < streets.Count; i++)
            {
                if (streets[i].Count > 1)
                    for (int j = 0; j < streets[i].Count - 1; j++)
                    {
                        if (selectedStreet[i] == true)
                            Gizmos.color = Color.red;
                        else
                            Gizmos.color = streetColor;
                        Gizmos.DrawLine(streets[i][j], streets[i][j + 1]);
                    }
            }
        
        }

        void DrawBrush()
        {
            if (mouseOverGrid)
            {
                Gizmos.color = brushColor;

                /*if (!brushx2)
                {
                    Vector3 _brPos = new Vector3((Mathf.Floor(mouseWorldPosition.x / 1) + 0.5f) * globalScale, (transform.position.y + 1.05f + mapIndex) + globalScale, (Mathf.Floor(mouseWorldPosition.z / globalScale) + 0.5f) * globalScale);
                    Gizmos.DrawCube(_brPos, new Vector3(1, 0.05f, 1));
                }*/
                //else
                //{
                for (int y = 0; y < brushSize; y++)
                {
                    for (int x = 0; x < brushSize; x++)
                    {
                        Vector3 _pos = new Vector3((Mathf.Floor(mouseWorldPosition.x / globalScale) + 0.5f + x) * globalScale, (transform.position.y + (1.05f * globalScale)), (Mathf.Floor(mouseWorldPosition.z / globalScale) + 0.5f + y) * globalScale);
                    
                        if (_pos.x > 0 + transform.position.x && _pos.z > 0 + transform.position.z && _pos.x < transform.position.x + (width * globalScale) && _pos.z < transform.position.z + (height * globalScale))
                        {
                            Gizmos.DrawCube(_pos, new Vector3(1 * globalScale, 0.05f, 1 * globalScale));
                        }
                    }
                }
            }
        }

    }
}
