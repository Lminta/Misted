using System;
using System.Collections.Generic;
using UnityEngine;

namespace Minigames.Misted
{
    public class MistedObject : MonoBehaviour
    {
        [SerializeField] 
        ComputeShader _resultShader;
        [SerializeField] 
        CustomRenderTexture _texture;
        [SerializeField]
        CustomRenderTexture _maskPrefab;
        CustomRenderTexture _mask;
        [SerializeField] 
        Texture _dirt;
        [SerializeField]
        Material _effectMaterial;
        [SerializeField]
        Material _maskMaterial;
        [SerializeField]
        RectTransform _transform;
        [SerializeField] 
        float _requiredProgress = 95f;
        [SerializeField] 
        float _brushSize = 0.1f;

        public void SetBrushSize(float value)
        {
            _brushSize = value;
            _maskMaterial.SetFloat("_BrushSize", _brushSize);
        }

        bool _win;
        int _kernelHandle;
        ComputeBuffer _buffer;
        int[] _resultBuff;
        TouchController _touchController;
        Vector2 _previousPosition;
        Stack<RenderTexture> _steps = new Stack<RenderTexture>();

        const int PIXELS_FOR_GROUP = 8;
        public bool HasSteps => _steps.Count > 0;
        public event Action OnClean;

        void Awake()
        {
#if UNITY_EDITOR
            _touchController = gameObject.AddComponent<MouseController>();
#elif UNITY_ANDROID || UNITY_IOS
            _touchController = gameObject.AddComponent<TouchScreenController>();
#endif
            _touchController.TouchStart += SaveStep;
            _touchController.TouchStart += ComputeEndPoints;
            _touchController.TouchMove += ComputeSegment;
            _touchController.TouchEnd += CheckResult;
            
            _win = false;
            _mask = Instantiate(_maskPrefab);
        }

        public void Undo()
        {
            if (!HasSteps)
            {
                return;
            }

            if (_win)
            {
                Restart();
                return;
            }

            var tmp = _steps.Pop();
            Graphics.CopyTexture(tmp, _mask);
            Graphics.CopyTexture(tmp, _mask.GetDoubleBufferRenderTexture());
            RenderTexture.ReleaseTemporary(tmp);
        }

        void Restart()
        {
            RenderTexture tmp = _steps.Pop();
            while (HasSteps)
            {
                RenderTexture.ReleaseTemporary(tmp);
                tmp = _steps.Pop();
            }
            Graphics.CopyTexture(tmp, _mask);
            Graphics.CopyTexture(tmp, _mask.GetDoubleBufferRenderTexture());
            RenderTexture.ReleaseTemporary(tmp);
            _win = false;
        }

        void SaveStep(Vector2 pos)
        {
            var tmp = RenderTexture.GetTemporary(_mask.descriptor);
            Graphics.CopyTexture(_mask, tmp);
            _steps.Push(tmp);
        }

        void Start()
        {
            InitMask();
            InitEffectMaterial();
            InitResultShader();
        }

        void Update()
        {
            ComputeEffect();
        }

        void OnDestroy()
        {
            _buffer.Release();
            foreach (var step in _steps)
            {
                RenderTexture.ReleaseTemporary(step);
            }
            _steps.Clear();
        }

        void InitMask()
        {
            _mask.Initialize();
            var ratio = _transform.rect.height / _transform.rect.width;
            _maskMaterial.SetFloat("_AspectRatio", ratio);
            SetBrushSize(_brushSize);
        }

        void InitEffectMaterial()
        {
            _texture.Initialize();
            _effectMaterial.SetTexture("_TexDirt", _dirt);
            _effectMaterial.SetTexture("_Mask", _mask);
            _texture.Update();
        }

        void InitResultShader()
        {
            _kernelHandle = _resultShader.FindKernel("ComputeResult");
            _buffer = new ComputeBuffer(_mask.width, sizeof(int));
            _resultBuff = new int[_mask.width];
        }

        void ComputeSegment(Vector2 pos)
        {
            pos.x /= Screen.width;
            pos.y /= Screen.width;
            
            _mask.shaderPass = 1;
            _maskMaterial.SetVector("_DrawPositionA", _previousPosition);
            _maskMaterial.SetVector("_DrawPositionB", pos);
            var distance = (pos - _previousPosition).magnitude;
            _maskMaterial.SetFloat("_DistanceAB", distance);
            _previousPosition = pos;
            _mask.Update();
        }

        void ComputeEndPoints(Vector2 pos)
        {
            _mask.shaderPass = 0;
            pos.x /= Screen.width;
            pos.y /= Screen.width;
            _maskMaterial.SetVector("_DrawPosition", pos);
            _previousPosition = pos;
            _mask.Update();
        }

        void ComputeEffect()
        {
            _texture.Update();
        }

        int ComputeResult()
        {
            _resultShader.SetInt("_BuffSize", _mask.width);
            _resultShader.SetTexture(_kernelHandle, "_TexMask", _mask);
            _buffer.SetData(_resultBuff);
            _resultShader.SetBuffer(_kernelHandle, "_Output", _buffer);
            _resultShader.Dispatch(_kernelHandle, 
                _mask.width / PIXELS_FOR_GROUP, 1, 1);
            _buffer.GetData(_resultBuff);
            
            int sum = 0;
            for (int j = 0; j < _resultBuff.Length; j++)
            {
                sum += _resultBuff[j];
            }

            return sum;
        }

        void CheckResult(Vector2 pos)
        {
            if (_win)
            {
                return;
            }

            int sum = ComputeResult();
            var result = 100 - (float) sum / (_mask.height * _mask.width) * 100;
            if (result > _requiredProgress)
            {
                _win = true;
                OnClean?.Invoke();
            }
        }
    }
}
