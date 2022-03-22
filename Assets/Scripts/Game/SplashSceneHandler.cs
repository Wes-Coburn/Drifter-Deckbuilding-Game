using UnityEngine;
using UnityEngine.SceneManagement;

public class SplashSceneHandler : MonoBehaviour
{
    private void Start() => SceneManager.LoadScene("ManagerScene");
}
