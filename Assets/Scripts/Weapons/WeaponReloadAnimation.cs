using UnityEngine;

public class WeaponReloadAnimation : MonoBehaviour
{
    [Header("Movimento")]
    public Vector3 reloadOffset = new Vector3(0f, -0.2f, -0.1f);
    public float reloadSpeed = 6f;
    public float reloadDuration = 0.5f;

    [Header("Rotação")]
    public Vector3 reloadRotationOffset = new Vector3(15f, 20f, 0f);

    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private Vector3 targetPosition;
    private Quaternion targetRotation;

    private bool isReloading = false;
    private float reloadTimer = 0f;

    void Start()
    {
        originalPosition = transform.localPosition;
        originalRotation = transform.localRotation;
        targetPosition = originalPosition;
        targetRotation = originalRotation;
    }

    void Update()
    {
        if (isReloading)
        {
            reloadTimer += Time.deltaTime;

            if (reloadTimer < reloadDuration / 2f)
            {
                targetPosition = originalPosition + reloadOffset;
                targetRotation = originalRotation * Quaternion.Euler(reloadRotationOffset);
            }
            else
            {
                targetPosition = originalPosition;
                targetRotation = originalRotation;
            }

            if (reloadTimer >= reloadDuration)
            {
                isReloading = false;
                reloadTimer = 0f;
                targetPosition = originalPosition;
                targetRotation = originalRotation;
            }
        }
        transform.localPosition = Vector3.Lerp(transform.localPosition, targetPosition, Time.deltaTime * reloadSpeed);
        transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, Time.deltaTime * reloadSpeed);
    }

    public void PlayReloadAnimation()
    {
        isReloading = true;
        reloadTimer = 0f;
    }
}