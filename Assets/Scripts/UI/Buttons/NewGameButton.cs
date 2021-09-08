using UnityEngine;
using UnityEngine.UI;

public class NewGameButton : MonoBehaviour
{
    public void OnClick()
    {
        UIManager.Instance.CreateExplicitLanguagePopup();
        GetComponent<Button>().interactable = false;
    }
}
