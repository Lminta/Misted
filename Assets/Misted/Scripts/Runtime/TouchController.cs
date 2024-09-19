using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Minigames.Misted
{
    public abstract class TouchController : MonoBehaviour
    {
        public Action<Vector2> TouchStart;
        public Action<Vector2> TouchMove;
        public Action<Vector2> TouchEnd;

        protected abstract bool IsOverGameObject(out Vector2 inputPosition);
        protected bool IsOverButton()
        {
            bool isOverTaggedElement = false;

            if (IsOverGameObject(out var inputPosition))
            {
                PointerEventData pointerData = new PointerEventData(EventSystem.current)
                {
                    pointerId = -1,
                };
                pointerData.position = inputPosition;

                List<RaycastResult> results = new List<RaycastResult>();
                EventSystem.current.RaycastAll(pointerData, results);

                if (results.Count > 0)
                {
                    for (int i = 0; i < results.Count; ++i)
                    {
                        if (results[i].gameObject.layer == LayerMask.NameToLayer("UI"))
                        {
                            isOverTaggedElement = true;
                            break;
                        }
                    }
                }
            }

            return isOverTaggedElement;
        }
    }
}