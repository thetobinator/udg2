using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Triangulation;
using System.Linq;

namespace MeshOperation
{
    public class MeshOps
    {

        public static void AddQuad(List<Vector3> vertexList, List<int> triangleList)
        {
            //triangle1
            triangleList.Add(vertexList.Count - 4);
            triangleList.Add(vertexList.Count - 3);
            triangleList.Add(vertexList.Count - 2);
            //triangle2
            triangleList.Add(vertexList.Count - 1);
            triangleList.Add(vertexList.Count - 2);
            triangleList.Add(vertexList.Count - 3);
        }

        public static void AddTriangle(List<Vector3> vertexList, List<int> triangleList)
        {
            triangleList.Add(vertexList.Count - 3);
            triangleList.Add(vertexList.Count - 2);
            triangleList.Add(vertexList.Count - 1);
        }

        public static void CreateParall(Vector3 position, float offsetX, float offsetY, float offsetZ, List<Vector3> vertexList, List<int> triangleList, List<Vector2> uvList)
        {
            //floor
            vertexList.Add(new Vector3(position.x - offsetX / 2, position.y, position.z - offsetZ / 2)); //top left 
            vertexList.Add(new Vector3(position.x + offsetX / 2, position.y, position.z - offsetZ / 2)); //top right
            vertexList.Add(new Vector3(position.x - offsetX / 2, position.y, position.z + offsetZ / 2)); //bot left
            vertexList.Add(new Vector3(position.x + offsetX / 2, position.y, position.z + offsetZ / 2)); //bot right 

            AddQuad(vertexList, triangleList);

            uvList.Add(new Vector2(position.x - offsetX / 2, position.y)); //top left
            uvList.Add(new Vector2(position.x + offsetX / 2, position.y)); //top right
            uvList.Add(new Vector2(position.x - offsetX / 2, position.y)); //bot left
            uvList.Add(new Vector2(position.x + offsetX / 2, position.y)); //bot right

            //front
            vertexList.Add(new Vector3(position.x - offsetX / 2, position.y + offsetY, position.z - offsetZ / 2)); //top left
            vertexList.Add(new Vector3(position.x + offsetX / 2, position.y + offsetY, position.z - offsetZ / 2)); //top right
            vertexList.Add(new Vector3(position.x - offsetX / 2, position.y, position.z - offsetZ / 2)); //bot left 
            vertexList.Add(new Vector3(position.x + offsetX / 2, position.y, position.z - offsetZ / 2)); //bot right

            AddQuad(vertexList, triangleList);

            uvList.Add(new Vector2(position.x - offsetX / 2, position.y + offsetY)); //top left
            uvList.Add(new Vector2(position.x + offsetX / 2, position.y + offsetY)); //top right
            uvList.Add(new Vector2(position.x - offsetX / 2, position.y)); //bot left
            uvList.Add(new Vector2(position.x + offsetX / 2, position.y)); //bot right

            //back
            vertexList.Add(new Vector3(position.x + offsetX / 2, position.y + offsetY, position.z + offsetZ / 2)); //top left
            vertexList.Add(new Vector3(position.x - offsetX / 2, position.y + offsetY, position.z + offsetZ / 2)); //top right
            vertexList.Add(new Vector3(position.x + offsetX / 2, position.y, position.z + offsetZ / 2)); //bot left 
            vertexList.Add(new Vector3(position.x - offsetX / 2, position.y, position.z + offsetZ / 2)); //bot right

            AddQuad(vertexList, triangleList);

            uvList.Add(new Vector2(position.x + offsetX / 2, position.y + offsetY)); //top left
            uvList.Add(new Vector2(position.x - offsetX / 2, position.y + offsetY)); //top right
            uvList.Add(new Vector2(position.x + offsetX / 2, position.y)); //bot left
            uvList.Add(new Vector2(position.x - offsetX / 2, position.y)); //bot right

            //left wall
            vertexList.Add(new Vector3(position.x - offsetX / 2, position.y + offsetY, position.z + offsetZ / 2)); //top left
            vertexList.Add(new Vector3(position.x - offsetX / 2, position.y + offsetY, position.z - offsetZ / 2)); //top right
            vertexList.Add(new Vector3(position.x - offsetX / 2, position.y, position.z + offsetZ / 2)); //bot left
            vertexList.Add(new Vector3(position.x - offsetX / 2, position.y, position.z - offsetZ / 2)); //bot right 

            AddQuad(vertexList, triangleList);

            uvList.Add(new Vector2(position.x - offsetX / 2, position.y + offsetY)); //top left
            uvList.Add(new Vector2(position.x - offsetX / 2, position.y + offsetY)); //top right
            uvList.Add(new Vector2(position.x - offsetX / 2, position.y)); //bot left
            uvList.Add(new Vector2(position.x - offsetX / 2, position.y)); //bot right

            //right wall
            vertexList.Add(new Vector3(position.x + offsetX / 2, position.y + offsetY, position.z - offsetZ / 2)); //top left
            vertexList.Add(new Vector3(position.x + offsetX / 2, position.y + offsetY, position.z + offsetZ / 2)); //top right
            vertexList.Add(new Vector3(position.x + offsetX / 2, position.y, position.z - offsetZ / 2)); //bot left
            vertexList.Add(new Vector3(position.x + offsetX / 2, position.y, position.z + offsetZ / 2)); //bot right

            AddQuad(vertexList, triangleList);

            uvList.Add(new Vector2(position.x + offsetX / 2, position.y + offsetY)); //top left
            uvList.Add(new Vector2(position.x + offsetX / 2, position.y + offsetY)); //top right
            uvList.Add(new Vector2(position.x + offsetX / 2, position.y)); //bot left
            uvList.Add(new Vector2(position.x + offsetX / 2, position.y)); //bot right

            //top wall
            vertexList.Add(new Vector3(position.x - offsetX / 2, position.y + offsetY, position.z + offsetZ / 2)); //top left
            vertexList.Add(new Vector3(position.x + offsetX / 2, position.y + offsetY, position.z + offsetZ / 2)); //top right
            vertexList.Add(new Vector3(position.x - offsetX / 2, position.y + offsetY, position.z - offsetZ / 2)); //bot left
            vertexList.Add(new Vector3(position.x + offsetX / 2, position.y + offsetY, position.z - offsetZ / 2)); //bot right

            AddQuad(vertexList, triangleList);

            uvList.Add(new Vector2(position.x - offsetX / 2, position.y + offsetY)); //top left
            uvList.Add(new Vector2(position.x + offsetX / 2, position.y + offsetY)); //top right
            uvList.Add(new Vector2(position.x - offsetX / 2, position.y + offsetY)); //bot left
            uvList.Add(new Vector2(position.x + offsetX / 2, position.y + offsetY)); //bot right
        }

