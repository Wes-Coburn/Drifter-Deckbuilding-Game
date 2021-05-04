using UnityEngine;

public class AbilityIconEvents : MonoBehaviour
{
    public bool IsZoomIcon = false;

    public void OnClick()
    {
        if (IsZoomIcon) UIManager.Instance.DestroyAllZoomObjects();
        else gameObject.GetComponentInParent<CardZoom>().OnClick();
    }
}