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
    public AnimationClip[] attackAnimations;    //���� �ִϸ��̼ǵ��� ���(�ִϸ��̼Ǳ��̸� 0:10 ����)
    public GameObject slashEffect;
    public Transform hitbox;    //���������� ����ų ��Ʈ�ڽ�
    public Transform hitpoint;  //�ǰ������� ����ų ����
    public PanelEffect panelEffect; //�ǰ� �� �и� ����Ʈ�� ��½�ų �г�

    private CameraShake cameraShake;    //ī�޶�ȿ���� �ֱ����� ī�޶��� ������Ʈ

    public float damage; //�����Ѵ�� ������ ������
    public float _attackSpeed;
    public float attackSpeed   //�ѹ� �����ϴµ� �ɸ��� �ð� (�������� ������ ����)
    {
        get { return _attackSpeed; }
        set { _attackSpeed = value; SetAttackSpeed(value); }
    }
    public float maxGuardTime; //�ִ� ���� �����ð�
    public float parryingTime;  //���и� ����� �и��� ������ �ð�
    public float guardCoolTime; //���� ���� ���ð�

    private Animator anim;  //�÷��̾� �ִϸ�����
    private PlayerInput pi; //�÷��̾� ��ǲ

    private bool isDie = false; //�÷��̾ �׾�����
    private bool isAttack = false;  //�÷��̾ ���� ������
    private bool isGuard = false;   //�÷��̾ ���� ������
    private bool isParrying = false; //�и��� ������ ��������
    private Coroutine guardRoutine = null;  //������� �ڷ�ƾ ����
    private bool canGuard = true;   //���Ⱑ �������� ����
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
    //���ݼӵ� ������Ű��
    private void SetAttackSpeed(float targetDuration)
    {
        float currentDuration = attackAnimations[0].length;
        float speed = currentDuration / targetDuration;
        anim.SetFloat("AttackSpeed", speed);
    }

    //�����ϱ�
    public void Attack()
    {
        if (isDie == false)
        {
            if (isAttack == false && isGuard == false)
            {
                isAttack = true;

                int random = Random.Range(1, attackAnimations.Length + 1); // ������ ���� ���� (1���� numberOfAttacks����)
                anim.SetInteger("AttackType", random); // �ִϸ������� AttackType �Ķ���Ϳ� ������ �� ����
                anim.SetTrigger("Attack");

                //Ʈ������ �պ�
                float startPos = -1f;
                float endPos = startPos + 0.5f;
                Sequence sequence = DOTween.Sequence();
                sequence.Append(transform.DOMoveX(endPos, attackSpeed / 2).SetEase(Ease.OutExpo));
                sequence.Append(transform.DOMoveX(startPos, attackSpeed / 2).SetEase(Ease.InExpo));
            }
        }
    }

    //���ݽ�, �ڽ��ݶ��̴� ����
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

    //�ǰݵ� ������ ����Ʈ �߻�
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

    //������ ���� �� ����
    public void OnAttackAnimationEnd()
    {
        anim.SetInteger("AttackType", 0);
        isAttack = false;
    }
    #endregion

    #region Hitted
    //����ϱ�
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
                //���
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

    //���� ������ �Լ�
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

    //������ �ִ����ӽð��� ����
    IEnumerator GuardDuration()
    {
        yield return new WaitForSeconds(parryingTime);
        isParrying = false;
        yield return new WaitForSeconds(maxGuardTime - parryingTime);
        OnGuardEnd();
    }

    //���� ���� ��Ÿ�� ����
    IEnumerator GuardCoolTime()
    {
        yield return new WaitForSeconds(guardCoolTime);
        canGuard = true;
    }

    //���� ������ ������ ����
    public void OnHit(float damage)
    {
        isAttack = false;

        if (isGuard)
        {
            if (isParrying)
            {
                //�и� ����
                anim.SetTrigger("Parring");
                cameraShake.VibrateForTime(0.2f, 0.05f, 2);
                panelEffect.OnParring();
                SoundManager.instance.PlaySfx(SoundManager.Sfx.Parring);
            }
            else
            {
                //�Ϲ� ����
                anim.SetTrigger("Block");
                cameraShake.VibrateForTime(0.1f, 0.05f, 3);
                GameManager.instance.currentHp -= damage / 2;
                SoundManager.instance.PlaySfx(SoundManager.Sfx.Guard);
            }
        }
        else
        {
            //�ǰ�
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

    //���ݹ����� sceneâ�� ǥ��(Visiable)
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(hitbox.position, hitbox.localScale);
    }
}