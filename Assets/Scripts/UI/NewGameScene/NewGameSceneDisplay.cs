using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NewGameSceneDisplay : MonoBehaviour
{
    [SerializeField] private GameObject skillCard_1;
    [SerializeField] private GameObject skillCard_2;
    [SerializeField] private GameObject heroPortrait;
    [SerializeField] private GameObject heroDescription;

    private GameObject currentSkill_1;
    private GameObject currentSkill_2;

    [SerializeField] private PlayerHero[] playerHeroes;
    private int selectedHero;
    public PlayerHero SelectedHero { get => playerHeroes[selectedHero]; }

    private void Start()
    {
        selectedHero = 0;
        DisplaySelectedHero();
    }

    public void NextHeroRight() => SelectNextHero(RightOrLeft.Right);
    public void NextHeroLeft() => SelectNextHero(RightOrLeft.Left);

    public enum RightOrLeft { Right, Left }
    private void SelectNextHero(RightOrLeft rol)
    {
        int lastHero = playerHeroes.Length - 1;
        if (rol == RightOrLeft.Right)
        {
            if (++selectedHero > lastHero) selectedHero = 0;
        }
        else
        {
            if (--selectedHero < 0) selectedHero = lastHero;
        }
        DisplaySelectedHero();
    }
    private void DisplaySelectedHero()
    {
        heroPortrait.GetComponent<Image>().sprite = SelectedHero.HeroPortrait;
        heroDescription.GetComponent<TextMeshProUGUI>().SetText(SelectedHero.HeroDescription);

        if (selectedHero > 1) return; // FOR TESTING ONLY

        if (currentSkill_1 != null)
        {
            Destroy(currentSkill_1);
            currentSkill_1 = null;
        }
        if (currentSkill_2 != null)
        {
            Destroy(currentSkill_2);
            currentSkill_2 = null;
        }

        currentSkill_1 = CardManager.Instance.ShowCard(SelectedHero.HeroSkills[0]);
        currentSkill_2 = CardManager.Instance.ShowCard(SelectedHero.HeroSkills[1]);
        currentSkill_1.transform.SetParent(skillCard_1.transform, false);
        currentSkill_2.transform.SetParent(skillCard_2.transform, false);
        Vector2 vec2 = new Vector2(4, 4);
        currentSkill_1.transform.localScale = vec2;
        currentSkill_2.transform.localScale = vec2;
    }
}
