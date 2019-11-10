using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public AudioClip spawnSound;
    public AudioClip destroySound;
    private void OnEnable()
    {
        //Enemy.OnEnemyDestroyed
    }

    private void OnDisable()
    {
        
    }

    private void HandleEnemyDestroyed(Enemy e)
    {
        
    }

    private void HandleEnemySpawned(Enemy e)
    {

    }
}
