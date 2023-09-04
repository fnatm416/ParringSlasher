using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Warrior : Enemy
{
    private Sequence sequence;

    //공격자세를 잡고나서 지연
    public void OnAttackDelay(float delay)
    {
        StartCoroutine(AttackDelay(delay));
    }

    //공격 중간에 삽입할 딜레이
    IEnumerator AttackDelay(float delay)
    {
        //delay만큼 지연하고 애니메이션 재개
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
        //위의 Apeend과 완료되면 아래 Append 실행
        sequence.Append(transform.DOMoveY(endPos, 1.0f).SetEase(Ease.OutCubic));
        sequence.Append(transform.DOMoveY(startPos, 0.5f).SetEase(Ease.InCubic));
        sequence.AppendCallback(() => 
        { 
            anim.speed = 1;
        });
    }
}
