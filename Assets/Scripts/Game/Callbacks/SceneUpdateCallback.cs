using UnityEngine;

public class SceneUpdateCallback : MonoBehaviour
{
    private bool isFirstUpdate = true;

    private void Update()
    {
        if (isFirstUpdate)
        {
            isFirstUpdate = false;
            SceneLoader.SceneUpdateCallback();
        }
    }
}
