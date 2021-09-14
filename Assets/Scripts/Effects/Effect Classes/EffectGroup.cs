using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Effect Group", menuName = "Effects/Effect Group")]
public class EffectGroup : ScriptableObject
{
    public EffectTargets Targets;
    public List<Effect> Effects;

    [TextArea]
    [Tooltip("Description of the effects")]
    public string EffectsDescription;

    [Header("EFFECT GROUP SOUND")]
    [Tooltip("Sound will be played unless Sound2 is not null")]
    public string EffectGroupSound;
    [Tooltip("Sound2 will be played only unless null")]
    public Sound EffectGroupSound2;
}
