using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ParticleAttack", menuName = "Attacks/Particle Attack")]
public class SOparticleAttack : SOattack
{
    public ParticleSystem particleSystem;
    public int Emit = 1;
    public bool Hold;
    public float DamagingCooldown = 0.2f;
}
