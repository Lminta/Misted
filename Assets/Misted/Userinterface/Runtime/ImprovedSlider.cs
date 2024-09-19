using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using DG.Tweening;
using System;

namespace Minigames.Misted.Userinterface
{
    public class ImprovedSlider : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        Action<float> _onSliderValueChanged;

        [SerializeField]
        Transform _handler;

        [SerializeField]
        Image _areol;

        [SerializeField]
        Image _location;

        [SerializeField]
        Slider _slider;

        [SerializeField]
        TextMeshProUGUI _progressText;

        Sequence _animation;
        const float _animationTime = 0.12f;

        internal void Setup(Action<float> onSliderValueChanged)
        {
            _onSliderValueChanged = onSliderValueChanged;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (_animation != null)
                _animation.Kill();

            _animation = DOTween.Sequence();
            _animation.OnComplete(() => { _animation = null; });
            _animation.Append(_areol.DOFade(0.1f, _animationTime))
                .Join(_location.DOFade(1f, _animationTime))
                .Join(_location.transform.DOScale(1f, _animationTime))
                .Join(_handler.DOScale(0.5f, _animationTime))
                .Join(_progressText.DOFade(1f, _animationTime))
                .Join(_progressText.transform.DOScale(1f, _animationTime));
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (_animation != null)
                _animation.Kill();

            _animation = DOTween.Sequence();
            _animation.OnComplete(() => { _animation = null; });
            _animation.Append(_areol.DOFade(0f, _animationTime))
                .Join(_location.DOFade(0f, _animationTime))
                .Join(_location.transform.DOScale(0.4f, _animationTime))
                .Join(_handler.DOScale(1f, _animationTime))
                .Join(_progressText.transform.DOScale(0.5f, _animationTime))
                .Join(_progressText.DOFade(0f, _animationTime));
        }

        public void OnValueChanged(float _value)
        {
            int progress = Mathf.RoundToInt(_value * 100f);
            _progressText.text = progress.ToString();
            _onSliderValueChanged?.Invoke(_value);
        }
    }
}