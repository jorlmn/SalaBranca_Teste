using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

///<summary>
/// Componente anexado a elementos UI, fazendo aparecer uma tooltip ao lado do elemento
/// com um texto que explica sua função.
///</summary>
public class ButtonTooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    bool _isTooltipActive;
    [SerializeField] string _headerText;
    [SerializeField] string _contentText;
    [SerializeField] Vector3 _tooltipOffset;
    RectTransform _buttonRect = null;

    void Start()
    {
        _buttonRect = GetComponent<RectTransform>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // Ativa o painel da Tooltip com os textos deste MonoBehaviour OnPointerEnter
        _isTooltipActive = true;
        SystemManager.instance.TooltipManager.ShowTooltip(_headerText, _contentText, _buttonRect, _tooltipOffset);
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        _isTooltipActive = false;
         SystemManager.instance.TooltipManager.HideTooltip();
    }

    public void SetTooltip(string newHeader)
    {
        // Permite trocar o texto associado a esta tooltip no RunTime.
        _headerText = newHeader;
    }

    void OnDisable()
    {
        if (_isTooltipActive)
        {
            // Caso este botão seja desativado e a tooltip ainda esteja aparecendo, desativa ela
            SystemManager.instance.TooltipManager.HideTooltip();
        }
    }
}
