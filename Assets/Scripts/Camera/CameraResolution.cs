using UnityEngine;

public class CameraResolution : MonoBehaviour
{
    public int width;
    public int height;

    void Start()
    {
        //width : height ������ �ػ� ����
        Camera camera = GetComponent<Camera>();
        Rect rect = camera.rect;
        float scaleheight = ((float)Screen.width / Screen.height) / ((float)width / height); // (���� / ����)
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

    //�߸��� �κ��� ���������� ä��
    void OnPreCull() => GL.Clear(true, true, Color.black);
}