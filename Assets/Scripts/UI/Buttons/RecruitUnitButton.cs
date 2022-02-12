using UnityEngine;
using TMPro;

public class RecruitUnitButton : MonoBehaviour
{
    [SerializeField] private GameObject recruitCost;
    private UnitCard unitCard;
    private int RecruitCost
    {
        set
        {
            recruitCost.GetComponent<TextMeshProUGUI>().SetText(value.ToString());
        }
    }
    public UnitCard UnitCard
    {
        get => unitCard;
        set
        {
            unitCard = value;
            RecruitCost = GameManager.Instance.GetRecruitCost(unitCard);
        }
    }
    public void OnClick() =>
        FindObjectOfType<CardPageDisplay>().RecruitUnitButton_OnClick(UnitCard);
}
