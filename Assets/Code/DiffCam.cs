using System;
using UnityEngine;
public class DiffCam : MonoBehaviour
{
    const int waitingWidth = 16;

    bool showDiff = false;

    public int width = 640;
    public int height = 360;

    public event Action<int, int, int[,]> ReportPixels;

    Color32[] pixels;

    WebCamTexture cam;
    Texture2D tex;

    int[,] thisBuf;
    int[,] thatBuf;

    int[,] diffBuf;

    int[,] newBuf;
    int[,] oldBuf;

    Action update;
    void Start()
    {
        update = WaitingForCam;

        cam = new WebCamTexture(WebCamTexture.devices[0].name, width, height);
        cam.Play();
    }
    private void Update()
    {
        update();

        if (Input.GetKeyDown(KeyCode.Space))
        {
            showDiff = !showDiff;
        }
    }
    void WaitingForCam()
    {
        if (cam.width > waitingWidth)
        {
            width = cam.width;
            height = cam.height;
            thisBuf = new int[width, height];
            thatBuf = new int[width, height];
            diffBuf = new int[width, height];
            pixels = new Color32[cam.width * cam.height];
            cam.GetPixels32(pixels);

            LoadBuf(pixels, thisBuf);

            oldBuf = thisBuf;
            newBuf = thatBuf;

            tex = new Texture2D(cam.width, cam.height, TextureFormat.RGBA32, false);
            Renderer renderer = GetComponent<Renderer>();
            renderer.material.mainTexture = tex;
            update = CamIsOn;
        }
    }
    void CamIsOn()
    {
        if (cam.didUpdateThisFrame)
        {
            cam.GetPixels32(pixels);

            MirrorPixels(width, height, pixels);

            if (!showDiff)
            {
                tex.SetPixels32(pixels);
                tex.Apply();
            }

            LoadBuf(pixels, newBuf);

            DiffBufs(newBuf, oldBuf, diffBuf, pixels);

            if (showDiff)
            {
                tex.SetPixels32(pixels);
                tex.Apply();
            }

            int[,] tempBuf = newBuf;
            newBuf = oldBuf;
            oldBuf = tempBuf;

            if (ReportPixels != null)
            {
                ReportPixels(width, height, diffBuf);
            }
        }
    }

    void LoadBuf(Color32[] p, int[,] b)
    {
        int index = 0;

        for (int y = 0; y < height; ++y)
        {
            for (int x = 0; x < width; ++x)
            {
                b[x, y] = (p[index].r + p[index].g + p[index].b) / 3;

                index = index + 1;
            }
        }
    }

    void DiffBufs(int[,] n, int[,] o, int[,] d, Color32[] p)
    {
        int index = 0;

        for (int y = 0; y < height; ++y)
        {
            for (int x = 0; x < width; ++x)
            {
                d[x,y] = 
                p[index].r =
                p[index].g =
                p[index].b = (byte)(Mathf.Abs(n[x, y] - o[x, y]));

                index = index + 1;
            }
        }
    }

    void MirrorPixels(int width, int height, Color32[] pixels)
    {
        for (int y = 0; y < height; ++y)
        {
            int offsetLft = y * width;
            int offsetRgt = offsetLft + width - 1;

            for (int x = 0; x < width / 2; ++x)
            {
                Color32 temp = pixels[offsetLft + x];
                pixels[offsetLft + x] = pixels[offsetRgt - x];
                pixels[offsetRgt - x] = temp;
            }
        }
    }
}