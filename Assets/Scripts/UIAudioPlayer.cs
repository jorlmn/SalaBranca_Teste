using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///<summary>
/// Sistema que gerencia os sons da UI
/// quando botões são clicados, por exemplo.
///</summary>
public class UIAudioPlayer : MonoBehaviour
{
    [SerializeField] AudioSource _audioSource;

    [SerializeField] AudioClip _normalClip;

    [SerializeField] AudioClip _lightClip;

    [SerializeField] AudioClip _backClip;
    
    public void PlayNormal()
    {
        _audioSource.PlayOneShot(_normalClip);
    }

    public void PlayLight()
    {
        _audioSource.PlayOneShot(_lightClip);
    }

    public void PlayBack()
    {
        _audioSource.PlayOneShot(_backClip);
    }
}
