using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWithAnimator : Enemy
{
    private Animator _animator;

    private void Start()
    {
        _animator = GetComponent<Animator>();
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        foreach (Renderer r in renderers)
        {
            mats.Add(r.material);
        }
    }


    protected override void PlayAnimation(string animationName)
    {
        if (_animator)
            _animator.Play(animationName);
    }
}
