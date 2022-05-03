using UnityEngine;

public class CombatTestButton : MonoBehaviour
{
    [Header("DEVELOPER MODE")]
    [SerializeField] private bool developerMode;
    [SerializeField] private bool addStartUnits;
    [SerializeField] private bool addStartSkills;
    [SerializeField] [Range(0, 3)] private int reputationTier;
    [SerializeField] private PlayerHero developerTestHero;
    [SerializeField] private EnemyHero enemyTestHero;
    [Header("GAUNTLET")]
    [SerializeField] private EnemyHero gauntletEnemy;
    [SerializeField] private HeroAugment[] testAugments;
    [SerializeField] private HeroItem[] testItems;
    [Header("TEST CARDS")]
    [SerializeField] private bool enableTestCards_1;
    [SerializeField] private Card[] testCards_1;
    [SerializeField] private bool enableTestCards_2;
    [SerializeField] private Card[] testCards_2;
    [SerializeField] private bool enableTestCards_3;
    [SerializeField] private Card[] testCards_3;
    [SerializeField] private bool enableTestCards_4;
    [SerializeField] private Card[] testCards_4;

    private GameManager gMan;
    private PlayerManager pMan;

    private void Start()
    {
        gMan = GameManager.Instance;
        pMan = PlayerManager.Instance;

        if (developerMode)
        {
            Debug.LogWarning("DEVELOPER MODE!");
            gameObject.SetActive(true);
            return;
        }

        gMan.CheckSave();
        gameObject.SetActive(gMan.Achievement_BETA_Finish);
    }

    public void OnClick()
    {
        if (SceneLoader.SceneIsLoading) return;
        //UIManager.Instance.ShakeCamera(EZCameraShake.CameraShakePresets.Bump); // TESTING
        gMan.IsCombatTest = true;
        SceneLoader.LoadAction += () => LoadCombatTest();
        SceneLoader.LoadScene(SceneLoader.Scene.CombatScene);
    }

    private void LoadCombatTest()
    {
        // Enemy Hero
        EnemyHero eh = ScriptableObject.CreateInstance<EnemyHero>();
        EnemyHero loadEnemy;
        if (developerMode && enemyTestHero != null) loadEnemy = enemyTestHero;
        else loadEnemy = gauntletEnemy;
        eh.LoadHero(loadEnemy);
        // Player Hero
        PlayerHero ph = ScriptableObject.CreateInstance<PlayerHero>();
        if (developerMode && developerTestHero != null) ph.LoadHero(developerTestHero);
        else
        {
            PlayerHero[] playerHeroes = Resources.LoadAll<PlayerHero>("Heroes");
            int randomHero = Random.Range(0, playerHeroes.Length - 1);
            ph.LoadHero(playerHeroes[randomHero]);
        }

        DialogueManager.Instance.EngagedHero = eh;
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
            foreach (SkillCard skill in pMan.PlayerHero.HeroSkills)
                for (int i = 0; i < GameManager.PLAYER_START_SKILLS; i++)
                    caMan.AddCard(skill, GameManager.PLAYER);
        }
        // Reputation
        int reputation;
        int tier;
        if (developerMode) tier = reputationTier;
        else tier = 0;
        switch (tier)
        {
            case 0:
                reputation = 0;
                break;
            case 1:
                reputation = GameManager.REPUTATION_TIER_1;
                break;
            case 2:
                reputation = GameManager.REPUTATION_TIER_2;
                break;
            case 3:
                reputation = GameManager.REPUTATION_TIER_3;
                break;
            default:
                Debug.LogError("INVALID TIER!");
                return;
        }

        gMan.Reputation_Mages = reputation;
        gMan.Reputation_Mutants = reputation;
        gMan.Reputation_Rogues = reputation;
        gMan.Reputation_Techs = reputation;
        gMan.Reputation_Warriors = reputation;
    }
}
