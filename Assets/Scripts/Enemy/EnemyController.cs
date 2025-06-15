using UnityEngine;
using System.Collections;
using System;

public class EnemyController : MonoBehaviour
{
    [Header("Target Settings")]
    [SerializeField] private Transform target;
    [SerializeField] private float heightIgnoreThreshold = 1.5f;

    [Header("Movement Settings")]
    [SerializeField] private float detectionRange = 5f;
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float rotationSpeed = 5f;

    [Header("Attack Settings")]
    [SerializeField] private float attackRange = 1.5f;
    [SerializeField] private int damage = 10;
    [SerializeField] private float attackCooldown = 1f;
    [SerializeField] private float attackDelay = 0.5f;

    [Header("References")]
    [SerializeField] private Transform model;
    [SerializeField] private PlayerStats playerStats;

    public event Action<bool> OnMovementStateChanged;
    public event Action OnAttack;

    private bool isMoving = false;
    private bool isAttacking = false;
    private Coroutine attackCoroutine;

    private void Update()
    {
        if (isAttacking) return;

        Vector3 targetPosition = GetFlattenedTargetPosition();
        float distanceToTarget = Vector3.Distance(transform.position, targetPosition);

        if (distanceToTarget <= attackRange)
        {
            if (attackCoroutine == null)
            {
                attackCoroutine = StartCoroutine(AttackRoutine());
            }
        }
        else if (!isMoving && distanceToTarget <= detectionRange)
        {
            StartCoroutine(MoveToTargetRoutine());
        }
    }

    private Vector3 GetFlattenedTargetPosition()
    {
        // Ignora completamente a diferença de altura no cálculo de direção
        return new Vector3(target.position.x, transform.position.y, target.position.z);
    }

    private IEnumerator MoveToTargetRoutine()
    {
        isMoving = true;
        OnMovementStateChanged?.Invoke(true);

        while (true)
        {
            Vector3 targetPosition = GetFlattenedTargetPosition();
            float distanceToTarget = Vector3.Distance(transform.position, targetPosition);

            if (distanceToTarget > detectionRange || distanceToTarget <= attackRange || isAttacking)
                break;

            // Calcula direção apenas no eixo XZ
            Vector3 direction = (targetPosition - transform.position).normalized;

            // Rotação apenas no eixo Y
            if (direction != Vector3.zero)
            {
                Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
                model.rotation = Quaternion.Slerp(
                    model.rotation,
                    lookRotation,
                    rotationSpeed * Time.deltaTime
                );
            }

            // Movimento mantendo a posição Y original
            transform.position = Vector3.MoveTowards(
                transform.position,
                targetPosition,
                moveSpeed * Time.deltaTime
            );

            yield return null;
        }

        isMoving = false;
        OnMovementStateChanged?.Invoke(false);
    }

    private IEnumerator AttackRoutine()
    {
        isAttacking = true;
        OnAttack?.Invoke();

        yield return new WaitForSeconds(attackDelay);

        if (Vector3.Distance(transform.position, GetFlattenedTargetPosition()) <= attackRange)
        {
            playerStats?.ReceiveDamage(damage);
        }

        yield return new WaitForSeconds(attackCooldown);

        isAttacking = false;
        attackCoroutine = null;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}