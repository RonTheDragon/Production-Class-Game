using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Enemy : CharacterAI , IpooledObject
{
    new void Awake()
    {
        TheBody = transform.GetChild(0);
        base.Awake();
    }
    new void Start()
    {
        base.Start();
        Targetable = GameManager.instance.enemiesCanAttack;
        CanSee = GameManager.instance.enemiesCanSee;
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
                chasingTarget = true;
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

            if (TheWall == null)
            {
                Wander();
            }
            else
            {
                AttackWall();
            }
        }

        if (eas.Stamina < eas.Tired) { NMA.speed = GetSpeed() * 0.5f; }
    }
}