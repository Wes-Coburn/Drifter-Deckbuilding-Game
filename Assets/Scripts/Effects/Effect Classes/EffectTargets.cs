using UnityEngine;

[CreateAssetMenu(fileName = "New Effect Targets", menuName = "Effects/Effect Targets")]
public class EffectTargets : ScriptableObject
{
    [Header("TARGET NUMBER")]
    [Range(1, 10)]
    public int TargetNumber;

    [Header("TARGETS ALL POSSIBLE (for the selected heroes)")]
    public bool TargetsAll;

    [Header("PLAYER")]
    public bool PlayerHero;
    public bool PlayerFollower;
    public bool PlayerHand;

    [Header("ENEMY")]
    public bool EnemyHero;
    public bool EnemyFollower;
}
