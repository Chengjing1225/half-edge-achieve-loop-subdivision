using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using CommonTool;

public class Partdivisor : MonoBehaviour
{

    List<Vector3> hitPos = new List<Vector3>();

    public Material material;

    public int baseCount = 1;  //两个基础点之间的取点数量   值越大曲线就越平滑  但同时计算量也也越大
    public static List<Vector3> lsPoint = new List<Vector3>();


    LineRenderer lineRender;
    public float timeScale = 20;

    public MeshFilter meshFilter;
    void Start()
    {
        Init();
        meshFilter = gameObject.GetComponent<MeshFilter>();
        if (!gameObject.GetComponent<Collider>())
        {
            print(gameObject.GetComponent<Collider>());
            gameObject.AddComponent<MeshCollider>();
        }
        
    }

    float time = 1000;
    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Debug.DrawLine(ray.origin, Input.mousePosition,Color.green);
        RaycastHit hitInfo;
        if (Physics.Raycast(ray, out hitInfo))
        {
            Debug.Log("click object name is ---->" + hitInfo.collider.gameObject.name);

            if (Input.GetMouseButton(0) && hitInfo.collider.transform.gameObject == gameObject && time > timeScale * Time.deltaTime)
            {
                // Init();
                Vector3 hitCubePos = transform.InverseTransformPoint(hitInfo.point);
                hitPos.Add(hitCubePos);
                Debug.Log(transform.InverseTransformPoint(hitInfo.point));
               //Debug.Log(hitInfo.point);
                time = 0;
            }
           
        }
        if (Input.GetMouseButtonUp(0))
        {
            if (hitPos.Count > 2)
            {
                GetTrackPoint(hitPos.ToArray());
                FindReleInfo fri = new FindReleInfo(meshFilter.mesh, lsPoint, Vector3.Normalize(transform.InverseTransformPoint( Input.mousePosition) - ray.origin));
                List<int> tri = fri.SearchReleTri();
                for (int k = 1; k < tri.Count; k++)
                    print(tri[k]);
            }
            else
            {
                print("请重新划线");
            }
                
            
            //hitPos.Add(transform.InverseTransformPoint(hitInfo.point));

        }



    }

    private void Init()
    {
        if(gameObject.GetComponent<LineRenderer>() != null)
        {
            Destroy(GetComponent<LineRenderer>());
            print("null");
            
        }
        lineRender = gameObject.AddComponent<LineRenderer>();
        lineRender.GetComponent<LineRenderer>().material = material;
        lineRender.GetComponent<LineRenderer>().startWidth = 0.05f;
    }




    /// <summary>
    /// 根据设定节点 绘制指定的曲线
    /// </summary>
    /// <param name="hitPos">所有指定节点的信息</param>
    public void GetTrackPoint(Vector3[] hitPos)
    {
        Vector3[] vector3s = PathControlPointGenerator(hitPos);
        int SmoothAmount = hitPos.Length * baseCount;
        lineRender.positionCount = SmoothAmount;
        for (int i = 0; i < SmoothAmount; i++)
        {
            float pm = (float)i / SmoothAmount;
            Vector3 currPt = Interp(vector3s, pm);
            lineRender.SetPosition(i, currPt);
            if (lsPoint.Count == 0)
                lsPoint.Add(currPt);
            if(!Equals(lsPoint[lsPoint.Count - 1 ], currPt,0.1f) )
                lsPoint.Add(currPt);
        }

        
    }

    /// <summary>
    /// 计算所有节点以及控制点坐标
    /// </summary>
    /// <param name="path">所有节点的存储数组</param>
    /// <returns></returns>
    public Vector3[] PathControlPointGenerator(Vector3[] path)
    {
        Vector3[] suppliedPath;
        Vector3[] vector3s;

        suppliedPath = path;
        int offset = 2;
        vector3s = new Vector3[suppliedPath.Length + offset];
        Array.Copy(suppliedPath, 0, vector3s, 1, suppliedPath.Length);
        vector3s[0] = vector3s[1] + (vector3s[1] - vector3s[2]);
        vector3s[vector3s.Length - 1] = vector3s[vector3s.Length - 2] + (vector3s[vector3s.Length - 2] - vector3s[vector3s.Length - 3]);
        if (vector3s[1] == vector3s[vector3s.Length - 2])
        {
            Vector3[] tmpLoopSpline = new Vector3[vector3s.Length];
            Array.Copy(vector3s, tmpLoopSpline, vector3s.Length);
            tmpLoopSpline[0] = tmpLoopSpline[tmpLoopSpline.Length - 3];
            tmpLoopSpline[tmpLoopSpline.Length - 1] = tmpLoopSpline[2];
            vector3s = new Vector3[tmpLoopSpline.Length];
            Array.Copy(tmpLoopSpline, vector3s, tmpLoopSpline.Length);
        }
        return (vector3s);
    }


    /// <summary>
    /// 计算曲线的任意点的位置
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="t"></param>
    /// <returns></returns>
    public Vector3 Interp(Vector3[] pos, float t)
    {
        int length = pos.Length - 3;
        int currPt = Mathf.Min(Mathf.FloorToInt(t * length), length - 1);
        float u = t * (float)length - (float)currPt;
        Vector3 a = pos[currPt];
        Vector3 b = pos[currPt + 1];
        Vector3 c = pos[currPt + 2];
        Vector3 d = pos[currPt + 3];
        return .5f * (
           (-a + 3f * b - 3f * c + d) * (u * u * u)
           + (2f * a - 5f * b + 4f * c - d) * (u * u)
           + (-a + c) * u
           + 2f * b
       );
    }

    private bool Equals(Vector3 v0, Vector3 v1, float range = 0.001f)
    {
        return Mathf.Abs(v0.x - v1.x) < range && Mathf.Abs(v0.y - v1.y) < range && Mathf.Abs(v0.z - v1.z) < range;
    }
}
