using UnityEngine;

[CreateAssetMenu(fileName = "New Narrative", menuName = "Narrative")]
public class Narrative : ScriptableObject
{
    [TextArea] [SerializeField] string[] narrativeText;
    public string[] NarrativeText { get => narrativeText; }
}
