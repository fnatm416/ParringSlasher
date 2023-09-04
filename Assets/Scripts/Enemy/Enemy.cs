using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class Enemy : MonoBehaviour
{
    #region Attribute
    protected Animator anim;
    public AnimationClip[] attackAnimations;

    public float damage;  //���ݷ�
    public float _health;  //ü��
    public float health
    {
        get { return _health; }
        set
        {
            _health = value;
            if (_health <= 0)
            {
                isDie = true;
                anim.SetTrigger("Die");
            }    
        }
    }
    public Transform hitbox;   //���������� ����ų ��Ʈ�ڽ�
    public Transform hitpoint;  //�ǰ������� ����ų ����
    public float attackDelay;   //���� ���� ���ð�
    public int score;   //�׿����� �ִ� ����

    protected bool canAttack = false;
    protected Coroutine attackRoutine = null;
    protected SpriteRenderer spr;

    protected bool _isDie = false;
    public bool isDie
    {
        get { return _isDie; }
        protected set { _isDie = value; }
    }
    #endregion

    #region AI
    protected void Awake()
    {
        anim = GetComponent<Animator>();
        spr = GetComponent<SpriteRenderer>();
    }

    protected void Start()
    {
        //�÷��̾�� ��ġ�ϴ� �������� �����ؾ���
        StartCoroutine(AttackCoolTime());
    }

    protected virtual void Update()
    {
        if (isDie == false && canAttack == true)
            Attack();
    }
    #endregion

    #region Attack
    public void Attack()
    {
        canAttack = false; 
        anim.SetTrigger("Attack");
    }

    //���� ����
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

    //���� ��
    public void OnAttackEnd()
    {
        spr.sortingOrder = 0;
        StartCoroutine(AttackCoolTime());
    }

    //���� ��Ÿ�� �ڷ�ƾ
    protected IEnumerator AttackCoolTime()
    {
        yield return new WaitForSeconds(attackDelay);
        canAttack = true;
    }
    #endregion

    #region Hitted
    //�÷��̾�� ���ݴ��� �� 
    public void OnHit(float damage)
    {
        if (isDie == false)
            health -= damage;
    }

    //���� �� �ڷ�ƾ ȣ��
    public void OnDie()
    {
        GameManager.instance.currentScore += score;
        StartCoroutine(DeathRoutine());
    }
   
    IEnumerator DeathRoutine()
    {
        yield return new WaitForSeconds(1.0f);
        gameObject.SetActive(false);
        GameManager.instance.NextStage();
    }
    #endregion

    //���ݹ��� ǥ�� (Visiable)
    protected void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(hitbox.position, hitbox.localScale);
    }
}