using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHeroDisplay : HeroDisplay
{
    [SerializeField] private GameObject surgeMeter, surgeValue;

    private Slider surgeSlider;
    private TextMeshProUGUI surgeText;

    private void Awake()
    {
        if (Managers.G_MAN.IsTutorial) surgeMeter.SetActive(false);
        else
        {
            surgeSlider = surgeMeter.GetComponent<Slider>();
            surgeText = surgeValue.GetComponent<TextMeshProUGUI>();
        }
    }

    public void DisplaySurgeProgress(int turnsLeft, int surgeValue, int surgeDelay)
    {
        float progress = (surgeDelay - turnsLeft) / (float)surgeDelay;
        string text = $"<u>{turnsLeft} {(turnsLeft != 1 ? "turns" : "turn")}</u> until <b>Surge</b> ({surgeValue}x)";

        surgeSlider.SetValueWithoutNotify(progress);
        surgeText.SetText(text);
    }
}
