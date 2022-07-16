using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WallParticleAttack", menuName = "Combat/Wall/Particle")]
public class SOwallParticle : SOwallAttack
{
    public ParticleSystem particleSystem;
    public Vector3 Position;
    public Vector3 Rotation;
}
