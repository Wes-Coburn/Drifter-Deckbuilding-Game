using UnityEngine;

public class CardSelect : MonoBehaviour
{
    public GameObject CardOutline;
    public void OnClick()
    {
        if (UIManager.Instance.PlayerIsTargetting)
        {
            Debug.Log("Card Select OnClick()!");
            EffectManager.Instance.SelectTarget(gameObject);
        }
    }
}
