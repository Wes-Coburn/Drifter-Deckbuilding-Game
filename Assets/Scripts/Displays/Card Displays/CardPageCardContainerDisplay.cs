using UnityEngine;

public class CardPageCardContainerDisplay : MonoBehaviour
{
    [SerializeField] private GameObject cardPageCard;
    [SerializeField] private GameObject cardCostButton;

    public GameObject CardPageCard { get => cardPageCard; }
    public GameObject CardCostButton { get => cardCostButton; }
}
