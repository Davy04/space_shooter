using System.Collections;
using Unity.Mathematics;
using UnityEngine;

public class Pistol : Gun
{

    public override void Update()
    {
        base.Update();

        if (Input.GetButtonDown("Fire1"))
        {
            TryShoot();
        }
        if(Input.GetKeyDown(KeyCode.R)) 
        {
            TryReload();
        }
    }

    public override void Shoot()
    {
        RaycastHit hit;
        Vector3 target = Vector3.zero;
        if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out hit, gunData.shootingRange,gunData.targetLayerMask)) 
        {
            Debug.Log(gunData.gunName + " hit " + hit.collider.name);
            target = hit.point;
        }
        else
        {
            target = cameraTransform.position + cameraTransform.forward * gunData.shootingRange;
        }
        StartCoroutine(Bulletfire(target, hit));
    }

    private IEnumerator Bulletfire(Vector3 target, RaycastHit hit)
    {
        GameObject bulletTrail = Instantiate(gunData.bulletTrailPrefab, gunMuzzle.position, quaternion.identity);
        while (bulletTrail != null && Vector3.Distance(bulletTrail.transform.position, target) > 0.1f)
        {
            bulletTrail.transform.position = Vector3.MoveTowards(bulletTrail.transform.position, target,
                Time.deltaTime * gunData.bulletSpeed);
            yield return null;
        }
        Destroy(bulletTrail);

        if (hit.collider != null)
        {
            BulletHitFX(hit);
        }
    }

    private void BulletHitFX(RaycastHit hit)
    {
        Vector3 hitPosition = hit.point + hit.normal * 0.01f;

        if (bulletHitParticlePrefab != null)
        {
            GameObject explosion = Instantiate(bulletHitParticlePrefab, hitPosition, Quaternion.identity);
            Destroy(explosion, 2f); // Destroi depois de 2 segundos (tempo da explosão)
        }
    }
}
