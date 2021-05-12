using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using System.IO;
using UnityEngine;

public class MissileShotWeapon : WeaponBase
{
    [Header("Debug Variables")]
    [SerializeField] GameObject target;
    [SerializeField] string targetTag;

    /*
     * If the weapon is able to fire again it will SphereCast with the missile until it hits the max distance to see if it finds a target.
     * If a target is found it tells the missile to track that object.
     * Multiplayer Needs to call and rpc when shot so that the whole network knows who shot that missile.
     */
    public override void MultiplayerFire()
    {
        if (rateOfFire >= stats.fireCoolDown)
        {
            RaycastHit hit;
            if (Physics.SphereCast(gunOrigin.transform.position, stats.collisionBoundsRadius, CrossHairTransform.forward, out hit, stats.LockOnDistance))
            {
                if (hit.transform.gameObject.CompareTag(stats.PlayerLockTag))
                {
                    if (hit.transform.gameObject.GetComponent<PhotonView>().ViewID != shipMain.getPlayerMain().gameObject.GetComponent<PhotonView>().ViewID)
                    {
                        target = hit.transform.gameObject;
                        targetTag = hit.transform.gameObject.tag;
                    }
                }
            }

            GameObject miss = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", stats.projectilePrefabMulti.name), gunOrigin.transform.position, Quaternion.LookRotation(CrossHairTransform.forward));
            miss.GetComponent<ProjectileBase>().callRPC(shipMain);
            miss.GetComponent<ProjectileBase>().setTarget(target);
            if (target != null)
            {
                target.GetComponent<MultiPlayer4>().CallTargetRPC(Player4Base.TargetType.Missile);
            }
            target = null;
            rateOfFire -= rateOfFire;
        }
    }

    public override void SingleplayerFire()
    {
        if (rateOfFire >= stats.fireCoolDown && !paused)
        {
            RaycastHit hit;
            if (Physics.SphereCast(gunOrigin.transform.position, stats.collisionBoundsRadius, CrossHairTransform.forward, out hit, stats.LockOnDistance))
            {
                if (hit.transform.CompareTag(stats.EnemyLockTag))
                {
                    target = hit.transform.gameObject;
                }
            }

            GameObject miss = Instantiate(stats.projectilePrefabSingle, gunOrigin.transform.position, Quaternion.LookRotation(CrossHairTransform.forward));
            miss.GetComponent<ProjectileBase>().setShooter(shipMain.getPlayerMain().gameObject);
            miss.GetComponent<ProjectileBase>().setTarget(target);
            target = null;
            rateOfFire -= rateOfFire;

        }
    }
}
