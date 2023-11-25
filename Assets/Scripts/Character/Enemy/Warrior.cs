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

    public void OnJump()
    {
        anim.speed = 0;
        sr.sortingOrder = 2;
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
