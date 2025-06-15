using UnityEngine;
using System.Collections.Generic;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance;

    private List<EnemyHealth> _enemies = new List<EnemyHealth>();
    private Timer _timer;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        _timer = FindObjectOfType<Timer>();
    }

    public void RegisterEnemy(EnemyHealth enemy)
    {
        _enemies.Add(enemy);
    }

    public void UnregisterEnemy(EnemyHealth enemy)
    {
        _enemies.Remove(enemy);
        CheckAllEnemiesDefeated();
    }

    private void CheckAllEnemiesDefeated()
    {
        if (_enemies.Count == 0 && _timer != null)
        {
            _timer.StopAndSaveTime();
        }
    }
}