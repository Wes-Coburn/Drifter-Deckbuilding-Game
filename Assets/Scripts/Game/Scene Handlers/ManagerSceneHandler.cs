using UnityEngine;

public class ManagerSceneHandler : MonoBehaviour
{
    private void Start() => BundleLoader.BuildBundle("managers", () =>
    SceneLoader.LoadScene(SceneLoader.Scene.TitleScene, false, false));
}

