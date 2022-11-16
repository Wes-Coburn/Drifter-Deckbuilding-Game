using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NewAugmentPopupDisplay : MonoBehaviour
{
    private PlayerManager pMan;
    private HeroAugment[] allAugments;
    private List<HeroAugment> availableAugments;
    private int selectedAugment;

    private HeroAugment LoadedAugment
    {
        get => availableAugments[selectedAugment];
    }
    
    [SerializeField] private GameObject augmentName;
    [SerializeField] private GameObject augmentImage;
    [SerializeField] private GameObject augmentDescription;

    private void Start()
    {
        pMan = PlayerManager.Instance;
        allAugments = Resources.LoadAll<HeroAugment>("Hero Augments");
        availableAugments = new List<HeroAugment>();

        foreach (HeroAugment aug in allAugments)
        {
            if (!pMan.GetAugment(aug.AugmentName))
                availableAugments.Add(aug);
        }

        if (availableAugments.Count < 1)
        {
            Debug.LogError("NO AVAILABLE AUGMENTS!");
            return;
        }

        if (availableAugments.Count > 4) selectedAugment = 4; // Start with Synaptic Stabilizer
        else selectedAugment = 0;

        DisplaySelectedAugment();
    }

    private void DisplaySelectedAugment()
    {
        augmentName.GetComponent<TextMeshProUGUI>().SetText(LoadedAugment.AugmentName);
        augmentImage.GetComponent<Image>().sprite = LoadedAugment.AugmentImage;
        augmentDescription.GetComponent<TextMeshProUGUI>().SetText(LoadedAugment.AugmentDescription);
    }

    public void NextButton_OnClick()
    {
        if (++selectedAugment > availableAugments.Count - 1)
            selectedAugment = 0;
        DisplaySelectedAugment();
    }

    public void PreviousButton_OnClick()
    {
        if (--selectedAugment < 0)
            selectedAugment = availableAugments.Count - 1;
        DisplaySelectedAugment();
    }

    public void ConfirmButton_OnClick()
    {
        pMan.AddAugment(LoadedAugment, true);
        UIManager.Instance.DestroyNewAugmentPopup();
        DialogueManager.Instance.DisplayDialoguePopup();

        AnimationManager.Instance.CreateParticleSystem(gameObject, ParticleSystemHandler.ParticlesType.ButtonPress); // TESTING
    }
}

