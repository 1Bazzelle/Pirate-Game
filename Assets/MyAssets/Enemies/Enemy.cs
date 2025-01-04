using MoreMountains.Tools;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : Entity
{
    [SerializeField] protected float visionRange;
    [SerializeField] protected float forgetRange;

    private EnemyState curState;

    private NavMeshAgent agent;

    [SerializeField] private Route route;
    private Route myRoute;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
    }
    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();

        myRoute = gameObject.AddComponent<Route>();
        if (route != null)
        {
            myRoute.CopyValues(route);
            TransitionToState(new Patrolling());
        }
        else
        {
            TransitionToState(new Idle());
        }
    }
    protected override void Update()
    {
        base.Update();

        if (isAlive)
        {
            curState.UpdateState(this);
        }
    }

    public virtual void TransitionToState(EnemyState newState)
    {
        curState?.ExitState(this);
        curState = newState;
        curState.EnterState(this);
    }

    public GameObject PlayerInLineOfSight()
    {
        foreach (Collider collider in Physics.OverlapSphere(transform.position, visionRange, Constants.EntityLayer))
        {
            if (collider.CompareTag(Constants.PlayerTag))
            {
                // Check line of sight to player center first
                if (Physics.Raycast(transform.position, collider.transform.position - transform.position, out RaycastHit hit))
                {
                    if (hit.collider.CompareTag(Constants.PlayerTag))
                    {
                        return hit.collider.gameObject;
                    }
                }
                // Then front end of the ship
                if (Physics.Raycast(transform.position, (collider.transform.position + collider.transform.forward * 0.9f * (collider.bounds.size.z/2)) - transform.position, out hit))
                {
                    if (hit.collider.CompareTag(Constants.PlayerTag))
                    {
                        return hit.collider.gameObject;
                    }
                }
                // Then back end of the ship
                if (Physics.Raycast(transform.position, (collider.transform.position - collider.transform.forward * 0.9f * (collider.bounds.size.z / 2)) - transform.position, out hit))
                {
                    if (hit.collider.CompareTag(Constants.PlayerTag))
                    {
                        return hit.collider.gameObject;
                    }
                }
            }
        }
        return null;
    }
    public virtual bool PlayerForgotten()
    {
        // Technically this could detect another player and it would keep running away, but whatever
        foreach (Collider collider in Physics.OverlapSphere(transform.position, forgetRange, Constants.EntityLayer))
        {
            if (collider.CompareTag(Constants.PlayerTag)) return false;
        }
        return true;
    }
    public void SetDestination(Vector3 destination)
    {
        agent.SetDestination(new Vector3(destination.x, 0, destination.z));
    }

    public NavMeshAgent GetAgent()
    {
        return agent;
    }

    public Route GetRoute()
    {
        return myRoute;
    }
}
