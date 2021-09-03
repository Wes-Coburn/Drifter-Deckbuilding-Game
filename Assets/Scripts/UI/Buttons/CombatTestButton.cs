using UnityEngine;

public class CombatTestButton : MonoBehaviour
{
    [SerializeField] private PlayerHero playerTestHero;
    [SerializeField] private HeroAugment heroAugment;
    [SerializeField] private EnemyHero enemyTestHero;

    public void OnClick()
    {
        EnemyHero eh = ScriptableObject.CreateInstance<EnemyHero>();
        eh.LoadHero(enemyTestHero);
        PlayerHero ph = ScriptableObject.CreateInstance<PlayerHero>();
        ph.LoadHero(playerTestHero);
        DialogueManager.Instance.EngagedHero = eh;
        PlayerManager.Instance.PlayerHero = ph;
        PlayerManager.Instance.HeroAugments.Add(heroAugment);
        SceneLoader.LoadScene(SceneLoader.Scene.CombatScene);
        GameManager.Instance.IsCombatTest = true;
    }
}
