using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AttackSystem : MonoBehaviour
{
    protected Animator Anim;
    protected float Charge = 1;
    protected float MaxCharge;
    protected float ChargeSpeed;
    [HideInInspector] public float Stamina;
    [HideInInspector] public float AttackCooldown;
    [HideInInspector] public float StaminaCost;
    public                   float MaxStamina      = 100;
    public                   float StaminaRegan    = 30;
    public                   float Tired           = 30;
    [SerializeField]         float StaggerDuration = 0.3f;
    [SerializeField] MeleeAttack meleeAttack;
    [SerializeField] DefensiveAbility Shield;
    [SerializeField] RangeAttack rangeAttack;
    [SerializeField] ParticleAttack particleAttack;
    //public List<SOability> Attacks = new List<SOability>();
    public float DamageMultiplier  = 1;
    [SerializeField] string HoldingAnAttack;
    [SerializeField] string PreviousAttack;
    [SerializeField] float timeToComboReset = 1;
                     float comboTimer;
    [HideInInspector] public float AttackMovementForce;
    [HideInInspector] public Vector2 AttackMovementDirection;
    protected void Start()
    {
        Stamina = MaxStamina;
    }

    protected void Update()
    {
        if (AttackCooldown > 0 && HoldingAnAttack == string.Empty)
            AttackCooldown -= Time.deltaTime;

        if (Stamina < MaxStamina)
        {
            if (StaminaRegan > 0)
                Stamina += Time.deltaTime * StaminaRegan;
        }
        else
        {
            Stamina = MaxStamina;
        }

        if (comboTimer > 0)
            comboTimer -= Time.deltaTime;
        else
            PreviousAttack = string.Empty;
        if (ChargeSpeed>0 && MaxCharge > Charge)
        {
            Charge += Time.deltaTime * ChargeSpeed;
        }
        if (Charge > MaxCharge + 1)
        {
            Charge = MaxCharge+1;
        }
        if (AttackMovementForce > 0)
        {
            AttackMovement();
        }
    }

    protected bool CanAttack()
    {
        if (AttackCooldown <= 0 && Stamina >= StaminaCost)
        {
            Stamina -= StaminaCost;
            return true;
        }
        return false;
    }
    protected void ResetCharge()
    {
        MaxCharge = 0;
        Charge = 1;
        ChargeSpeed = 0;
    }

    protected void SetUpAttack(List<SOability> Attacks, int attackType)
    {
        AttackMovementForce = 0;
        if (Attacks[attackType] is SOmeleeAttack && meleeAttack != null)
        {
            SOmeleeAttack SOM = (SOmeleeAttack)Attacks[attackType];
            if (meleeAttack.attackSystem == null)
            {
                meleeAttack.attackSystem = this;
            }
            meleeAttack.Damage = SOM.Damage * DamageMultiplier;
            meleeAttack.minStagger = SOM.minStagger;
            meleeAttack.maxStagger = SOM.maxStagger;
            meleeAttack.Knock = SOM.Knockback;
            meleeAttack.AttackCooldown = SOM.DamagingCooldown;
            meleeAttack.Charge = Charge;
            meleeAttack.AttackMovementForce = SOM.AttackMovementForce;
            meleeAttack.AttackMovementDirection = SOM.AttackMovementDirection;
        }
        else if (Attacks[attackType] is SOrangedAttack && rangeAttack != null)
        {
            SOrangedAttack SOR = (SOrangedAttack)Attacks[attackType];
            rangeAttack.Damage = SOR.Damage * DamageMultiplier;
            rangeAttack.minStagger = SOR.minStagger;
            rangeAttack.maxStagger = SOR.maxStagger;
            rangeAttack.Knock = SOR.Knockback;
            rangeAttack.Bullet = SOR.Projectile;
            rangeAttack.ProjectileSpeed = SOR.ProjectileSpeed;
            rangeAttack.Charge = Charge;
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
            particleAttack.minStagger = SOP.minStagger;
            particleAttack.maxStagger = SOP.maxStagger;
            particleAttack.Knock = SOP.Knockback;
            particleAttack.Hold = SOP.Hold;
            particleAttack.ParticleAmount = SOP.Emit;
            particleAttack.AttackCooldown = SOP.DamagingCooldown;
            particleAttack.Charge = Charge;
        }
        else if (Attacks[attackType] is SOdefence && Shield != null)
        {
            SOdefence SOD = (SOdefence)Attacks[attackType];
            Shield.HpProtection = SOD.HpProtection;
            Shield.KnockProtection = SOD.KnockProtection;
            Shield.ProtectionTime = SOD.ProtectionTime;
        }
        else if (Attacks[attackType] is SOcharge)
        {
            SOcharge SOD = (SOcharge)Attacks[attackType];
            MaxCharge = SOD.MaxCharge;
            ChargeSpeed = SOD.ChargeSpeed;
        }
        
        if (!(Attacks[attackType] is SOcharge))
        {
            ResetCharge();
        }

        AttackCooldown = Attacks[attackType].AttackCooldown;
        Anim.SetTrigger(Attacks[attackType].AnimationTrigger);
        PreviousAttack = Attacks[attackType].Name;
        comboTimer = timeToComboReset + AttackCooldown;
    }

    public void Stagger()
    {
        Anim.SetTrigger("Ouch");
        HoldingAnAttack = string.Empty;
        PreviousAttack = string.Empty;
        AttackCooldown = StaggerDuration;
        ResetCharge();
        AttackMovementForce = 0;
    }

    protected void AttemptToAttack(List<SOability> Attacks, int attackType)
    {
        
        StaminaCost = Attacks[attackType].StaminaCost;

        switch (Attacks[attackType].attackType)
        {
            case SOability.AttackType.Normal:
                NormalAttack(Attacks, attackType);
                break;
            case SOability.AttackType.NeedsRelease:
                HoldAttack(Attacks, attackType);
                break;
            case SOability.AttackType.CanRelease:
                ReleaseAttack(Attacks, attackType);
                break;
            case SOability.AttackType.Combo:
                ComboAttack(Attacks, attackType);
                break;
            case SOability.AttackType.StartCombo:
                StartComboAttack(Attacks, attackType);
                break;
            default:
                break;
        }
        #region trash
        //if (!CanAttack() && Attacks[attackType].attackType == SOability.AttackType.CanRelease &&
        //    HoldingAnAttack == Attacks[attackType].PreviousAttack)
        //{
        //    SetUpAttack(attackType);
        //    HoldingAnAttack = string.Empty;
        //    SetAttackParameters(attackType);
        //}

        //if (Attacks[attackType].attackType == SOability.AttackType.Combo &&
        //    Attacks[attackType].attackType != SOability.AttackType.CanRelease && CanAttack())
        //{
        //    if (PreviousAttack != Attacks[attackType].PreviousAttack)
        //    {
        //        return;
        //    }

        //    SetUpAttack(attackType);
        //    SetAttackParameters(attackType);
        //}
        //else
        //{
        //    if (Attacks[attackType].attackType == SOability.AttackType.StartCombo)
        //    {
        //        if (PreviousAttack != Attacks[attackType].Name)
        //        {
        //            return;
        //        }
        //            SetUpAttack(attackType);
        //            SetAttackParameters(attackType);
        //    }
        //    else
        //    {
        //        SetUpAttack(attackType);
        //        if (Attacks[attackType].attackType == SOability.AttackType.NeedsRelease)
        //        {
        //            HoldingAnAttack = Attacks[attackType].Name;
        //        }
        //        SetAttackParameters(attackType);
        //    }
        //}
        #endregion
    }

    void NormalAttack(List<SOability> Attacks, int attackType)
    {
        if (CanAttack())
        {
            SetUpAttack(Attacks,attackType);
        }
    }

    void HoldAttack(List<SOability> Attacks, int attackType)
    {
        if (CanAttack())
        {
            SetUpAttack(Attacks, attackType);
            HoldingAnAttack = Attacks[attackType].Name;
        }
    }

    void ReleaseAttack(List<SOability> Attacks, int attackType)
    {
        if (CanReleaseAttack(Attacks, attackType))
        {
            SetUpAttack(Attacks, attackType);
            HoldingAnAttack = string.Empty;
        }
    }

    void ComboAttack(List<SOability> Attacks, int attackType)
    {
        if (CanAttack() && PreviousAttack == Attacks[attackType].PreviousAttack)
        {
            SetUpAttack(Attacks, attackType);
        }
    }

    void StartComboAttack(List<SOability> Attacks, int attackType)
    {
        if (CanAttack() && PreviousAttack != Attacks[attackType].Name)
        {
            SetUpAttack(Attacks, attackType);
        }
    }

    protected bool CanReleaseAttack(List<SOability> Attacks, int attackType)
    {
        if (AttackCooldown > 0 && Stamina >= StaminaCost &&
            Attacks[attackType].attackType == SOability.AttackType.CanRelease &&
            HoldingAnAttack == Attacks[attackType].PreviousAttack)
        {
            Stamina -= StaminaCost;
            return true;
        }
        return false;
    }
    public abstract void AttackMovement();
}