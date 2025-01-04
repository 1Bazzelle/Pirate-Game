using UnityEngine;

public class FromPool : MonoBehaviour
{
    protected string objectTag;

    public virtual void ReturnToPool()
    {
        ObjectPool.Instance.ReturnToPool(objectTag, gameObject);
    }

    public void GiveTag(string tag)
    {
        objectTag = tag;
    }
}
