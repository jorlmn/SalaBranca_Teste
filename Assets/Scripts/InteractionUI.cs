using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///<summary>
/// Componente do botão UI pego e usado por cada InteractionPoint e pooled
/// no início da cena.
///</summary>
public class InteractionUI : MonoBehaviour
{
    [SerializeField] ButtonTooltip _tooltip;
    [SerializeField] CanvasGroup _buttonCanvas;
    public RectTransform Rect;

    string _detailedHeader;
    string _detailedContent;

    void OnEnable()
    {
        // Faz um Fade In do botão, ao contrário de aparecer instantaneamente
        StartCoroutine(FadeIn(1));
    }

    void OnDisable()
    {
        _buttonCanvas.alpha = 0;
    }

    public void ButtonPress()
    {
        // Altera o texo do Painel Principal para ter o conteúdo referente a este ponto
        SystemManager.instance.UiManager.ChangeMainTextPanel(_detailedContent, _detailedHeader);
        SystemManager.instance.UiAudio.PlayLight();
    }

    public void SetInteractionTexts(string detailedHeader, string detailedContent)
    {
        // Ao ser pego por um InteractionPoint, ele estabelece os textos
        // a serem usados por este botão UI de interação
        _tooltip.SetTooltip(detailedHeader);

        _detailedHeader = detailedHeader;
        _detailedContent = detailedContent;
    }

    IEnumerator FadeIn(float duration)
    {
        float finalValue = 1;
        float startValue = 0f;

        float timer = 0;

        yield return new WaitForSeconds(2);
        while (timer < duration)
        {
            timer += Time.deltaTime;

            _buttonCanvas.alpha = Mathf.Lerp(startValue, finalValue, timer / duration);
            yield return null;
        }

        _buttonCanvas.alpha = finalValue;
    } 
}
