using UnityEngine;

public class CombatTestButton : MonoBehaviour
{
    [Header("DEVELOPER MODE")]
    [SerializeField] private bool developerMode;
    [SerializeField] private bool setEnemyHealth1;
    [Space]
    [SerializeField] private bool addStartUnits;
    [SerializeField] private bool addStartSkills;
    [SerializeField] private bool addMoreSkills;

    [SerializeField] private PlayerHero[] playerTestHeroes;
    [SerializeField] private PlayerHero developerTestHero;
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

    private void Start()
    {
        if (developerMode)
        {
            Debug.LogWarning("DEVELOPER MODE!");
            gameObject.SetActive(true);
            return;
        }
        GameManager gMan = GameManager.Instance;
        gMan.CheckSave(); // TESTING
        gameObject.SetActive(gMan.Achievement_BETA_Finish);
    }

    public void OnClick()
    {
        if (SceneLoader.SceneIsLoading) return;
        EnemyHero eh = ScriptableObject.CreateInstance<EnemyHero>();
        eh.LoadHero(enemyTestHero);
        PlayerHero ph = ScriptableObject.CreateInstance<PlayerHero>();
        if (!developerMode || developerTestHero == null)
        {
            int randomHero = Random.Range(0, playerTestHeroes.Length - 1);
            ph.LoadHero(playerTestHeroes[randomHero]);
        }
        else ph.LoadHero(developerTestHero);
        DialogueManager.Instance.EngagedHero = eh;
        PlayerManager pMan = PlayerManager.Instance;
        pMan.PlayerHero = ph;
        
        // Test Augments
        foreach (HeroAugment aug in testAugments)
            pMan.AddAugment(aug);
        
        // Test Items
        HeroItem[] items = new HeroItem[testItems.Length];
        testItems.CopyTo(items, 0);
        items.Shuffle();
        for (int i = 0; i < 5; i++)
            pMan.AddItem(items[i]);

        // Test Cards
        if (developerMode)
        {
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
        }
        
        // Start Units
        CardManager caMan = CardManager.Instance;
        if (!developerMode || addStartUnits)
        {
            foreach (UnitCard uc in caMan.PlayerStartUnits)
                for (int i = 0; i < GameManager.PLAYER_START_UNITS; i++)
                    caMan.AddCard(uc, GameManager.PLAYER);
        }
        // Start Skills
        if (!developerMode || addStartSkills)
        {
            foreach (SkillCard skill in pMan.PlayerHero.HeroStartSkills)
                for (int i = 0; i < GameManager.PLAYER_START_SKILLS; i++)
                    caMan.AddCard(skill, GameManager.PLAYER);
        }
        // More Skills
        if (developerMode && addMoreSkills)
        {
            foreach (SkillCard skill in pMan.PlayerHero.HeroMoreSkills)
                for (int i = 0; i < GameManager.PLAYER_START_SKILLS; i++)
                    caMan.AddCard(skill, GameManager.PLAYER);
        }
        SceneLoader.LoadScene(SceneLoader.Scene.CombatScene);
        GameManager.Instance.IsCombatTest = true;
    }
}
