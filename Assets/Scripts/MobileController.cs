using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MobileController : MonoBehaviour
{
    //��ư �¿���� ���θ� ����
    public Transform[] presets; 
    private int currentPreset;

    private void Awake()
    {
        //�÷��̾ ����Ű ���������� �̷������� �ҷ�����
        if (PlayerPrefs.GetInt("Control") == 1)
            SwitchControl();    
    }

    //������ ���� ��� ���� Ȱ��ȭ
    public void ControlEnable()
    {
        foreach (Transform preset in presets)
        {
            foreach (Button button in preset.GetComponentsInChildren<Button>())
            {
                button.enabled = true;
                button.GetComponent<EventTrigger>().enabled = true;
            }
        }
    }

    //������ ���� ��� ���� ��Ȱ��ȭ
    public void ControlDisable()
    {        
        foreach (Transform preset in presets)
        {
            foreach (Button button in preset.GetComponentsInChildren<Button>())
            {
                button.enabled = false;
                button.GetComponent<EventTrigger>().enabled = false;
            }
        }
    }

    //������ ��ü
    public void SwitchControl()
    {       
        if (currentPreset == 0)
            currentPreset = 1;
        else if (currentPreset == 1)
            currentPreset = 0;
        //��ü�� ������ ��Ȱ��ȭ
        for (int i = 0; i < presets.Length; i++)
        {
            if (i == currentPreset)
                presets[i].gameObject.SetActive(true);
            else
                presets[i].gameObject.SetActive(false);
        }

        //���� â������ �̹��� ����
        UIManager.instance.SwitchControl();

        //�����°��� ����
        PlayerPrefs.SetInt("Control", currentPreset);
    }
}
