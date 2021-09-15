using UnityEngine;
using TMPro;

public class ChangeLayer : MonoBehaviour // UNUSED CLASS
{
    /* CHANGE_LAYER_DATA */
    private const string CARDS_LAYER = "Cards";
    private const string ACTIONS_LAYER = "Actions";
    private const string ZOOM_LAYER = "Zoom";
    private const string HAND_LAYER = "Hand";
    private const string UI_LAYER = "UI";

    private string renderLayer;
    private string RenderLayer
    {
        set
        {
            renderLayer = value;
            UpdateRenderLayer(transform);
        }
    }
    
    public void CardsLayer()
    {
        if (GetComponent<CardDisplay>() is UnitCardDisplay) 
            RenderLayer = CARDS_LAYER;
        else RenderLayer = ACTIONS_LAYER;
    }
    public void ZoomLayer() => RenderLayer = ZOOM_LAYER;
    public void UILayer() => RenderLayer = UI_LAYER;

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
            render.sortingLayerName = renderLayer;
        else if (tran.TryGetComponent(out SpriteMask sMask))
        {
            sMask.frontSortingLayerID = SortingLayer.NameToID(renderLayer);
            sMask.backSortingLayerID = SortingLayer.NameToID(renderLayer);
        }
        else if (tran.TryGetComponent(out TextMeshPro txtPro)) 
            txtPro.sortingLayerID = SortingLayer.NameToID(renderLayer);
        else if (tran.TryGetComponent(out MeshRenderer mesh)) 
            mesh.sortingLayerName = renderLayer;
    }
}
