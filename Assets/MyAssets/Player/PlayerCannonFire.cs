using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerCannonFire
{
    private string projectileID;

    private float fireTimeElapsed;
    private float rightReloadTimeElapsed;
    private float leftReloadTimeElapsed;

    private List<Transform> rightLoadedCannons;
    private List<Transform> rightUnloadedCannons;

    private List<Transform> leftLoadedCannons;
    private List<Transform> leftUnloadedCannons;

    private bool inactive;

    private Dir shootDirection;
    enum Dir
    {
        Null,
        Left,
        Right
    }
    public void Initialize(string projID, CannonPositions cannonPositions)
    {
        projectileID = projID;

        rightLoadedCannons = cannonPositions.rightCannons;
        leftLoadedCannons = cannonPositions.leftCannons;

        rightUnloadedCannons = new();
        leftUnloadedCannons = new();

        rightReloadTimeElapsed = 0;
        leftReloadTimeElapsed = 0;
    }
    public void Update(PlayerNetwork player, Vector3 lookDir, EntityStats stats, UIManager ui)
    {
        int rightLoadCount = rightLoadedCannons.Count;
        int leftLoadCount = leftLoadedCannons.Count;

        if (Input.GetKeyDown(Controls.FireCannons) && !inactive)
        {
            fireTimeElapsed = 0;

            Vector2 lookDir2D = new Vector2(lookDir.x, lookDir.z);
            Vector2 playerRight2D = new Vector2(player.transform.right.x, player.transform.right.z);

            float dotProduct = Vector2.Dot(lookDir2D, playerRight2D);

            if(dotProduct < 0)
                shootDirection = Dir.Right;
            else
                shootDirection = Dir.Left;
        }

        if (Input.GetKey(Controls.FireCannons) && !inactive)
        {
            if(fireTimeElapsed > stats.fireRate)
            {
                if (shootDirection == Dir.Right && rightLoadCount > 0)
                {
                    fireTimeElapsed = 0;

                    int index = Random.Range(0, rightLoadCount);

                    player.FireCannon(rightLoadedCannons[index], player.transform.right);

                    rightUnloadedCannons.Add(rightLoadedCannons[index]);
                    rightLoadedCannons.RemoveAt(index);
                }
                if (shootDirection == Dir.Left && leftLoadCount > 0)
                {
                    fireTimeElapsed = 0;

                    int index = Random.Range(0, leftLoadCount);

                    player.FireCannon(leftLoadedCannons[index], -player.transform.right);

                    leftUnloadedCannons.Add(leftLoadedCannons[index]);
                    leftLoadedCannons.RemoveAt(index);
                }
            }
            fireTimeElapsed += Time.deltaTime;
        }
        if (Input.GetKeyUp(Controls.FireCannons))
            shootDirection = Dir.Null;

        int rightUnloadCount = rightUnloadedCannons.Count;
        int leftUnloadCount = leftUnloadedCannons.Count;

        if (rightUnloadCount > 0)
        {
            rightReloadTimeElapsed += Time.deltaTime;
            if(rightReloadTimeElapsed > stats.reloadSpeed)
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

        ui.GetRightCannonBar().UpdateBar(rightLoadedCannons.Count);
        ui.GetLeftCannonBar().UpdateBar(leftLoadedCannons.Count);
    }

    public void SetActive(bool state)
    {
        if (state) inactive = false;
        else inactive = true;
    }
}
