using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using Prefabs;

namespace ProceduralCity
{ 
    [CustomEditor(typeof(GridCreator))]
    public class GridEditor : Editor
    {
        public string[] toolbarStrings = new string[] { "Draw Streets", "Draw Residential Zone", "Draw Skyscraper Zone", "Draw Forests", "Draw Cone Forests", "Draw Woods" };
        public int toolBarInt = 0;
        GridCreator grid;

        //Editor
        Plane groundPlane = new Plane(Vector3.up, new Vector3(0f, 0f, 0f));
        Vector3 currentCameraPosition;
        Vector3 lastCameraPosition;

        public LinkedList<GridCreator.CellType[,]> redoStack;
        public LinkedList<GridCreator.CellType[,]> undoStack;
        public GridCreator.CellType[,] current; 
        public const int maxStacksize = 5;
        private int streetNum;

        private bool showBuildingTypes = true;
        private bool showTreeTypes = true;

        [MenuItem("GameObject/Create Procedural/Procedural City Tool")]
        static void CreateProceduralCity()
        {
            var procCity = new GameObject("proceduralCity Tool").AddComponent<GridCreator>();
            procCity.transform.position = Vector3.zero;
        }

        private void OnEnable()
        {
            grid = (GridCreator)target;
            grid.GenerateMaps(true);
            redoStack = new LinkedList<GridCreator.CellType[,]>();
            undoStack = new LinkedList<GridCreator.CellType[,]>();
            current = (GridCreator.CellType[,])grid.worldMap.Clone();
            undoStack.AddLast(new LinkedListNode<GridCreator.CellType[,]>(grid.worldMap));
        }

        public override void OnInspectorGUI()
        {
            //base.OnInspectorGUI();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Grid Width:");
            var oldWidth = grid.width;
            grid.width = EditorGUILayout.IntSlider(grid.width, 0, 100);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Grid height:");
            var oldHeight = grid.height;
            grid.height = EditorGUILayout.IntSlider(grid.height, 0, 100);
            GUILayout.EndHorizontal();

            if (grid.width != oldWidth || grid.height != oldHeight)
            {
                grid.GenerateMaps(false);
            }
            
            ShowEdit();
            
            for (int i = 0; i < grid.streets.Count; i++)
            {
                if (grid.streets[grid.streets.Count - 1].Count > 1)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Street" + (i + 1) + ": ");

                    if (GUILayout.Button("Select"))
                    {
                        for (int j = 0; j < grid.selectedStreet.Count; j++)
                            grid.selectedStreet[j] = false;
                        grid.selectedStreet[i] = true;
                    }
                    if (GUILayout.Button("Deselect"))
                        grid.selectedStreet[i] = false;
                    if (GUILayout.Button("Delete"))
                    {
                        grid.streets.RemoveAt(i);
                        grid.selectedStreet.RemoveAt(i);
                        if (grid.selectedStreet.Count > i)
                            grid.selectedStreet[i] = false;
                    }
                    GUILayout.EndHorizontal();
                }
            }

            //select number of building types and add building prefabs
            showBuildingTypes = EditorGUILayout.Foldout(showBuildingTypes, "Show Building Types");

            if (showBuildingTypes)
            {
                GUILayout.BeginVertical("Box", GUILayout.ExpandWidth(true));

                EditorGUILayout.Space();
                AddTypesOfPrefabs("House Types", grid.housePrefab);

                EditorGUILayout.Space();
                AddTypesOfPrefabs("Skyscraper Types", grid.skyscraperPrefab);

                GUILayout.EndVertical();
            }

            EditorGUILayout.Space();
            //select number of building types and add building prefabs
            showTreeTypes = EditorGUILayout.Foldout(showTreeTypes, "Show Building Types");

