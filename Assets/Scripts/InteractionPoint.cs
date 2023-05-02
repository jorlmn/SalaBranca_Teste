using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///<summary>
/// Componente que gerencia o InteractionPoint atrelado aos objetos inspecionáveis.
/// Ativa o Line Renderer e o elemento UI associados a cada ponto
///</summary>
public class InteractionPoint : MonoBehaviour
{
    [SerializeField] string _detailedHeader;
    [SerializeField] string _detailedContent;

    [SerializeField] Transform _interactionPivot;
    [SerializeField] LineRenderer _lineRenderer;

    InteractionUI _uiInteractionPoint = null;
    Vector3 _position;


    void OnEnable()
    {
        _position = SystemManager.instance.CamManager.CameraCache.WorldToScreenPoint(transform.position);

        // Pega um botão UI da pool de botões, e o coloca na posição do canvas equivalente à posição
        // no Mundo deste ponto de interação
        _uiInteractionPoint = SystemManager.instance.InteractionPool.GetInteractionUI();
        _uiInteractionPoint.Rect.anchoredPosition = UIManager.ClampToCanvas(_position, _uiInteractionPoint.Rect);
        _uiInteractionPoint.gameObject.SetActive(true);
        _uiInteractionPoint.SetInteractionTexts(_detailedHeader, _detailedContent);


        // Faz o line renderer preencher entre o _interactionPivot e este objeto
        _lineRenderer.enabled = false;
        _lineRenderer.SetPosition(0, transform.position);
        _lineRenderer.SetPosition(1, _interactionPivot.position);

        StartCoroutine(DelayToEnableLine());
    }

    IEnumerator DelayToEnableLine()
    {
        yield return new WaitForSeconds(2.1f);
        _lineRenderer.enabled = true;
    }

    public void DisablePoints()
    {
        // Desativa os botões UI e o Line Renderer
        if (_uiInteractionPoint != null)
        {
            _uiInteractionPoint.gameObject.SetActive(false);
            _uiInteractionPoint = null;
        }
        
        _lineRenderer.enabled = false;
        gameObject.SetActive(false);
    }

    void Update()
    {
        // Atualiza a posição do botão UI no canvas de acordo com a posição do mundo deste InteractionPoint
        // Feito assim para a posição do botão ficar correta mesmo quando o usuário move a câmera
        _position = SystemManager.instance.CamManager.CameraCache.WorldToScreenPoint(transform.position);
        _uiInteractionPoint.Rect.anchoredPosition = UIManager.ClampToCanvas(_position, _uiInteractionPoint.Rect);
    }
}
