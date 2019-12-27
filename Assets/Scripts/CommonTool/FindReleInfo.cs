using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CommonTool
{

    struct INFO
    {
        public int tri;
        public int index;
    }
    class FindReleInfo
    {
        List<INFO> tri = new List<INFO>();
        public FindReleInfo(Mesh mesh, List<Vector3> points, Vector3 lightVec3)
        {
            _mesh = mesh;
            _points = points;
            _lightVec3 = lightVec3;
        }

        private Mesh _mesh { get; set; }
        private List<Vector3> _points { get; set; }
        private Vector3 _lightVec3 { get; set; }

        
        
        public List<int> SearchReleTri()
        {
            int c = 0;      
            for(int j = 0; j < _points.Count; j++ )
            {
                int k = -2;
                bool first = true;
                Vector3 point = _points[j];
                
                for (int i = c; i != c || first ; i = (i + 3) % _mesh.triangles.Length)
                {
                    first = false;
                    if (!(Vector3.Dot(_mesh.normals[_mesh.triangles[i]], _lightVec3) > 0))
                        continue;
                    //List<int> triIndex = new List<int>();
                    int u = -3;                    
                    if (tri.Count != 0 && k==-2)
                    {
                        for(int w=0;w < tri.Count;w += 3)
                        {
                            k = IsInclude(w, point);
                            if (k != -1)
                            {
                                u = k;
                                break;
                            }                                
                        }
                    }
                    else if (tri.Count == 0)
                        k = -1;
                    if ( k == -1)
                    { 
                        //if (tri.Select(t => t.index).ToList().FindIndex(s => s == i) != -1 )
                            //continue;
                        u = IsInclude(i, point);
                    }
                        
                    if (u == -1)
                        continue;
                    else if(u == -2)                        
                        break;
                    else
                    {
                        c = u;
                        break;
                    }
                }
            }

            return tri.Select(t => t.tri).ToList();
        }

        public int IsInclude(int i, Vector3 point)
        {
            Vector3 v0 = _mesh.vertices[_mesh.triangles[i]];
            Vector3 v1 = _mesh.vertices[_mesh.triangles[i + 1]];
            Vector3 v2 = _mesh.vertices[_mesh.triangles[i + 2]];

            Vector3 v01 = v1 - v0;
            Vector3 v20 = v0 - v2;
            Vector3 v12 = v2 - v1;

            Vector3 vp0 = v0 - point;
            Vector3 vp1 = v1 - point;
            Vector3 vp2 = v2 - point;


            bool isTrue = Vector3.Cross(v01, v20).normalized == Vector3.Cross(v01, vp0).normalized
                && Vector3.Cross(v12, v01).normalized == Vector3.Cross(v12, vp1).normalized
                && Vector3.Cross(v20, v12).normalized == Vector3.Cross(v20, vp2).normalized;
            if (isTrue)
            {
                int index = tri.FindIndex(z => z.tri == _mesh.triangles[i]);
                if(index != -1)
                {
                    if (tri[index + 1].tri == _mesh.triangles[i + 1] && tri[index + 2].tri == _mesh.triangles[i + 2])
                        return -2;               
                }
                for(int r = 0; r < 3; r++)
                {
                    INFO tri0 = new INFO();
                    tri0.tri = _mesh.triangles[i + r];
                    tri0.index = i;
                    tri.Add(tri0);
                }
                
                //tri.Add(_mesh.triangles[i]);
                //tri.Add(_mesh.triangles[i + 1]);
                //tri.Add(_mesh.triangles[i + 2]);
                //triIndex.Add(i);

                return i;
            }
            else return -1;
        }
    }
}
