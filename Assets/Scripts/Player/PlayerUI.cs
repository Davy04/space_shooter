using UnityEngine;
using TMPro;
using UnityEngine.UI; // Necessário para acessar Image
using System.Collections;

public class PlayerUI : MonoBehaviour
{
    [Header("Referências")]
    [SerializeField] private Gun playerGun;
    [SerializeField] private TMP_Text playerAmmoText;
    [SerializeField] private PlayerStats playerHealth;
    [SerializeField] private TMP_Text playerHealthText;
    [SerializeField] private Image hitEffect;

    [Header("Configurações do Efeito de Dano")]
    [SerializeField] private float hitEffectDuration = 0.3f;
    [SerializeField] private float hitEffectFadeSpeed = 3f;

    private Coroutine healthUpdateCoroutine;
    private Coroutine hitEffectCoroutine;

    private void OnEnable()
    {
        if (playerGun != null)
        {
            playerGun.OnAmmoChanged += UpdateAmmoUI;
            UpdateAmmoUI();
        }

        if (playerHealth != null)
        {
            playerHealth.OnHealthChanged += OnHealthChangedHandler;
            playerHealth.OnPlayerHit += TriggerHitEffect;
            UpdateHealthUI(playerHealth.currentHealth);
        }

        if (hitEffect != null)
        {
            hitEffect.color = new Color(hitEffect.color.r, hitEffect.color.g, hitEffect.color.b, 0);
        }
    }

    private void OnDisable()
    {
        if (playerGun != null)
        {
            playerGun.OnAmmoChanged -= UpdateAmmoUI;
        }

        if (playerHealth != null)
        {
            playerHealth.OnHealthChanged -= OnHealthChangedHandler;
            playerHealth.OnPlayerHit -= TriggerHitEffect;
        }

        if (healthUpdateCoroutine != null) StopCoroutine(healthUpdateCoroutine);
        if (hitEffectCoroutine != null) StopCoroutine(hitEffectCoroutine);
    }

    // Atualiza a munição
    public void UpdateAmmoUI()
    {
        if (playerGun != null && playerAmmoText != null)
        {
            playerAmmoText.text = $"{playerGun.gunData.magazineSize}/{playerGun.currentAmo}";
        }
    }

    // Handler para atualizar a vida com delay
    private void OnHealthChangedHandler(int currentHealth)
    {
        if (healthUpdateCoroutine != null) StopCoroutine(healthUpdateCoroutine);
        healthUpdateCoroutine = StartCoroutine(UpdateHealthWithDelay(currentHealth));
    }

    private IEnumerator UpdateHealthWithDelay(int currentHealth)
    {
        yield return new WaitForSeconds(1f);
        UpdateHealthUI(currentHealth);
    }

    // Atualiza a vida
    public void UpdateHealthUI(int currentHealth)
    {
        if (playerHealthText != null)
        {
            playerHealthText.text = $"{currentHealth}/{playerHealth.maxHealth}";
        }
    }

    // Dispara o efeito de dano
    private void TriggerHitEffect()
    {
        if (hitEffectCoroutine != null) StopCoroutine(hitEffectCoroutine);
        hitEffectCoroutine = StartCoroutine(ShowHitEffect());
    }

    // Mostra o efeito de dano (aparece e some gradualmente)
    private IEnumerator ShowHitEffect()
    {
        if (hitEffect == null) yield break;

        // Define a cor inicial (vermelho semi-transparente)
        Color effectColor = hitEffect.color;
        effectColor.a = 0.3f; // Alpha inicial (30% visível)
        hitEffect.color = effectColor;

        // Espera um curto tempo antes de começar a desaparecer
        yield return new WaitForSeconds(hitEffectDuration);

        // Faz o efeito desaparecer gradualmente
        while (hitEffect.color.a > 0)
        {
            effectColor.a -= Time.deltaTime * hitEffectFadeSpeed;
            hitEffect.color = effectColor;
            yield return null;
        }

        // Garante que fique totalmente transparente
        effectColor.a = 0;
        hitEffect.color = effectColor;
    }
}