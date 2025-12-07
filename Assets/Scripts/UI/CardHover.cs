using UnityEngine;
using UnityEngine.EventSystems;

public class CardHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public string unitName;
    [TextArea] public string stats;

    private bool hovering = false;
    private float hoverTime = 0.4f; // delay
    private float timer;

    public void OnPointerEnter(PointerEventData eventData)
    {
        hovering = true;
        timer = 0f;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        hovering = false;
        TooltipUI.Instance.Hide();
    }

    void Update()
    {
        if (hovering)
        {
            timer += Time.deltaTime;

            if (timer >= hoverTime)
            {
                TooltipUI.Instance.Show(unitName, stats, Input.mousePosition);
                hovering = false; // prevents repeat trigger
            }
        }
    }
}
