using UnityEngine;

public class DestroyAbilityPopup : MonoBehaviour
{
    public void OnPointerExit() => Destroy(AbilityZoom.AbilityPopup);
}
