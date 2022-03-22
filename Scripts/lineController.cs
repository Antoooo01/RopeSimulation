using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime;
using UnityEngine;

public class lineController : MonoBehaviour
{
    [SerializeField] private LineRenderer lr;
    [SerializeField] public GameObject linePoint;
    [SerializeField] private Transform[] points;

    private Vector2 lastPos;
    private Vector2 currentPos;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("started");
    }

    //Called by lineTesting Script
    public void SetUpLine(Transform[] points)
    {
        lr.positionCount = points.Length;
        this.points = points;
    }

    // Update is called once per frame
    void Update()
    {
        //Record the end-position
        currentPos = points[points.Length - 1].position;


        //Define the shape of the last rope segment
        Vector2 Segment = (points[points.Length - 1].position - points[points.Length - 2].position);
        float angle = Vector2.Angle(Vector2.right, Segment);

        //Check if a PolygonCollider is close
        RaycastHit2D[] hits = Physics2D.RaycastAll(points[points.Length - 2].position, Segment, Segment.magnitude);

        foreach (RaycastHit2D hit in hits)
        {
            if (hit == true)
            {
                //Save position for world-coordinates translations
                Vector2 localPos = hit.transform.position;

                //Define the PolygonCollider
                PolygonCollider2D polygon = hit.collider.GetComponent<PolygonCollider2D>();


                //Add all corner coordinates to a list
                List<Vector2> corners = new List<Vector2>();

                foreach (Vector2 points in polygon.points)
                {
                    Vector2 corner = localPos + points;
                    //Debug.Log(corner);
                    corners.Add(corner);
                }


                //for the last segment, check if need to unwrap
                if (points.Length > 2 && ShouldUnwrap(points[points.Length - 2].position, lastPos, currentPos, points[points.Length - 3].position))
                {
                    //Destroy second last linepoint, resize array, move endpoint back
                    GameObject.Destroy(points[points.Length - 2].gameObject);

                    points[points.Length - 2] = points[points.Length - 1];

                    Array.Resize<Transform>(ref points, points.Length - 1);
                }
                else
                {

                    //for each corner, check if need to wrap
                    foreach (Vector2 corner in corners)
                    {
                        if (ShouldWrap(points[points.Length - 2].position, lastPos, currentPos, corner) && corner != new Vector2(points[points.Length - 2].position.x, points[points.Length - 2].position.y))
                        {
                            //instantiate a transform, and place it second-last in points[]
                            GameObject point = Instantiate(linePoint);
                            point.transform.position = corner;

                            Array.Resize<Transform>(ref points, points.Length + 1);
                            Transform temp = points[points.Length - 2];
                            points[points.Length - 1] = temp;
                            points[points.Length - 2] = point.transform;
                        }
                    }
                }
            }
        }
        

        Vector3[] positions = new Vector3[points.Length];

        //set point positions
        for (int i = 0; i < points.Length; i++)
        {
            positions[i] = points[i].position;

        }
        lr.positionCount = positions.Length;
        lr.SetPositions(positions);


        //save the position for future comparison
        lastPos = currentPos;
    }

    public bool ShouldWrap(Vector2 anchorPoint, Vector2 lastPos, Vector2 currentPos, Vector2 point)
    {
        //create vectors to compare
        Vector2 oldVec = lastPos - anchorPoint;
        Vector2 newVec = currentPos - anchorPoint;
        Vector2 pointVec = point - anchorPoint;

        //compare the signs of the crossproducts
        float a = Mathf.Sign(Vector3.Cross(oldVec, pointVec).z);
        float b = Mathf.Sign(Vector3.Cross(pointVec, newVec).z);
        float c = Mathf.Sign(Vector3.Cross(oldVec, newVec).z);

        return (a == b && b == c && pointVec.magnitude < newVec.magnitude);
    }

    public bool ShouldUnwrap(Vector2 anchorPoint, Vector2 lastPos, Vector2 currentPos, Vector2 subAnchor)
    {
        //create vectors to compare
        Vector2 subSeg = anchorPoint - subAnchor;
        Vector2 oldVec = lastPos - anchorPoint;
        Vector2 newVec = currentPos - anchorPoint;

        //compare the signs of the crossproducts
        float a = Mathf.Sign(Vector3.Cross(subSeg, oldVec).z);
        float b = Mathf.Sign(Vector3.Cross(subSeg, newVec).z);

        return a != b;
    }
}
