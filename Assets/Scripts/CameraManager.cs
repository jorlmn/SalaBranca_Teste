using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

///<summary>
/// Sistema que gerencia a troca de prioridade das VirtualCameras
/// e também altera a distância da câmera ativada de acordo com o Input do usuário.
///</summary>
public class CameraManager : MonoBehaviour
{
    public Camera CameraCache;

    [SerializeField] List<CinemachineVirtualCamera> _virtualCameras;

    [SerializeField] float _zoomSensitivity = 1;


    List<CinemachineFramingTransposer> _virtualCameraComponents = new();

    int _activeCameraIndex = 0;

    float _currentCamDistance = 1.5f;

    float _minZoom = 0.2f;

    float _maxZoom = 1.5f;

    void Awake()
    {
        CameraCache = Camera.main;

        foreach (CinemachineVirtualCamera camera in _virtualCameras)
        {
            _virtualCameraComponents.Add(camera.GetCinemachineComponent<CinemachineFramingTransposer>());
        }
    }

    public void ChangeCamera(int cameraIndex)
    {
        // Se é uma câmera diferente
        if (cameraIndex != _activeCameraIndex)
        {
            // Inverte a prioridade da câmera anterior e da atual
            _virtualCameras[_activeCameraIndex].m_Priority = 0;
            _virtualCameras[cameraIndex].m_Priority = 1;

            // Reseta zoom da câmera anterior
            _virtualCameraComponents[_activeCameraIndex].m_CameraDistance = _maxZoom;

            _activeCameraIndex = cameraIndex;
            _currentCamDistance = _virtualCameraComponents[_activeCameraIndex].m_CameraDistance;
        }
    }

    public void ChangeZoom(float mouseMovement)
    {
        // Faz zoom in & out com mouse scrollwheel, parâmero passado pelo InputManager
        _currentCamDistance -= mouseMovement * _zoomSensitivity;
        _currentCamDistance = Mathf.Clamp(_currentCamDistance, _minZoom, _maxZoom);
        _virtualCameraComponents[_activeCameraIndex].m_CameraDistance = _currentCamDistance;
    }
}
