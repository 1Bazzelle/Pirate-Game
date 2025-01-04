using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using MoreMountains.Feedbacks;

public class Warship : Enemy
{
    [SerializeField] private CannonPositions cannonPositions;
    [SerializeField] private float attackRange;
    [SerializeField] private string projectileID;

    [SerializeField] private GameObject fireCannonEffect;

    private EnemyCannonFire cannons;

    // For raycasting
    private Dictionary<string, Transform[]> frontBackCannons;

    #region AI stuff

    private bool cannonsActive;
    private GameObject persecuting;

    #endregion
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        frontBackCannons = new();

        Transform[] rightFrontBack = new Transform[] { cannonPositions.rightCannons[0], cannonPositions.rightCannons[cannonPositions.rightCannons.Count - 1] };
        Transform[] leftFrontBack = new Transform[] { cannonPositions.leftCannons[0], cannonPositions.leftCannons[cannonPositions.leftCannons.Count - 1] };
        frontBackCannons.Add("right", rightFrontBack);
        frontBackCannons.Add("left", leftFrontBack);

        cannons = new();
        cannons.Initialize(projectileID, cannonPositions, frontBackCannons, attackRange);

        fireCannonEffect.GetComponent<MMFeedbacks>().Initialization(gameObject);
    }

    protected override void Update()
    {
        base.Update();
        
        if (isAlive)
        {
            if (cannonsActive) cannons.Update(transform, stats, fireCannonEffect);
        }
    }

    public GameObject PlayerInAttackRange()
    {
        GameObject player = null;

        foreach (Collider collider in Physics.OverlapSphere(transform.position, attackRange, Constants.EntityLayer))
        {
            if(player == null && collider.CompareTag(Constants.PlayerTag)) player = collider.gameObject;
            if (player != null && collider.CompareTag(Constants.PlayerTag) && 
                Vector3.Distance(player.transform.position, transform.position) > Vector3.Distance(collider.transform.position, transform.position))
            player = collider.gameObject;
        }
        return player;
    }

    public void ActivateCannons()
    {
        cannonsActive = true;
    }
    public void DeactivateCannons()
    {
        cannonsActive = false;
    }

    public float GetAttackRange()
    {
        return attackRange;
    }

    public GameObject GetPersecuting()
    {
        return persecuting;
    }
    public void SetPersecuting(GameObject player)
    {
        persecuting = player;
    }

    public override bool PlayerForgotten()
    {
        return Vector3.Distance(persecuting.transform.position, transform.position) > forgetRange;
    }
}
