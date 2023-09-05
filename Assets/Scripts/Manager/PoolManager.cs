using System.Collections;
using System.Collections.Generic;
using UnityEditor.UI;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    public static PoolManager instance = null;

    public GameObject[] prefabs;
    List<GameObject>[] pools;

    void Awake()
    {
        if (instance == null)
            instance = this;

        pools = new List<GameObject>[prefabs.Length];

        for (int index = 0; index < pools.Length; index++)
        {
            pools[index] = new List<GameObject>();
        }
    }

    public GameObject Get(int index)
    {
        GameObject select = null;

        foreach (GameObject item in pools[index])
        {
            //놀고있는 오브젝트를 select에 할당
            if (!item.activeSelf)
            {
                select = item;
                select.SetActive(true);
                break;
            }
        }

        //만약 놀고있는 오브젝트를 못찾았다면 새롭게 생성
        if (!select)
        {
            select = Instantiate(prefabs[index], this.transform);
            //새롭게 생성된 오브젝트를 풀에 등록
            pools[index].Add(select);
        }

        return select;
    }

}
