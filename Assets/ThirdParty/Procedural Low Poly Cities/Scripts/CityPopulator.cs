using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using SegmentMath;
using ClipperLib;
using MeshOperation;
using Triangulation;
using Prefabs;

namespace ProceduralCity
{
    public class CityPopulator : MonoBehaviour
    {
        public int streetWidth = 2;

        public Vector3 citysize;
        public Vector3 gridSize;

        private Vector3 min;
        private Vector3 max;

        private int maxTry = 100;
        private List<float> streetsLength = new List<float>();
        private List<List<int>> intersections;
        private List<HashSet<int>> intersectionClusters;

        public void Populate(GridCreator grid)
        {
            min = grid.startingPoint;
            max = grid.endingPoint;
            gridSize = new Vector3(max.x - min.x, 1f, max.z - min.z);
            List<List<Vector3>> sampledStreet = SampleStreets(grid.streets);
            if (sampledStreet.Count > 0)
            {
                ComputeStreetIntersections(sampledStreet);
                ComputeIntersectionClusters();
                List<GameObject> streetWeb = CreateStreets(sampledStreet); //offsetting from polyline
                CreateSideWalks(streetWeb, sampledStreet); //offsetting from polyline
                AddBuildings(grid, sampledStreet, 0.4f);
            }
            AddTrees(grid, 0.1f);
        }

        public void ComputeStreetIntersections(List<List<Vector3>> streets)
        {
           intersections = new List<List<int>>();
            for (int i = 0; i < streets.Count; i++)
            {
                List<int> streetIntersections = new List<int>();
                for (int j = 0; j < streets.Count; j++)
                {
                    for (int k = 0; k < streets[i].Count-1; k++)
                    {
                        for (int t = 0; t < streets[j].Count-1; t++)
                        {
                            Vector3 intersection = new Vector3();
                            if (Math3D.AreLineSegmentsCrossing(out intersection, streets[i][k], streets[i][k+1], streets[j][t], streets[j][t+1]))
                            {
                                streetIntersections.Add(j);
                            }
                        }
                    }
                }
                intersections.Add(streetIntersections);
            }
        }

        public void ComputeIntersectionClusters()
        {
            intersectionClusters = new List<HashSet<int>>();
            HashSet<int> cluster = new HashSet<int>();
            cluster.Add(0);
            foreach (int inter in intersections[0])
                cluster.Add(inter);
            intersectionClusters.Add(cluster);

            for (int i = 1; i < intersections.Count; i++)
            {
                bool indexFound = false;
                foreach (HashSet<int> clust in intersectionClusters)
                { 
                    if (clust.Contains(i))
                    {
                        indexFound = true;
                        foreach (int inter in intersections[i])
                        { 
                            clust.Add(inter);
                        }
                    }
                }
                if (!indexFound)
                {
                    cluster = new HashSet<int>();
                    cluster.Add(i);
                    foreach (int inter in intersections[i])
                        cluster.Add(inter);
                    intersectionClusters.Add(cluster);
                }
            }
        }

        public List<List<Vector3>> SampleStreets(List<List<Vector3>> streets)
        {
            List<List<Vector3>> sampledStreets = new List<List<Vector3>>();
            
            for (int i = 0; i < streets.Count; i++)
            {
                streetsLength.Add(0f);
                List<Vector3> currStreet = new List<Vector3>();
                currStreet.Add(GridToTerrain(streets[i][0]));
                var lastAdd = streets[i][0];
                var lastAngle = -1f;
                for (int j = 0; j < streets[i].Count - 1; j++)
                {
                    if (Vector3.Distance(lastAdd, streets[i][j + 1]) > 0.5f && Mathf.Abs(lastAngle - Vector3.Angle(lastAdd, streets[i][j + 1])) > 1f)
                    {
                        streetsLength[i] += Vector3.Distance(currStreet[currStreet.Count - 1], GridToTerrain(streets[i][j + 1]));
                        currStreet.Add(GridToTerrain(streets[i][j + 1]));
                        lastAngle = Vector3.Angle(lastAdd, streets[i][j + 1]);
                        lastAdd = streets[i][j + 1];
                    }
                }
                sampledStreets.Add(currStreet);
            }
            return sampledStreets;
        }

