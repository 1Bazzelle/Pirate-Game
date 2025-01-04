using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using Unity.Netcode;

public class ObjectPool : MonoBehaviour
{
    #region Singleton
    private static ObjectPool instance;
    private ObjectPool() { }
    public static ObjectPool Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<ObjectPool>();

                if (instance == null)
                {
                    GameObject obj = new GameObject("ObjectPool");
                    instance = obj.AddComponent<ObjectPool>();
                }
            }
            return instance;
        }
    }
    #endregion

    [System.Serializable]
    private struct Pool
    {
        public string tag;
        public GameObject prefab;
        public int amountToPool;
    }
    [SerializeField] private List<Pool> pools = new();
    private Dictionary<string, Queue<GameObject>> objectPoolDictionary;
    
    private void OnEnable()
    {
        objectPoolDictionary = new();

        for(int i = 0; i < pools.Count; i++)
        {
            Queue<GameObject> objectPool = new();

            for(int p = 0; p < pools[i].amountToPool; p++)
            {
                GameObject prefab = Instantiate(pools[i].prefab);
                
                prefab.SetActive(false);

                prefab.transform.SetParent(transform);

                prefab.GetComponent<FromPool>().GiveTag(pools[i].tag);

                objectPool.Enqueue(prefab);
            }

            objectPoolDictionary.Add(pools[i].tag, objectPool);
        }
    }

    public GameObject FetchFromPool(string tag)
    {
        if (!objectPoolDictionary.ContainsKey(tag))
        {
            Debug.LogError("ObjectPool with tag " + tag + " doesn't exist.");
            return null;
        }

        GameObject prefab = null;

        if (objectPoolDictionary[tag].Count > 0)
        {
            prefab = objectPoolDictionary[tag].Dequeue();
        }
        else
        {
            Pool pool = pools.Find(p => p.tag == tag);
            prefab = Instantiate(pool.prefab);
            prefab.SetActive(false);
            prefab.GetComponent<FromPool>().GiveTag(pool.tag);

            prefab.transform.SetParent(transform);
        }
        prefab.SetActive(true);

        return prefab;
    }

    public void ReturnToPool(string tag, GameObject prefab)
    {
        prefab.SetActive(false);
        objectPoolDictionary[tag].Enqueue(prefab);
    }
}
