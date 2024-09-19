using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Minigames.Misted.Userinterface
{
    public class Hint : MonoBehaviour
    {
        [SerializeField]
        Image _background;
        [SerializeField]
        Image _arrow;
        [SerializeField]
        TextMeshProUGUI _hintText;

        Sequence _animation;
        const float _animationTime = 0.5f;
        
        void OnEnable()
        {
            Animate(true);
        }

        void Update()
        {
            bool touch = false;
#if UNITY_EDITOR
            touch = Input.GetMouseButtonDown(0);
#elif UNITY_ANDROID || UNITY_IOS
            touch = Input.touchCount > 0;
#endif
            if (touch)
            {
                Animate(false, () => this.gameObject.SetActive(false));
            }
        }

        void Animate(bool show, Action onComplete = null)
        {
            if(show)
            {
                if (_animation != null)
                    _animation.Kill();

                _animation = DOTween.Sequence();
                var tmp = Color.gray;
                tmp.a = 0;
                _background.color = tmp;
                _arrow.color = tmp;
                _background.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);
                
                tmp = Color.white;
                tmp.a = 0;
                _hintText.color = tmp;
                _hintText.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);
                
                _animation.OnComplete(() =>
                {
                    _animation = null;
                    onComplete?.Invoke();
                });
                _animation.Append(_background.DOFade(1f, _animationTime))
                    .Join(_background.transform.DOScale(1f, _animationTime))
                    .Join(_hintText.DOFade(1f, _animationTime))
                    .Join(_hintText.transform.DOScale(1f, _animationTime))
                    .Join(_arrow.DOFade(1f, _animationTime));

            }
            else
            {
                if (_animation != null)
                    _animation.Kill();

                _animation = DOTween.Sequence();
                _animation.OnComplete(() =>
                {
                    _animation = null; 
                    onComplete?.Invoke();
                });
                _animation.Append(_background.DOFade(0f, _animationTime))
                    .Join(_background.transform.DOScale(0.4f, _animationTime))
                    .Join(_hintText.transform.DOScale(0.4f, _animationTime))
                    .Join(_hintText.DOFade(0f, _animationTime))
                    .Join(_arrow.DOFade(0f, _animationTime));
            }
        }

        void OnDestroy()
        {
            if (_animation != null)
                _animation.Kill();
        }
    }
}