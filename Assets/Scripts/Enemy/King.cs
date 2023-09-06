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

    //공격자세를 잡고나서 지연
    public void OnAttackDelay(float delay)
    {
        StartCoroutine(AttackDelay(delay));
    }

    //공격 중간에 삽입할 딜레이
    IEnumerator AttackDelay(float delay)
    {
        count++;
        anim.speed = 0;
        //delay만큼 지연하고 애니메이션 재개           
        yield return new WaitForSeconds(delay);
        anim.speed = 1;

        if (count < 3)
            Slash();
        else if (count >= 3)
            FinalSlash();
    }

    public void Slash()
    {
        spr.sortingOrder = 2;

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
        spr.sortingOrder = 2;

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
