using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfDestruct : InstantExplosion
{
    public override void LastEffect()
    {
        Attacker.transform.parent.gameObject.SetActive(false);
    }
}
