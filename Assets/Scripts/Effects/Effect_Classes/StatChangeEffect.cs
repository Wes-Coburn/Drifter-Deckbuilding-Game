using UnityEngine;

[CreateAssetMenu(fileName = "New Stat Change Effect", menuName = "Effects/StatChange")]
public class StatChangeEffect : Effect
{
    public bool IsDefenseChange;
    public bool IsTemporary;
}
