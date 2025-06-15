using UnityEngine;
using UnityEngine.Events;

public class EnemyHealth : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private int maxHealth = 100;

    public UnityEvent OnDamageTaken;
    public UnityEvent OnDeath;

    private int currentHealth;
    private bool isDead = false;

    private void Start()
    {
        EnemyManager.Instance.RegisterEnemy(this);
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damageAmount)
    {
        if (isDead) return;

        currentHealth -= damageAmount;
        OnDamageTaken?.Invoke();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        EnemyManager.Instance.UnregisterEnemy(this);
        isDead = true;
        OnDeath?.Invoke();
        Destroy(gameObject); 
    }
}