using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Helper : MonoBehaviour {

    public TerrainInfos infos;

    [HideInInspector]
    public float dimension;
    [HideInInspector]
    public int pointsAmount;
    [HideInInspector]
    public float maxHeight;

    [HideInInspector]
    public AnimationCurve mountainCurve;
    [HideInInspector]
    public AnimationCurve shapeCurve;

    [HideInInspector]
    public Vector3[] pointCloud;
    private List<Triangle> triangles;

    private PerlinNoise rough;
    private PerlinNoise medium;

    /* IO */
    public void writeTextureToFile(string filePath, Texture2D output)
    {
        filePath = Application.dataPath + "/output/" + filePath;
        Debug.Log("SAVE TO: " + filePath);
        byte[] bytes = output.EncodeToPNG();
        File.WriteAllBytes(filePath, bytes);
    }

    /* MESH */

    public void makeNoise()
    {
        Debug.Log("GENERATE PERLIN NOISE...");
        Random.InitState(1);
        rough = new PerlinNoise(2);
        medium = new PerlinNoise(8);
    }

    public void generateMesh()
    {
        makeNoise();

        Debug.Log("INIT " + pointsAmount + " RANDOM POINTS...");
        Random.InitState(42); // init random again to get controll about the mesh generation
        pointCloud = new Vector3[pointsAmount];
        for (int i = 0; i < pointsAmount; i++)
        {
            float x = Random.value;
            float y = Random.value;

            Vector3 randomVector = new Vector3(
                x * dimension,
                0f,
                y * dimension
            );

            pointCloud[i] = randomVector;
        }

        Debug.Log("TRIANGULATE...");
        triangles = Triangulator.Triangulate(pointCloud);

        Debug.Log("APPLY MESH...");
        for (int i = 0; i < pointsAmount; i++)
        {
            pointCloud[i].y = multiplePerlin(pointCloud[i].x / dimension, pointCloud[i].z / dimension) * maxHeight;
        }
        applyTerrainMesh();
    }

    public void applyTerrainMesh()
    {
        Vector3[] vertices = new Vector3[triangles.Count * 3];
        Vector2[] uv = new Vector2[triangles.Count * 3];
        int[] triangleList = new int[triangles.Count * 3];



        int t = 0;
        foreach (Triangle triangle in triangles)
        {
            Vector3 p1 = pointCloud[triangle.p1];
            Vector3 p2 = pointCloud[triangle.p2];
            Vector3 p3 = pointCloud[triangle.p3];

            //p1.y = multiplePerlin(p1.x / dimension, p1.z / dimension) * maxHeight;
            //p2.y = multiplePerlin(p2.x / dimension, p2.z / dimension) * maxHeight;
            //p3.y = multiplePerlin(p3.x / dimension, p3.z / dimension) * maxHeight;

            vertices[t] = p1;
            vertices[t + 1] = p2;
            vertices[t + 2] = p3;

            uv[t] = new Vector2(p1.x / dimension, p1.z / dimension);
            uv[t + 1] = new Vector2(p2.x / dimension, p2.z / dimension);
            uv[t + 2] = new Vector2(p3.x / dimension, p3.z / dimension);

            triangleList[t] = t;
            triangleList[t + 1] = t + 1;
            triangleList[t + 2] = t + 2;

            t += 3;
        }

        Mesh terrainMesh = GetComponent<MeshFilter>().mesh;
        terrainMesh.Clear();
        terrainMesh.vertices = vertices;
        terrainMesh.uv = uv;
        terrainMesh.triangles = triangleList;
        terrainMesh.RecalculateNormals();
    }

    /* PERLIN NOISE */

    public float multiplePerlin(float x, float y)
    {
        float v = Mathf.Sqrt(Mathf.Pow(x - 0.5f, 2f) + Mathf.Pow(y - 0.5f, 2f)); // distance to center
        v = v / 0.5f; // normalize distance to center
        v = 1f - v;
        v = mountainCurve.Evaluate(v);

        v += rough.perlinNoise(x, y) / 2f + 0.5f;
        v += medium.perlinNoise(x, y) / 2f + 0.5f;

        return shapeCurve.Evaluate(v / 3f);
    }

    public Vector2 gradientAtPoint(float x, float y)
    {
        float h = 0.0001f;
        float dx = (multiplePerlin(x + h, y) - multiplePerlin(x, y)) / h;
        float dy = (multiplePerlin(x, y + h) - multiplePerlin(x, y)) / h;

        return new Vector2(dx, dy);
    }

    /* TEXTURE */

    public float[] getPerlinTexture(int size)
    {
        float[] values = new float[size * size];

        int i = 0;
        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                i = x * size + y;

                float nx = (float)x / (float)size;
                float ny = (float)y / (float)size;
                values[i] = multiplePerlin(ny, nx);
            }
        }

        return values;
    }

    public Color[] colorsFromTexture(Texture2D texture)
    {
        Color[] colors = new Color[texture.width * texture.height];

        for (int x = 0; x < texture.width; x++)
        {
            for (int y = 0; y < texture.height; y++)
            {
                int i = y * texture.height + x;
                colors[i] = texture.GetPixel(x, y);
            }
        }

        return colors;
    }

    public Texture2D textureFromColors(int size, Color[] colors)
    {
        Texture2D landscapePaint = new Texture2D(size, size);

        landscapePaint.SetPixels(colors);
        landscapePaint.Apply();

        return landscapePaint;
    }

    public Color[] multiplyTextureColors(Color[] a, Color[] b)
    {
        Color[] colors = new Color[a.Length];
        for (int i = 0; i < a.Length; i++)
        {
            colors[i] = a[i] * b[i];
        }
        return colors;
    }
}
