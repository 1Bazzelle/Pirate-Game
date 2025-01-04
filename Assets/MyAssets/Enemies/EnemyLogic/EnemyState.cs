using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public abstract class EnemyState
{
    public abstract void EnterState(Enemy enemy);
    public abstract void UpdateState(Enemy enemy);
    public abstract void ExitState(Enemy enemy);
}


public class Idle : EnemyState
{
    public float waitTime;
    public bool infinite;
    public Idle(float waitTime)
    {
        this.waitTime = waitTime;
        infinite = false;
    }
    public Idle()
    {
        waitTime = 0;
        infinite = true;
    }
    public override void EnterState(Enemy enemy)
    {
        enemy.ChangeMode(new BalancedMode());
        Debug.Log("Entered State: " + this);
    }
    public override void UpdateState(Enemy enemy)
    {
        if (enemy is Warship)
        {
            Warship warship = enemy as Warship;
            GameObject player = warship.PlayerInLineOfSight();
            if (player != null)
            {
                warship.SetPersecuting(player);
                warship.TransitionToState(new Persecute());
            }
        }

        if (waitTime < 0 && !infinite && enemy.GetRoute() != null) enemy.TransitionToState(new Patrolling());

        waitTime -= Time.deltaTime;
    }
    public override void ExitState(Enemy enemy)
    {

    }
}

public class Patrolling : EnemyState
{
    private Route route;
    public override void EnterState(Enemy enemy)
    {
        route = enemy.GetRoute();

        enemy.ChangeMode(new SpeedMode());
        Debug.Log("Entered State: " + this);
        if (route != null)
        {
            if (!route.HasStarted())
            {
                enemy.SetDestination(route.BeginRoute());
            }
            else
                enemy.SetDestination(route.GetCurPoint().point.position);
        }
    }
    public override void UpdateState(Enemy enemy)
    {
        if(enemy is Warship)
        {
            Warship warship = enemy as Warship;

            #region Exit Conditions
            GameObject player = warship.PlayerInLineOfSight();
            if (player != null)
            {
                warship.SetPersecuting(warship.PlayerInLineOfSight());
                warship.TransitionToState(new Persecute());
            }
            #endregion
        }
        if (enemy is Merchant)
        {
            if (enemy.PlayerInLineOfSight() != null) enemy.TransitionToState(new Flee());
        }

        if (Vector3.Distance(enemy.transform.position, route.GetCurPoint().point.position) < Constants.PointReachAcceptance && route.HasStarted())
        {
            float waitTime = route.GetNextWaitTime();
            route.QueueNextPosition();
            enemy.TransitionToState(new Idle(waitTime));
        }
    }
    public override void ExitState(Enemy enemy)
    {

    }
}

public class Flee : EnemyState
{
    public override void EnterState(Enemy enemy)
    {
        enemy.ChangeMode(new SpeedMode());
        Debug.Log("Entered State: " + this);
    }
    public override void UpdateState(Enemy enemy)
    {
        if (enemy.GetHealth() < (100 / enemy.GetStats().maxHealth) * 10) enemy.ChangeMode(new LifeSupportMode());
        if (enemy.GetHealth() > (100 / enemy.GetStats().maxHealth) * 25) enemy.ChangeMode(new SpeedMode());
    }
    public override void ExitState(Enemy enemy)
    {

    }
}

public class Persecute : EnemyState
{
    float timeElapsed;
    public override void EnterState(Enemy enemy)
    {
        enemy.ChangeMode(new SpeedMode());
        Debug.Log("Entered State: " + this);

        if (enemy is Warship)
        {
            Warship warship = enemy as Warship;

            warship.SetDestination(warship.GetPersecuting().transform.position);

            timeElapsed = 0;
        }
    }
    public override void UpdateState(Enemy enemy)
    {
        if(enemy is Warship)
        {
            Warship warship = enemy as Warship;

            #region Exit Conditions
            GameObject player = warship.PlayerInAttackRange();

            if (player != null)
            {
                warship.TransitionToState(new Attacking());
            }
            else if (player == null && warship.PlayerForgotten() && warship.GetRoute() != null)
            {
                warship.SetPersecuting(null);
                warship.TransitionToState(new Patrolling());
            }
            else if (player == null && !warship.PlayerForgotten() || player == null && warship.GetRoute() == null) warship.TransitionToState(new Idle());
            if (warship.GetRoute() == null) warship.TransitionToState(new Idle());
            
            #endregion

            if(timeElapsed > 3)
            {
                timeElapsed = 0;
                warship.SetDestination(warship.GetPersecuting().transform.position);
            }
            timeElapsed += Time.deltaTime;
        }
    }
    public override void ExitState(Enemy enemy)
    {

    }
}

public class Attacking : EnemyState
{
    private RotationDirection rotationDirection;
    private float elapsedTime;

    enum RotationDirection
    {
        Clockwise,
        CounterClockWise
    }
    public override void EnterState(Enemy enemy)
    {
        Debug.Log("Entered State: " + this);
        if (enemy is Warship)
        {
            Warship warship = enemy as Warship;
            warship.ChangeMode(new AttackMode());
            warship.ActivateCannons();
            int i = Random.Range(0, 2);
            if (i == 0) rotationDirection = RotationDirection.Clockwise;
            else rotationDirection = RotationDirection.CounterClockWise;

            elapsedTime = 0;
        }
    }
    public override void UpdateState(Enemy enemy)
    {
        if (enemy is Warship)
        {
            Warship warship = enemy as Warship;

            #region Exit Conditions
            GameObject player = warship.PlayerInAttackRange();
            if (player == null)
            {
                warship.TransitionToState(new Persecute());
            }
            #endregion

            if (warship.GetAgent().remainingDistance < 10f || elapsedTime > 3f)
            {
                Vector2 desiredVector = new Vector2(
                    warship.transform.position.x - warship.GetPersecuting().transform.position.x,
                     warship.transform.position.z - warship.GetPersecuting().transform.position.z).normalized; 

                float desiredAngleIncrement = 20f * Mathf.Deg2Rad;
                if (rotationDirection != RotationDirection.Clockwise) desiredAngleIncrement *= -1;

                desiredVector = new Vector3(
                Mathf.Cos(desiredAngleIncrement) * desiredVector.x - Mathf.Sin(desiredAngleIncrement) * desiredVector.y,
                Mathf.Sin(desiredAngleIncrement) * desiredVector.x + Mathf.Cos(desiredAngleIncrement) * desiredVector.y);

                Vector3 newDestination = new Vector3(desiredVector.x, 0, desiredVector.y);

                warship.SetDestination(warship.GetPersecuting().transform.position + 0.66f * warship.GetAttackRange() * newDestination);

                elapsedTime = 0;
            }
            elapsedTime += Time.deltaTime;
        }
    }
    public override void ExitState(Enemy enemy)
    {
        if (enemy is Warship)
        {
            Warship warship = enemy as Warship;
            warship.DeactivateCannons();
        }
    }
}