        public void ComputeContourPoints(out List<Vector2> side1, out List<Vector2> side2, List<Vector3> polyline, float offset)
        {
            side1 = new List<Vector2>();
            side2 = new List<Vector2>();
            var firstTilePoint = polyline[0];
            firstTilePoint.y = 0f;
            var secondTilePoint = polyline[1];
            secondTilePoint.y = 0f;
            var tileDir = firstTilePoint - secondTilePoint;
            tileDir.Normalize();
            var tileNorm = Vector3.Cross(tileDir, tileDir.x >= 0f ? Vector3.forward : Vector3.back);
            var tilePerp = Vector3.Cross(tileNorm, tileDir);
            tilePerp.Normalize();
            tilePerp.y = 0f;

            var side1Point = ((streetWidth + offset) / 2) * tilePerp + firstTilePoint;
            side1.Add(new Vector2(side1Point.x, side1Point.z));
            var side2Point = -((streetWidth + offset) / 2) * tilePerp + firstTilePoint;
            side2.Add(new Vector2(side2Point.x, side2Point.z));

            for (int i = 0; i < polyline.Count - 2; i++)
            {
                firstTilePoint = polyline[i];
                firstTilePoint.y = 0f;
                secondTilePoint = polyline[i + 1];
                secondTilePoint.y = 0f;
                var thirdTilePoint = polyline[i + 2];
                thirdTilePoint.y = 0f;
                var vec1 = firstTilePoint - secondTilePoint;
                vec1.Normalize();
                var vec2 = thirdTilePoint - secondTilePoint;
                vec2.Normalize();
                var bisec = Vector3.zero;

                float angle = Vector3.Angle(vec1, vec2);
                float sign = Mathf.Sign(Vector3.Dot(Vector3.up, Vector3.Cross(vec1, vec2)));
                // angle in [-179,180]
                float signed_angle = angle * sign;
                // angle in [0,360] (not used but included here for completeness)
                float angle360 = (signed_angle + 180) % 360;
                float angle360Rad = (angle360 * 2f * Mathf.PI) / 360f;

                if (Mathf.Sin(angle360Rad) > 0)
                    bisec = vec2 + vec1;
                else if (Mathf.Sin(angle360Rad) < 0)
                    bisec = -(vec2 + vec1);
                else if (Mathf.Sin(angle360Rad) == 0)
                {
                    //perpendicular to vec1 
                    float xnew = vec1.z * -1;
                    float znew = -vec1.x * -1;
                    bisec = new Vector3(xnew, 0f, znew);
                }
                bisec.Normalize();
                bisec.y = 0f;

                side1Point = ((streetWidth + offset) / 2) * bisec + secondTilePoint;
                side2Point = ((streetWidth + offset) / 2) * -bisec + secondTilePoint;
                side1.Add(new Vector2(side1Point.x, side1Point.z));
                side2.Add(new Vector2(side2Point.x, side2Point.z));

            }

            //add last vertices
            firstTilePoint = polyline[polyline.Count - 2];
            firstTilePoint.y = 0f;
            secondTilePoint = polyline[polyline.Count - 1];
            secondTilePoint.y = 0f;
            tileDir = firstTilePoint - secondTilePoint;
            tileDir.Normalize();
            tileNorm = Vector3.Cross(tileDir, tileDir.x >= 0f ? Vector3.forward : Vector3.back);
            tilePerp = Vector3.Cross(tileNorm, tileDir);
            tilePerp.Normalize();
            tilePerp.y = 0f;

            side1Point = ((streetWidth + offset) / 2) * tilePerp + secondTilePoint;
            side1.Add(new Vector2(side1Point.x, side1Point.z));
            side2Point = -((streetWidth + offset) / 2) * tilePerp + secondTilePoint;
            side2.Add(new Vector2(side2Point.x, side2Point.z));

        }

        public GameObject UnifyAndClearMesh(List<GameObject> objs, string name, Material mat)
        {
            List<MeshFilter> meshFilters = new List<MeshFilter>();

            foreach (GameObject obj in objs)
            { 
                meshFilters.Add(obj.GetComponent<MeshFilter>());
            }
            CombineInstance[] combine = new CombineInstance[meshFilters.ToArray().Length];

            int j = 0;
            while (j < meshFilters.ToArray().Length)
            {
                combine[j].mesh = meshFilters[j].sharedMesh;
                combine[j].transform = meshFilters[j].transform.localToWorldMatrix;
                meshFilters[j].gameObject.SetActive(false);
                j++;
            }

            GameObject finalObj = new GameObject(name, typeof(MeshFilter), typeof(MeshRenderer));
            finalObj.transform.parent = this.transform;
            finalObj.GetComponent<MeshRenderer>().material = mat;
            finalObj.transform.GetComponent<MeshFilter>().sharedMesh = new Mesh();
            finalObj.transform.GetComponent<MeshFilter>().sharedMesh.CombineMeshes(combine);
            finalObj.transform.gameObject.SetActive(true);

            foreach (GameObject obj in objs)
                DestroyImmediate(obj);

            return finalObj;
        }

        public List<GameObject> CreateStreets(List<List<Vector3>> streetsPoly)
        {
            List<GameObject> streets = new List<GameObject>();

            for (int i = 0; i < streetsPoly.Count; i++)
            {
                GameObject street = new GameObject("street" + i, typeof(MeshRenderer), typeof(MeshFilter));
                street.transform.parent = this.transform;
                street.GetComponent<MeshRenderer>().material = (Material)Resources.Load("StreetMat");

                List<List<IntPoint>> solution = new List<List<IntPoint>>();
                //transform vertices of each mesh in points for clipper
                List<IntPoint> intPoint = FromVecToIntPoint(streetsPoly[i].ToArray());
                //offset each mesh
                ClipperOffset co = new ClipperOffset();
                co.AddPath(intPoint, JoinType.jtRound, EndType.etOpenRound);
                co.Execute(ref solution, 700.0);

                List<Vector2> vertices2D = new List<Vector2>();
                for (int j = 0; j < solution.Count; j++)
                {
                    vertices2D = vertices2D.Concat(FromIntPointToVec(solution[j])).ToList();
                }

                // Use the triangulator to get indices for creating triangles
                Triangulator tr = new Triangulator(vertices2D.ToArray());
                int[] indices = tr.Triangulate();

                // Create the Vector3 vertices
                Vector3[] vertices = new Vector3[vertices2D.Count];
                for (int k = 0; k < vertices.Length; k++)
                {
                    vertices[k] = new Vector3(vertices2D[k].x, 0f, vertices2D[k].y);
                }
                // Create the mesh
                Mesh msh = new Mesh();
                msh.vertices = vertices;
                msh.triangles = indices;
                msh.RecalculateNormals();
                msh.RecalculateBounds();

                // Set up game object with mesh;
                street.GetComponent<MeshFilter>().mesh = msh;
                street.AddComponent<MeshCollider>();
                street.transform.position = new Vector3(street.transform.position.x, 0.02f, street.transform.position.z);
                streets.Add(street);
            }
            DrawStreetLineMesh(streetsPoly, streets);
            return streets;
        }

