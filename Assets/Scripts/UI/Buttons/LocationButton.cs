using UnityEngine;

public class LocationButton : MonoBehaviour
{
    [SerializeField] private GameObject locationPopupPrefab;
    private GameObject locationPopup;
    private UIManager uMan;

    private void Start()
    {
        uMan = UIManager.Instance;
    }

    public void OnClick()
    {
        SceneLoader.LoadScene(SceneLoader.Scene.DialogueScene);
    }

    public void OnPointerEnter()
    {
        locationPopup = Instantiate(locationPopupPrefab, uMan.CurrentCanvas.transform);
    }

    public void OnPointerExit()
    {
        if (locationPopup != null)
        {
            Destroy(locationPopup);
            locationPopup = null;
        }
    }
}
