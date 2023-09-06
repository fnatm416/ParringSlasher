using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelEffect : MonoBehaviour
{
    public Color hitColor;
    public Color parringColor;

    public float flashTime;

    private Image panel;

    private void Awake()
    {
        panel = GetComponent<Image>();  
    }

    //피격 컬러로 플래시효과
    public void OnHit()
    {
        panel.color = hitColor;
        StartCoroutine(FlashRoutine(hitColor));
    }

    //패링 컬러로 플래시효과
    public void OnParring()
    {       
        panel.color = parringColor;
        StartCoroutine(FlashRoutine(parringColor));
    }

    //플래시효과 실행
    IEnumerator FlashRoutine(Color color)
    {
        float elapsedTime = 0;
        while (elapsedTime < flashTime)
        {
            panel.color = Color.Lerp(color, Color.clear, elapsedTime / flashTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }
}
