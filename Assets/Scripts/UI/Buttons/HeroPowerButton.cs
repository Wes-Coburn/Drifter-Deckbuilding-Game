using UnityEngine;
public class HeroPowerButton : MonoBehaviour
{
    public void OnClick() => PlayerManager.Instance.UseHeroPower();
}
