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
        Debug.LogWarning("COMBAT TEST BUTTON ENABLED!");
        if (!Application.isEditor) gameObject.SetActive(false);
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

        Managers.G_MAN.IsCombatTest = true;
        Managers.D_MAN.EngagedHero = eh;
        Managers.P_MAN.HeroScript = ph;

        // Test Augments
        foreach (HeroAugment aug in testAugments)
            Managers.P_MAN.AddAugment(aug);

        // Test Items
        HeroItem[] items = new HeroItem[testItems.Length];
        testItems.CopyTo(items, 0);
        items.Shuffle();
        for (int i = 0; i < 5; i++) Managers.P_MAN.AddItem(items[i]);

        // Test Cards
        if (enableTestCards_1)
        {
            foreach (Card c in testCards_1)
                Managers.CA_MAN.AddCard(c, Managers.P_MAN);
        }
        if (enableTestCards_2)
        {
            foreach (Card c in testCards_2)
                Managers.CA_MAN.AddCard(c, Managers.P_MAN);
        }
        if (enableTestCards_3)
        {
            foreach (Card c in testCards_3)
                Managers.CA_MAN.AddCard(c, Managers.P_MAN);
        }
        if (enableTestCards_4)
        {
            foreach (Card c in testCards_4)
                Managers.CA_MAN.AddCard(c, Managers.P_MAN);
        }

        // Start Units
        if (addStartUnits)
        {
            foreach (UnitCard uc in Managers.CA_MAN.PlayerStartUnits)
                for (int i = 0; i < GameManager.PLAYER_START_UNITS; i++)
                    Managers.CA_MAN.AddCard(uc, Managers.P_MAN);
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

        Managers.G_MAN.Reputation_Mages = reputation;
        Managers.G_MAN.Reputation_Mutants = reputation;
        Managers.G_MAN.Reputation_Rogues = reputation;
        Managers.G_MAN.Reputation_Techs = reputation;
        Managers.G_MAN.Reputation_Warriors = reputation;
    }
}
