using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutsceneManager : MonoBehaviour
{
    private PlayerNetwork player;

    private Transform fromPos;
    private Transform toPos;
    private float duration;
    private float timeElapsed;

    private bool beingMoved;

    private void OnEnable()
    {
        player = GetComponent<PlayerNetwork>();
    }

    private void Update()
    {
        if (!beingMoved) return;

        transform.position = Vector3.Lerp(fromPos.position, toPos.position, timeElapsed/duration);
        transform.rotation = Quaternion.Lerp(fromPos.rotation, toPos.rotation, timeElapsed / duration);

        timeElapsed += Time.deltaTime;

        if (timeElapsed > duration)
        {
            beingMoved = false;
            player.TogglePlayerMovement(true);
            player.ToggleUI(true);
        }
    }

    public void MoveFromTo(Transform from, Transform to, float dur)
    {
        player.TogglePlayerMovement(false);
        player.ToggleUI(false);

        fromPos = from;
        toPos = to;

        duration = dur;
        timeElapsed = 0;

        beingMoved = true;
    }
}
