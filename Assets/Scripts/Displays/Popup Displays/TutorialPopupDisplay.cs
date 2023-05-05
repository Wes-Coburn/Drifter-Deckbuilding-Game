using UnityEngine;

public class TutorialPopupDisplay : MonoBehaviour
{
    private GameManager gMan;
    private void Start() => gMan = GameManager.Instance;

    private void DestroySelf() =>
        UIManager.Instance.DestroyInteractablePopup(gameObject);

    public void CloseButton_OnClick() =>
        DestroySelf();

    public void PlayButton_OnClick()
    {
        DestroySelf();
        gMan.StartTutorial();
    }
    public void SkipButton_OnClick()
    {
        DestroySelf();
        gMan.NewGame();
    }
}
