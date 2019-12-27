using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plankton;
using UnityEngine;


class MeshConvertor
{

    public MeshConvertor()
    {
    }

    public PlanktonMesh convertMesh(Mesh mesh)
    {
        Mesh _mesh = mesh;
        PlanktonMesh pMesh = new PlanktonMesh();
        List<PlanktonXYZ> vertices = new List<PlanktonXYZ>();
        int[] indexList = new int[_mesh.vertices.Length];
        for(int i = 0; i < _mesh.vertices.Length; i++)
        {

            PlanktonXYZ vertice = new PlanktonXYZ(_mesh.vertices[i].x, _mesh.vertices[i].y, _mesh.vertices[i].z);
            int index = vertices.FindIndex(v => v == vertice);
            if(index == -1)
            {
                vertices.Add(vertice);
                indexList[i] = (int)vertices.Count - 1;
            }
            else
            {
                indexList[i] = index;
            }
                

        }
        pMesh.Vertices.AddVertices(vertices);
        for (int i = 0; i < _mesh.triangles.Length; i = i + 3)
        {
            pMesh.Faces.AddFace(indexList[_mesh.triangles[i]], indexList[_mesh.triangles[i + 1]], indexList[_mesh.triangles[i + 2]]);
        }

        return pMesh;
    }

    public Mesh convertPlanktonMesh(PlanktonMesh pMesh)
    {
        Mesh _mesh = new Mesh();
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        foreach(var v in pMesh.Vertices)
        {
            Vector3 vertice = new Vector3(v.X, v.Y, v.Z);
            vertices.Add(vertice);
        }

        foreach(var t in pMesh.Faces)
        {
            PlanktonHalfedge ph = pMesh.Halfedges[t.FirstHalfedge];
            int firstVertice = ph.StartVertex;
            triangles.Add(firstVertice);
            ph = pMesh.Halfedges[ph.NextHalfedge];
            int nextVertice = ph.StartVertex;
            
            while( nextVertice != firstVertice)
            {
                triangles.Add(nextVertice);
                ph =pMesh.Halfedges[ph.NextHalfedge];
                nextVertice = ph.StartVertex;

            }            
        }
        _mesh.vertices = vertices.ToArray();
        _mesh.triangles = triangles.ToArray();
        _mesh.RecalculateNormals();
        _mesh.RecalculateTangents();
        _mesh.RecalculateBounds();

        return _mesh;
    }



}

