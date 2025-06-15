using UnityEngine;

public class EnemyAnimations : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Animator animator;
    [SerializeField] private EnemyController enemyController;
    [SerializeField] private EnemyHealth enemyHealth;

    private void Awake()
    {
        enemyController.OnMovementStateChanged += HandleMovementState;
        enemyController.OnAttack += HandleAttack;
    }

    private void HandleMovementState(bool isMoving)
    {
        animator.SetBool("isMoving", isMoving);
    }

    private void HandleAttack()
    {
        animator.SetTrigger("Attack");
    }

    private void OnDestroy()
    {
        enemyController.OnMovementStateChanged -= HandleMovementState;
        enemyController.OnAttack -= HandleAttack;
    }
}