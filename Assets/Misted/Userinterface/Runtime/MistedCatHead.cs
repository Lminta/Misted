using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.UI;

namespace Minigames.Misted.Userinterface
{
    public class MistedCatHead : MonoBehaviour
    {
        [SerializeField]
        Image _head;

        [SerializeField]
        Sprite _headOnSide;
        [SerializeField]
        Sprite _blinkHeadOnSide;
        [SerializeField]
        Sprite _blink;

        [SerializeField]
        Vector2 _headOnSideFrequency;
        [SerializeField]
        Vector2 _headOnSideTime;
        [SerializeField]
        Vector2 _blinkFrequency;
        [SerializeField]
        Vector2 _blinkTime;
        
        float _blinkTimer;
        float _headOnSideTimer;
        
        bool _isHeadOnSide;
        bool _isBlinking;

        void Awake()
        {
            _blinkTimer = Random.Range(_blinkFrequency.x, _blinkFrequency.y);
            _headOnSideTimer = Random.Range(_headOnSideFrequency.x, _headOnSideFrequency.y);
        }

        void Update()
        {
            _blinkTimer -= Time.deltaTime;
            _headOnSideTimer -= Time.deltaTime;

            if (_blinkTimer <= 0)
            {
                Blink();
            }

            if (_headOnSideTimer <= 0)
            {
                MoveHead();
            }
            
        }

        void MoveHead()
        {
            if (_isHeadOnSide)
            {
                if (_isBlinking)
                {
                    _head.sprite = _blink;
                }
                else
                {
                    _head.color = Color.clear;
                }

                _headOnSideTimer = Random.Range(_headOnSideFrequency.x, _headOnSideFrequency.y);
            }
            else
            {
                if (_isBlinking)
                {
                    _head.sprite = _blinkHeadOnSide;
                }
                else
                {
                    _head.color = Color.white;
                    _head.sprite = _headOnSide;
                }
                _headOnSideTimer = Random.Range(_headOnSideTime.x, _headOnSideTime.y);
            }
            _isHeadOnSide = !_isHeadOnSide;
        }
        
        void Blink()
        {
            if (_isBlinking)
            {
                if (_isHeadOnSide)
                {
                    _head.sprite = _headOnSide;
                }
                else
                {
                    _head.color = Color.clear;
                }
                _blinkTimer = Random.Range(_blinkFrequency.x, _blinkFrequency.y);
            }
            else
            {
                if (_isHeadOnSide)
                {
                    _head.sprite = _blinkHeadOnSide;
                }
                else
                {
                    _head.color = Color.white;
                    _head.sprite = _blink;
                }
                _blinkTimer = Random.Range(_blinkTime.x, _blinkTime.y);
            }
            _isBlinking = !_isBlinking;
        }
    }
}