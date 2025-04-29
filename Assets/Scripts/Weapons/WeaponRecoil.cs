using UnityEngine;

public class WeaponRecoil : MonoBehaviour
{
    [Header("Recoil Settings")]
    public Vector3 recoilKickBack = new Vector3(0f, 0f, -0.1f);
    public float recoilSpeed = 10f;
    public float returnSpeed = 20f;

    private Vector3 originalPosition;
    private Vector3 currentRecoil;

    private void Start()
    {
        originalPosition = transform.localPosition;
    }

    private void Update()
    {
        currentRecoil = Vector3.Lerp(currentRecoil, Vector3.zero, returnSpeed * Time.deltaTime);
        transform.localPosition = originalPosition + currentRecoil;
    }

    public void ApplyRecoil()
    {
        currentRecoil += recoilKickBack;
    }
}