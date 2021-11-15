using UnityEngine;
using TMPro;

public class CloneUnitButton : MonoBehaviour
{
    [SerializeField] private GameObject cloneCost;
    private int CloneCost
    {
        set
        {
            cloneCost.GetComponent<TextMeshProUGUI>().SetText(value.ToString());
        }
    }
    public UnitCard UnitCard { get; set; }
    private void Awake()
    {
        CloneCost = GameManager.CLONE_UNIT_COST;
    }
    public void OnClick() =>
        FindObjectOfType<CardPageDisplay>().CloneUnitButton_OnClick(UnitCard);
}
