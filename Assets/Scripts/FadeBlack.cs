using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///<summary>
/// Componente anexado ao elemento UI FadeBlack (apenas uma imagem Preta com canvas group).
/// Faz um Fade In da cena ao ser iniciada
///</summary>
public class FadeBlack : MonoBehaviour
{
    [SerializeField] CanvasGroup _canvas;
    [SerializeField] float _fadeDuration = 1f;
    [SerializeField] float _startDelay = 1f;
    
    IEnumerator Start()
    {
        float finalValue = 0f;
        float startValue = 1;

        _canvas.blocksRaycasts = true;
        _canvas.interactable = true;

        float timer = 0;

        yield return new WaitForSeconds(_startDelay);
        while (timer < _fadeDuration)
        {
            timer += Time.unscaledDeltaTime;

            _canvas.alpha = Mathf.Lerp(startValue, finalValue, timer / _fadeDuration);
            yield return null;
        }

        _canvas.blocksRaycasts = false;
        _canvas.interactable = false;
        _canvas.alpha = finalValue;
    } 
}
