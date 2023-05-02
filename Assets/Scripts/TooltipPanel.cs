using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Localization.Settings;

///<summary>
/// Gerencia a ativação e o texto da Tooltip
/// Permite 2 tipos de tooltip, uma básica usada em botões E em elementos UI.
/// Neste caso, a posição da tooltip é fixa no canvas, pois os elementos UI também estarão fixos
/// O outro tipo é usado quando o Cursor fica sob os objetos 3d a serem inspecionados, DisplayedEquipment
/// Neste caso, a tooltip fica atualizando a sua posição para que ela acompanhe a posição no mundo do objeto
///</summary>
public class TooltipPanel : MonoBehaviour
{
    [Header("Tooltip Components")]
    [SerializeField] TextMeshProUGUI _tooltipHeader;
    [SerializeField] TextMeshProUGUI _tooltipContent;
    [SerializeField] RectTransform _tooltipRect;
    [SerializeField] LayoutElement _tooltipLayout;
    [SerializeField] GameObject _tooltipPanel;
    [SerializeField] CanvasGroup _tooltipCanvasGroup; 

    [Header("Fade")]
    [SerializeField] float _fadeDuration = 0.2f;
    Coroutine _canvasRoutine = null;
    Vector3 _position;
    Transform _pivot3d;
    string _placeholderForEmpty = "";


    public void ShowTooltip(string header, string content, RectTransform buttonRect, Vector3 offset)
    {
        _canvasRoutine = null;
        StopAllCoroutines();
        _tooltipPanel.SetActive(false);
        _tooltipCanvasGroup.alpha = 0;

        // Ativa tooltip para elementos UI fixos no Canvas
        StartCoroutine(ShowingUITooltip(header, content, buttonRect, offset));
    }

    public void ShowTooltip(string content, Transform position3d)
    {
        _canvasRoutine = null;
        StopAllCoroutines();
        _tooltipPanel.SetActive(false);
        _tooltipHeader.gameObject.SetActive(false);
        _tooltipCanvasGroup.alpha = 0;
        _pivot3d = position3d;


        // Tooltip para objetos 3d e com posições dinâmicas referente ao canvas
        // Assim, também inicia a CoRoutine que atualiza a posição da Tooltip
        StartCoroutine(UpdateTooltipPosition());
        StartCoroutine(ShowingInteractableTooltip(content));
    }

    public void HideTooltip()
    {
        if (_canvasRoutine != null)
        {
            StopCoroutine(_canvasRoutine);
        }

        _canvasRoutine = StartCoroutine(CanvasFade(0, false));
    }

    IEnumerator ShowingInteractableTooltip(string content)
    {
        // Delay para a tooltip começar a aparecer
        yield return new WaitForSeconds(0.4f);
        _tooltipPanel.SetActive(true);

        // Pega a versão do idioma atual referente à Key do texto do objeto inspecionado.
        var contentOp =  LocalizationSettings.StringDatabase.GetLocalizedStringAsync(content);
        while (!contentOp.IsDone)
        {
            yield return null;
        }
        _tooltipContent.text = contentOp.Result;
        _tooltipContent.gameObject.SetActive(true);

        // Faz o tamanho da Tooltip ser dinâmico referente ao tamanho do texto usado, porém com um máximo
        _tooltipLayout.enabled = Mathf.Max(_tooltipHeader.preferredWidth, _tooltipContent.preferredWidth) >= _tooltipLayout.preferredWidth;
        yield return null;

        if (_canvasRoutine != null)
        {
            StopCoroutine(_canvasRoutine);
        }

        _canvasRoutine = StartCoroutine(CanvasFade(_fadeDuration, true));
    }

    IEnumerator ShowingUITooltip(string header, string content, RectTransform buttonRect, Vector3 offset)
    {
        yield return new WaitForSeconds(0.4f);
        _tooltipPanel.SetActive(true);

        // Como alguns botões podem ter uma explicação com Título OU não,
        // faz o uso do título ser ativado ou desativado com a Key usada
        // Ou seja, se a Key passada é "-1" (escolhido arbitrariamente), 
        // o Tìtulo não é para ser usado
        if (header == "-1")
        {   
            _tooltipHeader.text = _placeholderForEmpty;
            _tooltipHeader.gameObject.SetActive(false);
        }
        else
        {
            var headerOp =  LocalizationSettings.StringDatabase.GetLocalizedStringAsync(header);
            while (!headerOp.IsDone)
            {
                yield return null;
            }
            _tooltipHeader.text = headerOp.Result;
            _tooltipHeader.gameObject.SetActive(true);
        }


        if (content == "-1")
        {   
            _tooltipContent.text = _placeholderForEmpty;
            _tooltipContent.gameObject.SetActive(false);
        }
        else
        {
            var contentOp =  LocalizationSettings.StringDatabase.GetLocalizedStringAsync(content);
            while (!contentOp.IsDone)
            {
                yield return null;
            }
            _tooltipContent.text = contentOp.Result;
            _tooltipContent.gameObject.SetActive(true);
        }

        // Faz a Tooltip aparecer ao lado do elemento UI de acordo com o offset providenciado pelo
        // ButtonTooltip, componente do próprio elemento
        Vector3 position = buttonRect.transform.position;
        if (offset != Vector3.zero)
        {
            position.x += offset.x;
            position.y += offset.y;
        }
        else
        {
            position.x += 10 + buttonRect.rect.width / 2;
            position.y += - 10;
        }

        _tooltipLayout.enabled = Mathf.Max(_tooltipHeader.preferredWidth, _tooltipContent.preferredWidth) >= _tooltipLayout.preferredWidth;
        yield return null;

        // Faz a Tooltip ficar sempre dentro das bordas do Canvas
        _tooltipRect.anchoredPosition = UIManager.ClampToCanvas(position, _tooltipRect);

        if (_canvasRoutine != null)
        {
            StopCoroutine(_canvasRoutine);
        }

        _canvasRoutine = StartCoroutine(CanvasFade(_fadeDuration, true));
    }

    IEnumerator UpdateTooltipPosition()
    {
        while (_pivot3d != null)
        {
            _position = SystemManager.instance.CamManager.CameraCache.WorldToScreenPoint(_pivot3d.position);
            _tooltipRect.anchoredPosition = UIManager.ClampToCanvas(_position, _tooltipRect);
            yield return null;
        }
    }

    IEnumerator CanvasFade(float duration, bool fadeIn)
    {
        float finalValue = fadeIn ? 1f : 0f;
        float startValue = _tooltipCanvasGroup.alpha;

        float timer = 0;

        while (timer < duration)
        {
            timer += Time.deltaTime;

            _tooltipCanvasGroup.alpha = Mathf.Lerp(startValue, finalValue, timer / duration);
            yield return null;
        }

        _tooltipCanvasGroup.alpha = finalValue;

        if (finalValue == 0)
        {
            _tooltipPanel.SetActive(false);
            _pivot3d = null;
            _canvasRoutine = null;
            StopAllCoroutines();
        }

        _canvasRoutine = null;
    }   
}
