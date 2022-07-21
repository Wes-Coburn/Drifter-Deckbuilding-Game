using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemPageDisplay : MonoBehaviour
{
    [SerializeField] private GameObject items;
    [SerializeField] private GameObject itemDescriptionPrefab;
    [SerializeField] private GameObject noItemsTooltip;

    [Header("PROGRESS BAR")]
    [SerializeField] private GameObject progressBar;
    [SerializeField] private GameObject progressFill;
    [SerializeField] private GameObject progressBarText;

    public List<HeroItem> CurrentItems { get; set; }

    private void Awake() => DisplayItems();
    public void SetProgressBar(int currentProgress, int newProgress, bool isReady, bool isFirstDisplay = false)
    {
        string progressText;
        if (isReady) progressText = "DISCOUNT APPLIED!";
        else progressText = newProgress + "/" + GameManager.SHOP_LOYALTY_GOAL + " Items Purchased";
        progressBarText.GetComponent<TextMeshProUGUI>().SetText(progressText);
        if (isFirstDisplay && newProgress < 1) return;
        AnimationManager.Instance.SetProgressBar(AnimationManager.ProgressBarType.Item,
            currentProgress, newProgress, isReady, progressBar, progressFill);
    }
    private void DisplayItems()
    {
        List<HeroItem> currentItems = GameManager.Instance.ShopItems;
        if (currentItems.Count < 1)
        {
            noItemsTooltip.SetActive(true);
            return;
        }
        noItemsTooltip.SetActive(false);

        ScrollRect scrollRect = GetComponent<ScrollRect>();

        int rows = currentItems.Count / 4;
        if (rows < 1) rows = 1;
        float height = rows * 450;
        RectTransform itemRect = scrollRect.content.GetComponent<RectTransform>();
        itemRect.sizeDelta = new Vector2(itemRect.rect.width, height);

        foreach (HeroItem item in currentItems)
        {
            GameObject description = Instantiate(itemDescriptionPrefab, items.transform);
            description.GetComponentInChildren<ItemDescriptionDisplay>().LoadedItem = item;
        }

        scrollRect.verticalNormalizedPosition = 1;

        int progress = GameManager.Instance.ShopLoyalty;
        SetProgressBar(0, progress, false, true);
    }

    public void CloseItemPageButton_OnClick()
    {
        if (SceneLoader.IsActiveScene(SceneLoader.Scene.DialogueScene))
            DialogueManager.Instance.DisplayDialoguePopup();
        UIManager.Instance.DestroyItemPagePopup();
    }
}

