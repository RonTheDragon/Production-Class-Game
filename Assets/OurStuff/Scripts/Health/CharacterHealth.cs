using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CharacterHealth : Health
{
    [SerializeField] float StaggerResistance;
    protected float TheKnockback;
    protected Vector3 TheImpactLocation;
    protected float Temperature;
    [SerializeField] GameObject OnFire;
    ParticleSystem onFire;
    bool ThereIsFire;

    float TempHpProtection = 1;
    float TempKnockProtection = 1;
    float TempTimeLeft;

    new protected void Start()
    {
        base.Start();
        if (OnFire != null)
        {
            ThereIsFire = true;
            onFire = OnFire.GetComponent<ParticleSystem>();
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
            if (HpRegan > 0)
                Hp += Time.deltaTime * HpRegan;
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

    public override void TakeDamage(float Damage, float knock, Vector2 Stagger, float Temperature, Vector3 ImpactLocation,GameObject Attacker)
    {
        if (TempTimeLeft > 0)
        {
            Hp -= Damage * TempHpProtection;
            TheKnockback = knock * TempKnockProtection;
        }
        else
        {
            Hp -= Damage;
            TheKnockback = knock;
            TryStagger(Stagger.x, Stagger.y);
            this.Temperature += Temperature;
            
        }
        TheImpactLocation = ImpactLocation;
    }

    public void TryStagger(float minStagger, float maxStagger)
    {
        float S = Random.Range(minStagger, maxStagger);
        if (S > StaggerResistance)
        {
            GetStaggered();
        }
    }

    protected abstract void GetStaggered();


    public void GainTempProtection(float hp, float knock, float time)
    {
        TempHpProtection = hp;
        TempKnockProtection = knock;
        TempTimeLeft = time;
    }
    public bool isDead()
    {
        return AlreadyDead;
    }
    protected abstract IEnumerator DisposeOfBody();

    void TemperatureManagement()
    {
        if (Temperature == 0)
        {
            StopFire();
        }
        else if (Temperature > 0) 
        {
            Temperature -= Time.deltaTime*20;
            if (Temperature > 20)
            {
                if (ThereIsFire)
                {
                    if (!OnFire.activeSelf) {
                        
                        OnFire.SetActive(true);
                    }
                    var e = onFire.emission;
                    e.rateOverTime = ((int)Temperature)-20;
                }
                Hp -= Time.deltaTime * Temperature * 0.1f;
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
        else
        {
            Temperature += Time.deltaTime * 20;
            StopFire();
        }
    }

    void StopFire()
    {
        if (ThereIsFire)
            if (OnFire.activeSelf) OnFire.SetActive(false);
    }
}