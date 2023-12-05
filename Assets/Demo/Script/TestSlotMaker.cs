using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Tori.UI
{
    public class TestSlotMaker : MonoBehaviour
    {
        [SerializeField] private Button _button;
        [SerializeField] private GameObject _slotPrefab;
        [SerializeField] private Transform[] _contents;
        [SerializeField] private int _slotCount;
        [SerializeField] private OptimizedScrollRect _verticalScrollRect;
        [SerializeField] private OptimizedScrollRect _horizontalScrollRect;

        private void Awake()
        {
            _button.onClick.AddListener(OnClick);
        }
        private void OnClick()
        {
            for(int i = 0; i < 3; i++)
            {
                for (int j= 0; j < _slotCount; j++)
                {
                    var slot = Instantiate(_slotPrefab, _contents[i]);
                    slot.name = $"Slot {i} {j}";
                }
            }
            _verticalScrollRect.Refresh();
            _horizontalScrollRect.Refresh();
        }
    }
}
