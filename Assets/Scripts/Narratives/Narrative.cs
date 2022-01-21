using UnityEngine;

[CreateAssetMenu(fileName = "New Narrative", menuName = "Narrative")]
public class Narrative : ScriptableObject
{
    [SerializeField] private string narrativeName;
    [TextArea] [SerializeField] private string[] narrativeText;
    [SerializeField] private Sprite narrativeBackground;
    [SerializeField] private Sound narrativeSoundscape;
    public string NarrativeName { get => narrativeName; }
    public string[] NarrativeText { get => narrativeText; }
    public Sprite NarrativeBackground { get => narrativeBackground; }
    public Sound NarrativeSoundscape { get => narrativeSoundscape; }
}
