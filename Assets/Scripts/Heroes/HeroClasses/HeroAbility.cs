using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Hero Ability", menuName = "Heroes/Hero Ability")]
public class HeroAbiliity : ScriptableObject
{
    [Header("ABILITY NAME")]
    public string AbilityName;
    [Header("ABILITY DESCRIPTION")]
    [TextArea]
    public string AbilityDescription;
    [Header("EFFECT GROUP")]
    public List<Effect> EffectGroup;
}
