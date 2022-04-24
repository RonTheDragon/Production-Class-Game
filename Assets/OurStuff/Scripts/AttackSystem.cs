using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AttackSystem : MonoBehaviour
{
    public float MaxStamina = 100;
    [HideInInspector]
    public float Stamina;
    public float StaminaRegan = 30;
    [HideInInspector]
    public float StaminaCost = 30;
    public float Tired = 30;
    [HideInInspector]
    public float AttackCooldown;
    [HideInInspector]
    public float Acooldown;

    // Start is called before the first frame update
    protected void Start()
    {
        Stamina = MaxStamina;
    }

    // Update is called once per frame
    protected void Update()
    {
        if (Acooldown > 0) { Acooldown -= Time.deltaTime; }

        if (Stamina < MaxStamina)
        {
            if (StaminaRegan > 0)
                Stamina += Time.deltaTime * StaminaRegan;
        }
        else
        {
            Stamina = MaxStamina;
        }
    }

    protected bool CanAttack()
    {
        if (Acooldown <= 0 && Stamina >= StaminaCost)
        {
            Stamina -= StaminaCost;
            return true;
        }
        return false;
    }

}