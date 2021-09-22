using UnityEngine;

public class CardCostButton : MonoBehaviour
{
    public SkillCard SkillCard { get; set; }
    public void OnClick() => 
        FindObjectOfType<CardPageDisplay>().LearnSkill(SkillCard);
}
