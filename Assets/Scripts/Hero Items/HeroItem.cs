using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Hero Item", menuName = "Hero Items/Hero Item")]
public class HeroItem : ScriptableObject
{
    [SerializeField] private string itemName;
    [SerializeField] [TextArea] private string itemDescription;
    [SerializeField] private Sprite itemImage;
    [SerializeField] private bool isRareItem;
    [SerializeField] private List<EffectGroup> effectGroupList;
    [SerializeField] private List<CardAbility> linkedAbilities;

    public string ItemName { get => itemName; }
    public string ItemDescription { get => itemDescription; }
    public Sprite ItemImage { get => itemImage; }
    public bool IsRareItem { get => isRareItem; }
    public List<EffectGroup> EffectGroupList { get => effectGroupList; }
    public List<CardAbility> LinkedAbilities { get => linkedAbilities; }
    public bool IsUsed { get; set; }
}