            if (showTreeTypes)
            {
                GUILayout.BeginVertical("Box", GUILayout.ExpandWidth(true));

                AddTypesOfPrefabs("ConeTree Types", grid.conetreePrefab);

                EditorGUILayout.Space();
                AddTypesOfPrefabs("WoodTree Types", grid.woodtreePrefab);

                GUILayout.EndVertical();
            }

            if (GUILayout.Button("Reset Map", GUILayout.Height(30)))
            {
                grid.showGrid = true;
                redoStack = new LinkedList<GridCreator.CellType[,]>();
                undoStack = new LinkedList<GridCreator.CellType[,]>();
                grid.GenerateMaps(true);
                current = (GridCreator.CellType[,])grid.worldMap.Clone();
                undoStack.AddLast(new LinkedListNode<GridCreator.CellType[,]>(grid.worldMap));
            }

            if (GUILayout.Button("Generate City", GUILayout.Height(30)))
            {
                if (!grid.showGrid)
                    Debug.Log("Please build grid to generete");
                else
                {
                    grid.showGrid = false;
                    TerrainData terrainData = new TerrainData();
                    GameObject city = Terrain.CreateTerrainGameObject(terrainData);
                    city.transform.position = Vector3.zero;
                    city.AddComponent<CityPopulator>();
                    city.GetComponent<CityPopulator>().citysize = terrainData.size;
                    city.GetComponent<CityPopulator>().Populate(grid);
                }  
            }

            if (GUILayout.Button("Done", GUILayout.Height(30)))
            {
                GameObject city = GameObject.Find("Terrain");

                if (city == null)
                    Debug.Log("Please generate map before");
                else
                { 
                    city.GetComponent<CityPopulator>().ClearFromScripts();
                    PrefabCreator.CreatePrefab(city.gameObject);
                }
            }
            GUI.enabled = true;
        

            SceneView.RepaintAll();

            if (GUI.changed)
            {
                grid.firstTimeBuild = true;
            }

        }

        void AddTypesOfPrefabs(string type, List<GameObject> transformList)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(type + ": ");
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Add New"))
            {
                IncreaseNumType(transformList);
            }

            if (GUILayout.Button("Remove"))
                ReduceNumType(transformList);

            GUILayout.EndHorizontal();

            EditorGUILayout.Space();

            for (int i = 0; i < transformList.Count; i++)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("    Element " + (i + 1));
                transformList[i] = (GameObject)EditorGUILayout.ObjectField(transformList[i], typeof(GameObject), true);

                if (GUILayout.Button("+", GUILayout.MaxWidth(20)))
                    InsertType(i, transformList);

                if (GUILayout.Button("-", GUILayout.MaxWidth(20)))
                    i -= RemoveType(i, transformList);

