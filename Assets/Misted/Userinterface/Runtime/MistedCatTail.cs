using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.UI;

namespace Minigames.Misted.Userinterface
{
    public class MistedCatTail : MonoBehaviour
    {
        [SerializeField]
        Image _tail;
        [SerializeField]
        int _totalFrames;
        [SerializeField]
        int _tailFrameRate;
        [SerializeField]
        Vector2 _tailMoveFrequency;
        
        float _tailTimer;
        int _tailFrameIndex;
        void Awake()
        {
            _tailTimer = Random.Range(_tailMoveFrequency.x, _tailMoveFrequency.y);
        }

        void Update()
        {
            _tailTimer -= Time.deltaTime;

            if (_tailTimer <= 0)
            {
                MoveTail();
            }
        }
        
        void MoveTail()
        {
            _tail.material.SetInt("_Index", _tailFrameIndex);
            _tailTimer = 1f / _tailFrameRate;
            _tailFrameIndex++;
            if (_tailFrameIndex >= _totalFrames)
            {
                _tailFrameIndex = 0;
                _tailTimer = Random.Range(_tailMoveFrequency.x, _tailMoveFrequency.y);
            }
        }
    }
}