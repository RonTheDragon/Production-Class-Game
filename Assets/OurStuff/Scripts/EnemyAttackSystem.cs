using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackSystem : AttackSystem
{
    Animator Anim;
    AudioManager Audio;
    [SerializeField] MeleeAttack meleeAttack;
    [SerializeField] RangeAttack rangeAttack;
    [SerializeField] ParticleAttack particleAttack;
    [SerializeField] List<SOattack> Attacks = new List<SOattack>();
    [SerializeField] float TryToAttackEvery = 0.2f;
    [SerializeField] float DamageMultiplier = 1;
    float TryAttack;

    new void Start()
    {
        base.Start();
        Audio = GetComponent<AudioManager>();
        Anim = transform.GetChild(0).GetChild(0).GetComponent<Animator>();
    }

    new void Update()
    {
        base.Update();
    }
    public void Attack(Vector3 Target)
    {
        if (TryAttack <= 0)
        {
            TryAttack = TryToAttackEvery;

            int N = Random.Range(0, Attacks.Count);
            bool TheChance = true;
            if (Attacks[N].Chance != 100)
            {
                float R = Random.Range(0f, 100f);
                if (R > Attacks[N].Chance)
                    TheChance = false;
            }
            float dist = Vector3.Distance(transform.position, Target);
            if (dist > Attacks[N].MinRange && dist < Attacks[N].MaxRange && TheChance)
            {
                if (Attacks[N] is SOmeleeAttack && meleeAttack != null)
                {
                    SOmeleeAttack SOM = (SOmeleeAttack)Attacks[N];
                    meleeAttack.Damage = SOM.Damage * DamageMultiplier;
                    meleeAttack.Knock = SOM.Knockback;
                    meleeAttack.AttackCooldown = SOM.DamagingCooldown;
                }
                else if (Attacks[N] is SOrangedAttack && rangeAttack != null)
                {
                    SOrangedAttack SOR = (SOrangedAttack)Attacks[N];
                    rangeAttack.Damage = SOR.Damage * DamageMultiplier;
                    rangeAttack.Knock = SOR.Knockback;
                    rangeAttack.Bullet = SOR.Projectile;
                    rangeAttack.ProjectileSpeed = SOR.ProjectileSpeed;
                }
                else if (Attacks[N] is SOparticleAttack && particleAttack != null)
                {
                    SOparticleAttack SOP = (SOparticleAttack)Attacks[N];
                    if (particleAttack.particle == null)
                    {
                        particleAttack.CreateParticleSystem(SOP.particleSystem,SOP.Name);
                    }
                    else if (particleAttack.pc.pName != SOP.Name)
                    {
                        particleAttack.ReplaceParticleSystem(SOP.particleSystem, SOP.Name);
                    }
                    particleAttack.Damage = SOP.Damage * DamageMultiplier;
                    particleAttack.Knock = SOP.Knockback;
                    particleAttack.Hold = SOP.Hold;
                    particleAttack.ParticleAmount = SOP.Emit;
                    particleAttack.AttackCooldown = SOP.DamagingCooldown;
                }

                StaminaCost = Attacks[N].StaminaCost;
                if (CanAttack())
                {
                    Acooldown = Attacks[N].AttackCooldown;
                    // Audio.PlaySound(Sound.Activation.Custom, "Attack");
                    Anim.SetTrigger(Attacks[N].AnimationTrigger);
                    Attacks[N].AttackMethod.Invoke();
                }
            }
        }
        else
        {
            TryAttack -= Time.deltaTime;
        }
    }
}