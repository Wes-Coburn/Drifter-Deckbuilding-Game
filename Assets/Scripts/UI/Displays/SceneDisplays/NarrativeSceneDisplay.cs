using UnityEngine;
using TMPro;

public class NarrativeSceneDisplay : MonoBehaviour
{
    [SerializeField] private GameObject narrativeText;
    private Narrative narrative;
    private int currentNarrative;
    
    public Narrative Narrative
    {
        get => narrative;
        set
        {
            narrative = value;
            SetCurrentNarrative(0);
        }
    }

    private void SetCurrentNarrative(int value)
    {
        currentNarrative = value;
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
        if (DialogueManager.Instance.CurrentTextRoutine != null)
        {
            DialogueManager.Instance.StopTimedText(true);
            return;
        }
        if (--currentNarrative < 0) currentNarrative = 0;
        else SetCurrentNarrative(currentNarrative);
    }
}
