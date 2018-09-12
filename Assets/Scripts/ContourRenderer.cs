using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContourRenderer : MonoBehaviour {
    
    public Texture2D render()
    {
        RenderTexture rTex = GetComponent<Camera>().targetTexture;

        Texture2D tex = new Texture2D(rTex.width, rTex.height, TextureFormat.RGB24, false);
        RenderTexture.active = rTex;
        tex.ReadPixels(new Rect(0, 0, rTex.width, rTex.height), 0, 0);
        tex.Apply();

        return tex;
    }


}
