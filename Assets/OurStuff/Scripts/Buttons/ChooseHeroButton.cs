using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ChooseHeroButton : MonoBehaviour
{
    int id;
    PlayerParameters Parameters;
    TMP_Text Role;
    TMP_Text Name;
    TMP_Text Health;
    TMP_Text Damage;

    // Start is called before the first frame update
    void Awake()
    {
        Role = transform.GetChild(0).GetComponent<TMP_Text>();
        Name = transform.GetChild(1).GetComponent<TMP_Text>();
        Health = transform.GetChild(2).GetComponent<TMP_Text>();
        Damage = transform.GetChild(3).GetComponent<TMP_Text>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChooseHero()
    {
        PlayerRespawnManager respawnManager = GameManager.instance.GetComponent<PlayerRespawnManager>();
        respawnManager.RespawnPlayer(Parameters);
        respawnManager.RemoveCard(id);
    }

    public void SetTheCharacter(PlayerParameters Parameters,int id)
    {
        this.id = id;
        this.Parameters = Parameters;
        Role.text = $"{Parameters.RoleName}";
        Name.text = $"Name: {Parameters.PersonalName}";
        Health.text = $"Health: {(int)Parameters.health}";
        Damage.text = $"Damage: {(int)(Parameters.damageMultiplier*100)}";
    }
}
