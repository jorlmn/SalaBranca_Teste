using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///<summary>
/// Componente que faz uma Pool dos elementos UI a serem usados
/// pelos InteractionPoints
///</summary>
public class InteractionUIPool : MonoBehaviour
{
    [SerializeField] InteractionUI _interactionPrefab;

    [SerializeField] Transform _parent;

    [SerializeField] int _amountToPool = 10;

    List<InteractionUI> _pooledInteractionPoints = new ();

    void Start()
    {
        // No início da cena, instanceia e adiciona os elementos UI à uma lista
        for (int i = 0; i < _amountToPool; i++)
        {   
            InteractionUI newInteractionPoint = Instantiate(_interactionPrefab, _parent);
            newInteractionPoint.gameObject.SetActive(false);
            _pooledInteractionPoints.Add(newInteractionPoint);
        }
    }

    public InteractionUI GetInteractionUI()
    {
        // InteractionPoint invoca este método para pegar o primeiro botão UI que está desativado,
        // ou seja, que não está sendo usado. Caso todos existentes estejam usados, ai de fato instanceia
        // mais um, porém o adicionando na Pool de qualquer forma, para ser reutilizado quando necessário

        int activePoints = 0;

        for (int i = 0; i < _pooledInteractionPoints.Count; i++)
        {
            if (_pooledInteractionPoints[i].gameObject.activeInHierarchy)
            {
                activePoints++;
            }

            if (!_pooledInteractionPoints[i].gameObject.activeInHierarchy)
            {
                return _pooledInteractionPoints[i];
            }
        }

        if (activePoints == _pooledInteractionPoints.Count)
        {
            InteractionUI newInteractionPoint = Instantiate(_interactionPrefab, _parent);
            newInteractionPoint.gameObject.SetActive(false);
            _pooledInteractionPoints.Add(newInteractionPoint);
            return newInteractionPoint;
        }

        return null;
    }
}
