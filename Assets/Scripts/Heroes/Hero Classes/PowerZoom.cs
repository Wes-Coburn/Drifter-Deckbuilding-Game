using UnityEngine;

public class PowerZoom : MonoBehaviour
{
    /* PREFABS */
    [SerializeField] private GameObject powerPopupPrefab;

    /* STATIC_CLASS_VARIABLES */
    public static GameObject PowerPopup { get; set; }

    /* CLASS_VARIABLES */
    private bool isHovering;

    private void Start() => isHovering = false;

    private void Update()
    {
        if (isHovering)
        {
            if (PowerPopup == null)
            {
                isHovering = false;
                return;
            }

            Vector3 hoverPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            float yPos = hoverPoint.y + 100;
            PowerPopup.transform.position = new Vector3(hoverPoint.x, yPos, -4);
        }
    }
    public void OnPointerEnter()
    {
        if (CardZoom.ZoomCardIsCentered || DragDrop.CardIsDragging || UIManager.Instance.PlayerIsTargetting) return;
        if (PowerPopup != null) Destroy(PowerPopup);
        isHovering = true;
        CreatePowerPopup();
    }
    public void OnPointerExit()
    {
        if (CardZoom.ZoomCardIsCentered || DragDrop.CardIsDragging || UIManager.Instance.PlayerIsTargetting) return;
        isHovering = false;
        Destroy(PowerPopup);
    }

    private void CreatePowerPopup()
    {
        Vector3 vec3 = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        float yPos = vec3.y + 100;
        Vector3 spawnPoint = new Vector3(vec3.x, yPos, -2);

        PowerPopup = Instantiate(powerPopupPrefab, spawnPoint, Quaternion.identity);
        PowerPopup.transform.localScale = new Vector2(2.5f, 2.5f);

        PowerPopup.GetComponent<PowerPopupDisplay>().PowerScript = gameObject.GetComponentInParent<HeroDisplay>().HeroScript.HeroPower;
    }
}
