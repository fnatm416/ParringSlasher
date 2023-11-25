using UnityEngine;

public class CameraResolution : MonoBehaviour
{
    public int width;
    public int height;

    void Start()
    {
        //width : height 비율로 해상도 고정
        Camera camera = GetComponent<Camera>();
        Rect rect = camera.rect;
        float scaleheight = ((float)Screen.width / Screen.height) / ((float)width / height); // (가로 / 세로)
        float scalewidth = 1f / scaleheight;
        if (scaleheight < 1)
        {
            rect.height = scaleheight;
            rect.y = (1f - scaleheight) / 2f;
        }
        else
        {
            rect.width = scalewidth;
            rect.x = (1f - scalewidth) / 2f;
        }
        camera.rect = rect;
    }

    //잘리는 부분을 검은색으로 채움
    void OnPreCull() => GL.Clear(true, true, Color.black);
}