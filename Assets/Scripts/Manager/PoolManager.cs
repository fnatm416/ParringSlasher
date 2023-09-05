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
            //����ִ� ������Ʈ�� select�� �Ҵ�
            if (!item.activeSelf)
            {
                select = item;
                select.SetActive(true);
                break;
            }
        }

        //���� ����ִ� ������Ʈ�� ��ã�Ҵٸ� ���Ӱ� ����
        if (!select)
        {
            select = Instantiate(prefabs[index], this.transform);
            //���Ӱ� ������ ������Ʈ�� Ǯ�� ���
            pools[index].Add(select);
        }

        return select;
    }

}
