using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    private float ShakeAmount;
    private float ShakeTime;
    private Vector3 initPos;

    private int priority;   //�������� �켱������ ����(���� ����ũ�� ���)
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
    
    //ī�޶����ũ
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

    //ī�޶� �������� On/Off
    public void PauseVibrate(bool isPlay)
    {
        isPause = isPlay;
    }
}