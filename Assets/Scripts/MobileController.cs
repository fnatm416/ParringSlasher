using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MobileController : MonoBehaviour
{
    //버튼 좌우반전 여부를 관리
    public Transform[] presets; 
    private int currentPreset;

    private void Awake()
    {
        //플레이어가 조작키 반전설정한 이력있으면 불러오기
        if (PlayerPrefs.GetInt("Control") == 1)
            SwitchControl();    
    }

    //프리셋 내의 모든 조작 활성화
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

    //프리셋 내의 모든 조작 비활성화
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

    //프리셋 교체
    public void SwitchControl()
    {       
        if (currentPreset == 0)
            currentPreset = 1;
        else if (currentPreset == 1)
            currentPreset = 0;
        //교체된 프리셋 비활성화
        for (int i = 0; i < presets.Length; i++)
        {
            if (i == currentPreset)
                presets[i].gameObject.SetActive(true);
            else
                presets[i].gameObject.SetActive(false);
        }

        //설정 창에서도 이미지 변경
        UIManager.instance.SwitchControl();

        //프리셋값을 저장
        PlayerPrefs.SetInt("Control", currentPreset);
    }
}
