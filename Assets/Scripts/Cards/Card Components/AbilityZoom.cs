using UnityEngine;

public class AbilityZoom : MonoBehaviour
{
    [SerializeField] private GameObject abilityPopupPrefab;
    private bool isHovering;
    public static GameObject AbilityPopup { get; set; }

    private void Start() => isHovering = false;

    private void Update()
    {        
        if (isHovering)
        {
            if (AbilityPopup == null)
            {
                isHovering = false;
                return;
            }

            Vector3 hoverPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            float yPos = hoverPoint.y + 100;
            AbilityPopup.transform.position = new Vector3(hoverPoint.x, yPos, -4);
        }
    }
    public void OnPointerEnter()
    {
        if (!CardZoom.ZoomCardIsCentered || DragDrop.CardIsDragging) return;
        if (AbilityPopup != null) Destroy(AbilityPopup);
        isHovering = true;
        CreateAbilityPopup();
    }
    public void OnPointerExit()
    {
        if (!CardZoom.ZoomCardIsCentered || DragDrop.CardIsDragging || UIManager.Instance.PlayerIsTargetting) return;
        isHovering = false;
        Destroy(AbilityPopup);
    }

    private void CreateAbilityPopup()
    {
        Vector3 vec3 = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        float yPos = vec3.y + 100;
        Vector3 spawnPoint = new Vector3(vec3.x, yPos, -2);

        AbilityPopup = Instantiate(abilityPopupPrefab, spawnPoint, Quaternion.identity);
        AbilityPopup.transform.localScale = new Vector2(2.5f, 2.5f);
        AbilityPopup.GetComponent<AbilityPopupDisplay>().ZoomAbilityScript = 
            gameObject.GetComponent<AbilityIconDisplay>().AbilityScript;
    }
}
