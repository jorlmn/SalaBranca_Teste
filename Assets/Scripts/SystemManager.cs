using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///<summary>
/// SINGLETON que possui referencias para todos os outros sistemas
/// Permite cada sistema a interagir com outros sistemas que devem ser acessíveis
/// sem ser necessário cada um ter seus próprios campos
/// Ou seja, por meio deste único SINGLETON, permite a comunicação entre os sistemas
///</summary>
public class SystemManager : MonoBehaviour
{
    public static SystemManager instance = null;
    public CameraManager CamManager;
    public TooltipPanel TooltipManager;
    public Canvas CanvasCache;
    public RectTransform CanvasRect;
    public UIManager UiManager;
    public UIAudioPlayer UiAudio;
    public InputManager InputManager;
    public InteractionUIPool InteractionPool;
    public DisplayedEquipment InspectedObject = null;
    public DisplayedEquipment SelectedObject = null;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
