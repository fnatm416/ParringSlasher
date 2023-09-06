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

    //�����ڼ��� ����� ����
    public void OnAttackDelay(float delay)
    {
        StartCoroutine(AttackDelay(delay));
    }

    //���� �߰��� ������ ������
    IEnumerator AttackDelay(float delay)
    {
        count++;
        //delay��ŭ �����ϰ� �ִϸ��̼� �簳
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

        //Ʈ������ �պ�    
        float length = attackAnimations[count - 1].length;
        float startPos = transform.position.x;
        float endPos = startPos - 0.5f;
        sequence = DOTween.Sequence();
        sequence.Append(transform.DOMoveX(endPos, length * (3 / 5f)).SetEase(Ease.OutExpo));
        sequence.Append(transform.DOMoveX(startPos, length * (1 / 5f)).SetEase(Ease.InExpo));

        count = 0;
    }
}
