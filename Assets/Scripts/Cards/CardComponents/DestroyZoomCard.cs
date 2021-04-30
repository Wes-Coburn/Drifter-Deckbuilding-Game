using UnityEngine;

public class DestroyZoomCard : MonoBehaviour
{
    private UIManager UIManager;

    private void Start() => UIManager = GameObject.Find("UIManager").GetComponent<UIManager>();
    //private void Start() => UIManager = UIManager.Instance; // SINGLETON
    public void OnClick() => UIManager.DestroyAllZoomObjects();
}
