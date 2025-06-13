using UnityEngine;
using UnityEngine.Events;

public class PlayerStats : MonoBehaviour
{
    [Header("Configuração de Vida")]
    [SerializeField] public int maxHealth = 100;
    public int currentHealth;

    public event UnityAction<int> OnHealthChanged;
    public event UnityAction OnDeath;
    public event UnityAction OnPlayerHit;

    private void Start()
    {
        currentHealth = maxHealth;
        OnHealthChanged?.Invoke(currentHealth);
    }
    
    public void ReceiveDamage(int damageAmount)
    {
        currentHealth -= damageAmount;
        currentHealth = Mathf.Max(currentHealth, 0);
        OnHealthChanged?.Invoke(currentHealth);
        OnPlayerHit?.Invoke();
        if (currentHealth <= 0) Die();
    }

    private void Die()
    {
        OnDeath?.Invoke();
    }
}