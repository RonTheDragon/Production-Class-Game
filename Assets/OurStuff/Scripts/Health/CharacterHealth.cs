using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CharacterHealth : Health
{
    [SerializeField]           float StaggerResistance;
                     protected float TheKnockback;
                     protected Vector3 TheImpactLocation;
                     public float Temperature;
    [SerializeField] GameObject OnFire;
                     ParticleSystem FireParticles;
    [SerializeField] GameObject OnIce;
    GameObject Ice;
    [HideInInspector] public ParticleSystem IceParticles;
    [HideInInspector] public bool Frozen;
    AttackSystem attackSystem;
    protected AudioManager Audio;
    bool ThereIsFire;
    bool ThereIsIce;
    [SerializeField] bool ImmuneToFire;
    [SerializeField] bool ImmuneToIce;

    float TempHpProtection = 1;
    float TempTimeLeft;

    public float TemperatureBalancer = 1;

    new protected void Start()
    {
        base.Start();
        Audio = GetComponent<AudioManager>();
        attackSystem = GetComponent<AttackSystem>();
        if (OnFire != null)
        {
            ThereIsFire = true;
            FireParticles = OnFire.GetComponent<ParticleSystem>();
        }
        if (OnIce != null)
        {
            ThereIsIce = true;
            IceParticles = OnIce.GetComponent<ParticleSystem>();
            Ice = OnIce.transform.GetChild(0).gameObject;
        }
    }


    new protected void Update()
    {
        base.Update();
        TemperatureManagement();
    }

    protected override void HealthMechanic()
    {

        if (Hp < MaxHp)
        {
            if (HpRegen > 0)
                Hp += Time.deltaTime * HpRegen;
        }
        else
        {
            Hp = MaxHp;
        }

        if (Hp <= 0)
        {
            Death();
        }

        if (TempTimeLeft > 0)
        {
            TempTimeLeft -= Time.deltaTime;
        }
    }

    public override void TakeDamage(float Damage, float knock, Vector2 Stagger, float Temperature,
        Vector3 ImpactLocation,GameObject Attacker)
    {
        if (Frozen)
        {
            float elementalEfficiency = 1;
            if (this is EnemyHealth)
            {
                elementalEfficiency = GameManager.instance.ElementEfficiency;
            }
                Hp -= Damage * (2 * elementalEfficiency);
                IceParticles.Emit((int)(Damage * (2 * elementalEfficiency) / 7.5f));
            if (Audio!=null)
            Audio.PlaySound(Sound.Activation.Custom, "Frozen Ah");
            if (Temperature > 0)
            {
                this.Temperature += Temperature;
            }
        }
        else
        {
            bool Unstopable = false;
            if (attackSystem != null)
            {
                Unstopable = attackSystem.Unstopable;
            }
            if (TempTimeLeft > 0 || Unstopable)
            {
                Hp -= Damage * TempHpProtection;
            }
            else
            {
                Hp -= Damage;
                TheKnockback = knock;
                TryStagger(Stagger.x, Stagger.y);
                this.Temperature += Temperature;
            }
        }
        GetComponent<CharacterAI>().ShowingData = 5;
        TheImpactLocation = ImpactLocation;
    }

    public void TryStagger(float minStagger, float maxStagger)
    {
        float S = Random.Range(minStagger, maxStagger);
        if (S > StaggerResistance && !AlreadyDead)
        {
            GetStaggered();
        }
    }

    protected abstract void GetStaggered();


    public void GainTempProtection(float hp, float time)
    {
        TempHpProtection    = hp;
        TempTimeLeft        = time;
    }
    public bool isDead()
    {
        return AlreadyDead;
    }
    protected abstract IEnumerator DisposeOfBody();

    void TemperatureManagement()
    {
        float elementalRecovery   = 1;
        float elementalEfficiency = 1;
        if (this is EnemyHealth)
        {
            elementalRecovery   = GameManager.instance.ElementRecovery;
            elementalEfficiency = GameManager.instance.ElementEfficiency;
        }
        if (Temperature == 0)
        {
            StopFire();
            StopIce();
        }
        else if (Temperature > 0)
        {
            StopIce();
            if (ImmuneToFire) Temperature = 0;
            else
            {
                Temperature -= Time.deltaTime * (20 * elementalRecovery);
                if (Temperature > 20)
                {
                    if (ThereIsFire)
                    {
                        if (!OnFire.activeSelf)
                        {
                            OnFire.SetActive(true);
                        }
                        var e = FireParticles.emission;
                        e.rateOverTime = ((int)Temperature) - 20;
                    }
                    Hp -= Time.deltaTime * Temperature * (0.1f * elementalEfficiency);
                    if (Temperature > 100)
                    {
                        Temperature = 100;
                    }
                }
                else
                {
                    StopFire();
                }
            }
        }
        else
        {
                StopFire();
            if (ImmuneToIce) Temperature = 0;
            else
            {
                Temperature += Time.deltaTime * 20;
                if (Temperature < -20)
                {
                    if (Temperature <= -100 && !AlreadyDead)
                    {
                        Frozen = true;
                        if (ThereIsIce)
                        {
                            if (!Ice.activeSelf)
                            {
                                Ice.SetActive(true);
                            }
                        }
                    }
                }
            }
        }
        if (!ImmuneToFire && !ImmuneToIce)
        {
            if (Temperature < 5 && Temperature > -5 && TemperatureBalancer > 0 && Temperature != 0)
            {
                TemperatureBalancer -= Time.deltaTime;
            }
            if (TemperatureBalancer <= 0)
            {
                Temperature = 0;
                TemperatureBalancer = 1;
            }
        }
    }

    void StopFire()
    {
        if (ThereIsFire)
            if (OnFire.activeSelf) OnFire.SetActive(false);
    }
    protected void StopIce()
    {
        Frozen = false;
        if (ThereIsIce)
            if (Ice.activeSelf) { Ice.SetActive(false); IceParticles.Emit(50); }
    }
}