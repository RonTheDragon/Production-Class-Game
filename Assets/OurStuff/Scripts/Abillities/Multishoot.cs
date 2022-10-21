using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Multishoot : Projectile , IpooledObject
{
    public float ExplosionRadius;
    List<RememberLocation> transforms = new List<RememberLocation>();
    bool started;

    // Start is called before the first frame update
    void Start()
    {
        foreach (Transform t in transform)
        {
            RememberLocation R = new RememberLocation();
            R.Position = t.localPosition ;
            R.Rotation = t.localRotation;
            transforms.Add(R);
        }
        started = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    new void FixedUpdate()
    {
        
    }

    public void OnObjectSpawn()
    {
        if (!started) Start();
        StartCoroutine(MultiShoot());
    }

    public IEnumerator MultiShoot() 
    {
        yield return null;
        int Count = 0;
        foreach (Transform t in transform)
        {
            t.gameObject.SetActive(false);
            t.localPosition = transforms[Count].Position;
            t.localRotation = transforms[Count].Rotation;
            Count++;
            Projectile p = t.GetComponent<Projectile>();
            p.Damage = Damage;
            p.Knock = Knock;
            p.Stagger = Stagger;
            p.Temperature = Temperature;
            p.Speed = Speed;
            p.Charge = Charge;
            p.Attackable = Attackable;
            p.Attacker = Attacker;
            p.Gravity = Gravity;
            if (p is ExplosiveProjectile)
            {
                ExplosiveProjectile E = (ExplosiveProjectile)p;
                ExplosionRadius = E.ExplosionRadius;
            }
            t.gameObject.SetActive(true);
        }
    }
}


class RememberLocation
{
    public Vector3 Position;
    public Quaternion Rotation;
}
