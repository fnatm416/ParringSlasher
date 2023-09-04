using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

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

    public void OnHit()
    {
        panel.color = hitColor;
        StartCoroutine(FlashRoutine(hitColor));
    }

    public void OnParring()
    {
        panel.color = parringColor;
        StartCoroutine(FlashRoutine(parringColor));
    }

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
