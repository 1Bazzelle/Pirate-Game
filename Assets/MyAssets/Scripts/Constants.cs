using UnityEngine;

public static class Constants
{
    #region Tags
    public static string EnvironmentTag = "Environment";
    public static string WaterTag = "Water";
    public static string PlayerTag = "Player";
    public static string EnemyTag = "Enemy";
    #endregion

    #region Layers
    public static LayerMask EntityLayer = 1 << 3;
    public static LayerMask EnvironmentLayer = 1 << 6;
    #endregion

    #region Names
    public static string CannonBall = "CannonBall";
    #endregion

    #region Values
    public static Vector3 worldSpawn = new(250, 0, 450);

    public static float camXSensitivity = 150f;
    public static float camYSensitivity = 1.5f;

    public static float PointReachAcceptance = 10;

    public static int defaultCamPrio = 25;
    public static int disabledCutSceneCamPrio = 20;
    public static int enabledCutSceneCamPrio = 30;
    #endregion
}
