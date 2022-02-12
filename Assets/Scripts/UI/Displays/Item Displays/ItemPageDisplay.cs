using System.Collections.Generic;
using UnityEngine;
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
        if (isReady) progressText = "NEXT ITEM DISCOUNTED!";
        else progressText = newProgress + "/" + GameManager.SHOP_LOYALTY_GOAL + " Items Purchased";
        progressBarText.GetComponent<TextMeshProUGUI>().SetText(progressText);

        if (isFirstDisplay && newProgress < 1) return; // TESTING
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
        foreach (HeroItem item in currentItems)
        {
            GameObject description = Instantiate(itemDescriptionPrefab, items.transform);
            description.GetComponentInChildren<ItemDescriptionDisplay>().LoadedItem = item;
        }

        int progress = GameManager.Instance.ShopLoyalty; // TESTING
        SetProgressBar(0, progress, false, true); // TESTING
    }

    public void CloseItemPageButton_OnClick()
    {
        if (SceneLoader.IsActiveScene(SceneLoader.Scene.DialogueScene))
            DialogueManager.Instance.DisplayDialoguePopup();
        UIManager.Instance.DestroyItemPagePopup();
    }
}

