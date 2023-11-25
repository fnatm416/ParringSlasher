using DG.Tweening;
using System.Collections;
using UnityEngine;

public class King : Enemy
{
    private Sequence sequence;
    private int count = 0;

    protected override void Update()
    {
        base.Update();

        if (isDie)
            sequence.Kill();
    }

    public override void Init()
    {
        base.Init();
        count = 0;
    }

    public override IEnumerator AttackDelay(float delay)
    {
        yield return base.AttackDelay(delay);

        count++;

        if (count < 3)
            Slash();
        else if (count >= 3)
            FinalSlash();
    }

    public void Slash()
    {
        sr.sortingOrder = 2;

        //트윈으로 왕복    
        float length = attackAnimations[count - 1].length;
        float startPos = 1.0f;
        float endPos = startPos - 0.5f;
        sequence = DOTween.Sequence();
        sequence.Append(transform.DOMoveX(endPos, length * (4 / 6f)).SetEase(Ease.OutExpo));
        sequence.Append(transform.DOMoveX(startPos, length * (2 / 6f)).SetEase(Ease.InExpo));
    }

    public void FinalSlash()
    {
        sr.sortingOrder = 2;

        //트윈으로 왕복    
        float length = attackAnimations[count - 1].length;
        float startPos = 1.0f;
        float endPos = startPos + 0.5f;
        sequence = DOTween.Sequence();

        sequence.Append(transform.DOMoveX(endPos, length * (2 / 6f)).SetEase(Ease.Linear));
        sequence.AppendCallback(() =>
        {
            sequence.Kill();
            anim.speed = 1;

            sequence = DOTween.Sequence();
            endPos = startPos - 1.0f;
            sequence.Append(transform.DOMoveX(endPos, length * (2 / 6f)).SetEase(Ease.OutExpo));
            sequence.Append(transform.DOMoveX(startPos, length * (2 / 6f)).SetEase(Ease.InExpo));

            count = 0;
        });
    }
}
