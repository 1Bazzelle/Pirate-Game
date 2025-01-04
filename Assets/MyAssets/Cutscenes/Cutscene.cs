using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Unity.Netcode;
public class Cutscene : MonoBehaviour
{ 
    [System.Serializable]
    private struct Shot
    { 
        public CinemachineVirtualCamera cam;
        public float duration;
    }

    [SerializeField] private List<Shot> angles;
    [SerializeField] private bool repeatable;

    [SerializeField] private Transform playerFrom;
    [SerializeField] private Transform playerTo;

    [SerializeField] private Transform playerPosOnExit;

    [SerializeField] private GameEvent cutSceneStarted;
    [SerializeField] private GameEvent cutsceneOver;

    private float timeElapsed;
    private int index;
    private bool playing;
    private void OnEnable()
    {
        foreach (Shot shot in angles)
        {
            shot.cam.Priority = Constants.disabledCutSceneCamPrio;
        }
    }
    private void Update()
    {
        if (playing)
        {
            if (angles[index].duration > timeElapsed)
            {
                if (index > 0)
                {
                    angles[index - 1].cam.Priority = Constants.disabledCutSceneCamPrio;
                }

                InitializeShot(angles[index]);
            }
            if (angles[index].duration < timeElapsed)
            {
                index++;

                if (angles.Count - 1 < index)
                {
                    if (!repeatable) Destroy(this);
                    index = 0;
                    timeElapsed = 0;

                    playing = false;

                    foreach (Shot angle in angles)
                    {
                        angle.cam.Priority = Constants.disabledCutSceneCamPrio;
                    }

                    cutsceneOver.Occured(playerPosOnExit);
                    return;
                }
                timeElapsed = 0;
            }
            timeElapsed += Time.deltaTime;
        }
    }

    public void PlayCutscene()
    {
        cutSceneStarted.Occured(playerFrom, playerTo, GetDuration());
        timeElapsed = 0;
        index = 0;
        playing = true;
    }

    private void InitializeShot(Shot shot)
    {
        shot.cam.Priority = Constants.enabledCutSceneCamPrio;
    }

    public float GetDuration()
    {
        float duration = 0;
        foreach (Shot shot in angles)
        {
            duration += shot.duration;
        }
        return duration;
    }
}