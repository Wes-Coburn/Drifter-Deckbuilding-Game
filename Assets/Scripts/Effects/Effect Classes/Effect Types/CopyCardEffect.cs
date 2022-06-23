using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Copy Card Effect", menuName = "Effects/Effect/CopyCard")]
public class CopyCardEffect : Effect
{
    [Header("COPY CARD EFFECT")]

    [SerializeField] private bool playCopy;
    [SerializeField] private bool isExactCopy;
    [SerializeField] private bool copyAction;
    [SerializeField] private bool copyUnit;
    [SerializeField] private List<Effect> additionalEffects;

    public bool PlayCopy { get => playCopy; }
    public bool IsExactCopy { get => isExactCopy; }
    public bool CopyAction { get => copyAction; }
    public bool CopyUnit { get => copyUnit; }
    public List<Effect> AdditionalEffects { get => additionalEffects; }

    public override void LoadEffect(Effect effect)
    {
        base.LoadEffect(effect);
        CopyCardEffect copyEffect = effect as CopyCardEffect;
        playCopy = copyEffect.PlayCopy;
        isExactCopy = copyEffect.IsExactCopy;
        copyAction = copyEffect.CopyAction;
        copyUnit = copyEffect.CopyUnit;
        additionalEffects = copyEffect.AdditionalEffects;
    }
}
