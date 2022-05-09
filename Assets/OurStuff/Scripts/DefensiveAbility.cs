using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefensiveAbility : MonoBehaviour
{
    public float HpProtection;
    public float KnockProtection;
    public float ProtectionTime;

    public bool AnimationTrigger;
    bool _alreadyON;
    public CharacterHealth EffectedHealth;
    
    void Start()
    {
        
    }

    
    void Update()
    {
        if (!_alreadyON && AnimationTrigger)
        {
            _alreadyON = true;
            sendProtection();
        }
        else if (_alreadyON && !AnimationTrigger)
        {
            _alreadyON = false;
        }
    }

    void sendProtection()
    {
        EffectedHealth.GainTempProtection(HpProtection, KnockProtection, ProtectionTime);
    }
}
