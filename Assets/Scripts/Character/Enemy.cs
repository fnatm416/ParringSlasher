using System.Collections;
using UnityEngine;

public class Enemy : Character
{
    [SerializeField] private float attackDelay;   //���� ���� ���ð�
    [SerializeField] private int score;   //�׿����� �ִ� ����

    protected bool canAttack = false;
    protected Coroutine routine = null;
    protected SpriteRenderer sr;

    void Awake()
    {
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
    }

    protected virtual void Update()
    {
        if (isDie == false && canAttack == true)
            Attack();
    }
    public override void Init()
    {
        isDie = false;
        currentHP = health;
        canAttack = false;

        if (routine != null)
            StopCoroutine(routine);
        routine = StartCoroutine(AttackCoolTime());
    }

    #region Attack
    public void Attack()
    {
        canAttack = false;
        anim.SetTrigger("Attack");
    }

    public void OnAttackDelay(float delay)
    {
        StartCoroutine(AttackDelay(delay));
    }

    public virtual IEnumerator AttackDelay(float delay)
    {
        //delay��ŭ �����ϰ� �ִϸ��̼� �簳
        anim.speed = 0;
        yield return new WaitForSeconds(delay);
        anim.speed = 1;
    }

    public void OnAttackHitbox()
    {
        Collider2D[] collider2Ds = Physics2D.OverlapBoxAll(hitbox.position, hitbox.localScale, 0);
        foreach (Collider2D colider in collider2Ds)
        {
            if (colider.CompareTag("Player"))
            {
                colider.GetComponent<Player>().OnHit(damage);
            }
        }
    }
    public void OnAttackEnd()
    {
        sr.sortingOrder = 0;
        StartCoroutine(AttackCoolTime());
    }

    protected IEnumerator AttackCoolTime()
    {
        yield return new WaitForSeconds(attackDelay);
        canAttack = true;
    }
    #endregion

    public override void OnDie()
    {
        GameManager.instance.currentScore += score;
        StartCoroutine(DeathRoutine());
    }

    IEnumerator DeathRoutine()
    {
        yield return new WaitForSeconds(0.5f);
        PoolManager.instance.Return(this.gameObject);
        GameManager.instance.NextStage();
    }

    //���ݹ��� ǥ�� (Visiable)
    //protected void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.red;
    //    Gizmos.DrawWireCube(hitbox.position, hitbox.localScale);
    //}
}