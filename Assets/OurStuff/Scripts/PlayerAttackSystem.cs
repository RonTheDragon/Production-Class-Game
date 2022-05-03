using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackSystem : AttackSystem
{
    [SerializeField] MeleeAttack meleeAttack;
    [SerializeField] RangeAttack rangeAttack;
    [SerializeField] ParticleAttack particleAttack;
    [SerializeField] List<SOattack> Attacks = new List<SOattack>();
    Animator Anim;
    CharacterController CC;
    AudioManager Audio;
    //public LayerMask OnlyFloor;

    [SerializeField] float damageMultiplier = 1;

    new void Start()
    {
        base.Start();
        CC = GetComponent<CharacterController>();
        Audio = GetComponent<AudioManager>();
        Anim = transform.GetChild(0).GetComponent<Animator>();
    }

    new void Update()
    {
        base.Update();

    }
    public void Attack(int attackType)
    {
        if (CanAttack())
        {
            if (Attacks[attackType] is SOmeleeAttack && meleeAttack != null)
            {
                SOmeleeAttack SOM = (SOmeleeAttack)Attacks[attackType];
                meleeAttack.Damage = SOM.Damage * damageMultiplier;
                meleeAttack.Knock = SOM.Knockback;
                meleeAttack.AttackCooldown = SOM.DamagingCooldown;
            }
            else if (Attacks[attackType] is SOrangedAttack && rangeAttack != null)
            {
                SOrangedAttack SOR = (SOrangedAttack)Attacks[attackType];
                rangeAttack.Damage = SOR.Damage * damageMultiplier;
                rangeAttack.Knock = SOR.Knockback;
                rangeAttack.Bullet = SOR.Projectile;
                rangeAttack.ProjectileSpeed = SOR.ProjectileSpeed;
            }
            else if (Attacks[attackType] is SOparticleAttack && particleAttack != null)
            {
                SOparticleAttack SOP = (SOparticleAttack)Attacks[attackType];
                if (particleAttack.particle == null)
                {
                    particleAttack.CreateParticleSystem(SOP.particleSystem, SOP.Name);
                }
                else if (particleAttack.pc.pName != SOP.Name)
                {
                    particleAttack.ReplaceParticleSystem(SOP.particleSystem, SOP.Name);
                }
                particleAttack.Damage = SOP.Damage * damageMultiplier;
                particleAttack.Knock = SOP.Knockback;
                particleAttack.Hold = SOP.Hold;
                particleAttack.ParticleAmount = SOP.Emit;
                particleAttack.AttackCooldown = SOP.DamagingCooldown;
            }

            StaminaCost = Attacks[attackType].StaminaCost;

            Acooldown = Attacks[attackType].AttackCooldown;
            //Audio.PlaySound(Sound.Activation.Custom, "Attack");
            Anim.SetTrigger(Attacks[attackType].AnimationTrigger);
        }
    }
}