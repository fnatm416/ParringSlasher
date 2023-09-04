using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class Enemy : MonoBehaviour
{
    #region Attribute
    protected Animator anim;
    public AnimationClip[] attackAnimations;

    public float damage;  //공격력
    public float _health;  //체력
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
    public Transform hitbox;   //공격판정을 일으킬 히트박스
    public Transform hitpoint;  //피격판정을 일으킬 지점
    public float attackDelay;   //공격 재사용 대기시간
    public int score;   //죽였을때 주는 점수

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
        //플레이어와 대치하는 지점으로 수정해야함
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
            health -= damage;
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
        gameObject.SetActive(false);
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