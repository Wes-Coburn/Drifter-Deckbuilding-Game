using UnityEngine;

public class ManagerSceneHandler : MonoBehaviour
{
    private void Start() => BundleLoader.BuildManagers(() =>
    SceneLoader.LoadScene(SceneLoader.Scene.TitleScene, false, false));
}

