using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AttackSystem : MonoBehaviour
{
    protected Animator Anim;
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
    public List<SOability> Attacks = new List<SOability>();
    public float DamageMultiplier  = 1;
    [SerializeField] string HoldingAnAttack;
    [SerializeField] string PreviousAttack;
    [SerializeField] float timeToComboReset = 1;
                     float comboTimer;
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

    protected void SetUpAttack(int attackType)
    {
        if (Attacks[attackType] is SOmeleeAttack && meleeAttack != null)
        {
            SOmeleeAttack SOM = (SOmeleeAttack)Attacks[attackType];
            meleeAttack.Damage = SOM.Damage * DamageMultiplier;
            meleeAttack.minStagger = SOM.minStagger;
            meleeAttack.maxStagger = SOM.maxStagger;
            meleeAttack.Knock = SOM.Knockback;
            meleeAttack.AttackCooldown = SOM.DamagingCooldown;
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
        }
        else if (Attacks[attackType] is SOdefence && Shield != null)
        {
            SOdefence SOD = (SOdefence)Attacks[attackType];
            Shield.HpProtection = SOD.HpProtection;
            Shield.KnockProtection = SOD.KnockProtection;
            Shield.ProtectionTime = SOD.ProtectionTime;
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
    }

    protected void AttemptToAttack(int attackType)
    {
        StaminaCost = Attacks[attackType].StaminaCost;

        switch (Attacks[attackType].attackType)
        {
            case SOability.AttackType.Normal:
                NormalAttack(attackType);
                break;
            case SOability.AttackType.NeedsRelease:
                HoldAttack(attackType);
                break;
            case SOability.AttackType.CanRelease:
                ReleaseAttack(attackType);
                break;
            case SOability.AttackType.Combo:
                ComboAttack(attackType);
                break;
            case SOability.AttackType.StartCombo:
                StartComboAttack(attackType);
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

    void NormalAttack(int attackType)
    {
        if (CanAttack())
        {
            SetUpAttack(attackType);
        }
    }

    void HoldAttack(int attackType)
    {
        if (CanAttack())
        {
            SetUpAttack(attackType);
            HoldingAnAttack = Attacks[attackType].Name;
        }
    }

    void ReleaseAttack(int attackType)
    {
        if (CanReleaseAttack(attackType))
        {
            SetUpAttack(attackType);
            HoldingAnAttack = string.Empty;
        }
    }

    void ComboAttack(int attackType)
    {
        if (CanAttack() && PreviousAttack == Attacks[attackType].PreviousAttack)
        {
            SetUpAttack(attackType);
        }
    }

    void StartComboAttack(int attackType)
    {
        if (CanAttack() && PreviousAttack != Attacks[attackType].Name)
        {
            SetUpAttack(attackType);
        }
    }

    protected bool CanReleaseAttack(int attackType)
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
}