using System;
using Minigames.Misted.Userinterface;
using UnityEngine;

namespace Minigames.Misted
{
    public class MistedInitializer : MonoBehaviour
    {
        [SerializeField]
        Transform _parent;
        [SerializeField]
        MistedWindow _prefab;
        MistedWindow _instance;

        public void Open()
        {
            _instance = Instantiate(_prefab, _parent);
            _instance.Setup(this);
        }

        public void Close()
        {
            _instance.OnClose(() =>
            {
                Destroy(_instance.gameObject);
            });
        }
    }
}
