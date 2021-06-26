using UnityEngine;

public abstract class Card : ScriptableObject
{
    [Header("CARD SPRITES")]
    public Sprite CardArt;
    public Sprite CardBorder;
    
    [Header("ANIMATOR OVERRIDE CONTROLLER")]
    public AnimatorOverrideController animatorOverrideController;

    [Header("CARD DETAILS")]
    public string CardName;
    public string CardType;
    public string CardSubType;
    public int ActionCost;

    [Header("CARD DESCRIPTION")]
    [TextArea]
    public string CardDescription;
}
