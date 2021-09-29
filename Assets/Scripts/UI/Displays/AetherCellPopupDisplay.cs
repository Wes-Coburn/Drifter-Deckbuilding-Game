using UnityEngine;
using TMPro;

public class AetherCellPopupDisplay : MonoBehaviour
{
    private DialogueManager dMan;
    private CombatManager coMan;

    [SerializeField] private GameObject aetherQuantity;
    [SerializeField] private GameObject totalAether;

    public int AetherQuantity
    {
        set
        {
            aetherQuantity.GetComponent<TextMeshProUGUI>().SetText(value + "X");
            PlayerManager.Instance.AetherCells += value;
        }
    }
    public int TotalAether
    {
        set
        {
            totalAether.GetComponent<TextMeshProUGUI>().SetText("Total Aether: " + value);
        }
    }

    private void Start()
    {
        dMan = DialogueManager.Instance;
        coMan = CombatManager.Instance;
    }

    public void OnClick()
    {
        // play sound
        UIManager.Instance.DestroyAetherCellPopup();
        if (!coMan.IsInCombat) dMan.DisplayDialoguePopup();
        else if (dMan.EngagedHero.NextDialogueClip is CombatRewardClip crc)
        {
            dMan.EngagedHero.NextDialogueClip = crc.NextDialogueClip;
            SceneLoader.LoadScene(SceneLoader.Scene.DialogueScene);
            coMan.IsInCombat = false;
        }
        else Debug.LogError("NEXT CLIP IS NOT COMBAT_REWARD_CLIP!");
    }
}