        public static Mesh CreateSolid(Mesh msh, float height)
        {
            List<Vector3> vertexList = new List<Vector3>(); // Vertex list
            List<Vector2> uvList = new List<Vector2>(); // UV list
            List<int> triangleList = new List<int>(); // Triangle list

            for (int i = 0; i < msh.vertices.Length - 1; i++)
            {
                vertexList.Add(new Vector3(msh.vertices[i].x, msh.vertices[i].y + 0.2f, msh.vertices[i].z));
                vertexList.Add(new Vector3(msh.vertices[i + 1].x, msh.vertices[i + 1].y + 0.2f, msh.vertices[i + 1].z));
                vertexList.Add(msh.vertices[i]);
                vertexList.Add(msh.vertices[i + 1]);

                AddQuad(vertexList, triangleList);

                uvList.Add(new Vector2(msh.vertices[i].x, msh.vertices[i].y + height)); //top left
                uvList.Add(new Vector2(msh.vertices[i + 1].x, msh.vertices[i + 1].y + height)); //top right
                uvList.Add(new Vector2(msh.vertices[i].x, msh.vertices[i].y)); //bot left
                uvList.Add(new Vector2(msh.vertices[i + 1].x, msh.vertices[i + 1].y)); //bot right
            }

            Mesh newMesh = new Mesh();
            newMesh.vertices = vertexList.ToArray();
            newMesh.triangles = triangleList.ToArray();
            newMesh.uv = uvList.ToArray();
            newMesh.RecalculateNormals();
            newMesh.RecalculateBounds();

            return newMesh;
        }

        public static Mesh Triangulate(List<Vector2> side1, List<Vector2> side2)
        {
            side1.Reverse();
            List<Vector2> vertices2D = new List<Vector2>();
            vertices2D = side2.Concat(side1).ToList();

            // Use the triangulator to get indices for creating triangles
            Triangulator tr = new Triangulator(vertices2D.ToArray());
            int[] indices = tr.Triangulate();

            // Create the Vector3 vertices
            Vector3[] vertices = new Vector3[vertices2D.Count];
            for (int k = 0; k < vertices.Length; k++)
            {
                vertices[k] = new Vector3(vertices2D[k].x, 0, vertices2D[k].y);
            }
            // Create the mesh
            Mesh msh = new Mesh();
            msh.vertices = vertices;
            msh.triangles = indices;
            msh.RecalculateNormals();
            msh.RecalculateBounds();

            if (msh.normals[0] != Vector3.up)
            {
                //reverse
                vertices2D.Reverse();

                // Use the triangulator to get indices for creating triangles
                tr = new Triangulator(vertices2D.ToArray());
                indices = tr.Triangulate();

                // Create the Vector3 vertices
                vertices = new Vector3[vertices2D.Count];
                for (int k = 0; k < vertices.Length; k++)
                {
                    vertices[k] = new Vector3(vertices2D[k].x, 0, vertices2D[k].y);
                }
                // Create the mesh
                Mesh msh2 = new Mesh();
                msh2.vertices = vertices;
                msh2.triangles = indices;
                msh2.RecalculateNormals();
                msh2.RecalculateBounds();

            }

            return msh;
        }

    }

}