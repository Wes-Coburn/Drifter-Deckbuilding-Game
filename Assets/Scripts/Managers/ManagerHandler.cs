using UnityEngine;

public static class ManagerHandler
{
    public static AnimationManager AN_MAN => GetManager(AnimationManager.Instance) as AnimationManager;
    public static AudioManager AU_MAN => GetManager(AudioManager.Instance) as AudioManager;
    public static CardManager CA_MAN => GetManager(CardManager.Instance) as CardManager;
    public static CombatManager CO_MAN => GetManager(CombatManager.Instance) as CombatManager;
    public static DialogueManager D_MAN => GetManager(DialogueManager.Instance) as DialogueManager;
    public static EffectManager EF_MAN => GetManager(EffectManager.Instance) as EffectManager;
    public static EnemyManager EN_MAN => GetManager(EnemyManager.Instance) as EnemyManager;
    public static EventManager EV_MAN => GetManager(EventManager.Instance) as EventManager;
    public static GameManager G_MAN => GetManager(GameManager.Instance) as GameManager;
    public static PlayerManager P_MAN => GetManager(PlayerManager.Instance) as PlayerManager;
    public static UIManager U_MAN => GetManager(UIManager.Instance) as UIManager;

    private static MonoBehaviour GetManager(MonoBehaviour manager)
    {
        if (manager == null) Debug.LogError("MANAGER IS NULL!");
        return manager;
    }
}
