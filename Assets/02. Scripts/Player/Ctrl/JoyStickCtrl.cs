using UnityEngine;
using UnityEngine.EventSystems;

public class JoyStickCtrl : MonoBehaviour, IDragHandler , IPointerDownHandler, IPointerUpHandler
{
    [SerializeField]
    private RectTransform m_back_ground;
    [SerializeField]
    private RectTransform m_handle;

    private float m_back_ground_radius;

    private Vector2 m_start_position;
    private Vector2 m_input_vector;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        m_back_ground_radius = m_back_ground.rect.width / 2;
        m_back_ground.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnPointerDown(PointerEventData eventData)
    {
        m_start_position= eventData.position;
        m_back_ground.position= m_start_position;
        m_back_ground.gameObject.SetActive(true);      
    }

    void IDragHandler.OnDrag(PointerEventData eventData)
    {
        Vector2 dir = eventData.position - m_start_position;
        m_input_vector = Vector2.ClampMagnitude(dir, m_back_ground_radius);
        m_handle.anchoredPosition= m_input_vector;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        m_input_vector = Vector2.zero;
        m_handle.anchoredPosition = m_input_vector;
        m_back_ground.gameObject.SetActive(false) ; 
    }

    public Vector2 GetInputVector()
    {
        return m_input_vector != null ? m_input_vector/ m_back_ground_radius : Vector2.zero;
    }

}
