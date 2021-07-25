using UnityEngine;

public class NewGameSceneDisplay : MonoBehaviour
{
    [SerializeField] private GameObject skillCard_1;
    [SerializeField] private GameObject skillCard_2;
    [SerializeField] private GameObject selected_Hero_Portrait;

    [SerializeField] private Hero hero_Kili;
    [SerializeField] private Hero hero_Shaydra;
    [SerializeField] private Hero hero_Fentis;

    private void Start() => DisplayNewGameScene();
    public void DisplayNewGameScene()
    {
        GameObject skill_1 = CardManager.Instance.ShowCard(hero_Kili.HeroSkills[0]);
        GameObject skill_2 = CardManager.Instance.ShowCard(hero_Kili.HeroSkills[1]);
        skill_1.transform.SetParent(skillCard_1.transform, false);
        skill_2.transform.SetParent(skillCard_2.transform, false);
        Vector2 vec2 = new Vector2(4, 4);
        skill_1.transform.localScale = vec2;
        skill_2.transform.localScale = vec2;
    }
}
