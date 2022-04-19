using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RecruitUnitButton : MonoBehaviour
{
    [SerializeField] private GameObject recruitCost;
    private UnitCard unitCard;
    public UnitCard UnitCard
    {
        get => unitCard;
        set
        {
            unitCard = value;
            TextMeshProUGUI txtGui = recruitCost.GetComponent<TextMeshProUGUI>();
            txtGui.SetText(GameManager.Instance.GetRecruitCost(unitCard, out bool isDiscounted).ToString());
            if (isDiscounted)
            {
                Button button = GetComponent<Button>();
                if (button == null)
                {
                    Debug.LogError("BUTTON IS NULL!");
                    return;
                }

                var colors = button.colors;
                colors.normalColor = Color.green;
                button.colors = colors;
            }
        }
    }
    public void OnClick() =>
        FindObjectOfType<CardPageDisplay>().RecruitUnitButton_OnClick(UnitCard);
}