        public void DrawStreetLineMesh(List<List<Vector3>> streets, List<GameObject> streetList)
        {
            //compute segments of street line
            //subtract polygon
            for (int i = 0; i < streets.Count; i++)
            {
                List<GameObject> lines = new List<GameObject>();
                List<Mesh> intersectionPolygons = new List<Mesh>();
                intersectionPolygons = ComputeIntersectionPoly(streetList, intersections[i], i);
                ComputeStreetLine(streets[i], intersectionPolygons, streetsLength[i], lines);
                GameObject streetLines = UnifyAndClearMesh(lines, "streetLine", (Material)Resources.Load("LineMat"));
                streetLines.transform.parent = streetList[i].transform;
                streetLines.transform.localPosition = new Vector3(streetLines.transform.localPosition.x, 0.01f, streetLines.transform.localPosition.z);
            }
        }

        public List<Mesh> ComputeIntersectionPoly(List<GameObject> objs, List<int> intersections, int index)
        {
            List<Mesh> polygons = new List<Mesh>();

            List<List<IntPoint>> subj = new List<List<IntPoint>>();
            List<List<IntPoint>> clip = new List<List<IntPoint>>();

            List<IntPoint> path = FromVecToIntPoint(objs[index].GetComponent<MeshFilter>().sharedMesh.vertices);
            subj.Add(path);

            foreach (int j in intersections)
            {
                List<IntPoint> path2 = FromVecToIntPoint(objs[j].GetComponent<MeshFilter>().sharedMesh.vertices);
                clip.Add(path2);
            }

            List<List<IntPoint>> solution = new List<List<IntPoint>>();

            Clipper c = new Clipper();
            c.AddPaths(subj, PolyType.ptSubject, true);
            c.AddPaths(clip, PolyType.ptClip, true);
            c.Execute(ClipType.ctIntersection, solution, PolyFillType.pftEvenOdd, PolyFillType.pftEvenOdd);

            List<Vector2> intersectionPoly = new List<Vector2>();
            for (int i = 0; i < solution.Count; i++)
            {
                intersectionPoly = (FromIntPointToVec(solution[i])).ToList();

                Triangulator tr = new Triangulator(intersectionPoly.ToArray());
                int[] indices = tr.Triangulate();

                // Create the Vector3 vertices
                Vector3[] vertices = new Vector3[intersectionPoly.Count];
                Vector2[] uvs = new Vector2[intersectionPoly.Count];
                for (int k = 0; k < vertices.Length; k++)
                {
                    vertices[k] = new Vector3(intersectionPoly[k].x, 0, intersectionPoly[k].y);
                    uvs[k] = new Vector2(intersectionPoly[k].x, 0);
                }
                // Create the mesh
                Mesh msh = new Mesh();
                msh.vertices = vertices;
                msh.triangles = indices;
                msh.uv = uvs;
                msh.RecalculateNormals();
                msh.RecalculateBounds();

                polygons.Add(msh);
            }

            return polygons;
        }

        public void ComputeStreetLine(List<Vector3> street, List<Mesh> intersectionPolys, float streetLength, List<GameObject> lines)
        {
            float segmentLength = 1f;
            float segmentsSpace = 1f;

            float currDistance = streetLength;
            Vector3 currPoint = street[0];
            
            int i = 0;

            List<Vector3> segment = new List<Vector3>();
            bool segAdded = false;
            
            while (currDistance > segmentLength)
            {
                segment.Clear();
                segment.Add(currPoint);

                float lengthMissing = segmentLength;
                float localDistance = Vector3.Distance(currPoint, street[i + 1]);

                while (localDistance < lengthMissing && i < (street.Count - 2))
                {
                    i++;
                    currPoint = street[i];
                    segment.Add(currPoint);
                    lengthMissing -= localDistance;
                    localDistance = Vector3.Distance(currPoint, street[i + 1]);
                }
                if (localDistance >= lengthMissing)
                    currPoint = currPoint + (lengthMissing / Vector3.Distance(street[i], street[i + 1]) * (street[i + 1] - street[i]));
                if (localDistance == lengthMissing)
                    i++;
                segment.Add(currPoint);

                DrawSegment(segment, intersectionPolys, lines);
                segAdded = true;
                lengthMissing = segmentsSpace;
                localDistance = Vector3.Distance(currPoint, street[i + 1]);
                
                while (localDistance < lengthMissing && i < (street.Count - 2))
                {
                    i++;
                    currPoint = street[i];
                    lengthMissing -= localDistance;
                    localDistance = Vector3.Distance(currPoint, street[i + 1]);
                }
                if (localDistance >= lengthMissing)
                    currPoint = currPoint + (lengthMissing / Vector3.Distance(street[i], street[i + 1]) * (street[i + 1] - street[i]));
                if (localDistance == lengthMissing)
                    i++;

                currDistance -= (segmentLength + segmentsSpace);
                segAdded = false;
            }
            if (!segAdded)
            {
                segment.Clear();
                segment.Add(currPoint);
                segment.Add(street[street.Count - 1]);
                DrawSegment(segment, intersectionPolys, lines);
            }
        }

