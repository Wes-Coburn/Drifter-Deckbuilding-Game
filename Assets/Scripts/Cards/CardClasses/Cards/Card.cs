using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Card : ScriptableObject
{
    public Sprite CardArt;
    public Sprite CardBorder;
    
    public AnimatorOverrideController animatorOverrideController;

    public string CardName;
    public string CardType;
    public string CardSubType;
    public int ActionCost;
}
