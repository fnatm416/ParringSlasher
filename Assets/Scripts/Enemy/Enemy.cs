using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    protected Animator anim;
    public AnimationClip[] attackAnimations;

    public float damage;  //���ݷ�
    public float health;  //ü��
    public float _currentHealth;  //���� ü��
    public float currentHealth
    {
        get { return _currentHealth; }
        set
        {
            _currentHealth = value;
            if (_currentHealth <= 0)
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
    public bool isDie { get;  protected set; }

    protected bool canAttack = false;
    protected Coroutine routine = null;
    protected SpriteRenderer spr;

    void Awake()
    {
        anim = GetComponent<Animator>();
        spr = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        StartCoroutine(AttackCoolTime());
    }

    protected virtual void Update()
    {
        if (isDie == false && canAttack == true)
            Attack();
    }

    //�ʱ�ȭ
    public void Init()
    {
        isDie = false;
        currentHealth = health;
        canAttack = false;

        if (routine != null)
            StopCoroutine(routine);
        routine = StartCoroutine(AttackCoolTime());
    }

    #region Attack
    //���� ����
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
            currentHealth -= damage;
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
        PoolManager.instance.Return(this.gameObject);
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