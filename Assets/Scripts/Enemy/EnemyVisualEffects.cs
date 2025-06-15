using System.Collections;
using UnityEngine;

public class EnemyVisualEffects : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Renderer enemyRenderer;
    [SerializeField] private EnemyHealth enemyHealth;
    [SerializeField] private Animator animator;

    [Header("Hit Effects")]
    [SerializeField] private Color hitFlashColor = Color.red;
    [SerializeField] private float hitFlashDuration = 0.1f;

    [Header("Death Effects")]
    [SerializeField] private GameObject explosionPrefab;
    [SerializeField] private float explosionDuration = 2f;

    private Color originalColor;

    private void Awake()
    {
        originalColor = enemyRenderer.material.color;

        enemyHealth.OnDamageTaken.AddListener(HandleDamageTaken);
        enemyHealth.OnDeath.AddListener(HandleDeath);
    }

    private void HandleDamageTaken()
    {
        StartCoroutine(HitFlash());
    }

    private IEnumerator HitFlash()
    {
        enemyRenderer.material.color = hitFlashColor;
        yield return new WaitForSeconds(hitFlashDuration);
        enemyRenderer.material.color = originalColor;
    }

    private void HandleDeath()
    {
        enemyRenderer.enabled = false;
        GetComponent<Collider>().enabled = false;

      
        if (explosionPrefab != null)
        {
            GameObject explosion = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
            Destroy(explosion, explosionDuration);
        }
       
        Destroy(gameObject, 2f);
    }
}