                GUILayout.EndHorizontal();
            }

        }

        void IncreaseNumType(List<GameObject> transformList)
        {
            if (transformList.Count == 0)
                transformList.Add(null);

            else
                transformList.Add(transformList[transformList.Count - 1]);
        }

        void ReduceNumType(List<GameObject> transformList)
        {
            if (transformList.Count == 0)
                return;

            transformList.RemoveAt(transformList.Count - 1);
        }

        void InsertType(int ID, List<GameObject> transformList)
        {
            transformList.Insert(ID, transformList[ID]);
        }

        int RemoveType(int ID, List<GameObject> transformList)
        {
            transformList.RemoveAt(ID);
            return 1;
        }

        public void ShowEdit()
        {
            GUILayout.BeginVertical("Box", GUILayout.ExpandWidth(false));

            EditorGUILayout.HelpBox("Left Click Add / Right Click Remove Cell" + "\n" + "Z undo, Y redo" +  "\n" + "Show/Hide grid: H", MessageType.Info);

            GUILayout.BeginHorizontal();
            GUILayout.Label("Show Grid:");
            grid.showGrid = EditorGUILayout.Toggle(grid.showGrid);

            GUILayout.Label("Show Grid on deselect:");
            grid.showGridAlways = EditorGUILayout.Toggle(grid.showGridAlways);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            toolBarInt = GUILayout.SelectionGrid(toolBarInt, toolbarStrings, 3, "Button");

            if (toolbarStrings[toolBarInt] == "Draw Streets")
            {
                grid.paintStreets = true;
                grid.paintForestArea = -1;
            }
            if (toolbarStrings[toolBarInt] == "Draw Residential Zone")
            {
                grid.paintStreets = false;
                grid.paintForestArea = -1;
                grid.residential = true;
            }
            if (toolbarStrings[toolBarInt] == "Draw Skyscraper Zone")
            {
                grid.paintStreets = false;
                grid.paintForestArea = -1;
                grid.residential = false;
            }
            
            if (toolbarStrings[toolBarInt] == "Draw Forests")
            {
                grid.paintStreets = false;
                grid.paintForestArea = 0;
            }
            if (toolbarStrings[toolBarInt] == "Draw Cone Forests")
            {
                grid.paintStreets = false;
                grid.paintForestArea = 1;
            }
            if (toolbarStrings[toolBarInt] == "Draw Woods")
            {
                grid.paintStreets = false;
                grid.paintForestArea = 2;
            }
            GUILayout.EndHorizontal();

            grid.brushSize = EditorGUILayout.IntSlider("Brush size:", grid.brushSize, 2, 5);

            grid.streetColor = EditorGUILayout.ColorField("Street Color:", grid.streetColor);
            grid.residentialColor = EditorGUILayout.ColorField("Residential Zone Color:", grid.residentialColor);
            grid.skyScraperZoneColor = EditorGUILayout.ColorField("Skyscraper Zone Color:", grid.skyScraperZoneColor);
            grid.forestColor = EditorGUILayout.ColorField("Forest Color:", grid.forestColor);
            grid.coneForestColor = EditorGUILayout.ColorField("Cone Forest Color:", grid.coneForestColor);
            grid.woodsColor = EditorGUILayout.ColorField("Woods Color:", grid.woodsColor);
            grid.brushColor = EditorGUILayout.ColorField("Brush Color:", grid.brushColor);
            //grid.floorCellColor = EditorGUILayout.ColorField("Floor Color:", grid.floorCellColor);
            GUILayout.EndVertical();
            EditorGUILayout.Space();
        }
    
        private void OnSceneGUI()
        {
            if (!grid.showGrid)
            {
                Event eH = Event.current;

                //Show grid
                if (eH.type == EventType.KeyDown)
                {
                    if (eH.keyCode == KeyCode.H)
                    {
                        grid.showGrid = true;
                    }
                }

                return;
            }

            Event e = Event.current;

            //get mouse worldposition
            grid.mouseWorldPosition = GetWorldPosition(e.mousePosition);

            //Check Mouse Events
            if (e.type == EventType.MouseDown)
            {
                //On Mouse Down store current camera position and set last position to current
                currentCameraPosition = Camera.current.transform.position;
                lastCameraPosition = currentCameraPosition;

                if (e.button == 0)
                {
                    grid.streets.Add(new List<Vector3>());
                    streetNum = grid.streets.Count - 1; 
                    if (IsMouseOverGrid(e.mousePosition) && lastCameraPosition == currentCameraPosition )
                    {
                        //no possibility of painting streets
                        if (grid.paintForestArea > -1)
                        {
                            var typeOfCell = GridCreator.CellType.Empty;
                            if (grid.paintForestArea == 0)
                                typeOfCell = GridCreator.CellType.Forest;
                            else if (grid.paintForestArea == 1)
                                typeOfCell = GridCreator.CellType.ConeForest;
                            else
                                typeOfCell = GridCreator.CellType.Woods;

                            PaintCell(typeOfCell);
                            if (grid.streets.Count == streetNum + 1)
                                grid.streets.RemoveAt(streetNum);
                        }
                        else if (grid.paintForestArea == -1 && !grid.paintStreets)
                        {
                            var typeOfCell = grid.residential? GridCreator.CellType.Residential : GridCreator.CellType.SkyScraper;

                            PaintCell(typeOfCell);
                            if (grid.streets.Count == streetNum + 1)
                                grid.streets.RemoveAt(streetNum);

                        }
                    }
                }
                else if (e.button == 1)
                {
                    if (IsMouseOverGrid(e.mousePosition) && lastCameraPosition == currentCameraPosition)
                    {
                        PaintCell(GridCreator.CellType.Empty);
                        e.Use();
                    }
                }
            }
            // If Mouse Drag and left click. Start paint
            else if (e.type == EventType.MouseDrag && e.type != EventType.KeyDown)
            {
                currentCameraPosition = Camera.current.transform.position;
                //add cell
                if (e.button == 0)
                {
                    if (IsMouseOverGrid(e.mousePosition) && lastCameraPosition == currentCameraPosition)
                    {
                        if (grid.paintStreets)
                        {
                            PaintStreet(streetNum);
                            if (grid.streets.Count > grid.selectedStreet.Count)
                                grid.selectedStreet.Add(false);
                        }
                        else if (grid.paintForestArea > -1)
                        {
                            var typeOfCell = GridCreator.CellType.Empty;
                            if (grid.paintForestArea == 0)
                                typeOfCell = GridCreator.CellType.Forest;
                            else if (grid.paintForestArea == 1)
                                typeOfCell = GridCreator.CellType.ConeForest;
                            else
                                typeOfCell = GridCreator.CellType.Woods;

                            PaintCell(typeOfCell);
                            if (grid.streets.Count == streetNum + 1)
                                grid.streets.RemoveAt(streetNum);
                        }
                        else if (grid.paintForestArea == -1 && !grid.paintStreets)
                        {
                            var typeOfCell = grid.residential ? GridCreator.CellType.Residential : GridCreator.CellType.SkyScraper;

                            PaintCell(typeOfCell);
                            if (grid.streets.Count == streetNum + 1)
                                grid.streets.RemoveAt(streetNum);

                        }
                    }

                }

                //remove cell
                else if (e.button == 1)
                {
                    if (IsMouseOverGrid(e.mousePosition) && lastCameraPosition == currentCameraPosition)
                    {
                        PaintCell(GridCreator.CellType.Empty);
                        e.Use();
                    }
                }
            }
            else if (e.type == EventType.MouseMove)
            {
                //check if mouse pointer is over grid
                grid.mouseOverGrid = IsMouseOverGrid(e.mousePosition);
            }
            else if (e.type == EventType.MouseUp)
            {
                if (grid.streets.Count > 0 && grid.streets[grid.streets.Count - 1].Count < 2)
                    grid.streets.RemoveAt(grid.streets.Count - 1);
                if (undoStack.Count == maxStacksize - 1)
                    undoStack.RemoveFirst();
                undoStack.AddLast(new LinkedListNode<GridCreator.CellType [,]>(current));
                current = (GridCreator.CellType[,])grid.worldMap.Clone();

                currentCameraPosition = Camera.current.transform.position;
                lastCameraPosition = currentCameraPosition;
            
		        Cursor.visible = true;
            }
            else if (e.type == EventType.Layout)
            {
                HandleUtility.AddDefaultControl(GUIUtility.GetControlID(GetHashCode(), FocusType.Passive));
            }
            else if (e.type == EventType.KeyDown)
            {
                if (e.keyCode == KeyCode.H)
                    grid.showGrid = false;
                else if (e.keyCode == KeyCode.Z)
                {
                    LinkedListNode<GridCreator.CellType[,]> node = undoStack.Last;
                    if (node != null)
                    {
                        if (redoStack.Count == maxStacksize - 1)
                            redoStack.RemoveFirst();
                        redoStack.AddLast(new LinkedListNode<GridCreator.CellType[,]>(current));
                        grid.worldMap = (GridCreator.CellType[,])node.Value.Clone();
                        current = (GridCreator.CellType[,])grid.worldMap.Clone();
                        undoStack.RemoveLast();
                    }
                }
                else if (e.keyCode == KeyCode.Y)
                {
                    LinkedListNode<GridCreator.CellType[,]> node = redoStack.Last;
                    if (node != null)
                    {
                        if (undoStack.Count == maxStacksize - 1)
                            undoStack.RemoveFirst();
                        undoStack.AddLast(new LinkedListNode<GridCreator.CellType[,]>(current));
                        grid.worldMap = (GridCreator.CellType[,])node.Value.Clone();
                        current = (GridCreator.CellType[,])grid.worldMap.Clone();
                        redoStack.RemoveLast();
                    }
                }
            }
        }

        void PaintStreet(int streetNum)
        {
            Vector3 gridPos = GetGridPosition(grid.mouseWorldPosition);

            //if x or z position of mouse is out of grid
            //set a correct int value
            if (gridPos.x < 0)
                gridPos.x = -1;
            if (gridPos.z < 0)
                gridPos.z = -1;

            if (gridPos.x >= 0 && gridPos.z >= 0 && gridPos.x < grid.worldMap.GetLength(0) && gridPos.z < grid.worldMap.GetLength(1))
            {
                var toAdd = grid.mouseWorldPosition;
                var lastPos = grid.streets[streetNum].Count - 1;
                if (lastPos == -1 || grid.streets[streetNum][lastPos] != toAdd)
                    grid.streets[streetNum].Add(grid.mouseWorldPosition);
                
            }

        }

        //Paint cell true = add // false = delete
        void PaintCell(GridCreator.CellType type)
        {
            Vector3 gridPos = GetGridPosition(grid.mouseWorldPosition);

            //if x or z position of mouse is out of grid
            //set a correct int value
            if (gridPos.x < 0)
                gridPos.x = -1;
            if (gridPos.z < 0)
                gridPos.z = -1;

            for (int y = 0; y < grid.brushSize; y++)
            {
                for (int x = 0; x < grid.brushSize; x++)
                {
                    //paint cells
                    if (gridPos.x + x >= 0 && gridPos.z + y >= 0 && gridPos.x + x < grid.worldMap.GetLength(0) && gridPos.z + y < grid.worldMap.GetLength(1))
                    {
                        grid.worldMap[(int)gridPos.x + x, (int)gridPos.z + y] = type;
                    }
                }
            }
            
            EditorUtility.SetDirty(grid);
            Cursor.visible = false;
        }
    
        // check if mouse pointer is over grid
        bool IsMouseOverGrid(Vector2 mousePos)
        {
            Ray ray = HandleUtility.GUIPointToWorldRay(mousePos); 
            float dist = 0.0f;

            if (groundPlane.Raycast(ray, out dist))
            {
                Vector3 worldPos = ray.origin + ray.direction.normalized * dist;

                if (worldPos.x > grid.transform.position.x - 1 && worldPos.x < (grid.width * grid.globalScale) + grid.transform.position.x && worldPos.z > grid.transform.position.z - 1 && worldPos.z < (grid.height * grid.globalScale) + grid.transform.position.z)
                    return true;
            }
            return false;
        }

        //return mouse world position
        Vector3 GetWorldPosition(Vector2 mousePos)
        {
            Ray ray = HandleUtility.GUIPointToWorldRay(mousePos);
            float dist = 0.0f;

            if (groundPlane.Raycast(ray, out dist))
                return ray.origin + ray.direction.normalized * dist;

            return new Vector3(0,0);
        }

        //return the exact grid / cell position
        Vector3 GetGridPosition(Vector3 mousePos)
        {
            Vector3 gridPos = new Vector3((Mathf.Floor(mousePos.x - grid.transform.position.x / 1) / grid.globalScale), 0.05f, (Mathf.Floor(mousePos.z - grid.transform.position.z / 1) / grid.globalScale));

            return gridPos;
        }

    }
}