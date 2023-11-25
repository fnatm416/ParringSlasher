using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    private float ShakeAmount;
    private float ShakeTime;
    private Vector3 initPos;

    private int priority;   //낮을수록 우선순위가 높음(이전 쉐이크를 상쇄)
    private bool isShaking;
    private bool isPause = false;


    private Coroutine shakeRoutine = null;

    void Start()
    {
        initPos = Camera.main.transform.position;
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
    
    //카메라셰이크
    IEnumerator Vibrate()
    {
        transform.position = initPos;

        while (ShakeTime > 0 && isPause == false)
        {
            yield return null;
            transform.position = Random.insideUnitSphere * ShakeAmount + initPos;
            ShakeTime -= Time.deltaTime;
        }

        ShakeTime = 0.0f;
        isShaking = false;
        transform.position = initPos;
    }

    //카메라 움직임을 On/Off
    public void PauseVibrate(bool isPlay)
    {
        isPause = isPlay;
    }
}