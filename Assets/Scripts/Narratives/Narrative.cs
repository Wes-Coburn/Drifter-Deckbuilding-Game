using UnityEngine;

[CreateAssetMenu(fileName = "New Narrative", menuName = "Narrative")]
public class Narrative : ScriptableObject
{
    [SerializeField] private string narrativeName;
    [TextArea][SerializeField] private string[] narrativeText;
    [SerializeField] private Sprite narrativeBackground;
    [SerializeField] private Sound narrativeStartSound;
    [SerializeField] private Sound narrativeSoundscape;
    [SerializeField] private bool isGameEnd;
    public string NarrativeName { get => narrativeName; }
    public string[] NarrativeText { get => narrativeText; }
    public Sprite NarrativeBackground { get => narrativeBackground; }
    public Sound NarrativeStartSound { get => narrativeStartSound; }
    public Sound NarrativeSoundscape { get => narrativeSoundscape; }
    public bool IsGameEnd { get => isGameEnd; }
}
