using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class visionCorners : MonoBehaviour
{
    Ray[] rays;
    public GameObject obstaclesParrent;
    int numberOfRays;
    Vector3[] rayHitPoints;
    public bool drawRays;

    public GameObject Line;
    GameObject linesFolder;

    Vector3 mousePos;

    public Material lightMaterial;

    void OnEnable()
    {
        Mesh mesh = GetComponent<MeshFilter>().mesh;

        GetComponent<MeshRenderer>().material = lightMaterial;

        linesFolder = GameObject.FindGameObjectWithTag("linesFolder");

        numberOfRays = obstaclesParrent.transform.childCount * 4;
        rays = new Ray[numberOfRays];
        rayHitPoints = new Vector3[numberOfRays];

        for (int i = 0; i < numberOfRays; i++)
        {
            Instantiate(Line, linesFolder.transform);
        }
    }

  

    void Update()
    {
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;

        int numberOfDrawnRays = 0;

        //we will be shooting rays only on the corners of walls, which allows us to minimise the number of rays used
        for (int i = 0; i < obstaclesParrent.transform.childCount; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                Vector3 rayDirection = obstaclesParrent.transform.GetChild(i).GetComponent<corners>().cornerCords[j] - mousePos;

                rays[numberOfDrawnRays] = new Ray(mousePos, rayDirection);

                if (Physics.Raycast(rays[numberOfDrawnRays], out RaycastHit info, Mathf.Infinity))
                {
                    if(drawRays)
                    {
                        linesFolder.transform.GetChild(numberOfDrawnRays).GetComponent<LineRenderer>().positionCount = 2;
                        linesFolder.transform.GetChild(numberOfDrawnRays).GetComponent<LineRenderer>().SetPosition(0, mousePos);

                    }

                    //if ray hits a corner of a wall
                    if (info.point == obstaclesParrent.transform.GetChild(i).GetComponent<corners>().cornerCords[j])
                    {
                        //use another ray from the corner following the same direction                        
                        if (Physics.Raycast(info.point, rayDirection, out RaycastHit info2))
                        {
                            //we need to determine if the ray is passing through a wall or passing the corners of a wall
                            //for that use a third ray starting from the hitpoint of the second ray but in opposite direction
                            if(Physics.Raycast(info2.point, -rayDirection, out RaycastHit info3))
                            {
                                //if the third ray hits the same spot in which the second ray begins, it means that the second ray is passing the corner of a wall and not going through the wall
                                if(info.point == info3.point)
                                {
                                    if (drawRays)
                                        linesFolder.transform.GetChild(numberOfDrawnRays).GetComponent<LineRenderer>().SetPosition(1, info2.point);
                                    rayHitPoints[numberOfDrawnRays] = info2.point;
                                }
                                else
                                {
                                    if (drawRays)
                                        linesFolder.transform.GetChild(numberOfDrawnRays).GetComponent<LineRenderer>().SetPosition(1, info.point);
                                    rayHitPoints[numberOfDrawnRays] = info.point;
                                }
                            }
                        }
                        else
                        {
                            if (drawRays)
                                linesFolder.transform.GetChild(numberOfDrawnRays).GetComponent<LineRenderer>().SetPosition(1, info.point);
                            rayHitPoints[numberOfDrawnRays] = info.point;
                        }
                    }
                    else
                    {
                        if (drawRays)
                            linesFolder.transform.GetChild(numberOfDrawnRays).GetComponent<LineRenderer>().SetPosition(1, info.point);
                        rayHitPoints[numberOfDrawnRays] = info.point;
                    }
                }
                numberOfDrawnRays += 1;
            }
        }

        //hit points have to be sorted so we can properly fill in the spaces in between with triangles
        sortRayHitPoints(numberOfDrawnRays);

        if(!drawRays)
        {
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit info4) && !info4.collider.CompareTag("wall"))
                fillInLines(numberOfDrawnRays);
            else
                GetComponent<MeshFilter>().mesh.Clear();

        }

        //hides the lines representing the rays
        if (!drawRays)
        {
            for (int i = 0; i < linesFolder.transform.childCount; i++)
            {
                linesFolder.transform.GetChild(i).gameObject.GetComponent<LineRenderer>().positionCount = 0;
            }
        }


    }

    private void sortRayHitPoints(int numberOfDrawnRays)
    {
        for (int i = 0; i < numberOfDrawnRays - 1; i++)
        {
            for (int j = 0; j < numberOfDrawnRays - i - 1; j++)
            {
                if (AngleBetweenVector3(mousePos, rayHitPoints[j]) > AngleBetweenVector3(mousePos, rayHitPoints[j + 1]))
                {
                    Vector3 temp = rayHitPoints[j];
                    rayHitPoints[j] = rayHitPoints[j + 1];
                    rayHitPoints[j + 1] = temp;
                }
            }
        }
    }

    private void fillInLines( int numberOfDrawnRays)
    {
        //Makes triangles going from center to two neighboring points
        int vertexCount = numberOfDrawnRays + 1;
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

    private float AngleBetweenVector3(Vector3 vec1, Vector3 vec2)
    {
        //This method for calculating angle between vectors uses priciples based on answer of the user 'fermmmm' on the answers.Unity portal

        Vector2 diference = new Vector2(vec2.x, vec2.y) - new Vector2(vec1.x, vec1.y);
        float sign = (vec2.y < vec1.y) ? -1.0f : 1.0f;
        return Vector2.Angle(Vector2.right, diference) * sign;
    }
}