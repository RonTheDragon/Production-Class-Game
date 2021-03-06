using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Enemy : CharacterAI , IpooledObject
{
    float FixStuck = 5;
    new void Awake()
    {
        TheBody = transform;
        base.Awake();
    }
    new void Start()
    {
        base.Start();
        Targetable = GameManager.instance.enemiesCanAttack;
        CanSee = GameManager.instance.enemiesCanSee;
    }
    new void Update()
    {
        base.Update();
    }

    protected override void Behaviour()
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
                        chasingTarget = true;
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
                NMA.speed = 0; NMA.SetDestination(transform.position);
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
                NMA.speed = 0; NMA.SetDestination(transform.position);
            }

            if (TheWall == null)
            {
                Wander();
                eas.CancelAttack();
            }
            else
            {
                AttackWall();
            }
        }

        if (eas.Stamina < eas.Tired) { NMA.speed = GetSpeed() * 0.5f; }

        if (eas.AttackCooldown <= 0 && eas.Stamina == eas.MaxStamina && anim.GetInteger("Walk")==0)
        {
            FixStuck -= Time.deltaTime;
            if (FixStuck < 0)
            {
                Debug.Log("Stuck!!!!");
                NMA.SetDestination(GameManager.instance.Player.transform.position);
            }
        }
        else
        {
            FixStuck = 5;
        }
    }
}