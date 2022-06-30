using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChooseHeroButton : MonoBehaviour
{
    int id;
    PlayerParameters Parameters;
    TMP_Text Role;
    Image    Icon;
    TMP_Text Name;
    TMP_Text Health;
    TMP_Text Damage;

    // Start is called before the first frame update
    void Awake()
    {
        Role = transform.GetChild(0).GetComponent<TMP_Text>();
        Icon = transform.GetChild(1).GetComponent<Image>();
        Name = transform.GetChild(2).GetComponent<TMP_Text>();
        Health = transform.GetChild(3).GetComponent<TMP_Text>();
        Damage = transform.GetChild(4).GetComponent<TMP_Text>();
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
        Icon.sprite = Parameters.icon;
        Name.text = $"Name: {Parameters.PersonalName}";
        Health.text = $"Health: {(int)Parameters.health}";
        Damage.text = $"Damage: {(int)(Parameters.damageMultiplier*100)}";
    }
}
