using UnityEngine;

public class CombatTestButton : MonoBehaviour
{
    [SerializeField] private PlayerHero playerTestHero;
    [SerializeField] private EnemyHero enemyTestHero;

    public void OnClick()
    {
        EnemyHero eh = ScriptableObject.CreateInstance<EnemyHero>();
        eh.LoadHero(enemyTestHero);
        PlayerHero ph = ScriptableObject.CreateInstance<PlayerHero>();
        ph.LoadHero(playerTestHero);
        DialogueManager.Instance.EngagedHero = eh;
        PlayerManager.Instance.PlayerHero = ph;
        SceneLoader.LoadScene(SceneLoader.Scene.CombatScene);

        FunctionTimer.Create(() => ClearValues(), SceneLoader.SCENE_LOADER_DELAY + 2);
        void ClearValues()
        {
            GameManager.ActiveNPCHeroes.Clear();
            Destroy(ph);
            Destroy(eh);
            DialogueManager.Instance.EngagedHero = null;
            PlayerManager.Instance.PlayerHero = null;
            EnemyManager.Instance.EnemyHero = null;
        }
    }
}
