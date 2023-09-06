using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Warrior : Enemy
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
    }

    public void OnJump()
    {
        anim.speed = 0;
        spr.sortingOrder = 2;
        float startPos = transform.position.y;
        float endPos = startPos + 3.0f;
        sequence = DOTween.Sequence();
        //���� Apeend�� �Ϸ�Ǹ� �Ʒ� Append ����
        sequence.Append(transform.DOMoveY(endPos, 1.0f).SetEase(Ease.OutCubic));
        sequence.Append(transform.DOMoveY(startPos, 0.5f).SetEase(Ease.InCubic));
        sequence.AppendCallback(() => 
        { 
            anim.speed = 1;
        });
    }
}
