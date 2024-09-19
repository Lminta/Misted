using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Minigames.Misted
{
    public class MouseController : TouchController
    {
        bool _touch = false;
        void Update()
        {
            if (IsOverButton())
            {
                return;
            }
            
            if (Input.GetMouseButtonDown(0))
            {
                TouchStart?.Invoke(Input.mousePosition);
                _touch = true;
                return; 
            }
            if (Input.GetMouseButtonUp(0))
            {   
                TouchEnd?.Invoke(Input.mousePosition);
                _touch = false;
                return; 
            }

            if (_touch && Input.GetMouseButton(0))
            {
                TouchMove?.Invoke(Input.mousePosition);
            }
        }
        
        protected override bool IsOverGameObject(out Vector2 inputPosition)
        {
            inputPosition = Input.mousePosition;
            return EventSystem.current.IsPointerOverGameObject();
        }
    }
}