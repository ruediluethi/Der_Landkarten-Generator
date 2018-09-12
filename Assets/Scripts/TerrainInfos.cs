using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainInfos : MonoBehaviour {

    public int textureSize;
    public int pointsAmount;
    public float dimension;
    public float maxHeight;
    //public int riversAmount;
    //public int contoursAmount;

    public AnimationCurve mountainCurve;
    public AnimationCurve shapeCurve;
    public Gradient paint;
    //public float[] contourSizes;

    //public Material unlitMaterial;
    //private Material terrainMaterial;
    //public ContourRenderer contourRenderer;
    //public GameObject blackPlane;
    //public GameObject RiverBlueprint;

    private Texture2D terrain;
    private PerlinNoise rough;
    private PerlinNoise medium;

    private Vector3[] pointCloud;
    private List<Triangle> triangles;
    private List<int>[] pointNeighbours;

    private Color[] contourColors;

    private int startSequenceCounter = 0;
    private float timer = 0f;
    private long lastTime = 0;

    /*
	private void Start()
	{
        terrainMaterial = GetComponent<MeshRenderer>().material;
	}

	void Update()
    {
        System.DateTime now = System.DateTime.Now;
        if (startSequenceCounter < 100)
        {
            Debug.Log(startSequenceCounter+": t="+((now.Ticks - lastTime)/10000f)+"ms");
            startSequenceCounter++;
        }
        lastTime = now.Ticks;

        if (startSequenceCounter == 1)
        {
            Debug.Log("GENERATE PERLIN NOISE...");
            Random.InitState(1);
            rough = new PerlinNoise(2);
            medium = new PerlinNoise(8);
        }
        else if (startSequenceCounter == 2)
        {
            Debug.Log("INIT " + pointsAmount + " RANDOM POINTS...");
            Random.InitState(42); // init random again to get controll about the mesh generation
            pointCloud = new Vector3[pointsAmount];
            pointCloud[0] = new Vector3(dimension / 2f, 0f, dimension / 2f);
            // first add some points as river sources near the center
            for (int i = 0; i < riversAmount; i++)
            {
                float x = Random.value * 2f - 1f;
                float y = Random.value * 2f - 1f;

                Vector3 randomVector = new Vector3(
                    dimension / 2f + x * dimension * 0.2f,
                    0f,
                    dimension / 2f + y * dimension * 0.2f
                );

                pointCloud[i] = randomVector;
            }
            for (int i = riversAmount; i < pointsAmount; i++)
            {
                //float r = Random.value * 0.5f;
                //float phi = Random.value * Mathf.PI * 2f;
                //float x = 0.5f + r * Mathf.Cos(phi);
                //float y = 0.5f + r * Mathf.Sin(phi);

                float x = Random.value;
                float y = Random.value;

                Vector3 randomVector = new Vector3(
                    x * dimension,
                    0f,
                    y * dimension
                );

                pointCloud[i] = randomVector;
            }

        }
        else if (startSequenceCounter == 3)
        {
            Debug.Log("TRIANGULATE...");
            triangles = Triangulator.Triangulate(pointCloud);
        }
        else if (startSequenceCounter == 4)
        {
            Debug.Log("GENERATE TEXTURE(S)...");
            Color[] landscapePaint = generateLandscapePaint(textureSize);
            //Color[] contours = generateContours(textureSize, 10);
        
            //GetComponent<MeshRenderer>().material.SetTexture("_MainTex", textureFromColors(textureSize, contours));
            GetComponent<MeshRenderer>().material.SetTexture("_MainTex", textureFromColors(textureSize, landscapePaint));
            //GetComponent<MeshRenderer>().material.SetTexture("_MainTex", textureFromColors(textureSize, multiplyTextureColors(landscapePaint, contours)));
        }
        else if (startSequenceCounter == 5)
        {
            Debug.Log("APPLY MESH...");
            applyTerrainMesh();
        }
        else if (startSequenceCounter >= 6 && startSequenceCounter <= 6 + contoursAmount*2 + 2)
        {
            for (int i = 0; i < contoursAmount; i++)
            {
                if (startSequenceCounter == 6+i*2)
                {
                    blackPlane.transform.position = new Vector3(
                        blackPlane.transform.position.x,
                        (maxHeight / contoursAmount)*(i+1),
                        blackPlane.transform.position.z
                    );
                    GetComponent<MeshRenderer>().material = unlitMaterial;
                }
                else if (startSequenceCounter == 6 + i * 2 + 1)
                {
                    Debug.Log("RENDER CONTOUR: "+i+" ...");
                    Texture2D sliceTex = contourRenderer.render();
                    if (startSequenceCounter == 6+1)
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
            if (startSequenceCounter == 6 + contoursAmount*2 + 2)
            {
                Debug.Log("CONTOUR RENDERING DONE");
                GetComponent<MeshRenderer>().material = terrainMaterial;
                GetComponent<MeshRenderer>().material.SetTexture("_MainTex", textureFromColors((int)Mathf.Sqrt(contourColors.Length), contourColors));
            }
        }
        else if (startSequenceCounter == 99)
        {
            Debug.Log("ALL DONE");
            blackPlane.SetActive(false);
        }

        // calculate the point neighbours
        //pointNeighbours = new List<int>[pointsAmount];
        //for (int i = 0; i < pointsAmount; i++)
        //{
        //    pointNeighbours[i] = new List<int>();
        //}
        //foreach (Triangle a in triangles)
        //{
        //    foreach (Triangle b in triangles)
        //    {
        //        foreach (int pInA in a.points)
        //        {
        //            bool pointsMatch = false;
        //            foreach (int pInB in b.points)
        //            {
        //                if (pInA == pInB)
        //                {
        //                    pointsMatch = true;
        //                }
        //            }
        //            if (pointsMatch)
        //            {
        //                foreach (int pInB in b.points)
        //                {
        //                    if (pInA != pInB && !pointNeighbours[pInA].Contains(pInB))
        //                    {
        //                        pointNeighbours[pInA].Add(pInB);
        //                    }
        //                }
        //            }
        //        }
        //    }
        //}

        //for (int i = 0; i < riversAmount; i++){
        //    addRiver(i);    
        //}

        if (false)
        {
            for (int x = 0; x < textureSize; x++)
            {
                for (int y = 0; y < textureSize; y++)
                {
                    float nx = (float)x / textureSize;
                    float ny = (float)y / textureSize;
                    float z = multiplePerlin(nx, ny);
                    Vector2 v = gradientAtPoint(nx, ny) * -1f;
                    //v.Normalize();
                    //float vLength = dimension / textureSize / 2f;
                    float vLength = 1f;
                    Debug.DrawRay(new Vector3(nx * dimension, z * maxHeight, ny * dimension), new Vector3(v.x, 0f, v.y) * vLength, Color.black);
                }
            }
        }

        timer += Time.deltaTime;
        if (timer > 1f)
        {
            //Color[] landscapePaint = generateLandscapePaint(textureSize);
            //Color[] contours = generateContours(textureSize, contourSizes.Length);
            //GetComponent<MeshRenderer>().material.SetTexture("_MainTex", textureFromColors(textureSize, contours));
            timer -= 1f;
        }
	}
	*/

    private void applyTerrainMesh()
    {
        Vector3[] vertices = new Vector3[triangles.Count * 3];
        Vector2[] uv = new Vector2[triangles.Count * 3];
        int[] triangleList = new int[triangles.Count * 3];

        for (int i = 0; i < pointsAmount; i++){
            pointCloud[i].y = multiplePerlin(pointCloud[i].x / dimension, pointCloud[i].z / dimension) * maxHeight;
        }

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

    /* DECORATION SUTFF */


    private void addRiver(int p)
    {
        /*
        float riverHeight = 0.05f;
        GameObject river = Instantiate(RiverBlueprint) as GameObject;
        LineRenderer line = river.GetComponent<LineRenderer>();

        Vector3 v1 = pointCloud[p];
        line.SetPosition(0, new Vector3(v1.x, v1.y + riverHeight, v1.z));

        while(true){

            List<int> connectedPoints = pointNeighbours[p];
            int maxSlopeNeighbour = p;
            foreach(int neighbour in connectedPoints){
                if (pointCloud[neighbour].y < pointCloud[maxSlopeNeighbour].y){
                    maxSlopeNeighbour = neighbour;
                }
            }
            if (maxSlopeNeighbour == p){ // exit the loop
                break;
            }

            line.positionCount++;
            p = maxSlopeNeighbour;
            Vector3 v = pointCloud[p];
            line.SetPosition(line.positionCount - 1, new Vector3(v.x, v.y + riverHeight, v.z));
        }
        */
    }


    /* PERLIN HELPER */

    private float multiplePerlin(float x, float y)
    {
        float v = Mathf.Sqrt(Mathf.Pow(x - 0.5f, 2f) + Mathf.Pow(y - 0.5f, 2f)); // distance to center
        v = v / 0.5f; // normalize distance to center
        v = 1f - v;
        v = mountainCurve.Evaluate(v);

        v += rough.perlinNoise(x, y) / 2f + 0.5f;
        v += medium.perlinNoise(x, y) / 2f + 0.5f;

        return shapeCurve.Evaluate(v/3f);
    }

    private Vector2 gradientAtPoint(float x, float y)
    {
        float h = 0.0001f;
        float dx = (multiplePerlin(x + h, y) - multiplePerlin(x, y)) / h;
        float dy = (multiplePerlin(x, y + h) - multiplePerlin(x, y)) / h;

        return new Vector2(dx, dy);
    }


    /* TEXTURE PAINTERS */

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
                foreach(float v in neighbourValues)
                {
                    newR += v;
                }

                newR = 1f - (newR / neighbourValues.Count);

                colors[i] = new Color(newR, newR, newR);
            }
        }

        return colors;
    }

    private Color[] generateContours(int size, int amount)
    {
        //float lineRange = 0.1f;

        Color[] colors = new Color[size * size];
        int i = 0;
        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                i = x * size + y;

                float nx = (float)x / (float)size;
                float ny = (float)y / (float)size;
                float v = multiplePerlin(ny, nx);
                Vector2 nabla = gradientAtPoint(nx, ny);

                v = v * (float)(amount - 1);
                float discretV = Mathf.Round(v);
                float deltaV = Mathf.Abs(v - discretV);

                //float lineRange = nabla.magnitude;
                float lineRange = nabla.magnitude;
                float maxMagnitue = 3f;
                if (lineRange > maxMagnitue)
                {
                    lineRange = maxMagnitue;
                }else{
                    lineRange = lineRange / maxMagnitue;
                }
                lineRange *= 0.1f;
                if (deltaV < lineRange)
                {
                    //v = Mathf.Pow(deltaV / lineRange, 2);
                    //v = 0.6f + (deltaV / lineRange) * 0.4f;
                    v = deltaV / lineRange;
                    colors[i] = new Color(v, v, v);
                }
                else
                {
                    colors[i] = new Color(1f, 1f, 1f);
                }
            }
        }
        /*
        float[] perlinValues = getPerlinTexture(size);
        for (int i = 0; i < perlinValues.Length; i++)
        {
            float v = perlinValues[i] * (float)(amount-1);
            float discretV = Mathf.Round(v);
            float deltaV = Mathf.Abs(v - discretV);

            //lineRange = contourCurve.Evaluate(perlinValues[i]) * 0.2f;
            float lineRange = contourSizes[(int)discretV];
            if (deltaV < lineRange)
            {
                //v = Mathf.Pow(deltaV / lineRange, 2);
                v = 0.6f + (deltaV / lineRange)*0.4f;
                //v = deltaV / lineRange;
                colors[i] = new Color(v, v, v);
            }
            else
            {
                colors[i] = new Color(1f, 1f, 1f);
            }
        }
        */

        return colors;
    }

    private Color[] generateLandscapePaint(int size)
    {
        Color[] colors = new Color[size * size];
        float[] perlinValues = getPerlinTexture(size);

        for (int i = 0; i < perlinValues.Length; i++)
        {
            colors[i] = paint.Evaluate(perlinValues[i]);
        }

        return colors;
    }


    /* TEXTURE HELPERS */

    private float[] getPerlinTexture(int size)
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

    private Texture2D textureFromColors(int size, Color[] colors)
    {
        Texture2D landscapePaint = new Texture2D(size, size);

        landscapePaint.SetPixels(colors);
        landscapePaint.Apply();

        return landscapePaint;
    }

    private Color[] multiplyTextureColors(Color[] a, Color[] b)
    {
        Color[] colors = new Color[a.Length];
        for (int i = 0; i < a.Length; i++){
            colors[i] = a[i] * b[i];
        }
        return colors;
    }
}
