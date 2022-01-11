using System.Collections.Generic;
using UnityEngine;

public class ItemPageDisplay : MonoBehaviour
{
    [SerializeField] private GameObject items;
    [SerializeField] private GameObject itemDescriptionPrefab;
    [SerializeField] private GameObject noItemsTooltip;

    public List<HeroItem> CurrentItems { get; set; }

    private void Awake() => DisplayItems();

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
    }

    public void CloseItemPageButton_OnClick()
    {
        if (SceneLoader.IsActiveScene(SceneLoader.Scene.DialogueScene))
            DialogueManager.Instance.DisplayDialoguePopup();
        UIManager.Instance.DestroyItemPagePopup();
    }
}

