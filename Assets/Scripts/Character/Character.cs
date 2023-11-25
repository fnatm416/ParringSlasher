using UnityEngine;

public class Character : MonoBehaviour
{
    protected Animator anim;   //�ִϸ�����
    [SerializeField] protected AnimationClip[] attackAnimations;    //���ݾִϸ��̼� ���
    [SerializeField] protected Transform hitbox;   //���������� ����ų ��Ʈ�ڽ�
    public Transform hitpoint;  //�ǰ������� ����ų ����

    public float damage;  //���ݷ�
    public float health;  //ü��
    public float _currentHP;  //���� ü��
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
