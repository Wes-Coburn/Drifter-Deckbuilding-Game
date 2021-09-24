using UnityEngine;

public class LearnSkillButton : MonoBehaviour
{
    public SkillCard SkillCard { get; set; }
    public void OnClick() => 
        FindObjectOfType<CardPageDisplay>().LearnSkillButton_OnClick(SkillCard);
}
