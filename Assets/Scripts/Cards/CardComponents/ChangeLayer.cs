using UnityEngine;
using TMPro;

public class ChangeLayer : MonoBehaviour
{
    /* CHANGE_LAYER_DATA */
    private const string CARDS_LAYER = "Cards";
    private const string ACTIONS_LAYER = "Actions";
    private const string ZOOM_LAYER = "Zoom";

    private string renderLayer;
    public string RenderLayer
    {
        get => renderLayer;
        set
        {
            renderLayer = value;
            UpdateRenderLayer(gameObject.transform);
        }
    }

    public void CardsLayer() => RenderLayer = CARDS_LAYER;
    public void ActionsLayer() => RenderLayer = ACTIONS_LAYER;
    public void ZoomLayer() => RenderLayer = ZOOM_LAYER;
    private void UpdateRenderLayer(Transform tran)
    {
        //SyncLayer(tran);
        foreach (Transform tranChild in tran)
        {
            SyncLayer(tranChild);
            UpdateRenderLayer(tranChild);
        }
    }
    private void SyncLayer(Transform tran)
    {   
        if (tran.TryGetComponent<SpriteRenderer>(out SpriteRenderer spriteRend))
        {
            if (!spriteRend.sortingLayerName.Equals(RenderLayer)) spriteRend.sortingLayerName = RenderLayer;
        }
        else if (tran.TryGetComponent<TextMeshPro>(out TextMeshPro txtPro))
        {
            if (!SortingLayer.IDToName(txtPro.sortingLayerID).Equals(RenderLayer)) txtPro.sortingLayerID = SortingLayer.NameToID(RenderLayer);
        }
        else if (tran.TryGetComponent<MeshRenderer>(out MeshRenderer meshRend))
        {
            if (!meshRend.sortingLayerName.Equals(RenderLayer)) meshRend.sortingLayerName = RenderLayer;
        }
        //Debug.Log("[" + tran.gameObject.name + "] PREVIOUS LAYER WAS/NEXT LAYER IS [" + meshRend.sortingLayerName + "]");
    }
}
