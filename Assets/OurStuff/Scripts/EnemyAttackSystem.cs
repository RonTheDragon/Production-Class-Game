using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackSystem : AttackSystem
{
    AudioManager Audio;
    [SerializeField] float TryToAttackEvery = 0.2f;
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

            int attackType = Random.Range(0, Attacks.Count);
            bool TheChance = true;
            if (Attacks[attackType].Chance != 100)
            {
                float R = Random.Range(0f, 100f);
                if (R > Attacks[attackType].Chance)
                    TheChance = false;
            }
            float dist = Vector3.Distance(transform.position, Target);
            if (dist > Attacks[attackType].MinRange && dist < Attacks[attackType].MaxRange && TheChance)
            {
                StaminaCost = Attacks[attackType].StaminaCost;
                if (CanAttack())
                {
                    if (Attacks[attackType].attackType != SOability.AttackType.CanRelease)
                    {
                        SetUpAttack(attackType);
                        if (Attacks[attackType].attackType == SOability.AttackType.NeedsRelease)
                        {
                            HoldingAnAttack = true;
                        }
                        Acooldown = Attacks[attackType].AttackCooldown;
                        // Audio.PlaySound(Sound.Activation.Custom, "Attack");
                        Anim.SetTrigger(Attacks[attackType].AnimationTrigger);
                        //Attacks[N].AttackMethod.Invoke();
                    }
                }
                else if (Attacks[attackType].attackType == SOability.AttackType.CanRelease && HoldingAnAttack == true)
                {
                    SetUpAttack(attackType);
                    HoldingAnAttack = false;
                    Acooldown = Attacks[attackType].AttackCooldown;
                    Anim.SetTrigger(Attacks[attackType].AnimationTrigger);
                }
            }
        }
        else
        {
            TryAttack -= Time.deltaTime;
        }
    }

    
}