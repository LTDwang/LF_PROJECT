using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;


    public class PlayerInteraction : MonoBehaviour
    {
        [Header("输入键")]
        public KeyCode interactKey = KeyCode.E;

        [Header("选择策略")]
        public float maxInteractDistance = 3f;

        [Header("可选：把 UI 文本的 SetText/SetString 之类方法拖进来")]
        public UnityEvent<string> OnPromptChanged;

        private readonly Dictionary<IInteractable, Vector3> _candidates = new();
        private IInteractable _current;
        private IInteractable _active;
        private Transform _player;

        [Header("长按阈值")]
        public float holdThreshold = 0.5f;

        private bool _pressing = true;
        private float _pressStartTime = -1f;
        void Awake() => _player = transform;

        public void AddCandidate(IInteractable it, Vector2 hintPos)
        {
            if (!_candidates.ContainsKey(it)) _candidates.Add(it, hintPos);
        }

        public void RemoveCandidate(IInteractable it)
        {
            if (_candidates.ContainsKey(it)) _candidates.Remove(it);
            if (_current == it) _current = null;
            if (_active == it) _active = null;
        }

        void Update()
        {
            if (!_pressing)
                PickBest();

            if (_current != null && _current.CanInteract(_player))
                OnPromptChanged?.Invoke(_current.Prompt);
            else
                OnPromptChanged?.Invoke(string.Empty);

            if (_current != null && Input.GetKeyDown(interactKey))
            {
            _pressing = true;
            _pressStartTime = Time.time;
            _active = _current;              
            }
        if (_pressing&&Input.GetKey(interactKey))
        {
            if (_active!=null)
            {
                float time = Mathf.Max(0f, Time.time - _pressStartTime);
                float progress = Mathf.Clamp01(holdThreshold <= 0f ? 1f : time / holdThreshold);
            }
        }
        if (_pressing&&Input.GetKeyUp(interactKey))
        {
            _pressing = false;
            float held = Mathf.Max(0f, Time.time - _pressStartTime);
            _pressStartTime = -1f;

            // 目标消失/超距就不触发
            if (_active != null && _active.CanInteract(_player))
            {
                // 若交互物实现了短/长按接口，优先走该接口；否则退回到原始 Interact()
                if (_active is IShortLongInteractable sl)
                {
                    if (held >= holdThreshold) sl.LongInteract(_player);
                    else sl.ShortInteract(_player);
                }
                else
                {
                    // 保持向后兼容：没有实现可选接口时，仍调用你的原始单一 Interact() 行为
                    _active.Interact(_player);
                }
            }
        }
        }

        void PickBest()
        {
            float best = float.MaxValue;
            IInteractable bestIt = null;

            foreach (var kv in _candidates)
            {
                float d = Vector2.Distance(_player.position, kv.Value);
                if (d > maxInteractDistance) continue;
                if (d < best) { best = d; bestIt = kv.Key; }
            }
            _current = bestIt;
        }
    }

