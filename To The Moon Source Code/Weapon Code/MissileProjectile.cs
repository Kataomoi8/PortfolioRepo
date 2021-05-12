using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class MissileProjectile : ProjectileBase
{
    //Variables to save data temporarly when the game is paused.
    Vector3 position;
    Vector3 velocity;
    Transform cachedTransform;

    //When the projectile is made it stores its base values and sets up its starting speed.
    void Start()
    {
        cachedTransform = transform;
        position = cachedTransform.position;
        float startSpeed = (stats.minSpeed + stats.maxSpeed) * 0.5f;
        velocity = transform.forward * startSpeed;
        Destroy(this.gameObject, stats.bulletLife);
    }

    //During update the missiles check for flares to see if its target will change.
    private void Update()
    {
        if (paused)
        {
            return;
        }

        Collider[] colliders = Physics.OverlapSphere(transform.position, stats.detectRadius, stats.mask);
        foreach (Collider nearObj in colliders)
        {
            if (nearObj.CompareTag(FlareLockTag))
            {
                target.GetComponent<MultiPlayer4>().CallTargetRPC(Player4Base.TargetType.NoTarget);
                setTarget(nearObj.gameObject);
                break;
            }
        }
    }

    //Fixed Update Controls all the steering behaviors.
    //It checks its position verse the targets positon then decides if it needs to turn or not based on vector math.
    void FixedUpdate()
    {
        if (paused)
        {
            return;
        }

        Vector3 acceleration = Vector3.zero;
        if (target != null)
        {
            Vector3 offsetToTarget = (target.transform.position - position);
            Vector3 outVec = offsetToTarget.normalized * stats.maxSpeed - velocity;
            outVec = Vector3.ClampMagnitude(outVec, stats.steerForceCap);

            acceleration += outVec * stats.targetWeight;
        }
        else
        {
            acceleration += stats.minSpeed * transform.forward;
        }

        velocity += acceleration * Time.deltaTime;
        float speed = velocity.magnitude;
        Vector3 dir = velocity / speed;

        speed = Mathf.Clamp(speed, stats.minSpeed, stats.maxSpeed);

        velocity = dir * speed;

        cachedTransform.position += velocity * Time.deltaTime;
        cachedTransform.forward = dir;
        position = cachedTransform.position;
    }

    //Based on what the projectile collides with it calls different functions
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.tag != null)
        {
            if (collision.transform.CompareTag(EnemyLockTag))
            {
                collision.transform.gameObject.GetComponent<AIUnit>().TakeDamage((int)stats.damage, shooter);
                Instantiate(stats.onHitEffect, collision.contacts[0].point, collision.transform.rotation);
                Destroy(this.gameObject);
            }

            if (collision.transform.CompareTag(AsterLockTag))
            {
                Instantiate(stats.onHitEffect, collision.contacts[0].point, collision.transform.rotation);
                Destroy(this.gameObject);
            }

            if (collision.transform.CompareTag(FlareLockTag))
            {
                Instantiate(stats.onHitEffect, collision.contacts[0].point, collision.transform.rotation);
                Destroy(collision.gameObject);
                Destroy(this.gameObject);
            }
            if (collision.transform.CompareTag(CruserPartLockTag))
            {
                collision.transform.gameObject.GetComponent<Cruiserpart>().TakeDMG(stats.damage);
                Instantiate(stats.onHitEffect, collision.contacts[0].point, collision.transform.rotation);
                Destroy(this.gameObject);
            }
        } 
    }

    //Used to check collision with the player in multiplayer since the player uses a different type of collider.
    private void OnTriggerEnter(Collider collision)
    {
        if (GameManagerBase.Instance.isMulti())
        {
            if (collision.transform.CompareTag(PlayerLockTag))
            {
                if (!M_isSelf(collision.transform.GetComponent<PhotonView>().ViewID))
                {
                    collision.gameObject.GetComponentInParent<MultiPlayer4>().M_TakeDamage(stats.damage, m_shooter);
                    Instantiate(stats.onHitEffect, this.transform.position, collision.transform.rotation);
                    Destroy(this.gameObject);
                }
            }
        }
    }





}
