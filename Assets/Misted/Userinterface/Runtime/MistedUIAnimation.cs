using System;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections;

namespace Minigames.Misted.Userinterface
{
    public class MistedUIAnimation : MonoBehaviour
    {
        [SerializeField]
        Image _photoButton;
        [SerializeField]
        Image _exitButton;
        [SerializeField]
        Image _backButton;

        Sequence _animation;
        Sequence _backAnimation;
        float _defaultPos;
        float _defaultSize;

        const float ANIM_DURATION = 0.34f;
        const float INITIAL_SIZE_PHOTO_BUTTON = 60f;
        const float INITIAL_SCALE_CANCEL_BUTTON = 0.65f;
        const float START_PHOTO_POS = 48f;

        public void Animate(bool open, Action animFinalAction = null)
        {
            if (_animation != null)
                _animation.Kill();
            _animation = DOTween.Sequence();
                
            if (open)
            {
                _photoButton.rectTransform.anchoredPosition = Vector2.up * START_PHOTO_POS;
                _photoButton.rectTransform.sizeDelta = Vector2.one * INITIAL_SIZE_PHOTO_BUTTON;
                //UIGameColors.SetTransparent(_exitButton);
                _exitButton.color = Color.clear;
                _exitButton.transform.localScale = Vector3.one * INITIAL_SCALE_CANCEL_BUTTON;
                //UIGameColors.SetTransparent(_backButton);
                _exitButton.color = Color.clear;
                _backButton.transform.localScale = Vector3.one * INITIAL_SCALE_CANCEL_BUTTON;

                _animation.SetEase(Ease.OutSine);
                _animation.Append(_photoButton.rectTransform.DOSizeDelta(Vector2.one * _defaultSize, ANIM_DURATION))
                    .Join(_photoButton.rectTransform.DOAnchorPosY(_defaultPos, ANIM_DURATION))
                    .Join(_exitButton.transform.DOScale(1f, ANIM_DURATION))
                    .Join(_exitButton.DOFade(1f, ANIM_DURATION));

                _animation.OnComplete(() =>
                {
                    animFinalAction?.Invoke();
                    _animation = null;
                });
            }
            else
            {
                _animation.SetEase(Ease.InSine);
                _animation.Append(_photoButton.rectTransform.DOSizeDelta(Vector2.one * INITIAL_SIZE_PHOTO_BUTTON, ANIM_DURATION))
                    .Join(_photoButton.rectTransform.DOAnchorPosY(START_PHOTO_POS, ANIM_DURATION))
                    .Join(_exitButton.transform.DOScale(INITIAL_SCALE_CANCEL_BUTTON, ANIM_DURATION))
                    .Join(_exitButton.DOFade(0f, ANIM_DURATION))
                    .Join(_backButton.transform.DOScale(INITIAL_SCALE_CANCEL_BUTTON, ANIM_DURATION))
                    .Join(_backButton.DOFade(0f, ANIM_DURATION));

                _animation.OnComplete(() =>
                {
                    _animation = null;
                    animFinalAction?.Invoke();
                    _photoButton.rectTransform.anchoredPosition = Vector2.up * _defaultSize;
                    _photoButton.rectTransform.sizeDelta = Vector2.one * _defaultPos;
                    //UIGameColors.SetTransparent(_exitButton, 1f);
                    _exitButton.color = Color.black;
                    _exitButton.transform.localScale = Vector3.one;
                    //UIGameColors.SetTransparent(_backButton, 1f);
                    _exitButton.color = Color.black;
                    _backButton.transform.localScale = Vector3.one;
                });
            }
        }
        
        public void AnimateBackButton(bool show, Action animFinalAction = null)
        {
            if (_backAnimation != null)
                _backAnimation.Kill();
            _backAnimation = DOTween.Sequence();
                
            if (show)
            {
                //UIGameColors.SetTransparent(_backButton);
                _backButton.color = Color.clear;
                _backButton.transform.localScale = Vector3.one * INITIAL_SCALE_CANCEL_BUTTON;

                _backAnimation.SetEase(Ease.OutSine);
                _backAnimation.Append(_backButton.transform.DOScale(1f, ANIM_DURATION))
                    .Join(_backButton.DOFade(1f, ANIM_DURATION));

                _backAnimation.OnComplete(() =>
                {
                    animFinalAction?.Invoke(); 
                    _backAnimation = null;
                });
            }
            else
            {
                _backAnimation.SetEase(Ease.OutSine);
                _backAnimation.Append(_backButton.transform.DOScale(INITIAL_SCALE_CANCEL_BUTTON, ANIM_DURATION))
                    .Join(_backButton.DOFade(0f, ANIM_DURATION));

                _backAnimation.OnComplete(() =>
                {
                    _backAnimation = null;
                    animFinalAction?.Invoke();
                    //UIGameColors.SetTransparent(_backButton, 1f);
                    _backButton.color = Color.black;
                    _backButton.transform.localScale = Vector3.one;
                });
            }
        }

        void Awake()
        {
            _defaultPos = _photoButton.rectTransform.anchoredPosition.y;
            _defaultSize = _photoButton.rectTransform.sizeDelta.x;
        }

        void OnDestroy()
        {
            if (_backAnimation != null)
                _backAnimation.Kill();
            if (_animation != null)
                _animation.Kill();
        }
    }
}