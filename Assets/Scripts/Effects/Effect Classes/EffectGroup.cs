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
}
