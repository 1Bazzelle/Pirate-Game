using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageTaker
{
    public void TakeDamage(float damage, Vector3 dir);
}
