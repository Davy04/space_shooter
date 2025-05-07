using System;
using System.Collections;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float detectionRange;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float damage;
    [SerializeField] private Transform model;
    [SerializeField] private Animator animator;
    private bool isMoving = false;

    private void Update()
    {
        float distanceToTarget = Vector3.Distance(transform.position, target.position);
        if (!isMoving && distanceToTarget <= detectionRange)
        {
            StartCoroutine(MoveToTarget());
        }
        
    }

    private IEnumerator MoveToTarget()
    {
        isMoving = true;
        animator?.SetBool("isMoving", true);
        float distanceToTarget = Vector3.Distance(transform.position, target.position);

        while (distanceToTarget <= detectionRange)
        {
            Vector3 direction = (target.position - transform.position).normalized;
            direction.y = 0f;
            if (direction != Vector3.zero)
            {
                Quaternion lookRotation = Quaternion.LookRotation(direction);
                model.rotation = Quaternion.Slerp(model.rotation, lookRotation, Time.deltaTime * 5f);
            }
            transform.position = Vector3.MoveTowards(
                transform.position,
                target.position,
                moveSpeed * Time.deltaTime
            );

            yield return null;
            distanceToTarget = Vector3.Distance(transform.position, target.position);
        }

        isMoving = false;
        animator?.SetBool("isMoving", false);
    }

    
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}
