using System.Collections;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [Header("Target Settings")]
    [SerializeField] private Transform target;
    
    [Header("Movement Settings")]
    [SerializeField] private float detectionRange = 5f;
    [SerializeField] private float moveSpeed = 3f;
    
    [Header("Attack Settings")]
    [SerializeField] private float attackRange = 1.5f;
    [SerializeField] private int damage = 10;
    [SerializeField] private float attackCooldown = 1f;
    [SerializeField] private float attackDelay = 0.5f;
    
    [Header("References")]
    [SerializeField] private Transform model;
    [SerializeField] private Animator animator;
    
    [Header("Damage Settings")]
    [SerializeField] private PlayerStats playerStats; 
    
    private bool isMoving = false;
    private bool isAttacking = false;
    private float initialY;
    private Coroutine attackCoroutine;

    private void Start()
    {
        initialY = transform.position.y;
    }

    private void Update()
    {
        if (isAttacking) return;

        float distanceToTarget = Vector3.Distance(transform.position, target.position);
        
        if (distanceToTarget <= attackRange)
        {
            if (attackCoroutine == null)
            {
                attackCoroutine = StartCoroutine(Attack());
            }
        }
        else if (!isMoving && distanceToTarget <= detectionRange)
        {
            StartCoroutine(MoveToTarget());
        }
    }

    private IEnumerator MoveToTarget()
    {
        isMoving = true;
        animator?.SetBool("isMoving", true);

        while (Vector3.Distance(transform.position, target.position) <= detectionRange && 
               Vector3.Distance(transform.position, target.position) > attackRange &&
               !isAttacking)
        {
            Vector3 targetPosition = new Vector3(
                target.position.x,
                initialY,
                target.position.z
            );

            Vector3 direction = (targetPosition - transform.position).normalized;
            
            if (direction != Vector3.zero)
            {
                Quaternion lookRotation = Quaternion.LookRotation(direction);
                model.rotation = Quaternion.Slerp(model.rotation, lookRotation, Time.deltaTime * 5f);
            }

            transform.position = Vector3.MoveTowards(
                transform.position,
                targetPosition,
                moveSpeed * Time.deltaTime
            );

            yield return null;
        }

        isMoving = false;
        animator?.SetBool("isMoving", false);
    }

    private IEnumerator Attack()
    {
        isAttacking = true;
        animator?.SetTrigger("Attack");
    
        yield return new WaitForSeconds(attackDelay);
    
        if (Vector3.Distance(transform.position, target.position) <= attackRange)
        {
            playerStats?.ReceiveDamage(damage);
        }
    
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length - attackDelay + attackCooldown);
    
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