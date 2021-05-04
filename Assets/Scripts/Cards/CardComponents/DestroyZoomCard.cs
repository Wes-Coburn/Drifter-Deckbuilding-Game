using UnityEngine;

public class DestroyZoomCard : MonoBehaviour
{
    public void OnClick() => UIManager.Instance.DestroyAllZoomObjects();
}
