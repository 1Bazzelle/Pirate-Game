using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ShipMode
{
    public abstract void EnterState(ref EntityStats stats);
    public abstract void ExitState(ref EntityStats stats);
}

public class BalancedMode : ShipMode
{
    public override void EnterState(ref EntityStats stats)
    {
        stats.moveStats.maxMoveSpeed /= 1.25f;
        stats.moveStats.maxRotationSpeed /= 1.25f;
    }
    public override void ExitState(ref EntityStats stats)
    {
        stats.moveStats.maxMoveSpeed *= 1.25f;
        stats.moveStats.maxRotationSpeed *= 1.25f;
    }
}

public class AttackMode : ShipMode
{
    public override void EnterState(ref EntityStats stats)
    {
        stats.reloadSpeed /= 1.25f;

        stats.regen /= 1.25f;
        stats.moveStats.maxMoveSpeed /= 1.75f;
        stats.moveStats.maxRotationSpeed /= 1.75f;
    }
    public override void ExitState(ref EntityStats stats)
    {
        stats.reloadSpeed *= 1.25f;

        stats.regen *= 1.25f;
        stats.moveStats.maxMoveSpeed *= 1.75f;
    }
}

public class LifeSupportMode : ShipMode
{
    public override void EnterState(ref EntityStats stats)
    {
        stats.regen *= 1.5f;

        stats.reloadSpeed *= 2f;
        stats.moveStats.maxMoveSpeed /= 1.75f;
    }
    public override void ExitState(ref EntityStats stats)
    {
        stats.regen /= 1.5f;

        stats.reloadSpeed /= 2f;
        stats.moveStats.maxMoveSpeed *= 1.75f;
    }
}

public class SpeedMode : ShipMode
{
    public override void EnterState(ref EntityStats stats)
    {
        stats.regen /= 1.5f;
        stats.reloadSpeed *= 2f;
    }
    public override void ExitState(ref EntityStats stats)
    {
        stats.regen *= 1.5f;
        stats.reloadSpeed /= 2f;
    }
}