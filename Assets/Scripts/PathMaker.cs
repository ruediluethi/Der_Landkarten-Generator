using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathMaker : Helper {

    public GameObject blackPlane;
    public ContourRenderer contourRenderer;

    private int startSequenceCounter = 0;

    public int pathAmount;
    public int randomState;

    private float walkerRandomDegrees = 0f;
    private Vector3 startPos = Vector3.zero;
    private Vector3 targetPos = Vector3.zero;
    private Vector3 currentVelocity = Vector3.zero;

    private float maxFall = 7f;
    private Color[] pathColors;

    public AnimationCurve pathShapeCurve;

	void Start () {
        dimension = infos.dimension;
        pointsAmount = infos.pointsAmount;
        maxHeight = infos.maxHeight;
        mountainCurve = infos.mountainCurve;
        shapeCurve = infos.shapeCurve;

        //pathShapeCurve = shapeCurve;
	}
	
	void Update () {

        if (startSequenceCounter == 0)
        {
            shapeCurve = pathShapeCurve;
            makeNoise();

            blackPlane.SetActive(false);

            System.DateTime now = System.DateTime.Now;

            //Random.InitState(now.Millisecond);
            Random.InitState(randomState);
        }

        for (int k = 0; k < pathAmount; k++)
        {

            if (startSequenceCounter == 1 + k * 2)
            {
                
                startPos = new Vector3(
                    (0.1f + Random.value * 0.8f) * dimension,
                    0f,
                    (0.1f + Random.value * 0.8f) * dimension
                );

                targetPos = new Vector3(
                    (0.1f + Random.value * 0.8f) * dimension,
                    0f,
                    (0.1f + Random.value * 0.8f) * dimension
                );
                //startPos = new Vector3(0.7f * dimension, 0f, 0.7f * dimension);
                //targetPos = new Vector3(0.3f * dimension, 0f, 0.3f * dimension);

                //Debug.Log(startPos);
                //Debug.Log(targetPos);

                this.transform.position = startPos;

                LineRenderer line = GetComponent<LineRenderer>();
                int pathLength = 100;
                line.positionCount = pathLength;
                for (int i = 0; i < pathLength; i++)
                {
                    Vector3 walk = walker(1f, 2f, 90f); // circleRadius, circleCenterDistance, randomDegreeAmplitude
                    Vector3 seek = targetPos - this.transform.position;

                    float nx = this.transform.position.x / dimension;
                    float ny = this.transform.position.z / dimension;
                    if (nx < 0.1f || nx > 0.9f || ny < 0.1f || ny > 0.9f)
                    {
                        line.positionCount = i - 1;
                        break;
                    }

                    Vector2 v = gradientAtPoint(nx, ny) * -1f;
                    Vector3 fall = new Vector3(v.x, 0f, v.y);
                    //seek.Normalize();

                    if (seek.magnitude > 1f)
                    {
                        seek.Normalize();
                    }


                    if (fall.magnitude > maxFall)
                    {
                        fall.Normalize();
                        fall *= maxFall;
                    }
                    fall = fall * (1f / maxFall);

                    currentVelocity = walk + seek * 0.3f + fall;
                    currentVelocity.Normalize();
                    //currentVelocity *= Time.deltaTime;
                    currentVelocity *= 0.1f;
                    this.transform.position += currentVelocity;
                    line.SetPosition(i, this.transform.position);
                    //Debug.DrawRay(this.transform.position, currentVelocity, Color.yellow);    
                }

            }
            else if (startSequenceCounter == 1 + k * 2 + 1)
            {
                Debug.Log("RENDER PATH: " + k + " ...");
                Texture2D sliceTex = contourRenderer.render();
                if (startSequenceCounter == 1 + 1)
                {
                    pathColors = new Color[sliceTex.width * sliceTex.height];
                    for (int j = 0; j < pathColors.Length; j++)
                    {
                        pathColors[j] = Color.white;
                    }
                }
                Color[] onePath = colorsFromTexture(sliceTex);
                pathColors = multiplyTextureColors(pathColors, onePath);
            }
        }

        if (startSequenceCounter == 1 + pathAmount * 2 + 2)
        {
            Debug.Log("PATH RENDERING DONE");

            Texture2D output = textureFromColors((int)Mathf.Sqrt(pathColors.Length), pathColors);
            writeTextureToFile("paths_"+randomState+".png", output);

            randomState++;
            startSequenceCounter = 0;
            return;
            //GetComponent<MeshRenderer>().material.SetTexture("_MainTex", output);
        }

        /*
        if (startSequenceCounter > 0)
        {
            int textureSize = 32;
            for (int x = 0; x < textureSize; x++)
            {
                for (int y = 0; y < textureSize; y++)
                {
                    float nx = (float)x / textureSize;
                    float ny = (float)y / textureSize;
                    float z = multiplePerlin(nx, ny);
                    Vector2 v = gradientAtPoint(nx, ny) * -1f;
                    Color c = Color.black;
                    if (v.magnitude > maxFall)
                    {
                        v.Normalize();
                        v *= maxFall;
                        c = Color.red;
                    }
                    //v.Normalize();
                    float vLength = dimension / textureSize / 2f / maxFall;
                    Debug.DrawRay(new Vector3(nx * dimension, z * maxHeight, ny * dimension), new Vector3(v.x, 0f, v.y) * vLength, c);
                }
            }
        }
        */

        startSequenceCounter++;
	}

    private void createPath()
    {
        
    }


    public Vector3 walker(float circleRadius, float circleCenterDistance, float randomDegreeAmplitude)
    {

        // Walker
        //Vector3  circleCenter = this.transform.position + this.transform.up * circleCenterDistance;
        Vector3 lookDirection = currentVelocity;
        lookDirection.Normalize();
        Vector3 circleCenter = this.transform.position + lookDirection * circleCenterDistance;

        walkerRandomDegrees = walkerRandomDegrees + (Random.value * randomDegreeAmplitude - randomDegreeAmplitude / 2);
        float deltaX = Mathf.Sin((walkerRandomDegrees) * Mathf.PI / 180) * circleRadius;
        float deltaZ = Mathf.Cos((walkerRandomDegrees) * Mathf.PI / 180) * circleRadius;

        Vector3 pointOnCircle = new Vector3(circleCenter.x + deltaX, 0f, circleCenter.z + deltaZ);

        Vector3 walkerVector = pointOnCircle - this.transform.position;
        //Debug.DrawRay(this.transform.position, walkerVector, Color.magenta);

        // Debug the Circle
        /*
        for (float degrees = 0f; degrees < 360f; degrees++){

            float firstDeltaX = Mathf.Sin(degrees * Mathf.PI / 180f) * circleRadius;
            float firstDeltaZ = Mathf.Cos(degrees * Mathf.PI / 180f) * circleRadius;

            Vector3 firstPointOnCircle = new Vector3(circleCenter.x + firstDeltaX, circleCenter.y, circleCenter.z + firstDeltaZ);

            float nextDeltaX = Mathf.Sin((degrees + 1f) * Mathf.PI / 180f) * circleRadius;
            float nextDeltaZ = Mathf.Cos((degrees + 1f) * Mathf.PI / 180f) * circleRadius;

            Vector3 nextPointOnCircle = new Vector3(circleCenter.x + nextDeltaX, circleCenter.y, circleCenter.z + nextDeltaZ);

            Debug.DrawLine(firstPointOnCircle, nextPointOnCircle, Color.green);
        }
        */

        walkerVector.Normalize();
        return walkerVector;

    }
}
