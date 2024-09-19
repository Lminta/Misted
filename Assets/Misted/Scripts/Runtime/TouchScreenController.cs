using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Minigames.Misted
{
    public class TouchScreenController : TouchController 
    {
        int _touchID = -1;
        void Update()
        {
            if (Input.touchCount == 0 || IsOverButton())
            {
                return;
            }

            var touch = Input.GetTouch(0);
            switch (touch.phase)
            {
                case TouchPhase.Began:
                    TouchStart?.Invoke(touch.position);
                    _touchID = touch.fingerId;
                    break;
                case TouchPhase.Ended:
                    TouchEnd?.Invoke(touch.position);
                    _touchID = -1;
                    break;
                case TouchPhase.Moved:
                    if (_touchID == touch.fingerId)
                    {
                        TouchMove?.Invoke(touch.position);
                    }

                    break;
                default:
                    break;
            }
        }

        protected override bool IsOverGameObject(out Vector2 inputPosition)
        {
            inputPosition = Vector2.zero;
            inputPosition = Input.GetTouch(0).position;
            var fingerId = Input.GetTouch(0).fingerId;
            return EventSystem.current.IsPointerOverGameObject(fingerId);
        }
    }
}