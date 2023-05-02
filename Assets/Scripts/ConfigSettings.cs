using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Settings;
using TMPro;
using UnityEngine.Audio;
using UnityEngine.UI;


///<summary>
/// Sistema que gerencia opções de configuração disponíveis ao usuário
/// como volume do áudio, localização e toggle do AssistenteVirtual
///</summary>
public class ConfigSettings : MonoBehaviour
{
    [SerializeField] AudioMixer _mixer;
    [SerializeField] Slider _masterSlider;
    [SerializeField] TMP_Dropdown _localeDropdown;
    [SerializeField] Toggle _virtualAssistantToggle;


    public void ChangeAudioVolume()
    {
        _mixer.SetFloat("Master", Mathf.Log10(_masterSlider.value) * 20);
    }

    public void ChangeLocalization()
    {
        ILocalesProvider availableLocales = LocalizationSettings.AvailableLocales;
        switch (_localeDropdown.value)
        {
            case 0:
                LocalizationSettings.SelectedLocale = availableLocales.GetLocale("pt-BR");
               break;

            case 1:
                LocalizationSettings.SelectedLocale = availableLocales.GetLocale("en");
                break;

            default:
                LocalizationSettings.SelectedLocale = availableLocales.GetLocale("es");
                break;
        }

        SystemManager.instance.UiManager.UpdateOnLocalizationChange();

        SystemManager.instance.UiAudio.PlayLight();
    }

    public void ToggleVirtualAssistant()
    {
        SystemManager.instance.UiManager.VirtualAssistant.TogglePanel(_virtualAssistantToggle.isOn ? true : false);
        SystemManager.instance.UiAudio.PlayLight();
    }
}
