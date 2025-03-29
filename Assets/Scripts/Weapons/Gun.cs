using System.Collections;
using UnityEngine;

public abstract class Gun : MonoBehaviour
{
    public GunData gunData;
    [HideInInspector] public PlayerController playerController;
    [HideInInspector] public Transform cameraTransform;

    private float currentAmo = 0f;
    private float nextTimeToFire = 0f;

    private bool isReloading = false;

    private void Start()
    {
        currentAmo = gunData.magazineSize;

        playerController = transform.root.GetComponent<PlayerController>();
        cameraTransform = playerController.virtualCamera.transform;
    }

    public virtual void Update()
    {
        playerController.ResetRecoil(gunData);
    }

    public void TryReload()
    {
        if (!isReloading && currentAmo < gunData.magazineSize)
        {
            StartCoroutine(Reload());
        }
    }

    private IEnumerator Reload()
    {
        isReloading = true;
        Debug.Log(gunData.gunName + "is Reloading");
        
        yield return new WaitForSeconds(gunData.reloadTime);

        currentAmo = gunData .magazineSize;
        isReloading = false;

        Debug.Log(gunData.gunName + "is Reloaded");
    }

    public void TryShoot()
    {
        if (isReloading) 
        {
            Debug.Log("is reloading");
            return;
        }

        if (currentAmo <= 0)
        {
            Debug.Log("Ta sem bala");
        }

        if (Time.time >= nextTimeToFire)
        {
            nextTimeToFire = Time.time + (1/ gunData.fireRate);
            HandleShoot();
        }
    }

    private void HandleShoot()
    {
        currentAmo--;
        Debug.Log(currentAmo + "balas restantes");
        Shoot();

        playerController.ApplyRecoil(gunData);
    }

    public abstract void Shoot();
}
