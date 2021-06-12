using UnityEngine;

public class CardSelect : MonoBehaviour
{
    public GameObject CardOutline;
    public void OnClick()
    {
        if (UIManager.Instance.PlayerIsTargetting)
        {
            EffectManager.Instance.SelectTarget(gameObject);
        }
    }
}
