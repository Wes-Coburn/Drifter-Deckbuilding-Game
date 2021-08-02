using UnityEngine;

[CreateAssetMenu(fileName = "New NPC Hero", menuName = "Heroes/NPC Hero")]
public class NPCHero : Hero
{
    [Header("FIRST DIALOGUE CLIP")]
    public DialogueClip FirstDialogueClip;
    public DialogueClip NextDialogueClip { get; set; }
    public int RespectScore { get; set; }
    private void Awake() => NextDialogueClip = FirstDialogueClip;
}
