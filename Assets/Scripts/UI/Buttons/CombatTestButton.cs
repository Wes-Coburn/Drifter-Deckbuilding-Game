using UnityEngine;

public class CombatTestButton : MonoBehaviour
{
    [SerializeField] private PlayerHero playerTestHero;
    [SerializeField] private EnemyHero enemyTestHero;
    [SerializeField] private HeroAugment[] testAugments;
    [SerializeField] private HeroItem[] testItems;

    [SerializeField] private bool enableTestCards_1;
    [SerializeField] private Card[] testCards_1;
    [SerializeField] private bool enableTestCards_2;
    [SerializeField] private Card[] testCards_2;
    [SerializeField] private bool enableTestCards_3;
    [SerializeField] private Card[] testCards_3;
    [SerializeField] private bool enableTestCards_4;
    [SerializeField] private Card[] testCards_4;

    public void OnClick()
    {
        if (SceneLoader.SceneIsLoading) return;

        EnemyHero eh = ScriptableObject.CreateInstance<EnemyHero>();
        eh.LoadHero(enemyTestHero);
        PlayerHero ph = ScriptableObject.CreateInstance<PlayerHero>();
        ph.LoadHero(playerTestHero);
        DialogueManager.Instance.EngagedHero = eh;
        PlayerManager pMan = PlayerManager.Instance;
        pMan.PlayerHero = ph;

        // Test Augments
        foreach (HeroAugment aug in testAugments)
            pMan.AddAugment(aug);
        // Test Cards
        if (enableTestCards_1)
        {
            foreach (Card c in testCards_1)
                CardManager.Instance.AddCard(c, GameManager.PLAYER);
        }
        if (enableTestCards_2)
        {
            foreach (Card c in testCards_2)
                CardManager.Instance.AddCard(c, GameManager.PLAYER);
        }
        if (enableTestCards_3)
        {
            foreach (Card c in testCards_3)
                CardManager.Instance.AddCard(c, GameManager.PLAYER);
        }
        if (enableTestCards_4)
        {
            foreach (Card c in testCards_4)
                CardManager.Instance.AddCard(c, GameManager.PLAYER);
        }
        // Test Items
        foreach (HeroItem i in testItems)
            pMan.HeroItems.Add(i);

        CardManager caMan = CardManager.Instance;
        foreach (UnitCard uc in caMan.PlayerStartUnits)
            for (int i = 0; i < GameManager.PLAYER_START_FOLLOWERS; i++)
                caMan.AddCard(uc, GameManager.PLAYER);
        foreach (SkillCard skill in pMan.PlayerHero.HeroStartSkills)
            for (int i = 0; i < GameManager.PLAYER_START_SKILLS; i++)
                caMan.AddCard(skill, GameManager.PLAYER);

        SceneLoader.LoadScene(SceneLoader.Scene.CombatScene);
        GameManager.Instance.IsCombatTest = true;
    }
}
