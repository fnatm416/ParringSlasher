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

    //�����ڼ��� ����� ����
    public void OnAttackDelay(float delay)
    {
        StartCoroutine(AttackDelay(delay));
    }

    //���� �߰��� ������ ������
    IEnumerator AttackDelay(float delay)
    {
        count++;
        anim.speed = 0;
        //delay��ŭ �����ϰ� �ִϸ��̼� �簳           
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

        //Ʈ������ �պ�    
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

        //Ʈ������ �պ�    
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
