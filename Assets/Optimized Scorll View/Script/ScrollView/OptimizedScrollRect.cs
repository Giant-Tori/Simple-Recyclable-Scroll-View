using UnityEngine;
using UnityEngine.UI;

namespace Tori.UI
{
    public class OptimizedScrollRect : ScrollRect
    {
        [SerializeField] private GameObject _slotPrefab;
        [SerializeField] private GridLayoutGroup _gridLayoutGroup;
        [Header("Setting")]
        [SerializeField] private float _verticalPadding;
        [SerializeField] private float _horizontalPadding;
        [SerializeField] private int _gridCount = 1;

        private Rect _viewportRect;
        private Vector2 _prePosition;

        private int _slotCount;
        private float _slotHeight;
        private float _slotWidth;
        private int _currentStartIndex = 0;

        private readonly float _epsilon = 0.01f; // for float comparison
        private readonly int _buffer = 6; // slot buffer

        private int _verticalSlotCount => (Mathf.CeilToInt(_viewportRect.height / _slotHeight) + _buffer) * _gridCount;
        private int _horizontalSlotCount => (Mathf.CeilToInt(_viewportRect.width / _slotWidth) + _buffer) * _gridCount;
        protected override void Awake()
        {
            _viewportRect = viewport.rect;
        }
        protected override void OnEnable()
        {
            base.OnEnable();

            if (Application.isPlaying == false)
            {
                return;
            }

            if (horizontal)
            {
                onValueChanged.AddListener(OnValueChangedHorizontal);
            }
            else
            {
                onValueChanged.AddListener(OnValueChangedVertical);
            }

            Refresh();
        }
        protected override void OnDisable()
        {
            base.OnDisable();
            onValueChanged.RemoveAllListeners();
        }
        public void Refresh()
        {
            if (!TryInit())
            {
                return;
            }

            SetContentSIze();

            // Reset Data
            _currentStartIndex = 0;
            _prePosition = new Vector2(0f, 0f);
            _gridCount = Mathf.Max(1, _gridCount);

            SetSlotsActive();
            SetSlotPosition(_currentStartIndex);
        }

        private void OnValueChangedVertical(Vector2 normalizedPosition)
        {

            var current = content.anchoredPosition;
            var diff = current.y - _prePosition.y;
            var isDown = diff >= _slotHeight - _epsilon;
            var isUp = -diff >= _slotHeight - _epsilon;

            if (isDown)
            {
                var isLast = _currentStartIndex + _verticalSlotCount >= _slotCount;
                if (isLast)
                {
                    return;
                }

                // Move content
                content.anchoredPosition -= new Vector2(0, _slotHeight);
                _prePosition = content.anchoredPosition;

                // Set slot active
                for (int i = 0; i < _gridCount; i++)
                {
                    SetSlotActive(_currentStartIndex + i, false);
                    if (_currentStartIndex + _verticalSlotCount + i < _slotCount)
                    {
                        SetSlotActive(_currentStartIndex + _verticalSlotCount + i, true);
                    }
                }

                _currentStartIndex += _gridCount;

                // Set slot position
                SetSlotPosition(_currentStartIndex);

            }
            else if (isUp)
            {
                if (_currentStartIndex <= 0)
                {
                    return;
                }

                // Move content
                content.anchoredPosition += new Vector2(0, _slotHeight);
                _prePosition = content.anchoredPosition;

                // Set slot active
                for (int i = 0; i < _gridCount; i++)
                {
                    SetSlotActive(_currentStartIndex + _verticalSlotCount - 1 - i, false);
                    SetSlotActive(_currentStartIndex - 1 - i, true);
                }

                _currentStartIndex -= _gridCount;

                // Set slot position
                SetSlotPosition(_currentStartIndex);
            }
        }


