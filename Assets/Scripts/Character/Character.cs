using UnityEngine;

public class Character : MonoBehaviour
{
    protected Animator anim;   //애니메이터
    [SerializeField] protected AnimationClip[] attackAnimations;    //공격애니메이션 목록
    [SerializeField] protected Transform hitbox;   //공격판정을 일으킬 히트박스
    public Transform hitpoint;  //피격판정을 일으킬 지점

    public float damage;  //공격력
    public float health;  //체력
    public float _currentHP;  //현재 체력
    public float currentHP
    {
        get { return _currentHP; }
        protected set
        {
            _currentHP = value;
            if (_currentHP <= 0 && !isDie) 
            {
                isDie = true;
                anim.SetTrigger("Die");
            }
        }
    }

    public bool isDie { get; protected set; }

    public virtual void Init() { }

    public void GetDamage(float damage)
    {
        if (!isDie) { currentHP -= damage; }
    }

    public virtual void OnDie() { } 
}
