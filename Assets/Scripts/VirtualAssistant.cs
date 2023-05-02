using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Localization.Settings;

///<summary>
/// Componente que gerencia as mensagens de ajuda
///</summary>
public class VirtualAssistant : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _message;
    [SerializeField] CanvasGroup _messageCanvas;
    [SerializeField] float _fadeDuration = 1f;
    [SerializeField] float _timeDisplayed = 6f;
    [SerializeField] CanvasGroup _assistantCanvas;

    string _lastKey;
    int _lastKeyType;

    [Header("String Keys for Message Types")]
    public string IntroMessage;
    public string HighlightMessage;
    public string LmbRotateMessage;
    public string CameraZoomMessage;
    public string InteractionPointsMessage;


    public void SetVirtualMessage(int type)
    {
        // Escolhe uma mensagem para cada contexto da cena
        // Caso um contexto tenha mais de uma mensagem, randomiza entre elas
        StopAllCoroutines();
        _lastKeyType = type;

        switch (type)
        {
            case 0:
                _lastKey = IntroMessage;
                break;
            case 1:
                _lastKey = Random.Range(0, 2) == 0 ? HighlightMessage : CameraZoomMessage;
                break;
            default:
                _lastKey = Random.Range(0, 2) == 0 ? LmbRotateMessage : InteractionPointsMessage;
                break;
        }

        // Faz a localização da mensagem e o fade in & out
        StartCoroutine(ChangeMessageRoutine(_lastKey));
    }

    public void ToggleMessage()
    {
        // Usado no próprio Botão do VirtualAssistant, quando o usuário clica nele
        // Troca a mensagem caso o contexto tenha mais de uma possível. Caso contrário, desativa
        // a mensagem atual

        StopAllCoroutines();

        if (_messageCanvas.gameObject.activeInHierarchy)
        {
            if (_lastKey == HighlightMessage)
            {
                SystemManager.instance.UiAudio.PlayNormal();
                _lastKey = CameraZoomMessage;
                StartCoroutine(ChangeMessageRoutine(_lastKey));
            }
            else if (_lastKey == CameraZoomMessage)
            {
                SystemManager.instance.UiAudio.PlayNormal();
                _lastKey = HighlightMessage;
                StartCoroutine(ChangeMessageRoutine(_lastKey));
            }
            else if (_lastKey == InteractionPointsMessage)
            {
                SystemManager.instance.UiAudio.PlayNormal();
                _lastKey = LmbRotateMessage;
                StartCoroutine(ChangeMessageRoutine(_lastKey));
            }
            else if (_lastKey == LmbRotateMessage)
            {
                SystemManager.instance.UiAudio.PlayNormal();
                _lastKey = InteractionPointsMessage;
                StartCoroutine(ChangeMessageRoutine(_lastKey));
            }
            else
            {
                SystemManager.instance.UiAudio.PlayBack();
                StartCoroutine(FadeCanvas(false));
            }
        }
        else
        {
            SystemManager.instance.UiAudio.PlayNormal();
            SetVirtualMessage(_lastKeyType);
        }
    }

    public void LocalizeMessage()
    {
        // Usado quando o usuário muda a localização enquanto uma mensagem está ativada
        // Localiza a mensagem novamente para o novo idioma
        if (_messageCanvas.gameObject.activeInHierarchy)
        {
            StartCoroutine(LocalizeRoutine());
        }
    }

    IEnumerator LocalizeRoutine()
    {
        var contentOp =  LocalizationSettings.StringDatabase.GetLocalizedStringAsync(_lastKey);
        while (!contentOp.IsDone)
        {
            yield return null;
        }
        _message.text = contentOp.Result;
    }


    IEnumerator ChangeMessageRoutine(string key)
    {
        if (_messageCanvas.gameObject.activeInHierarchy)
        {
            yield return StartCoroutine(FadeCanvas(false));
        }

        var contentOp =  LocalizationSettings.StringDatabase.GetLocalizedStringAsync(key);
        while (!contentOp.IsDone)
        {
            yield return null;
        }
        _message.text = contentOp.Result;

        _messageCanvas.gameObject.SetActive(true);
        yield return StartCoroutine(FadeCanvas(true));

        yield return new WaitForSecondsRealtime(_timeDisplayed);
        yield return StartCoroutine(FadeCanvas(false));
    }

    IEnumerator FadeCanvas(bool fadeIn)
    {
        float finalValue = fadeIn ? 1f : 0;
        float startValue = _messageCanvas.alpha;

        _messageCanvas.blocksRaycasts = !fadeIn;
        _messageCanvas.interactable = !fadeIn;

        float timer = 0;

        while (timer < _fadeDuration)
        {
            timer += Time.unscaledDeltaTime;

            _messageCanvas.alpha = Mathf.Lerp(startValue, finalValue, timer / _fadeDuration);
            yield return null;
        }

        _messageCanvas.blocksRaycasts = fadeIn;
        _messageCanvas.interactable = fadeIn;
        _messageCanvas.alpha = finalValue;

        if (!fadeIn)
        {
            _messageCanvas.gameObject.SetActive(false);
        }
    }

    public void TogglePanel(bool toggleOn)
    {
        _assistantCanvas.alpha = toggleOn == true ? 1f: 0f;
        _assistantCanvas.interactable = toggleOn;
        _assistantCanvas.blocksRaycasts = toggleOn;
    }
}
