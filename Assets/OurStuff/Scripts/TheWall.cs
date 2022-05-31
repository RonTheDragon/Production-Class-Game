using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TheWall : MonoBehaviour
{
    public enum WallAttacks { None, Mines}
    public SOwall[] TheWallAttacks = new SOwall[4];
    public float WallLength = 90;
    public float Wallheight = 10;
    public float WallThickness = 0;
    public bool WallFacingZ;
    LayerMask Attackable;
    List<AbilityCoolDown> abilityCoolDowns = new List<AbilityCoolDown>();
    GameObject WallCooldowns;
    GameObject WallCooldownsStillAlive;

    // Start is called before the first frame update
    void Start()
    {
        Attackable = GameManager.instance.PlayerCanAttack;
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

                if (abilityCoolDowns.Count == WallCooldowns.transform.childCount)
                    if (abilityCoolDowns.Count > 0)
                {

                    for (int i = 0; i < abilityCoolDowns.Count; i++)
                    {
                        abilityCoolDowns[i].Cooldown -= Time.deltaTime;
                        WallCooldowns.transform.GetChild(i).GetChild(1).GetComponent<Image>().fillAmount = -(abilityCoolDowns[i].Cooldown / abilityCoolDowns[i].MaxCooldown) + 1;
                        if (abilityCoolDowns[i].Cooldown <= 0) { abilityCoolDowns.Remove(abilityCoolDowns[i]); Destroy(WallCooldowns.transform.GetChild(i).gameObject); break; }
                    }

                }
            }
        }
        else
        {
                if (WallCooldownsStillAlive == null)
                {
                    WallCooldownsStillAlive = GameManager.instance.GetComponent<PlayerRespawnManager>().WallCooldownsStorage;
                }

            if (abilityCoolDowns.Count == WallCooldownsStillAlive.transform.childCount)
            {
                if (abilityCoolDowns.Count > 0)
                {

                    for (int i = 0; i < abilityCoolDowns.Count; i++)
                    {
                        abilityCoolDowns[i].Cooldown -= Time.deltaTime;
                        WallCooldownsStillAlive.transform.GetChild(i).GetChild(1).GetComponent<Image>().fillAmount = -(abilityCoolDowns[i].Cooldown / abilityCoolDowns[i].MaxCooldown) + 1;
                        if (abilityCoolDowns[i].Cooldown <= 0) { abilityCoolDowns.Remove(abilityCoolDowns[i]); Destroy(WallCooldownsStillAlive.transform.GetChild(i).gameObject); break; }
                    }

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

            default: break;
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
        abilityCoolDowns.Add(new AbilityCoolDown(attack.Name, attack.AbilityCooldown));
        GameObject Circle = Instantiate(GameManager.instance.CooldownCircleObject, transform.position, WallCooldowns.transform.rotation, WallCooldowns.transform);
        if (attack.Image != null)
        {
            foreach (Transform c in Circle.transform)
            {
                c.GetComponent<Image>().sprite = attack.Image;
            }
        }
        Circle.name = attack.Name;
        WallAttack(attack);
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
            p.Damage = attack.Damage;
            p.Knock = attack.Knockback;
            p.Stagger = attack.Stagger;
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
                Debug.Log($"{Name} is in Cooldown ({(int)a.Cooldown}s)");
                return true;
            }
        }
        return false;
    }
}
