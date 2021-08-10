using UnityEngine;

[CreateAssetMenu(fileName = "New NPC Hero", menuName = "Heroes/NPC/NPC Hero")]
public class NPCHero : Hero
{
    [Header("FIRST DIALOGUE CLIP")]
    [SerializeField]
    private DialogueClip firstDialogueClip;

    public DialogueClip FirstDialogueClip { get => firstDialogueClip; }
    public DialogueClip NextDialogueClip { get; set; }
    public int RespectScore { get; set; }

    private void Awake() => NextDialogueClip = FirstDialogueClip;

    public override void LoadHero(Hero hero)
    {
        base.LoadHero(hero);
        NPCHero npcH = hero as NPCHero;
        firstDialogueClip = npcH.FirstDialogueClip;
        NextDialogueClip = npcH.NextDialogueClip;
        RespectScore = npcH.RespectScore;
    }
}
