using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Effect Group", menuName = "Effects/Effect Group")]
public class EffectGroup : ScriptableObject
{
    [SerializeField] [TextArea] private string developerNotes;
    public EffectTargets Targets;
    public List<Effect> Effects;
    [Tooltip("Description of the effects")]
    [TextArea] public string EffectsDescription;
}
