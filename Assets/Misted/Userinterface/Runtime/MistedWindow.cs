using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Minigames.Misted.Userinterface
{
    public class MistedWindow : MonoBehaviour
    {
        MistedUIAnimation _windowAnimation;

        [SerializeField]
        Canvas _canvas;
        [SerializeField]
        RectTransform _safeArea;
        [SerializeField]
        MistedObject _misted;
        [SerializeField]
        Button _backButton;
        [SerializeField]
        ImprovedSlider _slider;
        [SerializeField]
        List<GameObject> _buttons;
        
        MistedInitializer _initializer;

        public void Setup(MistedInitializer self)
        {
            _initializer = self;
            _slider.Setup(_misted.SetBrushSize);
        }
        
        public void OnShutterClick()
        {
            foreach (var button in _buttons)
            {
                button.SetActive(false);
            }
            _ = CaptureScreenshot(OnScreenshotMade);
        }
        
        public void OnExitButtonClick()
        {
            _initializer.Close();
        }
        
        public void OnBackButtonClick()
        {
            _misted.Undo();
        }

        public void OnClose(Action callback = null)
        {
            _windowAnimation.Animate(false, () => callback?.Invoke());
        }
        
        void Awake()
        {
            _windowAnimation = GetComponent<MistedUIAnimation>();
            _windowAnimation.Animate(true);
        }
        
        void Update()
        {
            if (_backButton.gameObject.activeInHierarchy != _misted.HasSteps)
            {

                if (_misted.HasSteps)
                {
                    _backButton.gameObject.SetActive(true);
                    _windowAnimation.AnimateBackButton(true, 
                        () => _backButton.interactable = true);
                }
                else
                {                
                    _backButton.interactable = false; 
                    _windowAnimation.AnimateBackButton(false, 
                        () => _backButton.gameObject.SetActive(false));
                }
            }
        }

        void OnScreenshotMade(Texture2D screenshot)
        {
            foreach (var button in _buttons)
            {
                button.SetActive(true);
            }
            
            SaveTextureAsPNG(screenshot, "Screenshot.png");
        }

        
        void SaveTextureAsPNG(Texture2D texture, string filename)
        {
            byte[] bytes = texture.EncodeToPNG();
            string filePath = Path.Combine(Application.persistentDataPath, filename);
            File.WriteAllBytes(filePath, bytes);

            Debug.LogFormat("Saved screenshot to: {0}", filePath);
        }
        
        async Task CaptureScreenshot(Action<Texture2D> onScreenshotMade)
        {
            await Task.Yield();

            RenderTexture rt = new RenderTexture(Screen.width, Screen.height, 24);
            
            var mainCamera = Camera.current;

            _canvas.renderMode = RenderMode.ScreenSpaceCamera;
            _canvas.planeDistance = 1f;
            _canvas.worldCamera = mainCamera;
            try
            {
                mainCamera.targetTexture = rt;
                mainCamera.Render();

                RenderTexture.active = rt;

                var screenshot = new Texture2D(Screen.width, Screen.height);
                screenshot.ReadPixels(new Rect(0, 0, screenshot.width, screenshot.height), 0, 0);
                screenshot.Apply();

                mainCamera.targetTexture = null;

                onScreenshotMade?.Invoke(screenshot);
            }
            catch (Exception e)
            {
                Debug.LogErrorFormat("Failed to capture screenshot: {0}", e);
            }
            finally
            {
                _canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                RenderTexture.active = null;
                rt.Release();
            }
        }
    }
}