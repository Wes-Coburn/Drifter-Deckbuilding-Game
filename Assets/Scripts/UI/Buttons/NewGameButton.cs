using UnityEngine;
public class NewGameButton : MonoBehaviour
{
    public void OnClick() => GameManager.Instance.NewGame();
}
