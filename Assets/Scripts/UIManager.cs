using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Localization.Settings;


///<summary>
/// Sistema que gerencia os botões da UI
/// e o painel de texto
///</summary>
public class UIManager : MonoBehaviour
{
    [SerializeField] float _fadeDuration = 0.4f;

    [Header("Back Button")]
    [SerializeField] CanvasGroup _backButtonCanvas;
    Coroutine _backButtonRoutine = null;


    [Header("Settings Button")]
    [SerializeField] CanvasGroup _settingsCanvas;
    Coroutine _settingsPanelRoutine = null;


    [Header("Main Text Panel")]
    [SerializeField] CanvasGroup _mainTextCanvas;
    [SerializeField] TextMeshProUGUI _headerText;
    [SerializeField] TextMeshProUGUI _contentText;
    Coroutine _mainPanelFadeRoutine = null;

    string _lastHeaderKey;
    string _lastContentKey;

    [Header("Virtual Assistant")]
    public VirtualAssistant VirtualAssistant;

    IEnumerator Start()
    {
        yield return new WaitForSecondsRealtime(2f);
        VirtualAssistant.SetVirtualMessage(0);
    }

    public void BackButtonPress()
    {
        if (_backButtonCanvas.gameObject.activeInHierarchy)
        {
            SystemManager.instance.SelectedObject.UnSelect();
            SystemManager.instance.SelectedObject = null;
            SystemManager.instance.CamManager.ChangeCamera(1);
            SystemManager.instance.UiAudio.PlayBack();
            ShowHideBackButton(false);
            ShowHideMainTextPanel(false);

            SystemManager.instance.UiManager.VirtualAssistant.SetVirtualMessage(1);
        }
        else
        {
            Application.Quit();
        }
    }
    
    public void ShowHideBackButton(bool visibility)
    {
        if (_backButtonRoutine != null)
        {
            StopCoroutine(_backButtonRoutine);
        }

        _backButtonRoutine = StartCoroutine(CanvasFade(_fadeDuration, visibility, _backButtonCanvas));
    }

    public void SettingsButtonPress()
    {
        if (_settingsPanelRoutine != null)
        {
            StopCoroutine(_settingsPanelRoutine);
        }

        bool panelActive = _settingsCanvas.gameObject.activeInHierarchy;

        if (panelActive)
        {
            SystemManager.instance.UiAudio.PlayBack();
        }
        else
        {
            SystemManager.instance.UiAudio.PlayNormal();
        }

        _settingsPanelRoutine = StartCoroutine(CanvasFade(_fadeDuration, !panelActive, _settingsCanvas));
    }

    public void EnterButtonPress()
    {
        SystemManager.instance.CamManager.ChangeCamera(1);
        SystemManager.instance.InputManager.ChangeInputChecks();
        SystemManager.instance.UiAudio.PlayNormal();
        SystemManager.instance.UiManager.VirtualAssistant.SetVirtualMessage(1);
    }

    public void ShowHideMainTextPanel(bool visibility, string content = null, string header = null)
    {
        if (_mainPanelFadeRoutine != null)
        {
            StopCoroutine(_mainPanelFadeRoutine);
        }

        if (header != null)
        {
            _lastContentKey = content;
            _lastHeaderKey = header;
            StartCoroutine(LocalizeText(content, header));
        }

        _mainPanelFadeRoutine = StartCoroutine(CanvasFade(_fadeDuration, visibility, _mainTextCanvas));
    }

    IEnumerator LocalizeText(string content, string header)
    {
        var headerOp =  LocalizationSettings.StringDatabase.GetLocalizedStringAsync(header);
        while (!headerOp.IsDone)
        {
            yield return null;
        }
        _headerText.text = headerOp.Result;

        var contentOp =  LocalizationSettings.StringDatabase.GetLocalizedStringAsync(content);
        while (!contentOp.IsDone)
        {
            yield return null;
        }
        _contentText.text = contentOp.Result;
    }

    public void ChangeMainTextPanel(string content, string header)
    {
        _lastContentKey = content;
        _lastHeaderKey = header;
        StartCoroutine(LocalizeText(content, header));
    }

    public void UpdateOnLocalizationChange()
    {
        if (_mainTextCanvas.gameObject.activeInHierarchy)
        {
            StartCoroutine(LocalizeText(_lastContentKey, _lastHeaderKey));
        }

        VirtualAssistant.LocalizeMessage();
    }

    public bool MainPanelStatus()
    {
        return _mainTextCanvas.gameObject.activeInHierarchy;
    }

    
    IEnumerator CanvasFade(float duration, bool fadeIn, CanvasGroup canvas)
    {
        float finalValue;
        float startValue = canvas.alpha;
        canvas.interactable = fadeIn;

        if (fadeIn)
        {
            canvas.gameObject.SetActive(true);
            finalValue = 1f;
        }
        else
        {
            finalValue = 0;
        }


        float timer = 0;

        while (timer < duration)
        {
            timer += Time.deltaTime;

            canvas.alpha = Mathf.Lerp(startValue, finalValue, timer / duration);
            yield return null;
        }

        canvas.alpha = finalValue;

        if (finalValue == 0)
        {
            canvas.gameObject.SetActive(false);
        }
    }

    public static Vector2 ClampToCanvas(Vector2 UIposition, RectTransform panelRect)
    {
        // Faz o rect passado ficar restrito à borda do canvas
        Vector2 anchoredPosition = UIposition / SystemManager.instance.CanvasRect.localScale.x;

        anchoredPosition.x = Mathf.Clamp(anchoredPosition.x, 0,  SystemManager.instance.CanvasRect.rect.width - panelRect.rect.width);
        anchoredPosition.y = Mathf.Clamp(anchoredPosition.y, 0,  SystemManager.instance.CanvasRect.rect.height - panelRect.rect.height);

        return anchoredPosition;
    }
}
