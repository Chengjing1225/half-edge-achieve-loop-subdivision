using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Plankton;
using Loopdivision;
using UnityEngine.UI;
public class MeshConvert : MonoBehaviour
{
    public MeshFilter meshFilter;
    MeshConvertor meshConvertor = new MeshConvertor();
    PlanktonMesh pMesh;
    public Text text;
    void Start()
    {
        text.text = meshFilter.mesh.vertices.Length.ToString();
    }

    // Update is called once per frame
    void Update()
    {
      
    }



    public void  ButtonClick()
    {        
        pMesh = meshConvertor.convertMesh(meshFilter.mesh);
        Loopdivisor lp = new Loopdivisor(pMesh);
        lp.CaculateEdgeVertice();
        meshFilter.mesh = lp.GenerateToMesh();
        text.text = meshFilter.mesh.vertices.Length.ToString();
        
    }
}
