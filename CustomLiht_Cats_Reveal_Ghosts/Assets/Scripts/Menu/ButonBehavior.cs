using UnityEngine;
using UnityEngine.EventSystems;

public class ButonBehavior : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private GameObject _description = null;

    private void Awake()
    {
        if (_description)
            _description.SetActive(false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_description)
            _description.SetActive(true);
    }
    
    public void OnPointerExit(PointerEventData eventData)
    {
        if (_description)
            _description.SetActive(false);
    }
}
