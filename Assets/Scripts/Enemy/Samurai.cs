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

    //공격자세를 잡고나서 지연
    public void OnAttackDelay(float delay)
    {
        StartCoroutine(AttackDelay(delay));
    }

    //공격 중간에 삽입할 딜레이
    IEnumerator AttackDelay(float delay)
    {
        count++;
        //delay만큼 지연하고 애니메이션 재개
        anim.speed = 0;
        yield return new WaitForSeconds(delay);
        anim.speed = 1;

        Slash();
        if (count == 2)
            count = 0;
    }

    public void Slash()
    {
        spr.sortingOrder = 2;

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
