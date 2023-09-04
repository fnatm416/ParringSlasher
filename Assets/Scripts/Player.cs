using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.Experimental.Rendering;
using UnityEngine.EventSystems;

public class Player : MonoBehaviour
{
    #region Attribute
    public AnimationClip[] attackAnimations;    //공격 애니메이션들을 등록(애니메이션길이를 0:10 통일)
    public GameObject slashEffect;
    public Transform hitbox;    //공격판정을 일으킬 히트박스
    public Transform hitpoint;  //피격판정을 일으킬 지점
    public PanelEffect panelEffect; //피격 및 패링 이펙트를 출력시킬 패널

    private CameraShake cameraShake;    //카메라효과를 주기위한 카메라의 컴포넌트

    public float damage; //공격한대당 입히는 데미지
    public float _attackSpeed;
    public float attackSpeed   //한번 공격하는데 걸리는 시간 (낮을수록 빠르게 공격)
    {
        get { return _attackSpeed; }
        set { _attackSpeed = value; SetAttackSpeed(value); }
    }
    public float maxGuardTime; //최대 막기 유지시간
    public float parryingTime;  //방패를 들고나서 패링이 가능한 시간
    public float guardCoolTime; //막기 재사용 대기시간

    private Animator anim;  //플레이어 애니메이터
    private PlayerInput pi; //플레이어 인풋

    private bool isDie = false; //플레이어가 죽었는지
    private bool isAttack = false;  //플레이어가 공격 중인지
    private bool isGuard = false;   //플레이어가 가드 중인지
    private bool isParrying = false; //패링이 가능한 상태인지
    private Coroutine guardRoutine = null;  //막기관련 코루틴 제어
    private bool canGuard = true;   //막기가 가능한지 여부
    #endregion

    #region LifeCycle
    private void Awake()
    {
        anim = GetComponent<Animator>();
        pi = GetComponent<PlayerInput>();

        SetAttackSpeed(attackSpeed);
        cameraShake = Camera.main.GetComponent<CameraShake>();
    }
    private void OnEnable()
    {
        pi.actions["Attack"].Enable();
        pi.actions["Guard"].Enable();

        Init();
        anim.SetBool("IsMove", false);
    }

    private void OnDisable()
    {
        pi.actions["Attack"].Disable();
        pi.actions["Guard"].Disable();

        if (isDie == false)
            anim.SetBool("IsMove", true);

        DOTween.KillAll();
    }


    #endregion

    #region Attack
    //공격속도 고정시키기
    private void SetAttackSpeed(float targetDuration)
    {
        float currentDuration = attackAnimations[0].length;
        float speed = currentDuration / targetDuration;
        anim.SetFloat("AttackSpeed", speed);
    }

    //공격하기
    public void Attack()
    {
        if (isDie == false)
        {
            if (isAttack == false && isGuard == false)
            {
                isAttack = true;

                int random = Random.Range(1, attackAnimations.Length + 1); // 랜덤한 정수 생성 (1부터 numberOfAttacks까지)
                anim.SetInteger("AttackType", random); // 애니메이터의 AttackType 파라미터에 랜덤한 값 전달
                anim.SetTrigger("Attack");

                //트윈으로 왕복
                float startPos = -1f;
                float endPos = startPos + 0.5f;
                Sequence sequence = DOTween.Sequence();
                sequence.Append(transform.DOMoveX(endPos, attackSpeed / 2).SetEase(Ease.OutExpo));
                sequence.Append(transform.DOMoveX(startPos, attackSpeed / 2).SetEase(Ease.InExpo));
            }
        }
    }

