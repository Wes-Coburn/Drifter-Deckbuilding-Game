using UnityEngine;
using UnityEngine.EventSystems;

public class PowerZoom : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private GameObject powerPopupPrefab;
    [SerializeField] private GameObject abilityPopupBoxPrefab;
    [SerializeField] private GameObject abilityPopupPrefab;

    [SerializeField] private bool abilityPopupOnly;
    [SerializeField] private bool isUltimate;
    [SerializeField] private bool isEnemyPower;

    private GameObject powerPopup;
    private GameObject abilityPopupBox;
    public const string POWER_POPUP_TIMER = "PowerPopupTimer";
    public const string ABILITY_POPUP_TIMER = "AbilityBoxTimer";

    public HeroPower LoadedPower { get; set; }

    public void DestroyPowerPopup()
    {
        FunctionTimer.StopTimer(POWER_POPUP_TIMER);
        if (powerPopup != null)
        {
            Destroy(powerPopup);
            powerPopup = null;
        }
    }
    public void DestroyAbilityPopup()
    {
        FunctionTimer.StopTimer(ABILITY_POPUP_TIMER);
        if (abilityPopupBox != null)
        {
            Destroy(abilityPopupBox);
            abilityPopupBox = null;
        }
    }

    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        if (CardZoom.ZoomCardIsCentered || DragDrop.DraggingCard) return;
        DestroyPowerPopup();
        if (!abilityPopupOnly) FunctionTimer.Create(() => CreatePowerPopup(), 0.5f, POWER_POPUP_TIMER);
        else FunctionTimer.Create(() =>
        ShowLinkedAbilities(LoadedPower, CardZoom.ZOOM_SCALE_VALUE), 0.5f, POWER_POPUP_TIMER);
    }
    public void OnPointerExit(PointerEventData pointerEventData)
    {
        DestroyPowerPopup();
        DestroyAbilityPopup();
    }

    private void CreatePowerPopup()
    {
        float newX;
        float newY;

        if (!isEnemyPower)
        {
            newX = 300;
            newY = -360;
        }
        else
        {
            newX = -250;
            newY = 320;
        }

        Vector3 spawnPoint = new Vector2(newX, newY);
        float scaleValue = 2.5f;
        powerPopup = Instantiate(powerPopupPrefab, Managers.U_MAN.CurrentWorldSpace.transform);
        powerPopup.transform.localPosition = spawnPoint;
        powerPopup.transform.localScale = new Vector2(scaleValue, scaleValue);

        HeroPower hp;
        if (!isEnemyPower)
        {
            PlayerHeroDisplay phd = GetComponentInParent<PlayerHeroDisplay>();
            if (isUltimate) hp = (phd.HeroScript as PlayerHero).HeroUltimate;
            else hp = phd.HeroScript.HeroPower;
        }
        else
        {
            EnemyHeroDisplay ehd = GetComponentInParent<EnemyHeroDisplay>();
            hp = ehd.HeroScript.HeroPower;
        }


        if (hp == null)
        {
            Debug.LogError("HERO POWER IS NULL!");
            return;
        }
        powerPopup.GetComponent<PowerPopupDisplay>().PowerScript = hp;
        FunctionTimer.Create(() => ShowLinkedAbilities(hp, scaleValue), 0.75f, ABILITY_POPUP_TIMER);
    }

    private void ShowLinkedAbilities(HeroPower hp, float scaleValue)
    {
        //if (this == null) return; // TESTING

        if (hp == null)
        {
            Debug.LogError("HERO POWER IS NULL!");
            return;
        }

        abilityPopupBox = Instantiate(abilityPopupBoxPrefab, Managers.U_MAN.CurrentZoomCanvas.transform);
        Vector2 position = new Vector2();

        if (!abilityPopupOnly) // Combat Scene
        {
            if (!isEnemyPower) position.Set(-75, -50);
            else position.Set(0, -50); // TESTING
        }
        else
        {
            /*
            if (SceneLoader.IsActiveScene(SceneLoader.Scene.HeroSelectScene))
                position.Set(350, 100);
             */

            if (SceneLoader.IsActiveScene(SceneLoader.Scene.HomeBaseScene))
                position.Set(350, 0);
        }
        abilityPopupBox.transform.localPosition = position;
        abilityPopupBox.transform.localScale = new Vector2(scaleValue, scaleValue);
        foreach (CardAbility ca in hp.LinkedAbilities)
            CreateAbilityPopup(ca, abilityPopupBox.transform, 1);
    }

    private void CreateAbilityPopup(CardAbility ca, Transform parent, float scaleValue)
    {
        GameObject abilityPopup = Instantiate(abilityPopupPrefab, parent);
        abilityPopup.transform.localScale = new Vector2(scaleValue, scaleValue);
        abilityPopup.GetComponent<AbilityPopupDisplay>().DisplayAbilityPopup(ca, false, !isEnemyPower);
    }
}
