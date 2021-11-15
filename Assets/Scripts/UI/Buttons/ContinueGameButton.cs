using UnityEngine;
using UnityEngine.UI;

public class ContinueGameButton : MonoBehaviour
{
    private void Start()
    {
        GetComponent<Button>().interactable =
            GameManager.Instance.LoadGame(true);
    }

    public void OnClick()
    {
        if (SceneLoader.SceneIsLoading) return; // TESTING

        if (GameManager.Instance.LoadGame())
        {
            GetComponent<SoundPlayer>().PlaySound(0);
            SceneLoader.LoadScene(SceneLoader.Scene.WorldMapScene);
        }
        else Debug.LogError("FILE NOT FOUND!");
    }
}
