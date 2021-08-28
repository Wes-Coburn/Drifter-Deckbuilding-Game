using UnityEngine;

public class PowerZoom : MonoBehaviour
{
    /* PREFABS */
    [SerializeField] private GameObject powerPopupPrefab;

    /* STATIC_CLASS_VARIABLES */
    public static GameObject PowerPopup { get; set; }
    public void OnPointerEnter()
    {
        if (CardZoom.ZoomCardIsCentered || DragDrop.DraggingCard) return;
        if (PowerPopup != null) Destroy(PowerPopup);
        CreatePowerPopup();
    }
    public void OnPointerExit()
    {
        if (PowerPopup != null)
        {
            Destroy(PowerPopup);
            PowerPopup = null;
        }
    }

    private void CreatePowerPopup()
    {
        Transform tran = CardManager.Instance.PlayerHero.transform;
        float newX = tran.position.x - 200;
        float newY = tran.position.y + 250;
        Vector3 spawnPoint = new Vector3(newX, newY, -2);

        PowerPopup = Instantiate(powerPopupPrefab, spawnPoint, Quaternion.identity);
        PowerPopup.transform.localScale = new Vector2(2.5f, 2.5f);
        HeroPower hp = gameObject.GetComponentInParent<PlayerHeroDisplay>().PlayerHero.HeroPower;
        PowerPopup.GetComponent<PowerPopupDisplay>().PowerScript = hp;
        ShowLinkedAbilities(hp);
    }

    private void ShowLinkedAbilities(HeroPower hp)
    {
        foreach (CardAbility ca in hp.LinkedAbilities)
        {
            // blank
        }
    }
}
