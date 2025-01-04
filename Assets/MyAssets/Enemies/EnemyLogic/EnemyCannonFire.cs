using MoreMountains.Feedbacks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class EnemyCannonFire
{
    private string projectileID;

    private float attackRange;

    private float rightFireTimeElapsed;
    private float leftFireTimeElapsed;

    private float rightReloadTimeElapsed;
    private float leftReloadTimeElapsed;

    private List<Transform> rightLoadedCannons;
    private List<Transform> rightUnloadedCannons;

    private List<Transform> leftLoadedCannons;
    private List<Transform> leftUnloadedCannons;

    private Dictionary<string, Transform[]> frontBackRaycast = new();

    private bool shootingRight;
    private bool shootingLeft;
    public void Initialize(string projID, CannonPositions cannonPositions, Dictionary<string, Transform[]> frontBackCannon, float attackRange)
    {
        this.attackRange = attackRange;

        projectileID = projID;

        rightLoadedCannons = cannonPositions.rightCannons;
        leftLoadedCannons = cannonPositions.leftCannons;

        rightUnloadedCannons = new();
        leftUnloadedCannons = new();

        rightReloadTimeElapsed = 0;
        leftReloadTimeElapsed = 0;

        rightFireTimeElapsed = 0;
        leftFireTimeElapsed = 0;

        frontBackRaycast = frontBackCannon;
    }
    public void Update(Transform enemyTransform, EntityStats stats, GameObject fireCannonEffect)
    {
        if (PlayerInCannonViewRight(enemyTransform))
            shootingRight = true;
        else
            shootingRight = false;
        if (PlayerInCannonViewLeft(enemyTransform))
            shootingLeft = true;
        else
            shootingLeft = false;

        if (shootingRight && rightFireTimeElapsed > stats.fireRate && rightLoadedCannons.Count > 0)
        {
            rightFireTimeElapsed = 0;

            int index = Random.Range(0, rightLoadedCannons.Count);

            FireCannon(rightLoadedCannons[index], stats, enemyTransform.right, fireCannonEffect);

            rightUnloadedCannons.Add(rightLoadedCannons[index]);
            rightLoadedCannons.RemoveAt(index);
        }
        if (shootingLeft && leftFireTimeElapsed > stats.fireRate && leftLoadedCannons.Count > 0)
        {
            leftFireTimeElapsed = 0;

            int index = Random.Range(0, leftLoadedCannons.Count);

            FireCannon(leftLoadedCannons[index], stats, -enemyTransform.right, fireCannonEffect);

            leftUnloadedCannons.Add(leftLoadedCannons[index]);
            leftLoadedCannons.RemoveAt(index);
        }

        rightFireTimeElapsed += Time.deltaTime;
        leftFireTimeElapsed += Time.deltaTime;

        int rightUnloadCount = rightUnloadedCannons.Count;
        int leftUnloadCount = leftUnloadedCannons.Count;

        if (rightUnloadCount > 0)
        {
            rightReloadTimeElapsed += Time.deltaTime;
            if (rightReloadTimeElapsed > stats.reloadSpeed)
            {
                int index = Random.Range(0, rightUnloadCount - 1);
                rightLoadedCannons.Add(rightUnloadedCannons[index]);

                rightUnloadedCannons.RemoveAt(index);

                rightReloadTimeElapsed = 0;
            }
        }

        if (leftUnloadCount > 0)
        {
            leftReloadTimeElapsed += Time.deltaTime;
            if (leftReloadTimeElapsed > stats.reloadSpeed)
            {
                int index = Random.Range(0, leftUnloadCount - 1);
                leftLoadedCannons.Add(leftUnloadedCannons[index]);

                leftUnloadedCannons.RemoveAt(index);

                leftReloadTimeElapsed = 0;
            }
        }
    }
    private void FireCannon(Transform cannon, EntityStats stats, Vector3 dir, GameObject fireCannonEffect)
    {
        GameObject cannonBall = ObjectPool.Instance.FetchFromPool(projectileID);
        cannonBall.transform.position = cannon.position;

        cannonBall.transform.LookAt(cannonBall.transform.position + dir);

        Vector3 shootVector = dir * stats.shootForce;

        cannonBall.GetComponent<Rigidbody>().isKinematic = false;

        cannonBall.GetComponent<Rigidbody>().velocity = new Vector3(
            shootVector.x + Random.Range(-stats.cannonInaccuracy, stats.cannonInaccuracy),
            stats.shootAngle + Random.Range(-stats.cannonInaccuracy / 4, stats.cannonInaccuracy / 4),
            shootVector.z + Random.Range(-stats.cannonInaccuracy, stats.cannonInaccuracy));

        cannonBall.GetComponent<Projectile>().SetProjectileDamage(stats.damage);

        MMFeedbacks effect = fireCannonEffect.GetComponent<MMFeedbacks>();
        effect.transform.position = cannon.position - shootVector.normalized * 2f;
        effect.transform.LookAt(effect.transform.position + shootVector);

        effect.PlayFeedbacks(cannon.position);
    }

    private bool PlayerInCannonViewRight(Transform enemyTransform)
    {
        float distance = (frontBackRaycast["right"][0].position - frontBackRaycast["right"][0].position).magnitude;

        Ray frontRay = new(frontBackRaycast["right"][0].position - 0.33f * distance * enemyTransform.forward, enemyTransform.right);
        Ray backRay = new(frontBackRaycast["right"][1].position + 0.33f * distance * enemyTransform.forward, enemyTransform.right);

        if (Physics.Raycast(frontRay, out RaycastHit hit, attackRange, Constants.EntityLayer))
        {
            if (hit.collider != null)
            {
                if (hit.collider.CompareTag(Constants.PlayerTag))
                    return true;
            }
        }
        if (Physics.Raycast(backRay, out hit, attackRange, Constants.EntityLayer))
        {
            if (hit.collider != null)
            {
                if (hit.collider.CompareTag(Constants.PlayerTag))
                    return true;
            }
        }
        return false;
    }
    private bool PlayerInCannonViewLeft(Transform enemyTransform)
    {
        float distance = (frontBackRaycast["left"][0].position - frontBackRaycast["left"][0].position).magnitude;

        Ray frontRay = new(frontBackRaycast["left"][0].position - 0.33f * distance * enemyTransform.forward, -enemyTransform.right);
        Ray backRay = new(frontBackRaycast["left"][1].position + 0.33f * distance * enemyTransform.forward, -enemyTransform.right);
        if (Physics.Raycast(frontRay, out RaycastHit hit, attackRange, Constants.EntityLayer))
        {
            if (hit.collider != null)
            {
                if (hit.collider.CompareTag(Constants.PlayerTag))
                    return true;
            }
        }
        if (Physics.Raycast(backRay, out hit, attackRange, Constants.EntityLayer))
        {
            if (hit.collider != null)
            {
                if (hit.collider.CompareTag(Constants.PlayerTag))
                    return true;
            }
        }
        return false;
    }
}