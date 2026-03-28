using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class ClickDebugger : MonoBehaviour
{
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            var results = new List<RaycastResult>();
            var eventData = new PointerEventData(EventSystem.current)
            {
                position = Input.mousePosition
            };

            EventSystem.current.RaycastAll(eventData, results);

            // 클릭 이벤트 받는 오브젝트 전체 출력
            foreach (var r in results)
            {
                Debug.Log($"Hit: {r.gameObject.name} | Layer: {LayerMask.LayerToName(r.gameObject.layer)} | Component: {string.Join(", ", System.Array.ConvertAll(r.gameObject.GetComponents<MonoBehaviour>(), m => m.GetType().Name))}");
            }
        }
    }
}