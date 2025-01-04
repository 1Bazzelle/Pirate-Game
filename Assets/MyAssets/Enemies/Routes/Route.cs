using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Route : MonoBehaviour
{
    private int curIndex;
    private int pointAmount;
    private bool routeStarted;
    private enum RouteType
    {
        Looping,
        PingPong,
        OneWay
    }

    [System.Serializable]
    public struct WayPoint
    {
        public float waitTime;
        public Transform point;
    }

    [SerializeField] private RouteType routeType;
    [SerializeField] private List<WayPoint> wayPoints;

    public void CopyValues(Route other)
    {
        routeType = other.routeType;
        wayPoints = other.wayPoints;
        routeStarted = false;
    }

    public bool HasStarted()
    {
        return routeStarted;
    }
    public Vector3 BeginRoute()
    {
        curIndex = 0;
        routeStarted = true;
        pointAmount = wayPoints.Count;
        return wayPoints[curIndex].point.position;
    }
    public WayPoint GetCurPoint()
    {
        return wayPoints[curIndex];
    }
    public void QueueNextPosition()
    {
        curIndex++;
        if (curIndex > pointAmount - 1)
        {
            switch (routeType)
            {
                case RouteType.Looping:
                    curIndex = 0;
                    break;

                case RouteType.PingPong:
                    wayPoints.Reverse();
                    curIndex = 0;
                    break;

                case RouteType.OneWay:
                    curIndex--;
                    routeStarted = false;
                    break;
            }
        }
    }
    public float GetNextWaitTime()
    {
        return wayPoints[curIndex].waitTime;
    }
}

