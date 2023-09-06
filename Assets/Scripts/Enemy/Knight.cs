using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class Knight : Enemy
{
    private Sequence sequence;

    protected override void Update()
    {
        base.Update();

        if (isDie)
            sequence.Kill();
    }

    //�����ڼ��� ����� ����
    public void OnAttackDelay(float delay)
    {
        StartCoroutine(AttackDelay(delay));
    }

    //���� �߰��� ������ ������
    IEnumerator AttackDelay(float delay)
    {
        //delay��ŭ �����ϰ� �ִϸ��̼� �簳
        anim.speed = 0;
        yield return new WaitForSeconds(delay);
        anim.speed = 1;
        Slash();
    }

    public void Slash()
    {
        spr.sortingOrder = 2;

        //Ʈ������ �պ�
        float length = attackAnimations[0].length;
        float startPos = transform.position.x;
        float endPos = startPos - 0.5f;
        sequence = DOTween.Sequence();
        sequence.Append(transform.DOMoveX(endPos, length * (4/8f)).SetEase(Ease.OutExpo));
        sequence.Append(transform.DOMoveX(startPos, length * (2/8f)).SetEase(Ease.InExpo));
    }
}