    //공격시, 박스콜라이더 생성
    public void OnAttackHitbox()
    {
        Collider2D[] collider2Ds = Physics2D.OverlapBoxAll(hitbox.position, hitbox.localScale, 0);
        foreach (Collider2D colider in collider2Ds)
        {
            if (colider.CompareTag("Enemy"))
            {
                Enemy enemy = colider.GetComponent<Enemy>();
                if (enemy.isDie == false)
                {
                    cameraShake.VibrateForTime(0.05f, 0.05f, 3);
                    StartCoroutine(SlashEffect(enemy.hitpoint.position));
                    enemy.OnHit(damage);
                    SoundManager.instance.PlaySfx(SoundManager.Sfx.Melee);
                }
            }
        }
    }

    //피격된 적에게 이펙트 발생
    IEnumerator SlashEffect(Vector2 pos)
    {
        float random = Random.Range(0, 360.0f);
        GameObject effect = Instantiate(slashEffect, pos, Quaternion.Euler(0, 0, random));
        while (effect.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime < 1)
        {
            yield return null;
        }
        Destroy(effect);
    }

    //공격이 끝날 때 실행
    public void OnAttackAnimationEnd()
    {
        anim.SetInteger("AttackType", 0);
        isAttack = false;
    }
    #endregion

    #region Hitted
    //방어하기
    public void Guard(InputAction.CallbackContext context)
    {
        if(context.performed) { OnGuard(); }
        else if(context.canceled) { OnGuardEnd(); }
    }

    public void OnGuard()
    {
        if (isDie == false)
        {
            if (isAttack == false && isGuard == false)
            {
                //방어
                if (canGuard == true)
                {
                    canGuard = false;
                    isGuard = true;
                    isParrying = true;

                    anim.SetTrigger("Guard");
                    anim.SetBool("OnGuard", true);
                    guardRoutine = StartCoroutine(GuardDuration());
                }
            }
        }
    }

    //가드 내릴때 함수
    public void OnGuardEnd()
    {
        if (isDie == false)
        {
            if (isGuard == true)
            {
                anim.SetBool("OnGuard", false);
                if (guardRoutine != null)
                    StopCoroutine(guardRoutine);
                isGuard = false;
                isParrying = false;
                StartCoroutine(GuardCoolTime());
            }
        }
    }

    //가드의 최대지속시간을 제한
    IEnumerator GuardDuration()
    {
        yield return new WaitForSeconds(parryingTime);
        isParrying = false;
        yield return new WaitForSeconds(maxGuardTime - parryingTime);
        OnGuardEnd();
    }

    //가드 재사용 쿨타임 적용
    IEnumerator GuardCoolTime()
    {
        yield return new WaitForSeconds(guardCoolTime);
        canGuard = true;
    }

    //적의 공격이 들어오면 실행
    public void OnHit(float damage)
    {
        isAttack = false;

        if (isGuard)
        {
            if (isParrying)
            {
                //패링 성공
                anim.SetTrigger("Parring");
                cameraShake.VibrateForTime(0.2f, 0.05f, 2);
                panelEffect.OnParring();
                SoundManager.instance.PlaySfx(SoundManager.Sfx.Parring);
            }
            else
            {
                //일반 가드
                anim.SetTrigger("Block");
                cameraShake.VibrateForTime(0.1f, 0.05f, 3);
                GameManager.instance.currentHp -= damage / 2;
                SoundManager.instance.PlaySfx(SoundManager.Sfx.Guard);
            }
        }
        else
        {
            //피격
            cameraShake.VibrateForTime(0.4f, 0.05f, 1);
            GameManager.instance.currentHp -= damage;
            panelEffect.OnHit();
            SoundManager.instance.PlaySfx(SoundManager.Sfx.Hit);
        }
    }
    #endregion

    public void Init()
    {
        isDie = false;
        isAttack = false;
        isGuard = false;
        isParrying = false;
        guardRoutine = null;
        canGuard = true;
    }

    public void OnGameOver()
    {
        isDie = true;
        this.enabled = false;
        anim.SetTrigger("Die");
    }

    public void OnDie()
    {
        GameManager.instance.GameOver();
    }

    //공격범위를 scene창에 표시(Visiable)
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(hitbox.position, hitbox.localScale);
    }
}