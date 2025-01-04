using JetBrains.Annotations;
using MoreMountains.Feedbacks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : FromPool
{
    protected float damage;
    protected float lifetime;
    protected Rigidbody rb;
    [SerializeField] protected GameObject onCollisionEffect;

    protected virtual void OnEnable()
    {
        onCollisionEffect.GetComponent<MMFeedbacks>().Initialization(gameObject);

        rb = GetComponent<Rigidbody>();
        lifetime = 5;

        GetComponent<BoxCollider>().enabled = true;
        GetComponent<MeshRenderer>().enabled = true;
    }

    protected virtual void OnCollisionEnter(Collision collision)
    {
        rb.isKinematic = true;

        if (collision.collider.CompareTag(Constants.WaterTag) || collision.collider.CompareTag(Constants.EnvironmentTag))
            ReturnToPool();

        IDamageTaker damageTaker = collision.gameObject.GetComponent<IDamageTaker>();
        damageTaker?.TakeDamage(damage, rb.transform.position + rb.transform.forward - rb.transform.position);

        // Remember to put ReturnToPool into the Completed Event of the feedback
        if (damageTaker != null)
        {
            rb.velocity = Vector3.zero;

            GetComponent<BoxCollider>().enabled = false;
            GetComponent<MeshRenderer>().enabled = false;

            onCollisionEffect.transform.position = transform.position;
            onCollisionEffect.transform.LookAt(transform.position - transform.forward);
            onCollisionEffect.GetComponent<MMFeedbacks>().PlayFeedbacks(transform.position);
        }
        else ReturnToPool();
    }

    public void SetProjectileDamage(float dmg)
    {
        damage = dmg;
    }

    public void Launch(Vector3 position, Vector3 velocity)
    {
        rb.isKinematic = false;

        transform.LookAt(position + velocity);
        transform.position = position;
        rb.velocity = velocity;

        Invoke(nameof(ReturnToPool), lifetime);
    }

    public override void ReturnToPool()
    {
        ObjectPool.Instance.ReturnToPool(objectTag, gameObject);
    }
}
