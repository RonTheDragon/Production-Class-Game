using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "class", menuName = "Combat/Player Class")]
public class SOclass : ScriptableObject
{
    public List<SOability> DownLeftClickAttacks = new List<SOability>();
    public List<SOability> DownRightClickAttacks = new List<SOability>();
    public List<SOability> UpLeftClickAttacks = new List<SOability>();
    public List<SOability> UpRightClickAttacks = new List<SOability>();
    public List<SOability> SpaceAbility = new List<SOability>();
    public List<SOability> F_Ability = new List<SOability>();
}
