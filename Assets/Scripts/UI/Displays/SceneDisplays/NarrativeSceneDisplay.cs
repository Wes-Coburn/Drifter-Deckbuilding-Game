using UnityEngine;
using TMPro;

public class NarrativeSceneDisplay : MonoBehaviour
{
    [SerializeField] private GameObject narrativeText;
    [SerializeField] private GameObject clipCounter;

    private Narrative narrative;
    private int currentNarrative;
    private TextMeshProUGUI clipCounterText;

    public Narrative Narrative
    {
        get => narrative;
        set
        {
            narrative = value;
            SetCurrentNarrative(0);
        }
    }

    private void Start() => 
        clipCounterText = clipCounter.GetComponent<TextMeshProUGUI>();

    private void SetCurrentNarrative(int value)
    {
        currentNarrative = value;
        clipCounterText.SetText(currentNarrative + 1 + "/" + 
            narrative.NarrativeText.Length);
        DialogueManager.Instance.TimedText(narrative.NarrativeText[value], 
            narrativeText.GetComponent<TextMeshProUGUI>());
    }
    
    public void NextNarrative()
    {
        if (DialogueManager.Instance.CurrentTextRoutine != null)
        {
            DialogueManager.Instance.StopTimedText(true);
            return;
        }
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
