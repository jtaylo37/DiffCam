using UnityEngine;

public class Fly : MonoBehaviour
{
    public int margin = 5;
    public int threshold = 80;
    public float scale = 1;

    const float screenWidth = 16;
    const float screenHeight = 9;
    const float halfScreenWidth = screenWidth / 2;
    const float halfScreenHeight = screenHeight / 2;
    const float half = 0.5f;

    DiffCam cam;

    Vector3 force;

    Rigidbody rb;

    int width = 0;
    int height;
    int[,] difference;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        GameObject display = GameObject.Find("Display");
        cam = display.GetComponent<DiffCam>();
        cam.ReportPixels += ReportPixels;
    }

    void ReportPixels(int width, int height, int[,] difference)
    {
        this.width = width;
        this.height = height;
        this.difference = difference;
    }

    private void FixedUpdate()
    {
        if (width == 0) return;

        float y = transform.localPosition.y;
        float x = transform.localPosition.x;

        int myRow = (int)(height * (y + halfScreenHeight) / screenHeight + half);
        int myCol = (int)(width * (x + halfScreenWidth) / screenWidth + half);
        
        force = Vector3.zero;

        if (myRow + margin < height && difference[myCol, myRow + margin] > threshold)
        {
            force = force - scale * Vector3.up;
        }

        if (myRow - margin > -1 && difference[myCol, myRow - margin] > threshold)
        {
            force = force + scale * Vector3.up;
        }

        if (myCol + margin < width && difference[myCol + margin, myRow] > threshold)
        {
            force = force - scale * Vector3.right;
        }

        if (myCol - margin > -1 && difference[myCol - margin, myRow] > threshold)
        {
            force = force + scale * Vector3.right;
        }

        rb.AddForce(force, ForceMode.Impulse);
    }
}
