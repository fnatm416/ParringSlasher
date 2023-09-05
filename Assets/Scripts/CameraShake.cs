using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    private float ShakeAmount;
    private float ShakeTime;
    private Vector3 initialPosition;

    private int priority;   //낮을수록 우선순위가 높음(이전 쉐이크를 상쇄)
    private bool isShaking;
    private bool isPause = false;


    private Coroutine shakeRoutine = null;

    void Start()
    {
        initialPosition = Camera.main.transform.position;
    }

    public void VibrateForTime(float amount ,float time, int priority)
    {
        if (isShaking == false || priority <= this.priority)
        {
            isShaking = true;
            this.priority = priority;

            ShakeAmount = amount;
            ShakeTime = time;

            if (shakeRoutine != null)
                shakeRoutine = null;
            shakeRoutine = StartCoroutine(Vibrate());
        }
    }

    IEnumerator Vibrate()
    {
        transform.position = initialPosition;

        while (ShakeTime > 0 && isPause == false)
        {
            yield return null;
            transform.position = Random.insideUnitSphere * ShakeAmount + initialPosition;
            ShakeTime -= Time.deltaTime;
        }

        ShakeTime = 0.0f;
        isShaking = false;
        transform.position = initialPosition;
    }

    public void PauseVibrate(bool isPlay)
    {
        if (isPlay == true)
            isPause = true;
        else if (isShaking == false)
            isPause = false;
    }
}