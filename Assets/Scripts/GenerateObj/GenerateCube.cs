using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class GenerateCube : MonoBehaviour
{
    GameObject cube;
    MeshFilter meshFilter;
    MeshRenderer mr;
    public int edgeLength = 1;
    public float num = 1f;
    
    public MeshFilter cmf;
    public Material mt;
    public void Start()
    {
        //cube = new GameObject("cube");
       
        cube = gameObject;
        
        meshFilter = cube.AddComponent<MeshFilter>();
        mr = cube.AddComponent<MeshRenderer>();
        meshFilter.mesh = CreateCube();
        gameObject.GetComponent<Renderer>().material = mt;
        gameObject.AddComponent<MeshCollider>();
    }
    //void Update()
    //{
    //    print("update");

    //    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
    //    Debug.DrawLine(ray.origin, Input.mousePosition);
    //    RaycastHit hitInfo;
    //    if (Physics.Raycast(ray, out hitInfo))
    //    {            
    //        Debug.Log("click object name is ---->" + hitInfo.collider.gameObject.name);

    //        if (Input.GetMouseButton(0) && hitInfo.collider.transform.gameObject == gameObject)
    //        {
    //            // Init();
    //           // hitPos.Add(transform.InverseTransformPoint(hitInfo.point));
    //            Debug.Log(transform.InverseTransformPoint(hitInfo.point));
    //            //time = 0;


    //        }

    //    }


    //}


    Mesh CreateCube()
    {
        float halfEdge = edgeLength / 2f;
        float subEdge = edgeLength / num;
        List<Vector3> backVertices = new List<Vector3>();
        List<int> backTriangles = new List<int>();        
        List<Vector3> forwardVertices = new List<Vector3>();
        List<int> forwardTriangles = new List<int>();

        List<Vector3> rightVertices = new List<Vector3>();
        List<int> rightTriangles = new List<int>();
        List<Vector3> leftVertices = new List<Vector3>();
        List<int> leftTriangles = new List<int>();

        List<Vector3> bottomVertices = new List<Vector3>();
        List<int> bottomTriangles = new List<int>();
        List<Vector3> topVertices = new List<Vector3>();
        List<int> topTriangles = new List<int>();

        List<Vector2> backUV = new List<Vector2>();
        List<Vector2> frontUV = new List<Vector2>();

        Vector3 zStartV = new Vector3(-halfEdge, -halfEdge, 0);
        //Vector3 zStartV = new Vector3(0, 0, 0);
        Vector3 xStartV = new Vector3(0, -halfEdge, -halfEdge);
        Vector3 yStartV = new Vector3(-halfEdge, 0, -halfEdge);

        Vector2 uv = new Vector2(0, 0);
        float uvs = 1f / num;
        //后面
        for (int n = 0; n < num + 1; n++)
        {
            for (int m = 0; m < num + 1; m++)
            {
                int back = 1;
                Vector3 vb = new Vector3(zStartV.x + m * subEdge, zStartV.y + n * subEdge, zStartV.z + back  * halfEdge);
                
                backVertices.Add(vb);
                int forward = -1;
                Vector3 vf = new Vector3(zStartV.x + m * subEdge, zStartV.y + n * subEdge, zStartV.z + forward  * halfEdge);
                forwardVertices.Add(vf);
                int right = 1;
                Vector3 vr = new Vector3(xStartV.x + right * halfEdge, xStartV.y + n * subEdge, xStartV.z + m * subEdge);
                rightVertices.Add(vr);
                int left = -1;
                Vector3 vl = new Vector3(xStartV.x + left * halfEdge, xStartV.y + n * subEdge, xStartV.z + m * subEdge);
                leftVertices.Add(vl);
                int bottom = -1;
                Vector3 vbo = new Vector3(yStartV.x + n * subEdge, yStartV.y +  bottom * halfEdge, yStartV.z + m * subEdge);
                bottomVertices.Add(vbo);
                int top = 1;
                Vector3 vt = new Vector3(yStartV.x + n * subEdge, yStartV.y +  top * halfEdge, yStartV.z + m * subEdge);
                topVertices.Add(vt);
                Vector2 uvb = new Vector2(m * uvs, n * uvs);
                backUV.Add(uvb);
                Vector2 uvf = new Vector2(1 - m * uvs, n * uvs);
                frontUV.Add(uvf);

            }

        }

        for (int j = 0; j < num; j++)
        {
            for (int i = 0; i < num; i++)
            {
                backTriangles.Add((int)(i + j * (num + 1)));
                backTriangles.Add((int)(i + 1 + j * (num + 1)));
                backTriangles.Add((int)(i + (j + 1) * (num + 1)));
                backTriangles.Add((int)(i + (j + 1) * (num + 1)));
                backTriangles.Add((int)(i + 1 + j * (num + 1)));
                backTriangles.Add((int)(i + 1 + (j + 1) * (num + 1)));

                forwardTriangles.Add((int)(i + j * (num + 1)));
                forwardTriangles.Add((int)(i + (j + 1) * (num + 1)));
                forwardTriangles.Add((int)(i + 1 + j * (num + 1)));
                forwardTriangles.Add((int)(i + (j + 1) * (num + 1)));
                forwardTriangles.Add((int)(i + 1 + (j + 1) * (num + 1)));
                forwardTriangles.Add((int)(i + 1 + j * (num + 1)));

                rightTriangles.Add((int)(i + j * (num + 1)));
                rightTriangles.Add((int)(i + (j + 1) * (num + 1)));
                rightTriangles.Add((int)(i + 1 + j * (num + 1)));
                rightTriangles.Add((int)(i + (j + 1) * (num + 1)));
                rightTriangles.Add((int)(i + 1 + (j + 1) * (num + 1)));
                rightTriangles.Add((int)(i + 1 + j * (num + 1)));

                leftTriangles.Add((int)(i + j * (num + 1)));
                leftTriangles.Add((int)(i + 1 + j * (num + 1)));
                leftTriangles.Add((int)(i + (j + 1) * (num + 1)));
                leftTriangles.Add((int)(i + (j + 1) * (num + 1)));
                leftTriangles.Add((int)(i + 1 + j * (num + 1)));
                leftTriangles.Add((int)(i + 1 + (j + 1) * (num + 1)));

                bottomTriangles.Add((int)(i + j * (num + 1)));
                bottomTriangles.Add((int)(i + (j + 1) * (num + 1)));
                bottomTriangles.Add((int)(i + 1 + j * (num + 1)));
                bottomTriangles.Add((int)(i + (j + 1) * (num + 1)));
                bottomTriangles.Add((int)(i + 1 + (j + 1) * (num + 1)));
                bottomTriangles.Add((int)(i + 1 + j * (num + 1)));

                topTriangles.Add((int)(i + j * (num + 1)));
                topTriangles.Add((int)(i + 1 + j * (num + 1)));
                topTriangles.Add((int)(i + (j + 1) * (num + 1)));
                topTriangles.Add((int)(i + (j + 1) * (num + 1)));
                topTriangles.Add((int)(i + 1 + j * (num + 1)));
                topTriangles.Add((int)(i + 1 + (j + 1) * (num + 1)));
            }

        }

        List<Vector3> verList = new List<Vector3>();
        verList.AddRange(backVertices);
        verList.AddRange(forwardVertices);
        verList.AddRange(rightVertices);
        verList.AddRange(leftVertices);
        verList.AddRange(bottomVertices);
        verList.AddRange(topVertices);

        List<Vector2> uvList = new List<Vector2>();
        uvList.AddRange(backUV);
        uvList.AddRange(frontUV);
        uvList.AddRange(backUV);
        uvList.AddRange(frontUV);
        uvList.AddRange(backUV);
        uvList.AddRange(frontUV);


        List<int> triList = new List<int>();
        triList.AddRange(backTriangles);
        triList.AddRange(forwardTriangles.Select(t => t + backVertices.Count).ToList());
        triList.AddRange(rightTriangles.Select(t => t + 2 *backVertices.Count).ToList());
        triList.AddRange(leftTriangles.Select(t => t +  3 * backVertices.Count).ToList());
        triList.AddRange(bottomTriangles.Select(t => t + 4 * backVertices.Count).ToList());
        triList.AddRange(topTriangles.Select(t => t + 5 * backVertices.Count).ToList());


        Mesh mesh = new Mesh();
        mesh.vertices = verList.ToArray();
        mesh.triangles = triList.ToArray();
        
        mesh.uv = uvList.ToArray();

        Mesh meshf = cmf.mesh;
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
        mesh.RecalculateTangents();
        return mesh;
    }


}