        public void DrawSegment(List<Vector3> segment, List<Mesh> intersectionPolys, List<GameObject> lines)
        {
            List<List<IntPoint>> solution = new List<List<IntPoint>>();
            //transform vertices of each mesh in points for clipper
            List<IntPoint> intPoint = FromVecToIntPoint(segment.ToArray());
            //offset each mesh
            ClipperOffset co = new ClipperOffset();
            co.AddPath(intPoint, JoinType.jtRound, EndType.etOpenRound);
            co.Execute(ref solution, 100.0);

            List<Vector2> vertices2D = new List<Vector2>();
            for (int j = 0; j < solution.Count; j++)
            {
                vertices2D = vertices2D.Concat(FromIntPointToVec(solution[j])).ToList();
            }

            // Use the triangulator to get indices for creating triangles
            Triangulator tr = new Triangulator(vertices2D.ToArray());
            int[] indices = tr.Triangulate();
            
            // Create the Vector3 vertices
            Vector3[] vertices = new Vector3[vertices2D.Count];
            for (int k = 0; k < vertices.Length; k++)
            {
                vertices[k] = new Vector3(vertices2D[k].x, 0f, vertices2D[k].y);
            }
            // Create the mesh
            Mesh msh = new Mesh();
            msh.vertices = vertices;
            msh.triangles = indices;
            msh.RecalculateNormals();
            msh.RecalculateBounds();

            Mesh newMesh = ExecuteMultiDifferencePoly(msh, intersectionPolys);
            GameObject lineSegment = new GameObject("segment", typeof(MeshFilter), typeof(MeshRenderer));
            lineSegment.GetComponent<MeshFilter>().sharedMesh = newMesh;
            lines.Add(lineSegment);
        }

        public Mesh ExecuteMultiDifferencePoly(Mesh mesh, List<Mesh> polys)
        {
            List<List<IntPoint>> subj = new List<List<IntPoint>>();
            List<List<IntPoint>> clip = new List<List<IntPoint>>();

            List<IntPoint> path = FromVecToIntPoint(mesh.vertices);
            subj.Add(path);

            foreach (Mesh poly in polys)
            {
                List<IntPoint> path2 = FromVecToIntPoint(poly.vertices);
                clip.Add(path2);
            }

            List<List<IntPoint>> solution = new List<List<IntPoint>>();

            Clipper c = new Clipper();
            c.AddPaths(subj, PolyType.ptSubject, true);
            c.AddPaths(clip, PolyType.ptClip, true);
            c.Execute(ClipType.ctDifference, solution, PolyFillType.pftEvenOdd, PolyFillType.pftEvenOdd);

            Mesh msh = new Mesh();
            List<Vector2> diff = new List<Vector2>();

            if (solution.Count > 0)
            {
                diff = (FromIntPointToVec(solution[0])).ToList();

                Triangulator tr = new Triangulator(diff.ToArray());
                int[] indices = tr.Triangulate();

                // Create the Vector3 vertices
                Vector3[] vertices = new Vector3[diff.Count];
                Vector2[] uvs = new Vector2[diff.Count];
                for (int k = 0; k < vertices.Length; k++)
                {
                    vertices[k] = new Vector3(diff[k].x, 0, diff[k].y);
                    uvs[k] = new Vector2(diff[k].x, 0);
                }
                // Create the mesh
                msh.vertices = vertices;
                msh.triangles = indices;
                msh.uv = uvs;
                msh.RecalculateNormals();
                msh.RecalculateBounds();
            }
            return msh;
        }

