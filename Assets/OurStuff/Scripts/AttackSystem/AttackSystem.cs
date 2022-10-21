using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AttackSystem : MonoBehaviour
{
                      protected Animator              Anim;
                      protected float                 Charge          = 1;
                      protected float                 MinCharge;
                      protected float                 MaxCharge;
                      protected float                 ChargeSpeed;
    [HideInInspector] public float                    Stamina;
    [HideInInspector] public float                    AttackCooldown;
    [HideInInspector] public float                    StaminaCost;
    [HideInInspector] public float                    StaminaDrain;
                      public float                    MaxStamina        = 100;
                      public float                    StaminaRegan      = 30;
                      public float                    Tired             = 30;
    [SerializeField]         float                    StaggerDuration   = 0.3f;
    public          List<GameObject>         AbilityObjects    = new List<GameObject>();
                             List<Ability>            abilities         = new List<Ability>();
    //public List<SOability> Attacks = new List<SOability>();
                      public float                    DamageMultiplier  = 1;
    [SerializeField]  string                          HoldingAnAttack;
    [SerializeField]  string                          PreviousAttack;
    [SerializeField]  float                           timeToComboReset  = 1;
                      float                           comboTimer;
    [HideInInspector] public float                    AttackMovementForce;
    [HideInInspector] public Vector2                  AttackMovementDirection;
                      protected List<AbilityCoolDown> abilityCoolDowns  = new List<AbilityCoolDown>();
                      protected GameObject            Attacker;
                                AudioManager          audioManager;
    [HideInInspector] public bool Unstopable;

    protected void Start()
    {
        SetUpAbilities();
        Stamina = MaxStamina;
        HoldingAnAttack= string.Empty;
        audioManager = GetComponent<AudioManager>();
    }

    protected void Update()
    {
        if (AttackCooldown > 0 && HoldingAnAttack == string.Empty)
            AttackCooldown -= Time.deltaTime;

        if (HoldingAnAttack != string.Empty)
        {
            AttackCooldown = 1;
            Stamina -= StaminaDrain * Time.deltaTime;
            if (Stamina <= 0)
            {
                CancelAttack();
            }
        }

        if (AttackCooldown<=0 && Unstopable)
        {
            Unstopable = false;
        }

        if (Stamina < MaxStamina)
        {
            if (StaminaRegan > 0 && HoldingAnAttack == string.Empty)
                Stamina += Time.deltaTime * StaminaRegan;
        }
        else
        {
            Stamina = MaxStamina;
        }

        if (Stamina < 0) { Stamina = 0; }

        if (comboTimer > 0)
            comboTimer -= Time.deltaTime;
        else
            PreviousAttack = string.Empty;

        // Charge Stuff 

        if (ChargeSpeed>0 && MaxCharge+1+MinCharge > Charge)
        {
            Charge += Time.deltaTime * ChargeSpeed;
        }

        if (Charge > MaxCharge + 1 + MinCharge) 
        {
            Charge = MaxCharge + 1 + MinCharge;
        }

        if (AttackMovementForce > 0)
        {
            AttackMovement();
        }
        if (AttackCooldown <= 0)
        {
            if (AttackMovementForce > 0)
            {
            AttackMovementForce = 0;
            }

            Aiming(false);
        }
        if (abilityCoolDowns.Count > 0)
        {
            DrainCooldowns();
        }
    }

    virtual protected void DrainCooldowns()
    {
        foreach (AbilityCoolDown a in abilityCoolDowns)
        {
            a.Cooldown -= Time.deltaTime;
            if (a.Cooldown <= 0) abilityCoolDowns.Remove(a); break;
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
        MinCharge = 0;
        Charge = 1;
        ChargeSpeed = 0;
    }
    
    protected void SetUpAttack(List<SOability> Attacks, int attackType)
    {
        AttackMovementForce = 0;

        if (Charge < MinCharge + 1)
        {
            CancelAttack();
            return;
        }

        if (this is PlayerAttackSystem && Attacks[attackType].aiming)
        {
            Aiming(true);
        }

        if (Attacks[attackType].sound != string.Empty)
        {
            if (audioManager != null)
            {
                audioManager.StopAllSound();
                audioManager.PlaySound(Sound.Activation.Custom, Attacks[attackType].sound);
            }
        }

        if (Attacks[attackType] is SOattack)
        {
            if (Attacks[attackType] is SOmeleeAttack)
            {
                SOmeleeAttack SOM;
                MeleeAttack meleeAttack;

                SOM = (SOmeleeAttack)Attacks[attackType];
                meleeAttack = AbilityGet<MeleeAttack>();

                if (meleeAttack != null)
                {
                    setAttack(meleeAttack, SOM);
                    if (meleeAttack.attackSystem == null)
                    {
                        meleeAttack.attackSystem = this;
                    }
                    meleeAttack.AttackCooldown = SOM.DamagingCooldown;
                    meleeAttack.AttackMovementForce = SOM.AttackMovementForce;
                    meleeAttack.AttackMovementDirection = SOM.AttackMovementDirection;
                }
            }
            else if (Attacks[attackType] is SOrangedAttack)
            {
                SOrangedAttack SOR;
                RangeAttack rangeAttack;

                SOR = (SOrangedAttack)Attacks[attackType];
                rangeAttack = AbilityGet<RangeAttack>();

                if (rangeAttack != null)
                {
                    setAttack(rangeAttack, SOR);
                    rangeAttack.Bullet = SOR.Projectile;
                    rangeAttack.ProjectileSpeed = SOR.ProjectileSpeed;
                    rangeAttack.Gravity = SOR.Gravity;

                    if (Attacks[attackType] is SOexplosiveRange)
                    {
                        SOexplosiveRange SOE;
                        SOE = (SOexplosiveRange)Attacks[attackType];

                        rangeAttack.ExplosionRadius = SOE.ExplosionRadius;
                    }
                }
            }
            else if (Attacks[attackType] is SOparticleAttack)
            {
                SOparticleAttack SOP;
                ParticleAttack particleAttack;

                SOP = (SOparticleAttack)Attacks[attackType];
                particleAttack = AbilityGet<ParticleAttack>();

                if (particleAttack != null)
                {
                    setAttack(particleAttack, SOP);
                    if (particleAttack.particle == null)
                    {
                        particleAttack.CreateParticleSystem(SOP.particleSystem, SOP.Name);
                    }
                    else if (particleAttack.pc.pName != SOP.Name)
                    {
                        particleAttack.ReplaceParticleSystem(SOP.particleSystem, SOP.Name);
                    }
                    particleAttack.particleType = SOP.particleType;
                    particleAttack.ParticleAmount = SOP.Emit;
                    particleAttack.AttackCooldown = SOP.DamagingCooldown;
                }
            }
        }
        else if (Attacks[attackType] is SOdefence)
        {
            SOdefence SOD;
            DefensiveAbility Shield;

            SOD = (SOdefence)Attacks[attackType];
            Shield = AbilityGet<DefensiveAbility>();

            if (Shield != null)
            {         
                Shield.HpProtection = SOD.HpProtection;
                Shield.ProtectionTime = SOD.ProtectionTime;
            }
        }
        else if (Attacks[attackType] is SOcharge)
        {
            SOcharge SOD = (SOcharge)Attacks[attackType];
            MaxCharge = SOD.MaxCharge;
            MinCharge = SOD.MinCharge;
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
        Unstopable = Attacks[attackType].UnStopable;
        if (Attacks[attackType].AbilityCooldown != 0)
        {
            AddCooldown(Attacks[attackType]);
            //abilityCoolDowns.Add(new AbilityCoolDown(Attacks[attackType].Name, Attacks[attackType].AbilityCooldown));
        }
    }

    protected virtual void AddCooldown(SOability a)
    {
        abilityCoolDowns.Add(new AbilityCoolDown(a.Name, a.AbilityCooldown, false));
    }

    void setAttack(Attack TheAttack,SOattack TheSOattack)
    {
        TheAttack.Damage = TheSOattack.Damage * DamageMultiplier;
        TheAttack.Stagger = TheSOattack.Stagger;
        TheAttack.Knock = TheSOattack.Knockback;
        TheAttack.Charge = Charge;
        TheAttack.Attacker = Attacker;
        TheAttack.Temperature = TheSOattack.Temperature;
    }

    protected T AbilityGet<T>() where T : Ability
    {
        foreach (Ability a in abilities)
        {
            if (a is T)
            {
                return (T)a;
            }
        }
        Debug.Log("THIS WASNT SUPPOSED TO HAPPEN");
        return null;
    }

    public void Stagger()
    {
        if (audioManager != null)
        {
        audioManager.StopAllSound();
            audioManager.PlaySound(Sound.Activation.Custom, "Ah");
        }
        Anim.SetTrigger("Ouch");
        HoldingAnAttack = string.Empty;
        PreviousAttack = string.Empty;
        AttackCooldown = StaggerDuration;
        ResetCharge();
        AttackMovementForce = 0;
    }

    protected void AttemptToAttack(List<SOability> Attacks, int attackType)
    {
        if (Attacks[attackType].AbilityCooldown != 0)
        {
            if (abilityCoolDowns.Count != 0)
            {
                if (CheckAbilityCooldown(Attacks[attackType].Name)) return;
            }
        }
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
            StaminaDrain = StaminaCost;
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
        if (PreviousAttack == Attacks[attackType].PreviousAttack)
        {
            if (CanAttack())
            SetUpAttack(Attacks, attackType);
        }
    }

    void StartComboAttack(List<SOability> Attacks, int attackType)
    {
        if (PreviousAttack != Attacks[attackType].Name)
        {
            if (CanAttack())
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

    protected void SetLayersForAttacks(LayerMask L)
    {
        foreach(Ability a in abilities)
        {
            if (a is Attack)
            {
                Attack A = (Attack)a;
                A.Attackable = L;
            }
        }
    }

    bool CheckAbilityCooldown(string Name)
    {
            foreach (AbilityCoolDown a in abilityCoolDowns)
            {
                if (a.AbilityName == Name)
                {
                    if (a.Cooldown > 0)
                    {
                        if (this is PlayerAttackSystem)
                        { 
                            Debug.Log($"{Name} is in Cooldown ({(int)a.Cooldown}s)");
                        }
                        return true;
                    }
                }
            }
            return false;      
    }

    public virtual void Aiming(bool aim)
    {

    }

    public void CancelAttack()
    {
        if (AttackCooldown > 0)
        {
            Anim.SetTrigger("Release");
        }
        HoldingAnAttack = string.Empty;
        AttackCooldown = 0;
        Unstopable = false;
        ResetCharge();
    }

    void SetUpAbilities()
    {
        foreach(GameObject g in AbilityObjects)
        {
            foreach(Ability a in g.GetComponents<Ability>())
            {
                abilities.Add(a);
            }
        }
    }
}

public class AbilityCoolDown
{
    public string AbilityName;
    public float Cooldown;
    public float MaxCooldown;
    public bool AlwaysShow;
    public AbilityCoolDown(string AbilityName, float Cooldown, bool AlwaysShow)
    {
        this.AbilityName = AbilityName;
        if (!AlwaysShow)
        this.Cooldown = Cooldown;
        this.MaxCooldown = Cooldown;
        this.AlwaysShow = AlwaysShow;
}
}