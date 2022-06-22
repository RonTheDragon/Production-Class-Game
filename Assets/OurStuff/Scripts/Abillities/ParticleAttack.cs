using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleAttack : Attack
{
    [HideInInspector]
    public ParticleSystem particle;
    public bool AnimationTrigger;
    public enum ParticleType { Single , Spam , Play}
    public ParticleType particleType;
    bool _alreadyON;
    public int ParticleAmount = 1;

    public float AttackCooldown;
    [HideInInspector]
    public float cooldown;
    [HideInInspector]
    public ParticleCollision pc;

    void Awake()
    {
       
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (cooldown > 0) { cooldown -= Time.deltaTime; }

        if (particleType == ParticleType.Spam)
        {
            if (AnimationTrigger)
            {
                Shoot();
            }
        }
        else 
        {
            if (!_alreadyON && AnimationTrigger)
            {
                _alreadyON = true;
                if (particleType == ParticleType.Single)
                Shoot();
                else if (particleType == ParticleType.Play)
                Play();
            }
            else if (_alreadyON && !AnimationTrigger)
            {
                _alreadyON = false;
            }
        }
    }

    public void Shoot()
    {
        particle.Emit(ParticleAmount);
    }

    public void Play()
    {
        particle.Play();
    }

    public void CreateParticleSystem(ParticleSystem p,string ParticleName)
    {
        particle = Instantiate(p, transform.position, transform.rotation, transform);
        pc = particle.gameObject.AddComponent<ParticleCollision>();
        pc.p = this;
        pc.pName = ParticleName;
    }

    public void ReplaceParticleSystem(ParticleSystem p, string ParticleName)
    {
        Destroy(pc.gameObject);
        CreateParticleSystem(p, ParticleName);
    }

}
