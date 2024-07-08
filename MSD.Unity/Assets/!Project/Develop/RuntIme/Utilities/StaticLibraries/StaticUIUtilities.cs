
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public static class StaticUIUtilities
{
    public static bool IsPointerOverUIElement(Vector2 mousePosition)
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = mousePosition;
        List<RaycastResult> raycastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, raycastResults);

        for (int index = 0; index < raycastResults.Count; index++)
        {
            RaycastResult currentRaysastResult = raycastResults[index];
            if (currentRaysastResult.gameObject.layer == LayerMask.NameToLayer("UI"))
                return true;
        }
        
        return false;
    }
}
