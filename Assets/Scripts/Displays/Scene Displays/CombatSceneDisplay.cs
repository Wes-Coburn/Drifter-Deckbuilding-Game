using UnityEngine;

public class CombatSceneDisplay : MonoBehaviour
{
    [SerializeField] private GameObject backgroundImage;
    private void Start()
    {
        GameManager gMan = GameManager.Instance;
        Sprite background = gMan.GetLocationBackground();
        if (background != null)
            backgroundImage.GetComponent<SpriteRenderer>().sprite = background;
        else Debug.LogError("BACKGROUND IS NULL!");
    }
}
