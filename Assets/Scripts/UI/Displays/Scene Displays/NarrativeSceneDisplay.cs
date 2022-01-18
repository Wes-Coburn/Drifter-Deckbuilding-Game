using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NarrativeSceneDisplay : MonoBehaviour
{
    [SerializeField] private GameObject background;
    [SerializeField] private GameObject narrativeText;
    [SerializeField] private GameObject clipCounter;

    private Narrative narrative;
    private int currentClip;
    private TextMeshProUGUI clipCounterText;

    public Narrative CurrentNarrative
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
        currentClip = value;
        clipCounterText.SetText(currentClip + 1 + "/" + 
            narrative.NarrativeText.Length);
        background.GetComponent<Image>().sprite = CurrentNarrative.NarrativeBackground;
        DialogueManager.Instance.TimedText(narrative.NarrativeText[value], 
            narrativeText.GetComponent<TextMeshProUGUI>());
    }
    
    public void NextNarrative()
    {
        if (SceneLoader.SceneIsLoading) return;
        GetComponent<SoundPlayer>().PlaySound(0);

        if (DialogueManager.Instance.CurrentTextRoutine != null)
        {
            DialogueManager.Instance.StopTimedText(true);
            return;
        }
        if (++currentClip > narrative.NarrativeText.Length - 1)
            GameManager.Instance.EndNarrative();
        else SetCurrentNarrative(currentClip);
    }
    public void PreviousNarrative()
    {
        if (SceneLoader.SceneIsLoading) return;
        GetComponent<SoundPlayer>().PlaySound(0);

        if (--currentClip < 0) currentClip = 0;
        else SetCurrentNarrative(currentClip);
    }
}
