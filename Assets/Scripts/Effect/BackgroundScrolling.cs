using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundScrolling : MonoBehaviour
{
    public float speed;
    public Transform[] backgrounds;

    private Dictionary<int, Vector3> points = new Dictionary<int, Vector3>();
    private Coroutine routine;

    float leftPosX = 0f;
    float rightPosX = 0f;
    float xScreenHalfSize;
    float yScreenHalfSize;


    private void Awake()
    {
        for (int i = 0; i < backgrounds.Length; i++)
            points[i] = backgrounds[i].transform.position;

        Init();
    }

    //초기화
    public void Init()
    {
        yScreenHalfSize = Camera.main.orthographicSize;
        xScreenHalfSize = yScreenHalfSize * Camera.main.aspect;

        leftPosX = -(xScreenHalfSize * 2);
        rightPosX = xScreenHalfSize * 2 * backgrounds.Length;

        for (int i = 0; i < backgrounds.Length; i++)
        {
            backgrounds[i].position = points[i];
        }

        if (routine != null )
            StopCoroutine(routine);
    }

    //외부에서 스크롤링 호출
    public void Scrolling()
    {
        routine = StartCoroutine(ScrollingRoutine());        
    }

    //맵 스크롤링
    IEnumerator ScrollingRoutine()
    {
        Transform currentPlat = backgrounds[1];
        while (currentPlat.position.x > leftPosX)
        {
            yield return null;

            for (int i = 0; i < backgrounds.Length; i++)
            {
                backgrounds[i].position += new Vector3(-speed, 0, 0) * Time.deltaTime;
                if (currentPlat.position.x <= leftPosX)
                    break;

                if (backgrounds[i].position.x < leftPosX)
                {
                    Vector3 nextPos = backgrounds[i].position;
                    nextPos = new Vector3(nextPos.x + rightPosX, nextPos.y, nextPos.z);
                    backgrounds[i].position = nextPos;    
                }
            }
        }

        Transform temp = backgrounds[0];
        backgrounds[0] = backgrounds[1];
        backgrounds[1] = backgrounds[2];
        backgrounds[2] = temp;

        for (int i =0; i<points.Count; i++)
            backgrounds[i].transform.position = points[i];

        GameManager.instance.StartBattle();
    }
}
