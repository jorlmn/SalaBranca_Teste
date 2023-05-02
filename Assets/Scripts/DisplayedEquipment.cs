using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

///<summary>
/// Componente que faz toda a lógica dos itens a serem inspecionados
/// pelo usuário. Gerencia também os pontos de interação em volta do objeto
///</summary>
public class DisplayedEquipment : MonoBehaviour
{
    public int CameraIndex;

    public bool IsRotatable = false;

    public Transform RotationPivot;

    [SerializeField] Outline _outline;
    [SerializeField] Transform _tooltipPosition;
    [SerializeField] string equipmentName;
    [SerializeField] string _descriptionContent;
    [SerializeField] List<InteractionPoint> interactionPoints = new ();

    bool _mouseEntered = false;
    bool _isSelected = false;

    void OnMouseOver()
    {
        if (!_mouseEntered)
        {
            // Ao deixar o Cursor em cima deste objeto, faz ele o objeto inspecionado
            // que ao pressionar botão esquerdo do mouse, o seleciona
            _mouseEntered = true;
            SystemManager.instance.InspectedObject = this;

            if (!_isSelected)
            {
                // Muda a cor da Outline e faz aparecer a tooltip com o nome deste equipamento
                _outline.OutlineColor = Color.black;
                SystemManager.instance.TooltipManager.ShowTooltip(equipmentName, _tooltipPosition);
            }
        }
    }

    void OnMouseExit()
    {
        if (!_isSelected)
        {
            // Muda a cor da Outline e esconde a tooltip
            _outline.OutlineColor = Color.white;
            SystemManager.instance.TooltipManager.HideTooltip();
        }

        _mouseEntered = false;
        SystemManager.instance.InspectedObject = null;
    }


    public void SelectObject()
    {
        // Método invocado pelo InputManager ao usuário apertar botão esquerdo do Mouse
        // enquanto este objeto estava sob o Cursor
        // Seleciona o objeto, e ativa os InteractionPoints e o painel dos textos
        _isSelected = true;
        _outline.OutlineColor = Color.white;
        _outline.enabled = false;
        SystemManager.instance.TooltipManager.HideTooltip();

        SystemManager.instance.UiManager.ShowHideMainTextPanel(true, _descriptionContent, equipmentName);

        ToggleInteractionPoints(true);
    }

    public void ToggleInteractionPoints(bool toggle)
    {
        // Ativa e desativa os InteractionPoints ao redor do objeto
        // Por exemplo quando este objeto é selecionado/desselecionado
        // ou quando o usuário está girando o objeto
        if (toggle)
        {
            foreach(InteractionPoint point in interactionPoints)
            {
                point.gameObject.SetActive(true);
            }
        }
        else
        {
            foreach(InteractionPoint point in interactionPoints)
            {
                point.DisablePoints();
            }
        }
    }

    public void UnSelect()
    {
        // Retorna o objeto ao seu estado padrão ao ser desselecionado

        _isSelected = false;
        _outline.enabled = true;

        if (RotationPivot != null)
        {
            RotationPivot.rotation = Quaternion.Euler(Vector3.zero);
        }

        ToggleInteractionPoints(false);
    }
}
