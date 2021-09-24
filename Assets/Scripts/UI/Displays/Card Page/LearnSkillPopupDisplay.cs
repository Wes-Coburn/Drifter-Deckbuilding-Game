using UnityEngine;
using TMPro;

public class LearnSkillPopupDisplay : MonoBehaviour
{
    [SerializeField] private GameObject popupText;

    private UIManager uMan;
    private PlayerManager pMan;
    private SkillCard skillCard;

    private string PopupText
    {
        set
        {
            popupText.GetComponent<TextMeshProUGUI>().SetText(value);
        }
    }

    private void Awake()
    {
        uMan = UIManager.Instance;
        pMan = PlayerManager.Instance;
    }

    public SkillCard SkillCard
    {
        set
        {
            int aether = pMan.AetherCells;
            skillCard = value;
            string text = "Learn " + skillCard.CardName + 
                " for 2 aether? (You have " + aether + " aether)";
            PopupText = text;
        }
    }

    public void ConfirmButton_OnClick()
    {
        CardManager.Instance.AddCard(skillCard, GameManager.PLAYER);
        pMan.AetherCells -= 2;
        CancelButton_OnClick();
        uMan.DestroyCardPagePopup(); // Temporary fix, eventually reload the page
        // Card added popup
    }

    public void CancelButton_OnClick() => 
        uMan.DestroyLearnSkillPopup();
}
