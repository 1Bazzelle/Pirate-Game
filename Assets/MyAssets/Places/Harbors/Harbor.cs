using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;

public class Harbor : NetworkBehaviour
{
    [SerializeField] private CinemachineVirtualCamera harborAngle;
    [SerializeField] private Transform playerPos;

    [SerializeField] private Cutscene enterHarbor;
    [SerializeField] private Cutscene exitHarbor;

    private BoxCollider col;

    private PlayerNetwork curPlayer;
    private bool playerInHarbor;

    private void OnEnable()
    {
        col = GetComponent<BoxCollider>();

        playerInHarbor = false;
    }

    private void Update()
    {
        if(playerInHarbor)
        {
            if (curPlayer != null)
            {
                curPlayer.TogglePlayerMovement(false);
            }
            if (curPlayer.IsOwner && Input.GetKeyDown(Controls.Forward))
            {
                playerInHarbor = false;
                harborAngle.Priority = Constants.disabledCutSceneCamPrio;
                exitHarbor.PlayCutscene();
            }
        }
    }

    public void OnEnterCutsceneOver(Transform start, Transform end, float dur)
    {
        harborAngle.Priority = Constants.enabledCutSceneCamPrio;
        playerInHarbor = true;
        curPlayer.ToggleUI(true);
    }

    public void OnExitCutsceneOver(Transform start, Transform end, float dur)
    {
        if (curPlayer != null)
        {
            curPlayer.TogglePlayerMovement(true);
            curPlayer.ToggleUI(true);
        }
        curPlayer = null;

        col.enabled = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(Constants.PlayerTag) && !playerInHarbor)
        {
            if (other.TryGetComponent(out curPlayer))
            {
                if (!curPlayer.IsOwner) return;

                enterHarbor.PlayCutscene();

                col.enabled = false;
            }
            else Debug.Log("Player not found");
        }
    }
}
