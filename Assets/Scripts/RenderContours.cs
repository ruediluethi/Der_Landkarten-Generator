using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RenderContours : Helper {

    public int contoursAmount;

    public GameObject blackPlane;
    public ContourRenderer contourRenderer;

    private int startSequenceCounter = 0;
    private Color[] contourColors;

	void Start () {
        dimension = infos.dimension;
        pointsAmount = infos.pointsAmount;
        maxHeight = infos.maxHeight;
        mountainCurve = infos.mountainCurve;
        shapeCurve = infos.shapeCurve;
	}
	
	void Update () {
        if (startSequenceCounter == 0){
            generateMesh();
        }

        for (int i = 0; i < contoursAmount; i++)
        {
            if (startSequenceCounter == 1 + i * 2)
            {
                blackPlane.transform.position = new Vector3(
                    blackPlane.transform.position.x,
                    (maxHeight / contoursAmount) * (i + 1),
                    blackPlane.transform.position.z
                );
            }
            else if (startSequenceCounter == 1 + i * 2 + 1)
            {
                Debug.Log("RENDER CONTOUR: " + i + " ...");
                Texture2D sliceTex = contourRenderer.render();
                if (startSequenceCounter == 1 + 1)
                {
                    contourColors = new Color[sliceTex.width * sliceTex.height];
                    for (int j = 0; j < contourColors.Length; j++)
                    {
                        contourColors[j] = Color.white;
                    }
                }
                Color[] oneSlice = contourFinder(sliceTex);
                contourColors = multiplyTextureColors(contourColors, oneSlice);
            }
        }
        if (startSequenceCounter == 1 + contoursAmount * 2 + 2)
        {
            Debug.Log("CONTOUR RENDERING DONE");

            Texture2D output = textureFromColors((int)Mathf.Sqrt(contourColors.Length), contourColors);
            writeTextureToFile("contours.png", output);

            GetComponent<MeshRenderer>().material.SetTexture("_MainTex", output);
            blackPlane.SetActive(false);

        }

        startSequenceCounter++;
	}

    private Color[] contourFinder(Texture2D texture)
    {
        Color[] colors = new Color[texture.width * texture.height];

        for (int x = 0; x < texture.width; x++)
        {
            for (int y = 0; y < texture.height; y++)
            {
                int i = y * texture.height + x;

                List<float> neighbourValues = new List<float>();

                float r = texture.GetPixel(x, y).r;

                neighbourValues.Add(Mathf.Abs(texture.GetPixel(x - 1, y).r - r)); // left
                neighbourValues.Add(Mathf.Abs(texture.GetPixel(x + 1, y).r - r)); // right
                neighbourValues.Add(Mathf.Abs(texture.GetPixel(x, y - 1).r - r)); // up
                neighbourValues.Add(Mathf.Abs(texture.GetPixel(x, y + 1).r - r)); // down

                neighbourValues.Add(Mathf.Abs(texture.GetPixel(x - 1, y - 1).r - r)); // upper left
                neighbourValues.Add(Mathf.Abs(texture.GetPixel(x + 1, y - 1).r - r)); // upper right
                neighbourValues.Add(Mathf.Abs(texture.GetPixel(x - 1, y + 1).r - r)); // lower left
                neighbourValues.Add(Mathf.Abs(texture.GetPixel(x + 1, y + 1).r - r)); // lower left

                float newR = 0f;
                foreach (float v in neighbourValues)
                {
                    newR += v;
                }

                newR = 1f - (newR / neighbourValues.Count);

                colors[i] = new Color(newR, newR, newR);
            }
        }

        return colors;
    }
}
