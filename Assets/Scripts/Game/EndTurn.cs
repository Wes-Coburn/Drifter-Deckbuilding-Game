using UnityEngine;

public class EndTurn : MonoBehaviour
{
    public void OnClick() => GMEndTurn();
    private void GMEndTurn() => GameManager.Instance.EndTurn();
}
