using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

public class Enemy : MonoBehaviour
{
    protected Transform _target;
    public string IDLE = "Anim_Idle";
    public string RUN = "Anim_Run";
    public string ATTACK = "Anim_Attack";
    public string DAMAGE = "Anim_Damage";
    public string DEATH = "Anim_Death";
    private Animation _anim;
    protected float _speed;
    [SerializeField] protected float _health;
    protected List<Material> mats = new List<Material>();
    public bool isAirBorn;
    public delegate void EnemyAction(Enemy e);
    public static event EnemyAction OnEnemyDestroyed;
    public static event EnemyAction OnEnemySpawned;
    public float damageAmount { get; private set; }
    public static float minPlayerDistance = 2f;
    [SerializeField] private Transform skeletonPivot;
    [SerializeField] private SphereCollider _collider;

    private void Start()
    {
        _anim = GetComponent<Animation>();
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        foreach (Renderer r in renderers)
        {
            mats.Add(r.material);
        }
    }

    protected void Update()
    {
        _collider.center = skeletonPivot.localPosition;
        if (Vector3.Distance(transform.position, _target.position) < minPlayerDistance)
            return;
        transform.DOLookAt(_target.position, 0f, AxisConstraint.Y);
        Vector3 trueDirection = (_target.position - transform.position).normalized;
        Vector3 flatDirection = new Vector3(trueDirection.x, transform.position.y, trueDirection.z);
        if (isAirBorn)
            transform.DOMove(transform.position + trueDirection * Time.deltaTime, 0f);
        else
            transform.DOMove(transform.position + flatDirection * Time.deltaTime, 0f);
    }

    public Enemy Init(float health , float speed, float damage)
    {
        _target = Player.S.transform;
        transform.LookAt(_target);
        PlayAnimation(ATTACK);
        _speed = speed;
        _health = health;
        damageAmount = damage;
        OnEnemySpawned?.Invoke(this);
        return this;
    }

    public void TakeDamage(float damage)
    {
        _health -= damage;
        PlayDamageEffect();
        if (_health <= 0)
            StartCoroutine(Die());
    }

    public void DestroyEnemyWithoutEffect()
    {
        OnEnemyDestroyed?.Invoke(this);
        Destroy(gameObject);
    }

    private IEnumerator Die()
    {
        PlayAnimation(DEATH);
        yield return new WaitForSeconds(1f);
        MeshExploder[] explosions = GetComponentsInChildren<MeshExploder>();
        foreach (MeshExploder mx in explosions)
        {
            mx.Explode();
        }
        OnEnemyDestroyed?.Invoke(this);
        Destroy(gameObject);
    }

    protected virtual void PlayAnimation(string animationName)
    {
        if (_anim)
            _anim.CrossFade(animationName);
    }

    private void PlayDamageEffect()
    {
        Sequence seq = DOTween.Sequence();

        foreach (Material mat in mats)
        {
            Color initialColor = mat.color;

            seq.Append(mat.DOColor(Color.black, 0.2f));
            seq.Append(mat.DOColor(initialColor, 0.2f));
        }
        seq.Play();
    }
    
}
