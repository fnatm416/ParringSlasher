using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plate : Enemy
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
        sr.sortingOrder = 2;
        float startPos = transform.position.y;
        float endPos = startPos + 1.5f;
        sequence = DOTween.Sequence();
        //위의 Apeend과 완료되면 아래 Append 실행
        sequence.Append(transform.DOMoveY(endPos, 1.0f).SetEase(Ease.OutCubic));
        sequence.Append(transform.DOMoveY(startPos, 0.5f).SetEase(Ease.InCubic));
        sequence.AppendCallback(() =>
        {
            anim.speed = 1;
        });
    }

    public void AnimStop()
    {
        anim.speed = 0;
    }
}