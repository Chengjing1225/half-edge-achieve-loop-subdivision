using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plankton;
using UnityEngine;


namespace Loopdivision
{
    struct faceInfo
    {
        public List<PlanktonVertex> pvList;
        public List<PlanktonHalfedge> phList;
    }
    class Loopdivisor
    {
        PlanktonMesh _pMesh;


        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        public Loopdivisor(PlanktonMesh pMesh)
        {
            _pMesh = pMesh;
        }

        
        /// <summary>
        /// 计算新增的边界点
        /// </summary>
        public void CaculateEdgeVertice()
        {
            int i = 0;
            foreach (var ph in _pMesh.Halfedges)
            {
                //PlanktonVertex pv = _pMesh.Vertices[_pMesh.Halfedges[pe.NextHalfedge].StartVertex];     //半边的对应角
                PlanktonHalfedge pph = _pMesh.Halfedges[_pMesh.Halfedges.GetPairHalfedge(i)];
                PlanktonVertex v0 = _pMesh.Vertices[ph.StartVertex];       //半边起始顶点
                PlanktonVertex v1 = _pMesh.Vertices[pph.StartVertex];      //半边终点
                PlanktonXYZ newPos;
                if (ph.AdjacentFace == -1 || pph.AdjacentFace == -1)
                {
                    newPos = (v0.ToXYZ() + v1.ToXYZ()) / 2;

                }
                else
                {
                    PlanktonVertex v2 = _pMesh.Vertices[_pMesh.Halfedges[ph.PrevHalfedge].StartVertex];
                    PlanktonVertex v3 = _pMesh.Vertices[_pMesh.Halfedges[pph.PrevHalfedge].StartVertex];
                    newPos = (v2.ToXYZ() + v3.ToXYZ()) * 0.125f + ((v0.ToXYZ() + v1.ToXYZ()) * 0.375f);
                }
                ph.newPos = newPos;
                i++;
            }

        }
        /// <summary>
        /// 计算原顶点的新位置
        /// </summary>
        public void  CaculateVertice()
        {
            int i = 0;
            foreach(var v in _pMesh.Vertices)
            {
                PlanktonHalfedge ph = _pMesh.Halfedges[v.OutgoingHalfedge];
                PlanktonHalfedge pqh = _pMesh.Halfedges[ph.PrevHalfedge];
                PlanktonXYZ newPos;
                if (ph.AdjacentFace == -1 && pqh.AdjacentFace == -1)
                {
                    PlanktonXYZ v0 = _pMesh.Vertices[_pMesh.Halfedges.GetVertices(v.OutgoingHalfedge)[1]].ToXYZ();
                    PlanktonXYZ v1 = _pMesh.Vertices[pqh.StartVertex].ToXYZ();
                    newPos = v.ToXYZ() * 0.75f + (v0 + v1) * 0.125f;

                }
                else
                {
                    List<PlanktonXYZ> va = new List<PlanktonXYZ>();
                    PlanktonHalfedge pph = _pMesh.Halfedges[_pMesh.Halfedges.GetPairHalfedge(v.OutgoingHalfedge)];
                    PlanktonXYZ v0 = _pMesh.Vertices[pph.StartVertex].ToXYZ();
                    va.Add(v0);
                    pph = _pMesh.Halfedges[_pMesh.Halfedges.GetPairHalfedge(pph.NextHalfedge)];
                    while (pph.AdjacentFace != -1 && va[0] != _pMesh.Vertices[pph.StartVertex].ToXYZ()) 
                    {
                        v0 = _pMesh.Vertices[pph.StartVertex].ToXYZ();
                        va.Add(v0);                        
                        pph = _pMesh.Halfedges[_pMesh.Halfedges.GetPairHalfedge(pph.NextHalfedge)];
                        
                    }
                    int num = va.Count;
                    float weight = (float)CalcVertexWeight(num);
                    newPos = v.ToXYZ() * (1 - num * weight);
                    foreach(var v1 in va)
                    {
                        newPos += v1 * weight;
                    }
                    
                }
                v.newPos = newPos;
                i++;
            }
        }

