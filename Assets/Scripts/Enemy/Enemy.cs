using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    protected Animator anim;
    public AnimationClip[] attackAnimations;

    public float damage;  //공격력
    public float health;  //체력
    public float _currentHealth;  //현재 체력
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
    public Transform hitbox;   //공격판정을 일으킬 히트박스
    public Transform hitpoint;  //피격판정을 일으킬 지점
    public float attackDelay;   //공격 재사용 대기시간
    public int score;   //죽였을때 주는 점수
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

    //초기화
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
    //공격 실행
    public void Attack()
    {
        canAttack = false;
        anim.SetTrigger("Attack");
    }

    //공격 판정
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

    //공격 끝
    public void OnAttackEnd()
    {
        spr.sortingOrder = 0;
        StartCoroutine(AttackCoolTime());
    }

    //공격 쿨타임 코루틴
    protected IEnumerator AttackCoolTime()
    {
        yield return new WaitForSeconds(attackDelay);
        canAttack = true;
    }
    #endregion

    #region Hitted
    //플레이어에게 공격당할 때 
    public void OnHit(float damage)
    {
        if (isDie == false)
            currentHealth -= damage;
    }

    //죽을 때 코루틴 호출
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

    //공격범위 표시 (Visiable)
    protected void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(hitbox.position, hitbox.localScale);
    }
}