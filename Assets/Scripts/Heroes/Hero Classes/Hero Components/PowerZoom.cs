using UnityEngine;

public class PowerZoom : MonoBehaviour
{
    [SerializeField] private GameObject powerPopupPrefab;
    [SerializeField] private GameObject abilityPopupBoxPrefab;
    [SerializeField] private GameObject abilityPopupPrefab;
    [SerializeField] private bool abilityPopupOnly;

    private UIManager uMan;
    private GameObject powerPopup;
    private GameObject abilityPopupBox;
    public const string POWER_POPUP_TIMER = "PowerPopupTimer";
    public const string ABILITY_POPUP_TIMER = "AbilityBoxTimer";

    public HeroPower LoadedPower { get; set; }

    private void Awake()
    {
        uMan = UIManager.Instance;
    }

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

    public void OnPointerEnter()
    {
        if (CardZoom.ZoomCardIsCentered || DragDrop.DraggingCard) return;
        DestroyPowerPopup();
        if (!abilityPopupOnly) FunctionTimer.Create(() => CreatePowerPopup(), 0.5f, POWER_POPUP_TIMER);
        else FunctionTimer.Create(() => ShowLinkedAbilities(LoadedPower, 3), 0.5f, POWER_POPUP_TIMER);
    }
    public void OnPointerExit()
    {
        DestroyPowerPopup();
        DestroyAbilityPopup();
    }

    private void CreatePowerPopup()
    {
        if (this == null) return; // TESTING
        Transform tran = CombatManager.Instance.PlayerHero.transform;
        float newX = tran.position.x - 200;
        float newY = tran.position.y + 300;
        Vector3 spawnPoint = new Vector2(newX, newY);
        float scaleValue = 2.5f;
        powerPopup = Instantiate(powerPopupPrefab, uMan.CurrentWorldSpace.transform);
        powerPopup.transform.localPosition = spawnPoint;
        powerPopup.transform.localScale = new Vector2(scaleValue, scaleValue);
        HeroPower hp = GetComponentInParent<PlayerHeroDisplay>().PlayerHero.HeroPower;
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
        if (this == null) return; // TESTING

        if (hp == null)
        {
            Debug.LogError("HERO POWER IS NULL!");
            return;
        }
        abilityPopupBox = Instantiate(abilityPopupBoxPrefab,
            uMan.CurrentZoomCanvas.transform);
        Vector2 position = new Vector2();
        if (!abilityPopupOnly) position.Set(-75, -50); // Combat Scene
        else position.Set(350, 100); // Hero Select Scene
        abilityPopupBox.transform.localPosition = position;
        abilityPopupBox.transform.localScale = new Vector2(scaleValue, scaleValue);
        foreach (CardAbility ca in hp.LinkedAbilities) 
            CreateAbilityPopup(ca, abilityPopupBox.transform, 1);
    }

    private void CreateAbilityPopup(CardAbility ca, Transform parent, float scaleValue)
    {
        GameObject abilityPopup = Instantiate(abilityPopupPrefab, parent);
        abilityPopup.transform.localScale = new Vector2(scaleValue, scaleValue);
        abilityPopup.GetComponent<AbilityPopupDisplay>().AbilityScript = ca;
    }
}
