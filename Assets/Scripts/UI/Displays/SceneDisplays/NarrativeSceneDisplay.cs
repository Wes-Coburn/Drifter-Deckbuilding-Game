using UnityEngine;
using TMPro;

public class NarrativeSceneDisplay : MonoBehaviour
{
    [SerializeField] Narrative narrative;
    [SerializeField] GameObject narrativeText;

    private int currentNarrative;
    private void SetCurrentNarrative(int value)
    {
        currentNarrative = value;
        narrativeText.GetComponent<TextMeshProUGUI>().SetText(narrative.NarrativeText[value]);
    }

    private void Awake() => SetCurrentNarrative(0);

    public void NextNarrative()
    {
        if (++currentNarrative > narrative.NarrativeText.Length - 1)
            GameManager.Instance.EndNarrative();
        else SetCurrentNarrative(currentNarrative);
    }
    public void PreviousNarrative()
    {
        if (--currentNarrative < 0) currentNarrative = 0;
        else SetCurrentNarrative(currentNarrative);
    }
}
