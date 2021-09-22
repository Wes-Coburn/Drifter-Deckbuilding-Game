using UnityEngine;
using TMPro;

public class LearnSkillPopupDisplay : MonoBehaviour
{
    [SerializeField] private GameObject popupText;

    private SkillCard skillCard;

    private string PopupText
    {
        set
        {
            popupText.GetComponent<TextMeshProUGUI>().SetText(value);
        }
    }
    public SkillCard SkillCard
    {
        get => skillCard;
        set
        {
            skillCard = value;
            string text = "Learn " + skillCard.CardName + " for 1 aether?";
            PopupText = text;
        }
    }

    public void ConfirmButton_OnClick()
    {
        CardManager.Instance.AddCard(skillCard, GameManager.PLAYER);
        CancelButton_OnClick();
        // Card added popup
    }

    public void CancelButton_OnClick() => 
        FindObjectOfType<CardPageDisplay>().DestroyLearnSkillPopup();
}
