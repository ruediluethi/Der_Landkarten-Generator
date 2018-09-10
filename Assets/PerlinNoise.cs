using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerlinNoise {

    private int noiseLength;

    private float[] randomSequence;
    private Vector2[,] randomMatrix;

    public PerlinNoise(int length){
        noiseLength = length;

        randomSequence = new float[noiseLength + 1]; // +1 because the borders has values too
        for (int i = 0; i < noiseLength + 1; i++)
        {
            float randomValue = Random.value * 2f - 1f;
            randomSequence[i] = randomValue;
        }

        randomMatrix = new Vector2[noiseLength + 1, noiseLength + 1];
        for (int x = 0; x < noiseLength + 1; x++)
        {
            for (int y = 0; y < noiseLength + 1; y++)
            {
                randomMatrix[x, y] = new Vector2(
                    Random.value * 2f - 1f,
                    Random.value * 2f - 1f
                );
            }
        }
    }

    private float scalarProduct(Vector2 a, Vector2 b)
    {
        return a.x * b.x + a.y * b.y;
    }

    // kubische Überblendungsfunktion
    private float s(float x)
    {
        // return 3f * Mathf.Pow(x, 2) - 2 * Mathf.Pow(x, 3); // [24]
        return Mathf.Pow(x, 3) * (x * (x * 6f - 15f) + 10f); // [28]
    }

    // 1D
    private float perlinNoise(float x)
    {

        int p = (int)Mathf.Floor(x);
        float g0 = randomSequence[p];
        float g1 = randomSequence[p + 1];

        float x_ = x - (float)p;

        // float a0 = 0;
        float a1 = g0;
        float a2 = -2 * g0 - g1;
        float a3 = g0 + g1;

        // third degree polynomial
        return a3 * Mathf.Pow(x_, 3) + a2 * Mathf.Pow(x_, 2) + a1 * x_;
    }

    // 2D
    public float perlinNoise(float x, float y)
    {
        x = x * (float)noiseLength;
        y = y * (float)noiseLength;

        int px = (int)Mathf.Floor(x);
        int py = (int)Mathf.Floor(y);
        Vector2 g00 = randomMatrix[px, py];
        Vector2 g01 = randomMatrix[px, py + 1];
        Vector2 g10 = randomMatrix[px + 1, py];
        Vector2 g11 = randomMatrix[px + 1, py + 1];

        float x_ = x - (float)px;
        float y_ = y - (float)py;

        float w00 = scalarProduct(g00, new Vector2(x_ - 0f, y_ - 0f));
        float w01 = scalarProduct(g01, new Vector2(x_ - 0f, y_ - 1f));
        float w10 = scalarProduct(g10, new Vector2(x_ - 1f, y_ - 0f));
        float w11 = scalarProduct(g11, new Vector2(x_ - 1f, y_ - 1f));

        float w0 = (1f - s(x_)) * w00 + s(x_) * w10;
        float w1 = (1f - s(x_)) * w01 + s(x_) * w11;

        return (1 - s(y_)) * w0 + s(y_) * w1;
    }
}
