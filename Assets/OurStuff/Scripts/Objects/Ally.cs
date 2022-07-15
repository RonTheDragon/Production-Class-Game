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
    protected override void AttackAI()
    {
        if (Target != null)
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
                chasingTarget = false;
                Target = null;
            }
        }
        else { SearchForTarget(); }

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
            
        }

        if (eas.Stamina < eas.Tired) { NMA.speed = GetSpeed() * 0.5f; }
    }

}
