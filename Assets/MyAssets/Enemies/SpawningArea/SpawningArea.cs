using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.AI;



public class SpawningArea : NetworkBehaviour
{
    [System.Serializable]
    private struct AreaData
    {
        public GameObject enemy;
        public int amount;
        public List<Route> possibleRoutes;
    }
    [SerializeField] private AreaData enemiesToSpawn;
    [SerializeField] private float range;
    [SerializeField] private float respawnRate;

    private List<GameObject> curExisting = new();

    public override void OnNetworkSpawn()
    {
        if(IsServer)
        {
            for (int i = 0; i < enemiesToSpawn.amount; i++)
            {
                AddEnemy();
            }
        }
    }

    private void Update()
    {
        for (int i = 0; i < curExisting.Count; i++)
        {
            if (curExisting[i] == null)
            {
                curExisting.RemoveAt(i);
                Invoke(nameof(AddEnemy), respawnRate);
            }
        }
    }

    private void AddEnemy()
    {
        Debug.Log("Adding Enemy");

        GameObject obj = Instantiate(enemiesToSpawn.enemy);

        if (!obj.TryGetComponent(out NavMeshAgent agent)) Debug.Log("AreaData object doesn't have a NavMeshAgent");
        agent.enabled = false;

        float rand = Random.Range(0, 2000) / 1000 * Mathf.PI;
        obj.transform.position = new Vector3(transform.position.x + range * Mathf.Sin(rand), 0, transform.position.z + range * Mathf.Cos(rand));

        agent.enabled = true;

        if (!obj.TryGetComponent(out NetworkObject network)) Debug.Log("AreaData object is not a NetworkBehaviour");

        network.Spawn();

        if (!obj.TryGetComponent(out Enemy enemy)) Debug.Log("AreaData object has no Enemy component");

        int routeCount = enemiesToSpawn.possibleRoutes.Count;

        if (routeCount > 0)
        {
            enemy.GetRoute().CopyValues(enemiesToSpawn.possibleRoutes[Random.Range(0, routeCount)]);
            enemy.TransitionToState(new Patrolling());
        }
        if (routeCount == 0)
        {
            enemy.TransitionToState(new Idle());
        }

        curExisting.Add(enemy.gameObject);
    }
}