        public Mesh Subtract(Vector3[] subFrom, Vector3[] toSub)
        {
            List<List<IntPoint>> subj = new List<List<IntPoint>>();
            List<List<IntPoint>> clip = new List<List<IntPoint>>();

            List<IntPoint> path = FromVecToIntPoint(subFrom);
            subj.Add(path);

            List<IntPoint> path2 = FromVecToIntPoint(toSub);
            clip.Add(path2);

            List<List<IntPoint>> solution = new List<List<IntPoint>>();

            Clipper c = new Clipper();
            c.AddPaths(subj, PolyType.ptSubject, true);
            c.AddPaths(clip, PolyType.ptClip, true);
            c.Execute(ClipType.ctDifference, solution, PolyFillType.pftEvenOdd, PolyFillType.pftEvenOdd);

            List<Vector2> subtraction = new List<Vector2>();
            for (int i = 0; i < solution.Count; i++)
            {
                subtraction = subtraction.Concat(FromIntPointToVec(solution[i])).ToList();
            }

            Triangulator tr = new Triangulator(subtraction.ToArray());
            int[] indices = tr.Triangulate();

            // Create the Vector3 vertices
            Vector3[] vertices = new Vector3[subtraction.Count];
            Vector2[] uvs = new Vector2[subtraction.Count];
            for (int k = 0; k < vertices.Length; k++)
            {
                vertices[k] = new Vector3(subtraction[k].x, 0, subtraction[k].y);
                uvs[k] = new Vector3(subtraction[k].x, 0);
            }
            // Create the mesh
            Mesh msh = new Mesh();
            msh.vertices = vertices;
            msh.triangles = indices;
            msh.uv = uvs;
            msh.RecalculateNormals();
            msh.RecalculateBounds();

            return msh;
        }

        public List<Mesh> ExecuteMultiDifference(Mesh mesh, List<GameObject> objs, List<int> intersections, int index)
        {
            List<Mesh> diffMeshes = new List<Mesh>();

            List<List<IntPoint>> subj = new List<List<IntPoint>>();
            List<List<IntPoint>> clip = new List<List<IntPoint>>();

            List<IntPoint> path = FromVecToIntPoint(mesh.vertices);
            subj.Add(path);

            foreach (int j in intersections)
            {
                List<IntPoint> path2 = FromVecToIntPoint(objs[j].GetComponent<MeshFilter>().sharedMesh.vertices);
                clip.Add(path2);
            }

            List<List<IntPoint>> solution = new List<List<IntPoint>>();

            Clipper c = new Clipper();
            c.AddPaths(subj, PolyType.ptSubject, true);
            c.AddPaths(clip, PolyType.ptClip, true);
            c.Execute(ClipType.ctDifference, solution, PolyFillType.pftEvenOdd, PolyFillType.pftEvenOdd);

            List<Vector2> diff = new List<Vector2>();
            for (int i = 0; i < solution.Count; i++)
            {
                diff = (FromIntPointToVec(solution[i])).ToList();

                Triangulator tr = new Triangulator(diff.ToArray());
                int[] indices = tr.Triangulate();

                // Create the Vector3 vertices
                Vector3[] vertices = new Vector3[diff.Count];
                Vector2[] uvs= new Vector2[diff.Count];
                for (int k = 0; k < vertices.Length; k++)
                {
                    vertices[k] = new Vector3(diff[k].x, 0, diff[k].y);
                    uvs[k] = new Vector2(diff[k].x, 0);
                }
                // Create the mesh
                Mesh msh = new Mesh();
                msh.vertices = vertices;
                msh.triangles = indices;
                msh.uv = uvs;
                msh.RecalculateNormals();
                msh.RecalculateBounds();

                diffMeshes.Add(msh);
            }

            return diffMeshes;
        }

        public void CreateSideWalks(List<GameObject> streetWeb, List<List<Vector3>> streetsPoly)
        {
            List<GameObject> sideWalks = new List<GameObject>();

            for (int i = 0; i < streetWeb.Count; i++)
            {
                GameObject sideWalk = new GameObject("sideWalk", typeof(MeshRenderer), typeof(MeshFilter));
                sideWalk.transform.parent = this.transform;
                sideWalk.GetComponent<MeshRenderer>().material = (Material)Resources.Load("SideWalkMat");

                List<List<IntPoint>> solution = new List<List<IntPoint>>();
                //transform vertices of each mesh in points for clipper
                
                //List<IntPoint> intPoint = FromVecToIntPoint(streetWeb[i].GetComponent<MeshFilter>().sharedMesh.vertices);
                List<IntPoint> intPoint = FromVecToIntPoint(streetsPoly[i].ToArray());
                
                //offset each mesh
                ClipperOffset co = new ClipperOffset();
                co.AddPath(intPoint, JoinType.jtRound, EndType.etOpenRound);
                co.Execute(ref solution, 1000.0);
                
                List<Vector2> vertices2D = new List<Vector2>();
                for (int j = 0; j < solution.Count; j++)
                {
                    vertices2D = vertices2D.Concat(FromIntPointToVec(solution[j])).ToList();
                }

                // Use the triangulator to get indices for creating triangles
                Triangulator tr = new Triangulator(vertices2D.ToArray());
                int[] indices = tr.Triangulate();

                // Create the Vector3 vertices
                Vector3[] vertices = new Vector3[vertices2D.Count];
                for (int k = 0; k < vertices.Length; k++)
                {
                    vertices[k] = new Vector3(vertices2D[k].x, 0f, vertices2D[k].y);
                }
                // Create the mesh
                Mesh msh = new Mesh();
                msh.vertices = vertices;
                msh.triangles = indices;
                msh.RecalculateNormals();
                msh.RecalculateBounds();

                // Set up game object with mesh;
                sideWalk.GetComponent<MeshFilter>().mesh = msh;

                sideWalks.Add(sideWalk);
            }

            //foreach intersectionCluster unify streets and sidewalks and subtract
            int swNumber = 1;
            foreach (HashSet<int> cluster in intersectionClusters)
            {
                Mesh newMesh = Subtract(UnifyPolygons(sideWalks, "sidewalks", cluster), UnifyPolygons(streetWeb, "streets", cluster));

                //build 3D sidewalk
                List<GameObject> objs = new List<GameObject>();

                GameObject sub = new GameObject("unifiedSideWalks", typeof(MeshFilter), typeof(MeshRenderer));
                sub.transform.parent = this.transform;
                sub.GetComponent<MeshRenderer>().material = (Material)Resources.Load("SideWalkMat");
                sub.GetComponent<MeshFilter>().mesh = newMesh;

                objs.Add(sub);

                GameObject sub1 = new GameObject("unifiedSideWalks", typeof(MeshFilter), typeof(MeshRenderer));
                sub1.transform.parent = this.transform;
                sub1.GetComponent<MeshRenderer>().material = (Material)Resources.Load("SideWalkMat");
                sub1.GetComponent<MeshFilter>().mesh = newMesh;
                sub1.transform.position = new Vector3(0f, 0.2f, 0f);

                objs.Add(sub1);

                GameObject sidewalk = new GameObject("sidewalk", typeof(MeshFilter), typeof(MeshRenderer));
                sidewalk.transform.parent = this.transform;
                sidewalk.GetComponent<MeshRenderer>().material = (Material)Resources.Load("SideWalkMat");
                sidewalk.GetComponent<MeshFilter>().sharedMesh = MeshOps.CreateSolid(newMesh, 0.15f);

                objs.Add(sidewalk);

                UnifyAndClearMesh(objs, "sideWalk" + swNumber, (Material)Resources.Load("SideWalkMat"));
                swNumber += 1;
            }

            foreach (GameObject sw in sideWalks)
                DestroyImmediate(sw.gameObject);
                    
        }

