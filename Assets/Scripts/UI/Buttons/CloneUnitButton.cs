using UnityEngine;
using TMPro;

public class CloneUnitButton : MonoBehaviour
{
    [SerializeField] private GameObject cloneCost;
    private UnitCard unitCard;
    private int CloneCost
    {
        set
        {
            cloneCost.GetComponent<TextMeshProUGUI>().SetText(value.ToString());
        }
    }
    public UnitCard UnitCard
    {
        get => unitCard;
        set
        {
            unitCard = value;
            CloneCost = GameManager.Instance.GetCloneCost(unitCard);
        }
    }
    public void OnClick() =>
        FindObjectOfType<CardPageDisplay>().CloneUnitButton_OnClick(UnitCard);
}