        private void OnValueChangedHorizontal(Vector2 normalizedPosition)
        {
            var current = content.anchoredPosition;
            var diff = current.x - _prePosition.x;
            var isRight = -diff >= _slotWidth - _epsilon;
            var isLeft = diff >= _slotWidth - _epsilon;

            if (isRight)
            {
                var isLast = _currentStartIndex + _horizontalSlotCount >= _slotCount;
                if (isLast)
                {
                    return;
                }
                // Move content
                content.anchoredPosition += new Vector2(_slotWidth, 0);
                _prePosition = content.anchoredPosition;

                // Set slot active
                for (int i = 0; i < _gridCount; i++)
                {
                    SetSlotActive(_currentStartIndex + i, false);
                    if (_currentStartIndex + _horizontalSlotCount + i < _slotCount)
                    {
                        SetSlotActive(_currentStartIndex + _horizontalSlotCount + i, true);
                    }
                }
                _currentStartIndex += _gridCount;

                // Set slot position
                SetSlotPosition(_currentStartIndex);

            }
            else if (isLeft)
            {
                if (_currentStartIndex <= 0)
                {
                    return;
                }
                // Move content
                content.anchoredPosition -= new Vector2(_slotWidth, 0);
                _prePosition = content.anchoredPosition;

                // Set slot active
                for (int i = 0; i < _gridCount; i++)
                {
                    SetSlotActive(_currentStartIndex + _horizontalSlotCount - 1 - i, false);
                    SetSlotActive(_currentStartIndex - 1 - i, true);
                }
                _currentStartIndex -= _gridCount;

                // Set slot position
                SetSlotPosition(_currentStartIndex);
            }
        }


        //Get slot prefab height and slot count
        private bool TryInit()
        {
            _slotCount = content.transform.childCount;

            if (!_slotPrefab.TryGetComponent<RectTransform>(out var slotRect))
            {
                Debug.LogError("Failed to get RectTransform from slotPrefab");
                return false;
            }

            _slotHeight = slotRect.rect.height;
            _slotWidth = slotRect.rect.width;

            return true;
        }

        private void SetSlotActive(int index, bool isActive)
        {
            content.GetChild(index).gameObject.SetActive(isActive);
        }

        private void SetSlotsActive()
        {
            var childCount = content.transform.childCount;
            for (int i = 0; i < childCount; i++)
            {
                if (vertical && i < _verticalSlotCount)
                {
                    content.GetChild(i).gameObject.SetActive(true);

                }
                else if (horizontal && i < _horizontalSlotCount)
                {
                    content.GetChild(i).gameObject.SetActive(true);
                }
                else
                {
                    content.GetChild(i).gameObject.SetActive(false);
                }
            }
        }
        private void SetContentSIze()
        {
            var size = content.sizeDelta;
            if (vertical)
            {
                size.x = _slotWidth * _gridCount;
                size.y = _slotHeight * _verticalSlotCount / _gridCount;
                size.y += _verticalPadding * (_verticalSlotCount - 1);
                size.x += _horizontalPadding * (_gridCount - 1);
            }
            else
            {
                size.x = _slotWidth * _horizontalSlotCount / _gridCount;
                size.y = _slotHeight * _gridCount;
                size.x += _horizontalPadding * (_horizontalSlotCount - 1);
                size.y += _verticalPadding * (_gridCount - 1);
            }
            content.sizeDelta = size;
        }

        private void SetSlotPosition(int start)
        {
            if (vertical)
            {
                SetVerticalSlotPosition(start);

            }
            else
            {
                SetHorizontalSlotPosition(start);
            }

        }


        private void SetVerticalSlotPosition(int start)
        {
            for (int i = start; i < start + _verticalSlotCount / _gridCount; i++)
            {
                for (int j = 0; j < _gridCount; j++)
                {
                    var index = start + (i - start) * _gridCount + j;
                    var rect = content.GetChild(index).GetComponent<RectTransform>();
                    var posX = _slotWidth / 2 + _slotWidth * j;
                    var posY = -_slotHeight / 2 - _slotHeight * (i - start);

                    // Set padding
                    posY -= _verticalPadding * (i - start);
                    posX += _horizontalPadding * j;

                    rect.anchoredPosition = new Vector2(posX, posY);
                    rect.sizeDelta = new Vector2(_slotWidth, _slotHeight);
                }
            }
        }

        private void SetHorizontalSlotPosition(int start)
        {
            for (int i = start; i < start + _horizontalSlotCount; i++)
            {
                for (int j = 0; j < _gridCount; j++)
                {
                    var index = start + (i - start) * _gridCount + j;
                    var rect = content.GetChild(index).GetComponent<RectTransform>();
                    var posX = _slotWidth / 2 + _slotWidth * (i - start);
                    var posY = -_slotHeight / 2 - _slotHeight * j;

                    // Set padding
                    posX += _horizontalPadding * (i - start);
                    posY -= _verticalPadding * j;

                    rect.anchoredPosition = new Vector2(posX, posY);
                    rect.sizeDelta = new Vector2(_slotWidth, _slotHeight);
                }
            }
        }

    }
}
