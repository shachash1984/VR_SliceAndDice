using System;
using System.Collections;
using System.Collections.Generic;
using OVR;
using UnityEngine;
using Random = UnityEngine.Random;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    static public Player S;

    [SerializeField] LayerMask _layerMask;
    [SerializeField] int _enemyLayer = 9;

    public const float MAX_HEALTH = 100f;
    private float _health = 100;
    private int _kills;
    private float _safeTimer;
    public float safeTime = 2f;
    public static event Action<float> OnPlayerTakeDamage;
    public static event Action OnPlayerDied;
    //public Button startButton;
    public Camera playerCam;
    public bool active = true;

    private void Awake()
    {
        if (S != null)
            Destroy(gameObject);
        S = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        ResetSafeTimer();
    }

    void Update()
    {
        if (!active)
            return;
        if (Input.GetMouseButtonDown(0) || OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger))
        {
            Shoot();
                
        }
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.Space))
            SceneManager.LoadScene(1);
#endif


    }

    void ResetSafeTimer()
    {
        _safeTimer = Time.time;
    }

    void Shoot()
    {
        RaycastHit hit;
        if (Physics.Raycast(playerCam.transform.position, playerCam.transform.forward, out hit, 100f, _layerMask))
        {
            Enemy enemy = hit.collider.GetComponent<Enemy>();
            if (enemy)
            {
                float damage = Random.Range(10, 51);
                enemy.TakeDamage(damage);
            }
        }
    }

    private void TakeDamage(float damage)
    {
        _health -= damage;
        if (_health <= 0)
            Die();
    }

    private void Die()
    {
        active = false;
        OnPlayerDied?.Invoke();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!active)
            return;
        if(Time.time - safeTime > _safeTimer)
        {
            Enemy e = collision.collider.GetComponent<Enemy>();
            if (e)
            {
                TakeDamage(e.damageAmount);
                OnPlayerTakeDamage?.Invoke(_health);
                e.DestroyEnemyWithoutEffect();
            }
        }
    }
}
