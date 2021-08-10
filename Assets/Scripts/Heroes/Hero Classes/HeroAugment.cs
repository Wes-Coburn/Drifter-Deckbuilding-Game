using UnityEngine;

[CreateAssetMenu(fileName = "New Hero Augment", menuName = "Heroes/Player/Hero Augment")]
public class HeroAugment : ScriptableObject
{
    [Header("AUGMENT NAME")]
    public string AugmentName;

    [Header("AUGMENT IMAGE")]
    public Sprite AugmentImage;

    [Header("AUGMENT DESCRIPTION")]
    [TextArea]
    public string AugmentDescription;
}
