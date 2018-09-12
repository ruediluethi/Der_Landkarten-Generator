using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeaAndForest : Helper {

    public float seaLevel;
    public float beginForest;
    public float endForest;

    public GameObject blackPlane;
    public ContourRenderer camRenderer;

    private int startSequenceCounter = 0;

	void Start () {
        dimension = infos.dimension;
        pointsAmount = infos.pointsAmount;
        maxHeight = infos.maxHeight;
        mountainCurve = infos.mountainCurve;
        shapeCurve = infos.shapeCurve;
	}

    void Update()
    {
        if (startSequenceCounter == 0)
        {
            generateMesh();
        }
        else if (startSequenceCounter == 1)
        {
            blackPlane.transform.position = new Vector3(
                blackPlane.transform.position.x,
                seaLevel,
                blackPlane.transform.position.z
            );
        }
        else if (startSequenceCounter == 2)
        {
            Texture2D seaTex = camRenderer.render();
            writeTextureToFile("sea.png", seaTex);
        }
        else if (startSequenceCounter == 3)
        {
            blackPlane.transform.position = new Vector3(
                blackPlane.transform.position.x,
                beginForest,
                blackPlane.transform.position.z
            );
        }
        else if (startSequenceCounter == 4)
        {
            Texture2D seaTex = camRenderer.render();
            writeTextureToFile("beginForest.png", seaTex);
        }
        else if (startSequenceCounter == 5)
        {
            blackPlane.transform.position = new Vector3(
                blackPlane.transform.position.x,
                endForest,
                blackPlane.transform.position.z
            );
        }
        else if (startSequenceCounter == 6)
        {
            Texture2D seaTex = camRenderer.render();
            writeTextureToFile("endForest.png", seaTex);
        }
        else if (startSequenceCounter == 7)
        {
            Random.InitState(3);
            PerlinNoise forestNoise = new PerlinNoise(10);

            for (int i = 0; i < pointsAmount; i++)
            {
                pointCloud[i].y = forestNoise.perlinNoise(pointCloud[i].x / dimension, pointCloud[i].z / dimension);
            }
            applyTerrainMesh();

            blackPlane.transform.position = new Vector3(
                blackPlane.transform.position.x,
                0f,
                blackPlane.transform.position.z
            );
        }
        else if (startSequenceCounter == 8)
        {
            Texture2D seaTex = camRenderer.render();
            writeTextureToFile("forestNoise.png", seaTex);
        }

        startSequenceCounter++;
    }
	
}
