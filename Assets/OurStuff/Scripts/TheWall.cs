using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TheWall : MonoBehaviour
{
    public enum WallAttacks { None, Mines , Particle}
    public SOwall[] TheWallAttacks = new SOwall[4];
    public float WallLength = 90;
    public float Wallheight = 10;
    public float WallThickness = 0;
    public bool WallFacingZ;
    LayerMask Attackable;
    List<AbilityCoolDown> abilityCoolDowns = new List<AbilityCoolDown>();
    GameObject WallCooldowns;
    GameObject UsedParticle;

    // Start is called before the first frame update
    void Start()
    {
        Attackable = GameManager.instance.PlayerCanAttack;
        SetUpAllCooldowns(true);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            if (TheWallAttacks[0]!=null)
            AttemptToWallAttack(TheWallAttacks[0]);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            if (TheWallAttacks[1] != null)
                AttemptToWallAttack(TheWallAttacks[1]);
        }
        else if(Input.GetKeyDown(KeyCode.Alpha3))
        {
            if (TheWallAttacks[2] != null)
                AttemptToWallAttack(TheWallAttacks[2]);
        }
        else if(Input.GetKeyDown(KeyCode.Alpha4))
        {
            if (TheWallAttacks[3] != null)
                AttemptToWallAttack(TheWallAttacks[3]);
        }

        if (GameManager.instance.Player != null)
        {
                if (WallCooldowns == null)
                {
                    WallCooldowns = GameManager.instance.Player.GetComponent<ThirdPersonMovement>().WallCooldowns;
                }
   
                if (abilityCoolDowns.Count == WallCooldowns.transform.childCount)
                {

                    for (int i = 0; i < abilityCoolDowns.Count; i++)
                    {
                            if (abilityCoolDowns[i].Cooldown > 0)
                            {
                                abilityCoolDowns[i].Cooldown -= Time.deltaTime;
                                WallCooldowns.transform.GetChild(i).GetChild(1).GetComponent<Image>().fillAmount = -(abilityCoolDowns[i].Cooldown / abilityCoolDowns[i].MaxCooldown) + 1;
                            }
                            else WallCooldowns.transform.GetChild(i).GetChild(1).GetComponent<Image>().fillAmount = 1;
                    }
                }
            
        }
        else
        {
            for (int i = 0; i < abilityCoolDowns.Count; i++)
            {
                if (abilityCoolDowns[i].Cooldown > 0)
                {
                    abilityCoolDowns[i].Cooldown -= Time.deltaTime;               
                }
            }
        }
       
    }

    public void WallAttack(SOwall attack)
    {
        switch (attack.wallAttack)
        {
            case WallAttacks.Mines:
                Mines(attack);
                break;
            case WallAttacks.Particle:
                ParticleRain(attack);
                break;
                

            default: break;
        }
    }

    public void Mines(SOwall attack)
    {
        int Amount = 10;
        if (attack is SOwallRanged)
        {
            SOwallRanged s = (SOwallRanged)attack;
            Amount = s.AmountOfShoots;
        }

        float X = (WallLength * -0.5f) + Random.Range(WallLength / Amount / 2, WallLength / Amount);
        for (int i = 0; i < Amount; i++)
        {
            X += Random.Range(WallLength / Amount / 2, WallLength/Amount);
            float Y = Random.Range(Wallheight, Wallheight+5);
            Shoot(attack, transform.position + (transform.forward* WallThickness) + transform.up * Y + transform.right*X, transform.rotation);
        }
        
    }

    public void ParticleRain(SOwall attack)
    {
        if (attack is SOwallParticle)
        {
            SOwallParticle AP = (SOwallParticle)attack;

            if (UsedParticle != null)
            {
                Destroy(UsedParticle);
                UsedParticle = null;
            }
            UsedParticle = Instantiate(AP.particleSystem.gameObject, transform.position+AP.Position, transform.rotation * Quaternion.Euler(AP.Rotation), transform);
            ParticleCollision pc = UsedParticle.GetComponent<ParticleCollision>();
            pc.w = this;
            pc.Damage = AP.Damage;
            pc.Knock = AP.Knockback;
            pc.Stagger = AP.Stagger;
        }
    }

    public void AttemptToWallAttack(SOwall attack)
    {
        if (attack.AbilityCooldown != 0)
        {
            if (abilityCoolDowns.Count != 0)
            {
                if (CheckAbilityCooldown(attack.Name)) return;
            }  
        }
        StartCooldown(attack);
        WallAttack(attack);
    }

    void StartCooldown(SOwall attack)
    {
        foreach (AbilityCoolDown c in abilityCoolDowns)
        {
            if (c.AbilityName == attack.Name)
            {
                c.Cooldown = c.MaxCooldown;
            }
        }
    }

    public void SetUpAllCooldowns(bool Reset)
    {
        if (WallCooldowns == null)
        {
            WallCooldowns = GameManager.instance.Player.GetComponent<ThirdPersonMovement>().WallCooldowns;
        }
        if (Reset)
        {
            abilityCoolDowns = new List<AbilityCoolDown>();
            foreach (Transform t in WallCooldowns.transform)
            {
                Destroy(t.gameObject);
            }
        }

        int count = 0;
        foreach (SOwall attack in TheWallAttacks)
        {
            if (attack != null)
            {
                if (Reset)
                {
                    AbilityCoolDown ac = new AbilityCoolDown(attack.Name, attack.AbilityCooldown);
                    ac.Cooldown = 0;
                    abilityCoolDowns.Add(ac);
                }
                GameObject Circle = Instantiate(GameManager.instance.CooldownCircleObject, transform.position, WallCooldowns.transform.rotation, WallCooldowns.transform);
                if (attack.Image != null)
                {
                    foreach (Transform c in Circle.transform)
                    {
                        c.GetComponent<Image>().sprite = attack.Image;
                    }
                }
                Circle.name = attack.Name;
            }
            else
            {
                if (Reset)
                {
                    abilityCoolDowns.Add(new AbilityCoolDown($"Empty Slot {count}", 0));
                }
                GameObject Circle = Instantiate(GameManager.instance.CooldownCircleObject, transform.position, WallCooldowns.transform.rotation, WallCooldowns.transform);
                Circle.name = $"Empty Slot {count}";
            }
            count++;
        }
    }

    public void Shoot(SOwall attack, Vector3 ShootFrom, Quaternion ShootDirection)
    {
        float ProjectileSpeed = 10;
        float ExplosionRadius = 10;
        string Bullet = "Mine";
        
        if (attack is SOwallRanged)
        {
            SOwallRanged a = (SOwallRanged)attack;
            ProjectileSpeed = a.ProjectileSpeed;
            Bullet = a.Projectile;
            if (attack is SOwallRangedExplode)
            {
                SOwallRangedExplode b = (SOwallRangedExplode)a;
                ExplosionRadius = b.ExplosionRadius;
            }
        }

        //GameObject TheBullet = Instantiate(Bullet, transform.position, transform.rotation);
        GameObject TheBullet = ObjectPooler.Instance.SpawnFromPool(Bullet, ShootFrom, ShootDirection);
        Projectile p = TheBullet.GetComponent<Projectile>();
        if (p != null)
        {
            if (attack is SOwallAttack)
            {
                SOwallAttack a = (SOwallAttack)attack;
                p.Damage = a.Damage;
                p.Knock = a.Knockback;
                p.Stagger = a.Stagger;
            }
            p.Speed = ProjectileSpeed;
            p.Attackable = Attackable;
            p.Attacker = gameObject;
            if (p is Mine)
            {
                Mine m = (Mine)p;
                m.ExplosionRadius = ExplosionRadius;
            }
        }
    }
    bool CheckAbilityCooldown(string Name)
    {
        foreach (AbilityCoolDown a in abilityCoolDowns)
        {
            if (a.AbilityName == Name)
            {
                if (a.Cooldown > 0)
                {
                    Debug.Log($"{Name} is in Cooldown ({(int)a.Cooldown}s)");
                    return true;
                }
            }
        }
        return false;
    }
}
