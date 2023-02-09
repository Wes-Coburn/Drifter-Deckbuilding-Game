using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ReputationPopupDisplay : MonoBehaviour
{
    [SerializeField] private GameObject reputationTitle;
    [SerializeField] private GameObject reputationScore;
    [SerializeField] private Slider bonusTrack;
    [SerializeField] private GameObject tier1_Title;
    [SerializeField] private GameObject tier1_Bonus;
    [SerializeField] private GameObject tier2_Title;
    [SerializeField] private GameObject tier2_Bonus;
    [SerializeField] private GameObject tier3_Title;
    [SerializeField] private GameObject tier3_Bonus;

    private int bonusTier;

    private int BonusTier
    {
        set
        {
            bonusTier = value;
            switch (value)
            {
                case 0:
                    bonusTrack.value = 0;
                    break;
                case 1:
                    bonusTrack.value = 0.3f;
                    break;
                case 2:
                    bonusTrack.value = 0.7f;
                    break;
                case 3:
                    bonusTrack.value = 1;
                    break;
                default:
                    Debug.LogError("INVALID TIER!");
                    return;
            }
        }
    }

    private string Tier1_Bonus
    {
        set
        {
            TextMeshProUGUI txtGui = tier1_Bonus.transform.parent.GetComponent<TextMeshProUGUI>();
            bool isActive = false;
            if (bonusTier > 0) isActive = true;
            txtGui.SetText(ColorText(txtGui.text, isActive));
            tier1_Bonus.GetComponent<TextMeshProUGUI>().SetText(Managers.CA_MAN.FilterKeywords(value));
        }
    }

    private string Tier2_Bonus
    {
        set
        {
            TextMeshProUGUI txtGui = tier2_Bonus.transform.parent.GetComponent<TextMeshProUGUI>();
            bool isActive = false;
            if (bonusTier > 1) isActive = true;
            txtGui.SetText(ColorText(txtGui.text, isActive));
            tier2_Bonus.GetComponent<TextMeshProUGUI>().SetText(Managers.CA_MAN.FilterKeywords(value));
        }
    }

    private string Tier3_Bonus
    {
        set
        {
            TextMeshProUGUI txtGui = tier3_Bonus.transform.parent.GetComponent<TextMeshProUGUI>();
            bool isActive = false;
            if (bonusTier > 2) isActive = true;
            txtGui.SetText(ColorText(txtGui.text, isActive));
            tier3_Bonus.GetComponent<TextMeshProUGUI>().SetText(Managers.CA_MAN.FilterKeywords(value));
        }
    }

    private string ColorText(string text, bool isActive)
    {
        string color = isActive ? "green" : "red";
        return "<color=\"" + color + "\">" + text + "</color>";
    }

    private void Awake()
    {
        string tier1Bonus = GameManager.REPUTATION_TIER_1 + " - " + (GameManager.REPUTATION_TIER_2 - 1);
        string tier2Bonus = GameManager.REPUTATION_TIER_2 + " - " + (GameManager.REPUTATION_TIER_3 - 1);
        string tier3Bonus = GameManager.REPUTATION_TIER_3 + "+";
        tier1_Title.GetComponent<TextMeshProUGUI>().SetText("Tier 1 Bonus(" + tier1Bonus + ")");
        tier2_Title.GetComponent<TextMeshProUGUI>().SetText("Tier 2 Bonus(" + tier2Bonus + ")");
        tier3_Title.GetComponent<TextMeshProUGUI>().SetText("Tier 3 Bonus(" + tier3Bonus + ")");
    }

    public void DisplayReputationPopup(int reputation, int bonusTier, ReputationBonuses bonuses)
    {
        TextMeshProUGUI txtPro = reputationTitle.GetComponent<TextMeshProUGUI>();
        txtPro.SetText(bonuses.ReputationType.ToString() + " Reputation");
        txtPro.color = bonuses.ReputationColor;
        reputationScore.GetComponent<Image>().color = bonuses.ReputationColor;
        reputationScore.GetComponentInChildren<TextMeshProUGUI>().SetText(reputation.ToString());
        BonusTier = bonusTier;
        Tier1_Bonus = bonuses.Tier1_Bonus;
        Tier2_Bonus = bonuses.Tier2_Bonus;
        Tier3_Bonus = bonuses.Tier3_Bonus;
    }
}
