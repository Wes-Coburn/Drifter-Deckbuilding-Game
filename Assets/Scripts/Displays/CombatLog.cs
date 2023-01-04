using UnityEngine;
using TMPro;
using UnityEngine.UI;

[RequireComponent(typeof(ScrollRect))]
public class CombatLog : MonoBehaviour
{
    [SerializeField] private GameObject logText;
    [SerializeField] private GameObject logInfo;

    private ScrollRect scrollRect;
    private TextMeshProUGUI logTMPro;
    private RectTransform logRect;
    private RectTransform contentRect;
    private string allEntries;

    public enum LogEntryType
    {
        PlayCard
    }

    private void Awake()
    {
        scrollRect = GetComponent<ScrollRect>();
        contentRect = scrollRect.content.GetComponent<RectTransform>();

        logTMPro = logText.GetComponent<TextMeshProUGUI>();
        logRect = logText.GetComponent<RectTransform>();

        allEntries = "";
        UpdateCombatLog();
        logInfo.SetActive(false);
    }
    public void ScheduleLogUpdate() => EventManager.Instance.NewDelayedAction(() => UpdateCombatLog(), 0, true);
    private void UpdateCombatLog()
    {
        logTMPro.SetText(allEntries);
        int lines = logTMPro.textInfo.lineCount;
        float height = 19.25f * (lines + 2);
        logRect.sizeDelta = new Vector2(logRect.rect.width, height);
        contentRect.sizeDelta = new Vector2(contentRect.rect.width, height);
        scrollRect.verticalNormalizedPosition = 0;
    }
    public void NewLogEntry(string entry)
    {
        entry += "\n";
        allEntries += entry;
        UpdateCombatLog();
    }
    public void NewLogEntry_PlayCard(GameObject card)
    {
        string logEntry = "";
        if (card.CompareTag(CombatManager.PLAYER_CARD)) logEntry += "You played <b><color=\"green\">";
        else logEntry += "Enemy played <b><color=\"red\">";
        logEntry += card.GetComponent<CardDisplay>().CardName + "</b></color> ";
        if (CombatManager.IsUnitCard(card)) logEntry += "(Unit).";
        else logEntry += "(Action).";
        NewLogEntry(logEntry);
    }
}