        public Vector3[] UnifyPolygons(List<GameObject> toUnify, string type, HashSet<int> cluster)
        {
            List<Vector2> union = new List<Vector2>();

            List<List<IntPoint>> subj = new List<List<IntPoint>>();
            List<List<IntPoint>> clip = new List<List<IntPoint>>();
            List<List<IntPoint>> solution = new List<List<IntPoint>>();

            HashSet<int> unifiedElements = new HashSet<int>();
            List<IntPoint> path = new List<IntPoint>();

            foreach (int index in cluster)
            {
                if (subj.Count == 0)
                {
                    path = FromVecToIntPoint(toUnify[index].GetComponent<MeshFilter>().sharedMesh.vertices);
                    subj.Add(path);
                    unifiedElements.Add(index); 
                }       
                foreach (int inter in intersections[index])
                {
                    if (!unifiedElements.Contains(inter))
                    {
                        path = FromVecToIntPoint(toUnify[inter].GetComponent<MeshFilter>().sharedMesh.vertices);
                        clip.Add(path);
                        unifiedElements.Add(inter);
                    }
                }
                //unify
                Clipper c = new Clipper();
                c.AddPaths(subj, PolyType.ptSubject, true);
                c.AddPaths(clip, PolyType.ptClip, true);
                c.Execute(ClipType.ctUnion, solution, PolyFillType.pftEvenOdd, PolyFillType.pftEvenOdd);

                subj.Clear();
                subj = new List<List<IntPoint>> (solution);
            }

            for (int j = 0; j < solution.Count; j++)
            {
                union = FromIntPointToVec(solution[j]).ToList();
            }

            Vector3[] vertices = new Vector3[union.Count];
            for (int k = 0; k < vertices.Length; k++)
            {
                vertices[k] = new Vector3(union[k].x, 0f, union[k].y);
            }

            return vertices;
        }

        public List<IntPoint> FromVecToIntPoint(Vector3[] vertices)
        {
            List<IntPoint> intPoint = new List<IntPoint>();

            foreach (Vector3 vertex in vertices)
                intPoint.Add(new IntPoint(vertex.x * 1000f, vertex.z * 1000f));

            return intPoint;
        }

        public List<Vector2> FromIntPointToVec(List<IntPoint> points)
        {
            List<Vector2> vertices = new List<Vector2>();

            for (int i = 0; i < points.Count; i++)
                vertices.Add(new Vector2(points[i].X /1000f, points[i].Y / 1000f));

            return vertices;
        }

