using DG.Tweening;
using System.Collections;
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

    public override IEnumerator AttackDelay(float delay)
    {
        yield return base.AttackDelay(delay);

        Slash();
    }

    public void Slash()
    {
        sr.sortingOrder = 2;

        //트윈으로 왕복
        float length = attackAnimations[0].length;
        float startPos = transform.position.x;
        float endPos = startPos - 0.5f;
        sequence = DOTween.Sequence();
        sequence.Append(transform.DOMoveX(endPos, length * (4/8f)).SetEase(Ease.OutExpo));
        sequence.Append(transform.DOMoveX(startPos, length * (2/8f)).SetEase(Ease.InExpo));
    }
}