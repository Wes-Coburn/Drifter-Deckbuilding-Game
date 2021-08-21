using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    /* SINGELTON_PATTERN */
    public static UIManager Instance { get; private set; }
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }

    [SerializeField] private GameObject screenDimmerPrefab;
    [SerializeField] private GameObject infoPopupPrefab;
    [SerializeField] private GameObject combatEndPopupPrefab;
    [SerializeField] private GameObject turnPopupPrefab;
    [SerializeField] private GameObject sceneFader;
    [SerializeField] private GameObject menuPopupPrefab;

    private GameObject screenDimmer;
    private GameObject infoPopup;
    private GameObject combatEndPopup;
    private GameObject turnPopup;

    private GameObject menuPopup;
    private GameObject selectedEnemy;
    private GameObject endTurnButton;

    private Coroutine sceneFadeRoutine;
    
    public bool PlayerIsTargetting { get; set; }
    public GameObject CurrentWorldSpace { get; set; }
    public GameObject CurrentCanvas { get; set; }

    public void Start()
    {
        CurrentWorldSpace = GameObject.Find("WorldSpace");
        CurrentCanvas = GameObject.Find("Canvas");
        PlayerIsTargetting = false;
    }

    /******
     * *****
     * ****** SET_SCENE_FADER
     * *****
     *****/
    public void SetSceneFader(bool fadeOut)
    {
        if (sceneFadeRoutine != null)
        {
            StopCoroutine(sceneFadeRoutine);
            sceneFadeRoutine = null;
        }
        sceneFadeRoutine = StartCoroutine(FadeSceneNumerator(fadeOut));
    }
    private IEnumerator FadeSceneNumerator(bool fadeOut)
    {
        Image img = sceneFader.GetComponent<Image>();
        float alphaChange = 0.01f;
        float changeDelay = 0.01f;

        if (fadeOut)
        {
            while (img.color.a < 1)
            {
                var tempColor = img.color;
                tempColor.a = img.color.a + alphaChange;
                if (tempColor.a > 1) tempColor.a = 1;
                img.color = tempColor;
                yield return new WaitForSeconds(changeDelay);
            }
        }
        else
        {
            while (img.color.a > 0)
            {
                var tempColor = img.color;
                tempColor.a = img.color.a - alphaChange;
                if (tempColor.a < 0) tempColor.a = 0;
                img.color = tempColor;
                yield return new WaitForSeconds(changeDelay);
            }
        }
        sceneFadeRoutine = null;
    }

    /******
     * *****
     * ****** CREATE_DESTROY_MENU_POPUP
     * *****
     *****/
    public void CreateMenuPopup()
    {
        if (menuPopup != null) return;
        GameObject uiCanvas = GameObject.Find("UI_Canvas");
        menuPopup = Instantiate(menuPopupPrefab, uiCanvas.transform);
    }
    public void DestroyMenuPopup()
    {
        if (menuPopup != null)
        {
            Destroy(menuPopup);
            menuPopup = null;
        }
    }

    /******
     * *****
     * ****** START_COMBAT_SCENE
     * *****
     *****/
    public void StartCombatScene()
    {
        endTurnButton = GameObject.Find("EndTurnButton");
    }

    /******
     * *****
     * ****** SELECT_ENEMY
     * *****
     *****/
    public void SelectEnemy(GameObject enemy, bool enabled, bool isSelected = false)
    {
        if (enabled) selectedEnemy = enemy;
        if (enemy.TryGetComponent(out CardSelect cs))
        {
            cs.CardOutline.SetActive(enabled);
            SpriteRenderer sr = cs.CardOutline.GetComponent<SpriteRenderer>();
            if (enabled)
            {
                if (isSelected) sr.color = cs.SelectedColor;
                else sr.color = cs.HighlightedColor;
            }
        }
        else if (enemy.TryGetComponent(out HeroSelect hs))
            hs.TargetIcon.SetActive(enabled);
        else
        {
            Debug.LogError("TARGET SELECT SCRIPT NOT FOUND!");
            return;
        }
    }

    /******
     * *****
     * ****** SETTERS
     * *****
     *****/
    public void UpdateEndTurnButton(bool isMyTurn)
    {
        BoxCollider2D bc = endTurnButton.GetComponent<BoxCollider2D>();
        bc.enabled = isMyTurn;
        EndTurnButtonDisplay etbd = endTurnButton.GetComponent<EndTurnButtonDisplay>();
        etbd.EndTurnSide.SetActive(isMyTurn);
        etbd.OpponentTurnSide.SetActive(!isMyTurn);
    }

    /******
     * *****
     * ****** DESTROY_ZOOM_OBJECT(S)
     * *****
     *****/
    public void DestroyZoomObjects()
    {
        static void DestroyObject(GameObject go)
        {
            Destroy(go);
            go = null;
        }

        SetScreenDimmer(false);
        CardZoom.ZoomCardIsCentered = false;
        List<GameObject> objectsToDestroy = new List<GameObject>
        {
            CardZoom.CurrentZoomCard,
            CardZoom.DescriptionPopup,
            CardZoom.AbilityPopupBox,
            AbilityZoom.AbilityPopup
        };
        foreach (GameObject go in objectsToDestroy)
            if (go != null) DestroyObject(go);
    }

    /******
     * *****
     * ****** SET_SCREEN_DIMMER
     * *****
     *****/
    public void SetScreenDimmer(bool screenIsDimmed)
    {
        if (screenDimmer != null)
        {
            Destroy(screenDimmer);
            screenDimmer = null;
        }
        if (screenIsDimmed)
        {
            GameObject prefab = screenDimmerPrefab;
            GameObject parent = CurrentWorldSpace;
            screenDimmer = Instantiate(prefab, new Vector3(0, 0, -3), Quaternion.identity, parent.transform);
        }
    }

    /******
     * *****
     * ****** INFO_POPUP
     * *****
     *****/
    public void CreateInfoPopup(string message)
    {
        DestroyInfoPopup();
        infoPopup = Instantiate(infoPopupPrefab, new Vector2(680, 0), Quaternion.identity, CurrentWorldSpace.transform);
        infoPopup.GetComponent<InfoPopupDisplay>().DisplayInfoPopup(message);
    }
    public void CreateFleetinInfoPopup(string message)
    {
        CreateInfoPopup(message);
        AnimationManager.Instance.ChangeAnimationState(infoPopup, "Enter_Exit");
    }
    public void DismissInfoPopup()
    {
        if (infoPopup != null)
            AnimationManager.Instance.ChangeAnimationState(infoPopup, "Exit");
    }
    public void DestroyInfoPopup()
    {
        if (infoPopup != null)
        {
            Destroy(infoPopup);
            infoPopup = null;
        }
    }

    /******
     * *****
     * ****** TURN_POPUP
     * *****
     *****/
    public void CreateTurnPopup(bool isPlayerTurn)
    {
        DestroyTurnPopup();
        turnPopup = Instantiate(turnPopupPrefab, CurrentCanvas.transform);
        turnPopup.GetComponent<TurnPopupDisplay>().DisplayTurnPopup(isPlayerTurn);
    }
    public void DestroyTurnPopup()
    {
        if (turnPopup != null)
        {
            Destroy(turnPopup);
            turnPopup = null;
        }
    }

    /******
     * *****
     * ****** CREATE_COMBAT_END_POPUP
     * *****
     *****/
    public void CreateCombatEndPopup(bool playerWins)
    {
        combatEndPopup = Instantiate(combatEndPopupPrefab, CurrentCanvas.transform);
        CombatEndPopupDisplay cepd = combatEndPopup.GetComponent<CombatEndPopupDisplay>();
        cepd.VictoryText.SetActive(playerWins);
        cepd.DefeatText.SetActive(!playerWins);
        cepd.GetComponent<SoundPlayer>().PlaySound(0);
    }

    public Vector3 GetPortraitPosition(string heroName, SceneLoader.Scene scene)
    {
        Vector3 vec3 = new Vector3(0, 0, 0);
        switch (heroName)
        {
            // KILI
            case "Kili, Neon Rider":
                switch(scene)
                {
                    case SceneLoader.Scene.DialogueScene:
                        vec3.Set(-90, -325, 0);
                        break;
                    case SceneLoader.Scene.CombatScene:
                        vec3.Set(-25, -75, 0);
                        break;
                }
                break;
            // YERGOV
            case "Yergov, Biochemist":
                switch (scene)
                {
                    case SceneLoader.Scene.DialogueScene:
                        vec3.Set(-290, -290, 0);
                        break;
                    case SceneLoader.Scene.CombatScene:
                        vec3.Set(-73, -70, 0);
                        break;
                }
                break;
            // FENTIS
            case "Fentis, Underworld Agent":
                switch (scene)
                {
                    case SceneLoader.Scene.DialogueScene:
                        break;
                    case SceneLoader.Scene.CombatScene:
                        break;
                }
                break;
        }
        return vec3;
    }
}
