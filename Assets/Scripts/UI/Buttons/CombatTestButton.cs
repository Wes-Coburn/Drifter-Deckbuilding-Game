using UnityEngine;

public class CombatTestButton : MonoBehaviour
{
    [SerializeField] private bool addStartUnits;
    [SerializeField] [Range(0, 3)] private int reputationTier;
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

    private GameManager gMan;
    private PlayerManager pMan;

    private void Start()
    {
        if (Application.isPlaying) Debug.LogWarning("COMBAT TEST BUTTON ENABLED!");
        else
        {
            gameObject.SetActive(false);
            return;
        }

        gMan = GameManager.Instance;
        pMan = PlayerManager.Instance;
    }

    public void OnClick()
    {
        if (SceneLoader.SceneIsLoading) return;
        //UIManager.Instance.ShakeCamera(EZCameraShake.CameraShakePresets.Bump); // TESTING
        AnimationManager.Instance.CreateParticleSystem(gameObject, ParticleSystemHandler.ParticlesType.ButtonPress); // TESTING
        gMan.IsCombatTest = true;
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

        DialogueManager.Instance.EngagedHero = eh;
        pMan.PlayerHero = ph;

        // Test Augments
        foreach (HeroAugment aug in testAugments)
            pMan.AddAugment(aug);

        // Test Items
        HeroItem[] items = new HeroItem[testItems.Length];
        testItems.CopyTo(items, 0);
        items.Shuffle();
        for (int i = 0; i < 5; i++) pMan.AddItem(items[i]);

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

        // Start Units
        CardManager caMan = CardManager.Instance;
        if (addStartUnits)
        {
            foreach (UnitCard uc in caMan.PlayerStartUnits)
                for (int i = 0; i < GameManager.PLAYER_START_UNITS; i++)
                    caMan.AddCard(uc, GameManager.PLAYER);
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

        gMan.Reputation_Mages = reputation;
        gMan.Reputation_Mutants = reputation;
        gMan.Reputation_Rogues = reputation;
        gMan.Reputation_Techs = reputation;
        gMan.Reputation_Warriors = reputation;
    }
}
