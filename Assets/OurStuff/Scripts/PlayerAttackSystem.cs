using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackSystem : AttackSystem
{
    CharacterController CC;
    AudioManager Audio;
    //public LayerMask OnlyFloor;

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
                //Audio.PlaySound(Sound.Activation.Custom, "Attack");
                Anim.SetTrigger(Attacks[attackType].AnimationTrigger);
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