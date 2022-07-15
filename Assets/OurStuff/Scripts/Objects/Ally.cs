using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ally : CharacterAI
{
    new void Awake()
    {
        TheBody = transform;
        base.Awake();
    }
    new void Start()
    {
        base.Start();
        Targetable = GameManager.instance.PlayerCanAttack;
        CanSee = GameManager.instance.AlliesCanSee;
    }
    new void Update()
    {
        base.Update();
    }
    protected override void AttackAI()
    {
        if (Target != null)
        {
            if (TargetHealth != null)
            {
                if (TargetHealth.Hp<=0)
                {
                    ForgetTarget();
                }
                else
                {
                    if (canSeeTarget())
                    {
                        alert += Time.deltaTime;
                    }
                    else if (alert > 0)
                    {
                        alert -= Time.deltaTime;
                    }

                    if (alert > timeToAlert)
                    {
                        chasingTarget = true;
                    }
                    else
                    {
                        ForgetTarget();
                    }
                }
            }
            else 
            {
                ForgetTarget();
            }
        }
        else {  SearchForTarget(); }

        if (chasingTarget)
        {
            DetectionRange = OriginalDetectionRange * 1.5f;
            if (eas.AttackCooldown <= 0)
            {
                NMA.speed = GetSpeed() * 1.5f;
                NMA.stoppingDistance = StoppingDistance;
            }
            else
            {
                NMA.speed = 0;
            }

            if (CheckBravery(BraveEnoughToFight))
            {
                ChaseTarget();
            }
            else
            {
                RunningAway();
                eas.CancelAttack();
            }
        }

        else
        {
            DetectionRange = OriginalDetectionRange;
            if (eas.AttackCooldown <= 0)
            {
                NMA.speed = GetSpeed();
                NMA.stoppingDistance = StoppingDistance - 2;
            }
            else
            {
                NMA.speed = 0;
            }

            
                Wander();
            eas.CancelAttack();

        }

        if (eas.Stamina < eas.Tired) { NMA.speed = GetSpeed() * 0.5f; }
    }

}
