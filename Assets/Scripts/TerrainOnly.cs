using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainOnly : Helper {

    private int startSequenceCounter = 0;

    public GameObject blackPlane;
    public ContourRenderer camRenderer;

    void Start()
    {
        dimension = infos.dimension;
        pointsAmount = infos.pointsAmount;
        maxHeight = infos.maxHeight;
        mountainCurve = infos.mountainCurve;
        shapeCurve = infos.shapeCurve;
    }

	// Update is called once per frame
	void Update () {
        if (startSequenceCounter == 1)
        {
            generateMesh();
            blackPlane.SetActive(false);
        }
        else if (startSequenceCounter == 2)
        {
            Texture2D mapTex = camRenderer.render();
            writeTextureToFile("map.png", mapTex);
        }

        startSequenceCounter++;
	}
}
