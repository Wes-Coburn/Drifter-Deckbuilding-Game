using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NewAugmentPopupDisplay : MonoBehaviour
{
    private HeroAugment[] allAugments;
    private List<HeroAugment> availableAugments;
    private int selectedAugment;

    private HeroAugment LoadedAugment
    {
        get => availableAugments[selectedAugment];
    }

    [SerializeField] private GameObject augmentName, augmentImage, augmentDescription;

    private void Start()
    {
        allAugments = Resources.LoadAll<HeroAugment>("Hero Augments");
        availableAugments = new List<HeroAugment>();

        int startIndex = -1;
        int index = 0;

        foreach (HeroAugment aug in allAugments)
        {
            if (Managers.P_MAN.GetAugment(aug.AugmentName)) continue;

            availableAugments.Add(aug);
            if (aug.AugmentName == "Synaptic Stabilizer") startIndex = index;
            index++;
        }

        if (availableAugments.Count < 1)
        {
            Debug.LogError("NO AVAILABLE AUGMENTS!");
            return;
        }

        if (startIndex != -1) selectedAugment = startIndex;
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
        Managers.P_MAN.AddAugment(LoadedAugment, true);
        Managers.U_MAN.DestroyNewAugmentPopup();
        Managers.D_MAN.DisplayDialoguePopup();
        Managers.AN_MAN.CreateParticleSystem(gameObject, ParticleSystemHandler.ParticlesType.ButtonPress);
    }
}

