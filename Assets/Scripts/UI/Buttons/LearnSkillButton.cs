using UnityEngine;
using TMPro;

public class LearnSkillButton : MonoBehaviour
{
    [SerializeField] private GameObject learnCost;
    private int LearnCost
    {
        set
        {
            learnCost.GetComponent<TextMeshProUGUI>().SetText(value.ToString());
        }
    }
    public SkillCard SkillCard { get; set; }
    private void Awake()
    {
        LearnCost = GameManager.LEARN_SKILL_COST;
    }
    public void OnClick() => 
        FindObjectOfType<CardPageDisplay>().LearnSkillButton_OnClick(SkillCard);
}