        public void AddBuildings(GridCreator grid, List<List<Vector3>> streets, float scale)
        {
            List<List<GameObject>> buildings = new List<List<GameObject>>();

            buildings.Add(CreateListOfPrefabs(grid.housePrefab, -1, 0, scale));
            buildings.Add(CreateListOfPrefabs(grid.skyscraperPrefab, -1, 1, scale));
            
            int buildingIndex = 1;
            for (int i = 0; i < streets.Count; i++)
            {
                List<Vector2> side1 = new List<Vector2>();
                List<Vector2> side2 = new List<Vector2>();

                ComputeContourPoints(out side1, out side2, streets[i], 4f);

                for (int j = 0; j < side1.Count-1; j++)
                {
                    Vector3 pointFrom = new Vector3(side1[j].x, 0f, side1[j].y);
                    Vector3 pointTo = new Vector3(side1[j+1].x, 0f, side1[j+1].y);
                    buildingIndex = FillWithBuildings(grid, scale, pointFrom, pointTo, buildingIndex, streets[i][j], streets[i][j + 1], buildings);
                    pointFrom = new Vector3(side2[j].x, 0f, side2[j].y);
                    pointTo = new Vector3(side2[j + 1].x, 0f, side2[j + 1].y);
                    buildingIndex = FillWithBuildings(grid, scale, pointFrom, pointTo, buildingIndex, streets[i][j], streets[i][j + 1], buildings);
                }
            }

            foreach (GameObject house in buildings[0])
            {
                if (house.GetComponent<Building>() != null)
                    DestroyImmediate(house.gameObject);
            }
            foreach (GameObject skyscraper in buildings[1])
            {
                if (skyscraper.GetComponent<Building>() != null)
                    DestroyImmediate(skyscraper.gameObject);
            }
        }

        //uncommenting the while and "currDist" makes the building density higher
        public int FillWithBuildings(GridCreator grid, float scale, Vector3 pointFrom, Vector3 pointTo, int buildingIndex, Vector3 firstTilePoint, Vector3 secondTilePoint, List<List<GameObject>> buildings)
        {
            int collisions = 0;
            int numOfTry = 0;

            GameObject building;

            Vector3 position = pointFrom;
            Vector3 streetPosition = firstTilePoint;

            var distance = Vector3.Distance(pointFrom, pointTo);
            var streetDistance = Vector3.Distance(firstTilePoint, secondTilePoint);

            var direction = (pointTo - pointFrom);
            direction.Normalize();
            direction.y = 0f;
            var streetDirection = (secondTilePoint - firstTilePoint);
            streetDirection.Normalize();
            streetDirection.y = 0f;

            var offset = 0.6f;
            var streetOffset = (streetDistance * offset) / distance;
            //while (currDist < distance)
            //{
                //check for collision in current position
                collisions = 0;
                numOfTry = 0;
                var buildingType = GetGridType(position, grid);

                do
                {
                    numOfTry += 1;
                    if (buildingType == GridCreator.CellType.Residential)
                        building = buildings[0][Random.Range(0, buildings[0].Count)];
                    else if (buildingType == GridCreator.CellType.SkyScraper)
                        building = buildings[1][Random.Range(0, buildings[1].Count)];
                    else
                    {
                        List<GameObject> currList = buildings[Random.Range(0, 2)];
                        building = currList[Random.Range(0, currList.Count)];
                    }
                    Vector3 colliderSize = building.gameObject.GetComponent<BoxCollider>().size;
                    collisions = Physics.OverlapSphere(position, scale * Mathf.Max(colliderSize.x, colliderSize.y, colliderSize.z) / 2).Length;
                }
                while (collisions > 1 && numOfTry < Mathf.Max(buildings[0].Count, buildings[1].Count));

                if (collisions > 0 && collisions <= 1)
                {
                    buildingIndex += 1;
                    building = Instantiate(building);
                    building.name = "building" + buildingIndex;
                    building.transform.position = position;
                    building.transform.parent = this.transform;
                    building.transform.localScale = new Vector3(scale, scale, scale);
                
                    building.transform.rotation = Quaternion.LookRotation((position - streetPosition).normalized);

                }
                position = position + (offset * direction);
                streetPosition = streetPosition + (streetOffset * streetDirection);

            //}
            return buildingIndex;
        }

        public List<GameObject> CreateListOfPrefabs(List<GameObject> prefabList, int cone, int house, float scale)
        {
            List<GameObject> prefabs = new List<GameObject>();

            if (prefabList.Count == 0)
            {
                if (house == -1)
                    Debug.Log("Please choose number of tree types");
                else
                    Debug.Log("Please choose number of building types");
            }
            else
            {
                for (int i = 0; i < prefabList.Count; i++)
                {
                    if (prefabList[i] == null)
                    {
                        if (house == -1)
                        {
                            ProcTree procTree = new GameObject("tree").AddComponent<ProcTree>();
                            procTree.RandomizeSettings();
                            procTree.Generate(cone == 0 ? true : false);
                            prefabs.Add(procTree.gameObject);
                        }
                        else
                        {
                            Building procBuilding = new GameObject("building").AddComponent<Building>();
                            procBuilding.RandomizeSettings();
                            procBuilding.Generate();
                            procBuilding.floors = house == 0 ? Random.Range(1, 5) : Random.Range(5, 9);
                            prefabs.Add(procBuilding.gameObject);
                        }
                    }
                    else
                    {
                        prefabs.Add(prefabList[i]);
                    }
                }
            }
            return prefabs;
        }

        public void AddTrees(GridCreator grid, float scale)
        {
            List<List<GameObject>> trees = new List<List<GameObject>>();
            
            trees.Add(CreateListOfPrefabs(grid.conetreePrefab, 0, -1, scale));
            trees.Add(CreateListOfPrefabs(grid.woodtreePrefab, 1, -1,scale));
            FillWithTrees(grid, trees, scale);
        }

