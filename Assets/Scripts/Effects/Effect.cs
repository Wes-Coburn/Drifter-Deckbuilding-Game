using UnityEngine;

[CreateAssetMenu(fileName = "New Effect", menuName = "Effect")]
public class Effect : ScriptableObject
{
    [Tooltip("Types: Damage, Healing, Mark")]
    public string Type;

    [Tooltip("The value of the effect")]
    [Range(1, 10)]
    public int Value;

    [Tooltip("Targets: Self, Opponent, Ally, Enemy")]
    public string Targets;

    [Tooltip("The number of targets")]
    [Range(1, 3)]
    public int TargetNumber;
}
