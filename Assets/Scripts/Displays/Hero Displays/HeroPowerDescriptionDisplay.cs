using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HeroPowerDescriptionDisplay : MonoBehaviour
{
    [SerializeField] private GameObject heroPowerCost, heroPowerImage, heroPowerDescription;

    public void DisplayHeroPower(HeroPower heroPower, bool isUltimate, bool unlocked = true)
    {
        var powerZoom = GetComponent<PowerZoom>();
        powerZoom.enabled = unlocked;
        powerZoom.LoadedPower = unlocked ? heroPower : null;
        heroPowerCost.GetComponent<TextMeshProUGUI>().SetText(unlocked ? heroPower.PowerCost.ToString() : UIManager.LOCK_TEXT_SHORT);
        heroPowerImage.GetComponent<Image>().sprite = heroPower.PowerSprite; // Keep image revealed

        string description;
        if (unlocked) description = $"<b><u>{heroPower.PowerName}" +
                $"{(isUltimate ? " (Ultimate)" : "")}:</b></u> {Managers.CA_MAN.FilterKeywords(heroPower.PowerDescription)}";
        else description = UIManager.LOCK_TEXT;
        heroPowerDescription.GetComponent<TextMeshProUGUI>().SetText(description);
    }
}
