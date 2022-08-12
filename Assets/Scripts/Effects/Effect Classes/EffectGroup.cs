using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Effect Group", menuName = "Effects/Effect Group")]
public class EffectGroup : ScriptableObject
{
    [Header("Effect Targets")] public EffectTargets Targets;
    [Header("Resolve Independent")] public bool ResolveIndependent;
    [TextArea, Header("Effects Description")] public string EffectsDescription;
    [Header("Effects List")] public List<Effect> Effects;
}
