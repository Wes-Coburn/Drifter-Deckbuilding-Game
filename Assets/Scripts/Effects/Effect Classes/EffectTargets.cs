using UnityEngine;

[CreateAssetMenu(fileName = "New Effect Targets", menuName = "Effects/Effect Targets")]
public class EffectTargets : ScriptableObject
{
    [Header("TARGET NUMBER")]
    [Range(1, 10)]
    public int TargetNumber;

    [Header("TARGETS ALL POSSIBLE")]
    public bool TargetsAll;

    [Header("TARGETS SELF")]
    [Tooltip("Used for Units with effects that target themselves")]
    public bool TargetsSelf;

    [Header("PLAYER")]
    public bool PlayerHero;
    public bool PlayerUnit;
    public bool PlayerHand;

    [Header("ENEMY")]
    public bool EnemyHero;
    public bool EnemyUnit;
}
