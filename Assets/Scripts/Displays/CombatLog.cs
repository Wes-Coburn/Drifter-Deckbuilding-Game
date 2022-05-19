using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CombatLog : MonoBehaviour
{
    [SerializeField] private GameObject combatLog;
    [SerializeField] private GameObject logText;

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
        scrollRect = combatLog.GetComponent<ScrollRect>();
        contentRect = scrollRect.content.GetComponent<RectTransform>();

        logTMPro = logText.GetComponent<TextMeshProUGUI>();
        logRect = logText.GetComponent<RectTransform>();

        allEntries = "";
        SetCombatLogText();
    }
    private void SetCombatLogText()
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
        SetCombatLogText();
    }
    public void NewLogEntry_PlayCard(GameObject card)
    {
        string logEntry = "";
        if (card.CompareTag(CombatManager.PLAYER_CARD)) logEntry += "You played <b><color=\"green\">";
        else logEntry += "Enemy played <b><color=\"red\">";
        logEntry += card.GetComponent<CardDisplay>().CardName + "</b></color> ";
        if (CombatManager.Instance.IsUnitCard(card)) logEntry += "(Unit).";
        else logEntry += "(Action).";
        NewLogEntry(logEntry);
    }
}