        public void FillWithTrees(GridCreator grid, List<List<GameObject>> trees, float scale)
        {
            int i = 1;
            int numOfTry = 0;
            float colliderRadius = 0f;
            Vector3 randPoint = new Vector3(Random.Range(min.x, max.x), Random.Range(min.y, max.y), Random.Range(min.z, max.z));
            randPoint.y = 0f;
            Vector3 gridPos = new Vector3((Mathf.Floor(randPoint.x - grid.transform.position.x / 1) / grid.globalScale), 0.05f, (Mathf.Floor(randPoint.z - grid.transform.position.z / 1) / grid.globalScale));

            GameObject tree;

            while (numOfTry < maxTry)
            {
                if (IsForestPoint(grid, gridPos))
                {
                    if (grid.worldMap[(int)gridPos.x, (int)gridPos.z] == GridCreator.CellType.ConeForest)
                        tree = trees[0][Random.Range(0, trees[0].Count)];
                    else if (grid.worldMap[(int)gridPos.x, (int)gridPos.z] == GridCreator.CellType.Woods)
                        tree = trees[1][Random.Range(0, trees[1].Count)];
                    else
                    { 
                        List<GameObject> currList = trees[Random.Range(0, 2)];
                        tree = currList[Random.Range(0, currList.Count)];
                    }

                    foreach (Transform child in tree.transform)
                        colliderRadius = child.GetComponent<CapsuleCollider>().radius * (scale + 0.02f);

                    int collisions = Physics.OverlapSphere(GridToTerrain(randPoint), colliderRadius).Length;
                    if (collisions <= 1)
                    {
                        numOfTry = 0;
                        tree = Instantiate(tree);
                        tree.name = "tree" + i;
                        tree.transform.position = GridToTerrain(randPoint);
                        tree.transform.parent = this.transform;
                        tree.transform.localScale = new Vector3(scale, scale, scale);
                        i += 1;
                    }
                    else
                        numOfTry += 1;
                }
                else
                {
                    numOfTry += 1;
                }

                randPoint = new Vector3(Random.Range(min.x, max.x), Random.Range(min.y, max.y), Random.Range(min.z, max.z));
                randPoint.y = 0f;
                gridPos = new Vector3((Mathf.Floor(randPoint.x - grid.transform.position.x / 1) / grid.globalScale), 0.05f, (Mathf.Floor(randPoint.z - grid.transform.position.z / 1) / grid.globalScale));
            }

            foreach (GameObject conetree in trees[0])
            { 
                if (conetree.GetComponent<ProcTree>() != null)
                    DestroyImmediate(conetree.gameObject);
            }
            foreach (GameObject woodtree in trees[1])
            {
                if (woodtree.GetComponent<ProcTree>() != null)
                    DestroyImmediate(woodtree.gameObject);
            }
            trees[0].Clear();
            trees[1].Clear();
            trees.Clear();
        }

        public bool IsForestPoint(GridCreator grid, Vector3 pos)
        {
            return (grid.worldMap[(int)pos.x, (int)pos.z] == GridCreator.CellType.Forest ||
                    grid.worldMap[(int)pos.x, (int)pos.z] == GridCreator.CellType.ConeForest ||
                    grid.worldMap[(int)pos.x, (int)pos.z] == GridCreator.CellType.Woods);
        }
        public Vector3 GridToTerrain(Vector3 point)
        {
            return new Vector3(point.x * citysize.x / gridSize.x, point.y * citysize.y / gridSize.y, point.z * citysize.z / gridSize.z);
        }

        public GridCreator.CellType GetGridType(Vector3 point, GridCreator grid)
        { 
            Vector3 gridPos = new Vector3((Mathf.Floor(point.x - grid.transform.position.x / 1) / grid.globalScale), 0.05f, (Mathf.Floor(point.z - grid.transform.position.z / 1) / grid.globalScale));
            
            //if x or z position of mouse is out of grid
            //set a correct int value
            if (gridPos.x< 0)
                gridPos.x = -1;
            if (gridPos.z< 0)
                gridPos.z = -1;

            if (gridPos.x >= 0 && gridPos.z >= 0 && gridPos.x < grid.worldMap.GetLength(0) && gridPos.z < grid.worldMap.GetLength(1))
            {
                return grid.worldMap[(int)gridPos.x, (int)gridPos.z];
            }

            return GridCreator.CellType.Empty;
        }

        public void ClearFromScripts()
        {
            foreach (Building obj in FindObjectsOfType(typeof(Building)))
            {
                DestroyImmediate(obj.GetComponent<Building>());
            }
            foreach (Floor obj in FindObjectsOfType(typeof(Floor)))
            {
                DestroyImmediate(obj.GetComponent<Floor>());
            }
            foreach (ProcTree obj in FindObjectsOfType(typeof(ProcTree)))
            {
                DestroyImmediate(obj.GetComponent<ProcTree>());
            }
            foreach (Branch obj in FindObjectsOfType(typeof(Branch)))
            {
                DestroyImmediate(obj.GetComponent<Branch>());
            }
            foreach (Crown obj in FindObjectsOfType(typeof(Crown)))
            {
                DestroyImmediate(obj.GetComponent<Crown>());
            }
        }

    }

}