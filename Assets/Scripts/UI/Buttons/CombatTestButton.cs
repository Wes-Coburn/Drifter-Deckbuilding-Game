using UnityEngine;

public class CombatTestButton : MonoBehaviour
{
    [SerializeField] private bool addStartUnits;
    [SerializeField][Range(0, 3)] private int reputationTier;
    [SerializeField] private PlayerHero developerTestHero;
    [SerializeField] private EnemyHero enemyTestHero;
    [Header("GAUNTLET")]
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


    private void Start()
    {
        if (Application.isPlaying) Debug.LogWarning("COMBAT TEST BUTTON ENABLED!");
        else
        {
            gameObject.SetActive(false);
            return;
        }
    }

    public void OnClick()
    {
        if (SceneLoader.SceneIsLoading) return;
        //UIManager.Instance.ShakeCamera(EZCameraShake.CameraShakePresets.Bump); // TESTING
        AnimationManager.Instance.CreateParticleSystem(gameObject, ParticleSystemHandler.ParticlesType.ButtonPress);
        SceneLoader.LoadAction += () => LoadCombatTest();
        SceneLoader.LoadScene(SceneLoader.Scene.CombatScene);
    }

    private void LoadCombatTest()
    {
        // Enemy Hero
        EnemyHero eh = ScriptableObject.CreateInstance<EnemyHero>();
        if (enemyTestHero != null) eh.LoadHero(enemyTestHero);
        else
        {
            Debug.LogError("ENEMY TEST HERO IS NULL!");
            return;
        }

        // Player Hero
        PlayerHero ph = ScriptableObject.CreateInstance<PlayerHero>();
        if (developerTestHero != null) ph.LoadHero(developerTestHero);
        else
        {
            Debug.LogError("DEVELOPER TEST HERO IS NULL!");
            return;
        }

        ManagerHandler.G_MAN.IsCombatTest = true; // TESTING
        ManagerHandler.D_MAN.EngagedHero = eh;
        ManagerHandler.P_MAN.HeroScript = ph;

        // Test Augments
        foreach (HeroAugment aug in testAugments)
            ManagerHandler.P_MAN.AddAugment(aug);

        // Test Items
        HeroItem[] items = new HeroItem[testItems.Length];
        testItems.CopyTo(items, 0);
        items.Shuffle();
        for (int i = 0; i < 5; i++) ManagerHandler.P_MAN.AddItem(items[i]);

        // Test Cards
        if (enableTestCards_1)
        {
            foreach (Card c in testCards_1)
                ManagerHandler.CA_MAN.AddCard(c, GameManager.PLAYER);
        }
        if (enableTestCards_2)
        {
            foreach (Card c in testCards_2)
                ManagerHandler.CA_MAN.AddCard(c, GameManager.PLAYER);
        }
        if (enableTestCards_3)
        {
            foreach (Card c in testCards_3)
                ManagerHandler.CA_MAN.AddCard(c, GameManager.PLAYER);
        }
        if (enableTestCards_4)
        {
            foreach (Card c in testCards_4)
                ManagerHandler.CA_MAN.AddCard(c, GameManager.PLAYER);
        }

        // Start Units
        if (addStartUnits)
        {
            foreach (UnitCard uc in ManagerHandler.CA_MAN.PlayerStartUnits)
                for (int i = 0; i < GameManager.PLAYER_START_UNITS; i++)
                    ManagerHandler.CA_MAN.AddCard(uc, GameManager.PLAYER);
        }

        // Reputation
        int reputation;
        switch (reputationTier)
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

        ManagerHandler.G_MAN.Reputation_Mages = reputation;
        ManagerHandler.G_MAN.Reputation_Mutants = reputation;
        ManagerHandler.G_MAN.Reputation_Rogues = reputation;
        ManagerHandler.G_MAN.Reputation_Techs = reputation;
        ManagerHandler.G_MAN.Reputation_Warriors = reputation;
    }
}
