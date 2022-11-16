using UnityEngine;

[CreateAssetMenu(fileName = "New Save Value Effect", menuName = "Effects/Effect/SaveValue")]
public class SaveValueEffect : Effect
{
    public SavedValueType ValueType;
    public enum SavedValueType
    {
        Target_Power,
        Target_Health
    }

    public override void LoadEffect(Effect effect)
    {
        base.LoadEffect(effect);
        SaveValueEffect sve = effect as SaveValueEffect;
        ValueType = sve.ValueType;
    }
}
