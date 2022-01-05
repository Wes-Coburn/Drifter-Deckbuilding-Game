using UnityEngine;
using UnityEngine.UI;

public class ContinueGameButton : MonoBehaviour
{
    private void Start()
    {
        bool savedGame = GameManager.Instance.LoadGame(true);
        GetComponent<Button>().interactable = savedGame;
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
