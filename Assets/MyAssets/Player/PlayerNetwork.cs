using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Unity.Netcode;
using UnityEngine.UIElements;
using MoreMountains.Feedbacks;



[System.Serializable]
public struct CannonPositions
{
    public List<Transform> rightCannons;
    public List<Transform> leftCannons;
}
public class PlayerNetwork : Entity, IDamageTaker
{
    [SerializeField] private GameObject visual;
    [SerializeField] private CinemachineFreeLook cam;
    [SerializeField] private CannonPositions cannonPositions;
    [SerializeField] private string projectileID;
    [SerializeField] private GameObject fireCannonEffect;

    [SerializeField] private ProgressBar healthBar;

    private UIManager ui;
    private ShipController controller;
    private PlayerCannonFire cannons;
    private Rigidbody rb;

    [SerializeField] private float timeForModeChange;
    private Vector3 mousePos;
    private float modeSelectTimeElapsed;
    private ShipMode queuedMode;

    private bool controlsActive;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        rb = GetComponent<Rigidbody>();

        controller = new();

        cannons = new();
        cannons.Initialize(projectileID, cannonPositions);

        if (!IsOwner || !IsClient) visual.SetActive(false);

        if (IsOwner) transform.position = Constants.worldSpawn;

        fireCannonEffect.GetComponent<MMFeedbacks>().Initialization(gameObject);

        healthBar.InitializeBar(stats.maxHealth);
    }
    private void OnEnable()
    {
        modeSelectTimeElapsed = 0;

        ui = GetComponent<UIManager>();

        ChangeMode(new BalancedMode());

        ui.QueueBalancedImage();
        ui.ChangeModeImage();

        ui.InitializeCannonBars(cannonPositions.rightCannons.Count);

        cam.Priority = Constants.defaultCamPrio;

        controlsActive = true;
    }
    protected override void Update()
    {
        if (!IsOwner || !IsClient) return;

        base.Update();

        if (isAlive && controlsActive)
        {
            controller.UpdateMovement(rb, stats.moveStats);
            cannons.Update(this, cam.transform.position - transform.position, stats, ui);
        }

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            ToggleCameraControl(false);
            mousePos = Input.mousePosition;
            ui.ToggleModeWheel(mousePos, true);
        }
        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            ToggleCameraControl(true);
            ui.ToggleModeWheel(Vector3.zero, false);
            // Top Right
            if (Input.mousePosition.x > mousePos.x && Input.mousePosition.y > mousePos.y)
            {
                ChangePlayerMode(new AttackMode());
                ui.QueueAttackImage();
            }
            // Bottom Right
            if (Input.mousePosition.x > mousePos.x && Input.mousePosition.y < mousePos.y)
            {
                ChangePlayerMode(new SpeedMode());
                ui.QueueSpeedImage();
            }
                // Top Left
            if (Input.mousePosition.x < mousePos.x && Input.mousePosition.y > mousePos.y)
            {
                ChangePlayerMode(new BalancedMode());
                ui.QueueBalancedImage();
            }
            // Bottom Left
            if (Input.mousePosition.x < mousePos.x && Input.mousePosition.y < mousePos.y)
            {
                ChangePlayerMode(new LifeSupportMode());
                ui.QueueLifeSupportImage();
            }
        }

        if(modeSelectTimeElapsed > timeForModeChange && queuedMode != null)
        {
            ChangeMode(queuedMode);
            queuedMode = null;

            ui.ChangeModeImage();
        }
        modeSelectTimeElapsed += Time.deltaTime;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.collider.CompareTag(Constants.EnvironmentTag))
        {
            controller.OnImmovableCollision();
        }
    }

    public void FireCannon(Transform cannon, Vector3 dir)
    {
        Vector3 projPos = cannon.position;

        Vector3 shootVector = dir * stats.shootForce;
        Vector3 shootVelocity = new Vector3(
            shootVector.x + Random.Range(-stats.cannonInaccuracy, stats.cannonInaccuracy),
            stats.shootAngle + Random.Range(-stats.cannonInaccuracy / 4, stats.cannonInaccuracy / 4),
            shootVector.z + Random.Range(-stats.cannonInaccuracy, stats.cannonInaccuracy));

        if (IsServer)
        {
            CreateCannonBall(projPos, shootVelocity);

            CannonBallSpawnedClientRpc(projPos, shootVelocity);
        }
        else
        {
            ShootRequestServerRpc(projPos, shootVelocity);
        }
    }

    [ServerRpc]
    private void ShootRequestServerRpc(Vector3 position, Vector3 velocity)
    {
        CreateCannonBall(position, velocity);

        CannonBallSpawnedClientRpc(position, velocity);
    }
    [ClientRpc]
    private void CannonBallSpawnedClientRpc(Vector3 position, Vector3 velocity)
    {
        if (!IsServer)
        {
            CreateCannonBall(position, velocity);
        }
    }

    private void CreateCannonBall(Vector3 position, Vector3 velocity)
    {
        GameObject cannonBall = ObjectPool.Instance.FetchFromPool(projectileID);

        Projectile proj = cannonBall.GetComponent<Projectile>();

        proj.Launch(position, velocity);

        proj.SetProjectileDamage(stats.damage);

        MMFeedbacks effect = fireCannonEffect.GetComponent<MMFeedbacks>();
        effect.transform.position = position - velocity.normalized * 2f;
        effect.transform.LookAt(effect.transform.position + velocity);

        effect.PlayFeedbacks(position);
    }

    private void ChangePlayerMode(ShipMode newMode)
    {
        queuedMode = newMode;

        modeSelectTimeElapsed = 0;
    }
    private void ToggleCameraControl(bool newState)
    {
        if(newState)
        {
            cam.m_XAxis.m_MaxSpeed = Constants.camXSensitivity;
            cam.m_YAxis.m_MaxSpeed = Constants.camYSensitivity;
        }
        if (!newState)
        {
            cam.m_XAxis.m_MaxSpeed = 0;
            cam.m_YAxis.m_MaxSpeed = 0;
        }
    }

    public void TogglePlayerMovement(bool newState)
    {
        rb.isKinematic = !newState;
        controlsActive = newState;
        controller.Reset();
    }

    public void ToggleUI(bool newState)
    {
        ui.ToggleUI(newState);
    }

    public override void TakeDamage(float damage, Vector3 dir)
    {
        if (IsServer && isAlive)
        {
            health -= damage;
            healthBar.UpdateBar(health);
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
}