using UnityEngine;
using Unity.Netcode;
using MoreMountains.Feedbacks;

[System.Serializable]
public struct EntityStats
{
    public float maxHealth, regen, damage, reloadSpeed, fireRate, shootForce, shootAngle, cannonInaccuracy;
    public EntityMoveStats moveStats;
}

[System.Serializable]
public struct EntityMoveStats
{
    public float maxMoveSpeed;
    public float forwardAcceleration, backwardAcceleration;
    public float deceleration;
    public float maxRotationSpeed, rotationAcceleration, rotationDeceleration;
}
public class Entity : NetworkBehaviour, IDamageTaker
{
    [SerializeField] private ShipStats baseStats;
    protected EntityStats stats;
    protected float health;
    protected bool isAlive;
    private float regenTimer = 0;

    [SerializeField] private GameObject deathParticles;

    private ShipMode curMode;

    private void Awake()
    {
        deathParticles.GetComponent<MMFeedbacks>().Initialization(gameObject);
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        baseStats.InitializeStats(ref stats);

        health = stats.maxHealth;
        isAlive = true;

        curMode = new BalancedMode();
        curMode.EnterState(ref stats);
    }

    protected virtual void Update()
    {
        if (isAlive)
        {
            transform.position = new Vector3(transform.position.x, 0, transform.position.z);

            if(regenTimer > 1/stats.regen)
            {
                health += stats.regen;
                regenTimer = 0;
            }
            regenTimer += Time.deltaTime;
        }
    }

    public void ChangeMode(ShipMode newMode)
    {
        curMode?.ExitState(ref stats);
        curMode = newMode;
        curMode.EnterState(ref stats);
    }

    public virtual void TakeDamage(float damage, Vector3 dir)
    {
        if (IsServer && isAlive)
        { 
            health -= damage;
            
            if (health <= 0)
            {
                isAlive = false;
                OnDeath();
            }
            TakenDamageClientRpc(damage);
        }
        if (!isAlive)
            health = 0;
    }

    [ClientRpc]
    protected void TakenDamageClientRpc(float damage)
    {
        health -= damage;
        if (health <= 0)
        {
            isAlive = false;
            OnDeath();
        }
    }

    protected virtual void OnDeath()
    {
        deathParticles.GetComponent<MMFeedbacks>().PlayFeedbacks(transform.position);

    }

    public void Despawn()
    {
        if(IsServer)
        gameObject.GetComponent<NetworkObject>().Despawn(true);
    }

    public EntityStats GetStats()
    {
        return stats;
    }
    public float GetHealth()
    {
        return health;
    }
}
