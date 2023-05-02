using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


///<summary>
/// Sistema que gerencia todo o Input do usuário
/// e permite a manipulação de câmeras e objetos
///</summary>
public class InputManager : MonoBehaviour
{
    delegate void InputChecks();
    InputChecks CurrentInputChecks;

    bool _isRotatingAnObject;
    Vector3 _rotationEuler;
    Transform _rotationPivot;
    [SerializeField] float _mouseSensitivity = 5f;

    void Update()
    {
        CurrentInputChecks();
    }

    void Awake()
    {
        CurrentInputChecks = IntroInputs;
    }

    public void ChangeInputChecks()
    {
        // Troca o método que checa os Inputs, quando o usuário aperta Enter no início da cena
        CurrentInputChecks = StandardInputs;
    }

    void StandardInputs()
    {
        // Inputs possíveis no estado padrão da cena, permitindo o usuário a selecionar, inspecionar
        // e rotacionar objetos

        if (Input.GetAxis("Mouse ScrollWheel") != 0)
        {
            SystemManager.instance.CamManager.ChangeZoom(Input.GetAxis("Mouse ScrollWheel"));
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SystemManager.instance.UiManager.BackButtonPress();
        }

        if (Input.GetKeyDown(KeyCode.Mouse0) && !EventSystem.current.IsPointerOverGameObject() && !_isRotatingAnObject)
        {
            if (SystemManager.instance.InspectedObject != null)
            {
                if (SystemManager.instance.SelectedObject != null)
                {
                    if (SystemManager.instance.SelectedObject != SystemManager.instance.InspectedObject)
                    {
                        SystemManager.instance.SelectedObject.UnSelect();
                        SelectObject(SystemManager.instance.InspectedObject);
                    }
                }
                else
                {
                    SelectObject(SystemManager.instance.InspectedObject);
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.Mouse2))
        {
            if (SystemManager.instance.SelectedObject != null && SystemManager.instance.SelectedObject.IsRotatable)
            {
                RotateObject(SystemManager.instance.SelectedObject);
            }
        }
    }

    void IntroInputs()
    {
        // Inputs possíveis no início da cena

        if (Input.GetAxis("Mouse ScrollWheel") != 0)
        {
            SystemManager.instance.CamManager.ChangeZoom(Input.GetAxis("Mouse ScrollWheel"));
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SystemManager.instance.UiManager.EnterButtonPress();
        }

        if (Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            SystemManager.instance.UiManager.EnterButtonPress();
        }

        if (Input.GetKeyDown(KeyCode.Return))
        {
            SystemManager.instance.UiManager.EnterButtonPress();
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            SystemManager.instance.UiManager.EnterButtonPress();
        }
    }

    void RotateObject(DisplayedEquipment selected)
    {
        // Método que inicia a Coroutine de rotação do objeto selecionado,
        // ao apertar o botão do meio do mouse
        _isRotatingAnObject = true;
        StopAllCoroutines();

        selected.ToggleInteractionPoints(false);

        _rotationPivot = selected.RotationPivot;
        _rotationEuler = _rotationPivot.eulerAngles;
        Cursor.lockState = CursorLockMode.Locked;
        StartCoroutine(RotateSelected());
    }

    void SelectObject(DisplayedEquipment interactable)
    {
        interactable.SelectObject();
        SystemManager.instance.SelectedObject = interactable;
        SystemManager.instance.CamManager.ChangeCamera(interactable.CameraIndex);
        SystemManager.instance.UiAudio.PlayLight();
                           
        SystemManager.instance.UiManager.ShowHideBackButton(true);
        SystemManager.instance.UiManager.VirtualAssistant.SetVirtualMessage(2);
    }

    IEnumerator RotateSelected()
    {
        // CoRoutine que roda o objeto selecionado de acordo
        // com o movimento do mouse, até o momento em que o
        // usuário solta o botão do meio

        while (_isRotatingAnObject)
        {
            if (Input.GetKeyUp(KeyCode.Mouse2))
            {
                _rotationPivot = null;
                _isRotatingAnObject = false;
                Cursor.lockState = CursorLockMode.Confined;
                SystemManager.instance.SelectedObject.ToggleInteractionPoints(true);
                yield break;
            }

            _rotationEuler.y += Input.GetAxis("Mouse X") * _mouseSensitivity * -1;
            _rotationPivot.rotation = Quaternion.Euler(_rotationEuler);

            yield return null;
        }
    }
}