        /// <summary>
        /// 生成新的面
        /// </summary>
        void GenerateNewFace()
        {
            CaculateEdgeVertice();
            CaculateVertice();
            foreach(var face in _pMesh.Faces)
            {
                faceInfo pList = searchVertice(face);
                GenerateTri(pList.pvList[0].newPos, pList.phList[0].newPos, pList.phList[2].newPos);
                GenerateTri(pList.phList[0].newPos, pList.pvList[1].newPos, pList.phList[1].newPos);
                GenerateTri(pList.phList[0].newPos, pList.phList[1].newPos, pList.phList[2].newPos);
                GenerateTri(pList.phList[2].newPos, pList.phList[1].newPos, pList.pvList[2].newPos) ;
            }
        }

        /// <summary>
        /// 将新生成的物体转化为网格mesh
        /// </summary>
        /// <returns></returns>
        public Mesh GenerateToMesh()
        {
            GenerateNewFace();
            Mesh mesh = new Mesh();
            mesh.vertices = vertices.ToArray();
            mesh.triangles = triangles.ToArray();
            mesh.RecalculateBounds();
            mesh.RecalculateNormals();
            return mesh;

        }



        /// <summary>
        /// 寻找面片所对应的边角信息
        /// </summary>
        /// <param name="pFace">输入面片</param>
        /// <returns>返回对应的边角信息，其中pvList对应与角，phList则表示以角为出点的边</returns>
        faceInfo searchVertice(PlanktonFace pFace)
        {
            List<PlanktonVertex> pvList = new List<PlanktonVertex>();
            List<PlanktonHalfedge> phList = new List<PlanktonHalfedge>();
            PlanktonHalfedge ph = _pMesh.Halfedges[pFace.FirstHalfedge];
            int firstIndex = ph.StartVertex;
            PlanktonVertex firstVer = _pMesh.Vertices[ph.StartVertex];
            pvList.Add(firstVer);
            phList.Add(ph);
            ph = _pMesh.Halfedges[ph.NextHalfedge];
            while (ph.StartVertex != firstIndex)
            {
                pvList.Add(_pMesh.Vertices[ph.StartVertex]);
                phList.Add(ph);
                ph = _pMesh.Halfedges[ph.NextHalfedge];
                
            }
            faceInfo pList = new faceInfo();
            pList.pvList = pvList; 
            pList.phList = phList; 

            return pList;
        }
        /// <summary>
        ///加入Mesh
        /// </summary>
        /// <param name="v0">加入Mesh的顶点</param>
        void GenerateMesh(Vector3 v0)
        {
            int Index = vertices.FindIndex(v => v == v0);
            if (Index == -1)
            {
                vertices.Add(v0);
                triangles.Add(vertices.Count - 1);
            }
            else
            {
                triangles.Add(Index);
            }
                

        }
        /// <summary>
        /// 在Mesh中加入三角形
        /// </summary>
        /// <param name="v0">角0</param>
        /// <param name="v1">角1</param>
        /// <param name="v2">角2</param>
        void GenerateTri(PlanktonXYZ v0,PlanktonXYZ v1,PlanktonXYZ v2)
        {
            GenerateMesh(PlanktoVec(v0));
            GenerateMesh(PlanktoVec(v1));
            GenerateMesh(PlanktoVec(v2));
        }

        /// <summary>
        /// 将半边结构转化为unity识别的三维结构
        /// </summary>
        /// <param name="v0"></param>
        /// <returns></returns>
        Vector3 PlanktoVec(PlanktonXYZ v0)
        {
            return new Vector3(v0.X, v0.Y, v0.Z);
        }
        
        /// <summary>
        /// 计算原顶点的位移权重
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        double CalcVertexWeight(int num)
        {
            double onePerNum = 1.0d / num;
            double inner = 0.375d + 0.25d * Math.Cos(2.0d * Math.PI * onePerNum);
            double outer = onePerNum * (0.625f - inner * inner);
            return outer;
        }
    }
}


