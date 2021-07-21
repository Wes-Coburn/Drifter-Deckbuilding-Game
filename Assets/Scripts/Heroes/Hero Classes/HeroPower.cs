using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Hero Power", menuName = "Heroes/Hero Power")]
public class HeroPower : ScriptableObject
{
    [Header("POWER NAME")]
    public string PowerName;
    [Header("POWER IMAGE")]
    public Sprite PowerSprite;
    [Header("POWER SOUND")]
    public string PowerSound;
    [Header("POWER DESCRIPTION")]
    [TextArea]
    public string PowerDescription;
    [Header("EFFECT GROUPS")]
    public List<EffectGroup> EffectGroupList;
}
