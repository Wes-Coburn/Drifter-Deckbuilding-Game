using UnityEngine;
using TMPro;

public class RecruitUnitButton : MonoBehaviour
{
    [SerializeField] private GameObject recruitCost;
    private int RecruitCost
    {
        set
        {
            recruitCost.GetComponent<TextMeshProUGUI>().SetText(value.ToString());
        }
    }
    public UnitCard UnitCard { get; set; }
    private void Awake()
    {
        RecruitCost = GameManager.LEARN_SKILL_COST;
    }
    public void OnClick() =>
        FindObjectOfType<CardPageDisplay>().RecruitUnitButton_OnClick(UnitCard);
}
