using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
public class ShipController
{
    private float moveVal = 0;
    private float startingMoveVal;
    private float moveTimeElapsed = 0;
    private float moveRemainingTime;
    private bool stoppedPressingMove;

    private float rotatVal = 0;
    private float startingRotatVal;
    private float rotatTimeElapsed = 0;
    private float rotatRemainingTime;
    private bool stoppedPressingRotat;

    private bool inactive;

    public void UpdateMovement(Rigidbody rb, EntityMoveStats moveStats)
    {
        if (inactive) return;
        if (Input.GetKeyDown(Controls.Forward))
        {
            stoppedPressingMove = false;
            moveRemainingTime = Mathf.Abs(moveStats.forwardAcceleration * (1 - moveVal));
            moveTimeElapsed = 0;
            startingMoveVal = moveVal;
        }
        if (Input.GetKey(Controls.Forward))
        {
            if (moveRemainingTime != 0) moveVal = Mathf.SmoothStep(startingMoveVal, 1, moveTimeElapsed / moveRemainingTime);
            moveTimeElapsed += Time.deltaTime;

            if(moveTimeElapsed > moveRemainingTime) moveTimeElapsed = moveRemainingTime;
        }

        if (Input.GetKeyDown(Controls.Backward))
        {
            stoppedPressingMove = false;
            moveRemainingTime = Mathf.Abs(moveStats.backwardAcceleration * (-1 - moveVal));
            moveTimeElapsed = 0;
            startingMoveVal = moveVal;
        }
        if (Input.GetKey(Controls.Backward))
        {
            if(moveRemainingTime != 0) moveVal = Mathf.SmoothStep(startingMoveVal, -1, moveTimeElapsed / moveRemainingTime);
            moveTimeElapsed += Time.deltaTime;

            if (moveTimeElapsed > moveRemainingTime) moveTimeElapsed = moveRemainingTime;
        }

        if(!Input.GetKey(Controls.Forward) && !Input.GetKey(Controls.Backward))
        {
            if(!stoppedPressingMove)
            {
                moveRemainingTime = Mathf.Abs(moveVal) * moveStats.deceleration;
                moveTimeElapsed = 0;
                startingMoveVal = moveVal;
                stoppedPressingMove = true;
            }
            if (moveRemainingTime != 0) moveVal = Mathf.SmoothStep(startingMoveVal, 0, moveTimeElapsed / moveRemainingTime);
            moveTimeElapsed += Time.deltaTime;

            if (moveTimeElapsed > moveRemainingTime) moveTimeElapsed = moveRemainingTime;
        }


        if (Input.GetKeyDown(Controls.RotateRight))
        {
            stoppedPressingRotat = false;
            rotatRemainingTime = Mathf.Abs(moveStats.rotationAcceleration * (1 - rotatVal));
            rotatTimeElapsed = 0;
            startingRotatVal = rotatVal;
        }
        if (Input.GetKey(Controls.RotateRight))
        {
            if (rotatRemainingTime != 0) rotatVal = Mathf.SmoothStep(startingRotatVal, 1, rotatTimeElapsed / rotatRemainingTime);
            rotatTimeElapsed += Time.deltaTime;

            if (rotatTimeElapsed > rotatRemainingTime) rotatTimeElapsed = rotatRemainingTime;
        }

        if (Input.GetKeyDown(Controls.RotateLeft))
        {
            stoppedPressingRotat = false;
            rotatRemainingTime = Mathf.Abs(moveStats.rotationAcceleration * (-1 - rotatVal));
            rotatTimeElapsed = 0;
            startingRotatVal = rotatVal;
        }
        if (Input.GetKey(Controls.RotateLeft))
        {
            if (rotatRemainingTime != 0) rotatVal = Mathf.SmoothStep(startingRotatVal, -1, rotatTimeElapsed / rotatRemainingTime);
            rotatTimeElapsed += Time.deltaTime;

            if (rotatTimeElapsed > rotatRemainingTime) rotatTimeElapsed = rotatRemainingTime;
        }

        if (!Input.GetKey(Controls.RotateRight) && !Input.GetKey(Controls.RotateLeft))
        {
            if (!stoppedPressingRotat)
            {
                rotatRemainingTime = Mathf.Abs(rotatVal) * moveStats.rotationDeceleration;
                rotatTimeElapsed = 0;
                startingRotatVal = rotatVal;
                stoppedPressingRotat = true;
            }
            if (rotatRemainingTime != 0) rotatVal = Mathf.SmoothStep(startingRotatVal, 0, rotatTimeElapsed / rotatRemainingTime);
            rotatTimeElapsed += Time.deltaTime;

            if (rotatTimeElapsed > rotatRemainingTime) rotatTimeElapsed = rotatRemainingTime;
        }

        rb.velocity = rb.transform.forward * moveStats.maxMoveSpeed * moveVal;
        rb.angularVelocity = new Vector3(rb.angularVelocity.x, moveStats.maxRotationSpeed * rotatVal, rb.angularVelocity.z);
    }

    public void OnImmovableCollision()
    {
        moveVal = 0;
    }

    public void SetActive(bool state)
    {
        if (state) inactive = false;
        else state = true;
    }

    public void Reset()
    {
        moveVal = 0;
        rotatVal = 0;
    }
}