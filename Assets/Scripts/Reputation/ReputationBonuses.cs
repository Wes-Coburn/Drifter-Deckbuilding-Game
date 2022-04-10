using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Reputation Bonuses", menuName = "Reputation Bonuses")]
public class ReputationBonuses : ScriptableObject
{
    [SerializeField] private GameManager.ReputationType reputationType;
    [SerializeField] private Color reputationColor;
    [Space]
    [SerializeField] [TextArea] private string tier1_Bonus;
    [SerializeField] private List<EffectGroup> tier1_Effects;
    [SerializeField] [Range(1, 3)] private int tier1_ResolveOrder;
    [Space]
    [SerializeField] [TextArea] private string tier2_Bonus;
    [SerializeField] private List<EffectGroup> tier2_Effects;
    [SerializeField] [Range(1, 3)] private int tier2_ResolveOrder;
    [Space]
    [SerializeField] [TextArea] private string tier3_Bonus;
    [SerializeField] private List<EffectGroup> tier3_Effects;
    [SerializeField] [Range(1, 3)] private int tier3_ResolveOrder;

    public GameManager.ReputationType ReputationType { get => reputationType; }
    public Color ReputationColor { get => reputationColor; }
    public string Tier1_Bonus { get => tier1_Bonus; }
    public List<EffectGroup> Tier1_Effects { get => tier1_Effects; }
    public int Tier1_ResolveOrder { get => tier1_ResolveOrder; }
    public string Tier2_Bonus { get => tier2_Bonus; }
    public List<EffectGroup> Tier2_Effects { get => tier2_Effects; }
    public int Tier2_ResolveOrder { get => tier2_ResolveOrder; }
    public string Tier3_Bonus { get => tier3_Bonus; }
    public List<EffectGroup> Tier3_Effects { get => tier3_Effects; }
    public int Tier3_ResolveOrder { get => tier3_ResolveOrder; }
}
