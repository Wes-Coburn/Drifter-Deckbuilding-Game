using UnityEngine;
using TMPro;

public class LocationPopupDisplay : MonoBehaviour
{
    [SerializeField] private GameObject locationName;
    [SerializeField] private GameObject locationDescription;
    [SerializeField] private GameObject objectivesDescription;

    public string LocationName
    {
        set
        {
            locationName.GetComponent<TextMeshProUGUI>().SetText(value);
        }
    }

    public string LocationDescription
    {
        set
        {
            locationDescription.GetComponent<TextMeshProUGUI>().SetText(value);
        }
    }

    public string ObjectivesDescription
    {
        set
        {
            objectivesDescription.GetComponent<TextMeshProUGUI>().SetText(value);
        }
    }

    public Vector2 WorldMapPosition
    {
        set
        {
            transform.position = value;
        }
    }
}
