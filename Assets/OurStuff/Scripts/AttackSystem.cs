using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AttackSystem : MonoBehaviour
{
    protected Animator Anim;
    public float MaxStamina = 100;
    [HideInInspector]
    public float Stamina;
    public float StaminaRegan = 30;
    [HideInInspector]
    public float StaminaCost = 30;
    public float Tired = 30;
    [HideInInspector]
    public float Acooldown;
    public MeleeAttack meleeAttack;
    public DefensiveAbility Shield;
    public RangeAttack rangeAttack;
    public ParticleAttack particleAttack;
    public List<SOability> Attacks = new List<SOability>();
    public float DamageMultiplier = 1;
    protected bool HoldingAnAttack;

    // Start is called before the first frame update
    protected void Start()
    {
        Stamina = MaxStamina;
    }

    // Update is called once per frame
    protected void Update()
    {
        if (Acooldown > 0 && !HoldingAnAttack) { Acooldown -= Time.deltaTime; }

        if (Stamina < MaxStamina)
        {
            if (StaminaRegan > 0)
                Stamina += Time.deltaTime * StaminaRegan;
        }
        else
        {
            Stamina = MaxStamina;
        }
    }

    protected bool CanAttack()
    {
        if (Acooldown <= 0 && Stamina >= StaminaCost)
        {
            Stamina -= StaminaCost;
            return true;
        }
        return false;
    }

    protected void SetUpAttack(int attackType)
    {
        if (Attacks[attackType] is SOmeleeAttack && meleeAttack != null)
        {
            SOmeleeAttack SOM = (SOmeleeAttack)Attacks[attackType];
            meleeAttack.Damage = SOM.Damage * DamageMultiplier;
            meleeAttack.Knock = SOM.Knockback;
            meleeAttack.AttackCooldown = SOM.DamagingCooldown;
        }
        else if (Attacks[attackType] is SOrangedAttack && rangeAttack != null)
        {
            SOrangedAttack SOR = (SOrangedAttack)Attacks[attackType];
            rangeAttack.Damage = SOR.Damage * DamageMultiplier;
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
            particleAttack.Damage = SOP.Damage * DamageMultiplier;
            particleAttack.Knock = SOP.Knockback;
            particleAttack.Hold = SOP.Hold;
            particleAttack.ParticleAmount = SOP.Emit;
            particleAttack.AttackCooldown = SOP.DamagingCooldown;
        }
        else if (Attacks[attackType] is SOdefence && Shield != null)
        {
            SOdefence SOD = (SOdefence)Attacks[attackType];
            Shield.HpProtection = SOD.HpProtection;
            Shield.KnockProtection = SOD.KnockProtection;
            Shield.ProtectionTime = SOD.ProtectionTime;
        }
    }
}