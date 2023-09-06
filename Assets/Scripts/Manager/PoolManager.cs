using System.Collections;
using System.Collections.Generic;
using UnityEditor.UI;
using UnityEngine;

public enum EnemyName
{
    Knight,
    Samurai,
    Warrior,
    Plate,
    King
}

public class PoolManager : MonoBehaviour
{
    public static PoolManager instance = null;

    public GameObject[] prefabs;
    Dictionary<string, List<GameObject>> pools = new Dictionary<string, List<GameObject>>();

    void Awake()
    {
        if (instance == null)
            instance = this;

        for (int index = 0; index < prefabs.Length; index++)
        {
            pools[prefabs[index].name] = new List<GameObject>();
        }
    }

    public GameObject Get(string name)
    {
        GameObject select = null;

        foreach (GameObject item in pools[name])
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
            for(int index = 0; index < prefabs.Length; index++)
            {
                if (prefabs[index].name == name)
                {
                    select = Instantiate(prefabs[index], this.transform);
                    //���Ӱ� ������ ������Ʈ�� Ǯ�� ���
                    pools[prefabs[index].name].Add(select);
                }
            }
        }

        return select;
    }

    public void Return(GameObject item)
    {
        item.SetActive(false);
        item.transform.SetParent(this.transform);
    }
}