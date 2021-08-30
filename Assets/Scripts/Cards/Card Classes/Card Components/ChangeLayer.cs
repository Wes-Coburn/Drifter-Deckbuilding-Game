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
        foreach (Transform tranChild in tran)
        {
            SyncLayer(tranChild);
            UpdateRenderLayer(tranChild);
        }
    }
    private void SyncLayer(Transform tran)
    {   
        if (tran.TryGetComponent(out SpriteRenderer render))
        {
            if (!render.sortingLayerName.Equals(RenderLayer)) 
                render.sortingLayerName = RenderLayer;
        }
        else if (tran.TryGetComponent(out TextMeshPro txtPro))
        {
            if (!SortingLayer.IDToName(txtPro.sortingLayerID).Equals(RenderLayer)) 
                txtPro.sortingLayerID = SortingLayer.NameToID(RenderLayer);
        }
        else if (tran.TryGetComponent(out MeshRenderer mesh))
        {
            if (!mesh.sortingLayerName.Equals(RenderLayer)) 
                mesh.sortingLayerName = RenderLayer;
        }
    }
}
