using UnityEngine;

public class LocationButton : MonoBehaviour
{
    public void OnClick()
    {
        SceneLoader.LoadScene(SceneLoader.Scene.DialogueScene);
    }
}
