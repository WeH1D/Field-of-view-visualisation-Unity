using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class vision : MonoBehaviour
{
    Ray[] rays;
    public int numberOfRays = 200;
    float angleIncrements;

    Vector3 mousePos;

    GameObject linesFolder;
    public GameObject Line;

    public bool drawRays;
    Vector3[] rayHitPoints;

    public Material lightMaterial;

    void OnEnable()
    {
        Mesh mesh = GetComponent<MeshFilter>().mesh;
        GetComponent<MeshRenderer>().material = lightMaterial;

        rays = new Ray[numberOfRays];
        angleIncrements = 360f / (numberOfRays);

        linesFolder = GameObject.FindGameObjectWithTag("linesFolder");

        for (int i = 0; i < numberOfRays; i++)
        {
            Instantiate(Line, linesFolder.transform);
        }
    }

    void Update()
    {
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;

        rayHitPoints = new Vector3[numberOfRays];

        // cast rays from mouse position outwards, 
        // angle of rays depends on how many rays are being casted, 
        // rays always cover 360 degres around mouse position
        for (int i = 0; i < numberOfRays; i++)
        {
            rays[i] = new Ray(mousePos, anglesToDirection(i * angleIncrements));

            if (Physics.Raycast(rays[i], out RaycastHit info, Mathf.Infinity))
            {
                if(drawRays)
                {
                    //make lines that visualy represent casted rays
                    linesFolder.transform.GetChild(i).GetComponent<LineRenderer>().positionCount = 2;
                    linesFolder.transform.GetChild(i).GetComponent<LineRenderer>().SetPosition(0, mousePos);
                    linesFolder.transform.GetChild(i).GetComponent<LineRenderer>().SetPosition(1, info.point);
                }
                rayHitPoints[i] = info.point;
            }
        }

        //fill in between lines with triangles
        if(!drawRays)
        {
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit info2) && !info2.collider.CompareTag("wall"))
                fillINLines();
            else
                GetComponent<MeshFilter>().mesh.Clear();
        }

        //hides the lines representing the rays
        if(!drawRays)
        {
            for (int i = 0; i < linesFolder.transform.childCount; i++)
            {
                linesFolder.transform.GetChild(i).gameObject.GetComponent<LineRenderer>().positionCount = 0;
            }
        }
   
    }

    private void fillINLines()
    {
        //Makes triangles going from center to two neighboring points
        int vertexCount = numberOfRays + 1;
        Vector3[] vertices = new Vector3[vertexCount];
        int[] triangles = new int[vertexCount * 3 + 3];

        vertices[0] = mousePos;

        for (int i = 0; i < vertexCount - 1; i++)
        {
            vertices[i + 1] = rayHitPoints[i];

            if (i < vertexCount - 2)
            {
                triangles[i * 3] = 0;
                triangles[i * 3 + 1] = i + 1;
                triangles[i * 3 + 2] = i + 2;
            }
        }

        // last triangle which connects the last vertex with the first has to be done manually
        triangles[triangles.Length - 1 - 2] = 0;
        triangles[triangles.Length - 1 - 1] = vertexCount - 1;
        triangles[triangles.Length - 1] = 1;

        GetComponent<MeshFilter>().mesh.vertices = vertices;
        GetComponent<MeshFilter>().mesh.triangles = triangles;
    }

    Vector3 anglesToDirection(float angle)
    {
        //returns a vector3 based on given angle, uses basic trigonometry to determine this
        return new Vector3(Mathf.Sin(angle * Mathf.Deg2Rad), Mathf.Cos(angle * Mathf.Deg2Rad), 0);
    }
}
