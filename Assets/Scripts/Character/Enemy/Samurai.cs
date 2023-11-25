using DG.Tweening;
using System.Collections;
using UnityEngine;

public class Samurai : Enemy
{
    private Sequence sequence;
    private int count = 0;

    protected override void Update()
    {
        base.Update();

        if (isDie)
            sequence.Kill();
    }

    public override IEnumerator AttackDelay(float delay)
    {
        yield return base.AttackDelay(delay);

        count++;

        Slash();
        if (count == 2)
            count = 0;
    }

    public void Slash()
    {
        sr.sortingOrder = 2;

        //트윈으로 왕복    
        float length = attackAnimations[count - 1].length;
        float startPos = transform.position.x;
        float endPos = startPos - 0.5f;
        sequence = DOTween.Sequence();
        sequence.Append(transform.DOMoveX(endPos, length * (3 / 5f)).SetEase(Ease.OutExpo));
        sequence.Append(transform.DOMoveX(startPos, length * (1 / 5f)).SetEase(Ease.InExpo));

        count = 0;
    }
}