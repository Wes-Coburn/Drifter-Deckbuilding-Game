using UnityEngine;

public class CombatTestButton : MonoBehaviour
{
    [SerializeField] private PlayerHero playerTestHero;
    [SerializeField] private HeroAugment heroAugment;
    [SerializeField] private EnemyHero enemyTestHero;
    [SerializeField] private Card[] testCards;

    public void OnClick()
    {
        EnemyHero eh = ScriptableObject.CreateInstance<EnemyHero>();
        eh.LoadHero(enemyTestHero);
        PlayerHero ph = ScriptableObject.CreateInstance<PlayerHero>();
        ph.LoadHero(playerTestHero);
        DialogueManager.Instance.EngagedHero = eh;
        PlayerManager pMan = PlayerManager.Instance;
        pMan.PlayerHero = ph;
        pMan.AddAugment(heroAugment);
        foreach (Card c in testCards)
        {
            CardManager.Instance.AddCard(c, GameManager.PLAYER);
        }
        SceneLoader.LoadScene(SceneLoader.Scene.CombatScene);
        GameManager.Instance.IsCombatTest = true;
    }
}
