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
    [SerializeField] float StaggerDuration = 0.3f;
    [HideInInspector]
    public float Acooldown;
    public MeleeAttack meleeAttack;
    public DefensiveAbility Shield;
    public RangeAttack rangeAttack;
    public ParticleAttack particleAttack;
    public List<SOability> Attacks = new List<SOability>();
    public float DamageMultiplier = 1;
    public string HoldingAnAttack;
    public string PreviousAttack;

    // Start is called before the first frame update
    protected void Start()
    {
        Stamina = MaxStamina;
    }

    // Update is called once per frame
    protected void Update()
    {
        if (Acooldown > 0 && HoldingAnAttack==string.Empty) { Acooldown -= Time.deltaTime; }

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
            meleeAttack.Stagger = SOM.Stagger;
            meleeAttack.Knock = SOM.Knockback;
            meleeAttack.AttackCooldown = SOM.DamagingCooldown;
        }
        else if (Attacks[attackType] is SOrangedAttack && rangeAttack != null)
        {
            SOrangedAttack SOR = (SOrangedAttack)Attacks[attackType];
            rangeAttack.Damage = SOR.Damage * DamageMultiplier;
            rangeAttack.Stagger = SOR.Stagger;
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
            particleAttack.Stagger = SOP.Stagger;
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
    public void Stagger()
    {
        Anim.SetTrigger("Ouch");
        HoldingAnAttack = string.Empty;
        PreviousAttack = string.Empty;
        Acooldown = StaggerDuration;
    }
    protected void AttemptToAttack(int attackType)
    {
        StaminaCost = Attacks[attackType].StaminaCost;
        if (CanAttack())
        {
            if (Attacks[attackType].attackType != SOability.AttackType.CanRelease)
            {
                if (Attacks[attackType].attackType != SOability.AttackType.Combo)
                {
                    SetUpAttack(attackType);
                    if (Attacks[attackType].attackType == SOability.AttackType.NeedsRelease)
                    {
                        HoldingAnAttack = Attacks[attackType].Name;
                    }
                    Acooldown = Attacks[attackType].AttackCooldown;
                    // Audio.PlaySound(Sound.Activation.Custom, "Attack");
                    Anim.SetTrigger(Attacks[attackType].AnimationTrigger);
                    //Attacks[N].AttackMethod.Invoke();
                    PreviousAttack = Attacks[attackType].Name;
                }
                else
                {
                    if ( PreviousAttack == Attacks[attackType].PreviousAttack)
                    {
                        SetUpAttack(attackType);
                        Acooldown = Attacks[attackType].AttackCooldown;
                        Anim.SetTrigger(Attacks[attackType].AnimationTrigger);
                        PreviousAttack = Attacks[attackType].Name;
                    }
                }
            }
        }
        else if (Attacks[attackType].attackType == SOability.AttackType.CanRelease && HoldingAnAttack == Attacks[attackType].PreviousAttack)
        {
            SetUpAttack(attackType);
            HoldingAnAttack = string.Empty;
            Acooldown = Attacks[attackType].AttackCooldown;
            Anim.SetTrigger(Attacks[attackType].AnimationTrigger);
            PreviousAttack = Attacks[attackType].Name;
        }
    }
}