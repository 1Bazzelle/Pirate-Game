using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(menuName = "ShipBaseStats")]
public class ShipStats : ScriptableObject
{
    public float maxHealth, regen, damage, reloadSpeed, fireRate, shootForce, shootAngle, cannonInaccuracy;
    public float maxMoveSpeed, forwardAcceleration, backwardAcceleration, deceleration;
    public float maxRotationSpeed, rotationAcceleration, rotationDeceleration;

    public void InitializeStats(ref EntityStats stats)
    {
        stats.maxHealth = maxHealth;
        stats.damage = damage;
        stats.reloadSpeed = reloadSpeed;
        stats.fireRate = fireRate;
        stats.shootForce = shootForce;
        stats.shootAngle = shootAngle;
        stats.cannonInaccuracy = cannonInaccuracy;
        stats.moveStats.maxMoveSpeed = maxMoveSpeed;
        stats.moveStats.forwardAcceleration = forwardAcceleration;
        stats.moveStats.backwardAcceleration = backwardAcceleration;
        stats.moveStats.deceleration = deceleration;
        stats.moveStats.maxRotationSpeed = maxRotationSpeed;
        stats.moveStats.rotationAcceleration = rotationAcceleration;
        stats.moveStats.rotationDeceleration = rotationDeceleration;
    }
}