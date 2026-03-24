using UnityEngine;

namespace NoWireAnim
{
    [RequireComponent(typeof(PlayableAnimPlayer))]
    public sealed class AnimStateMachineRunner : MonoBehaviour
    {
        [SerializeField] private AnimStateMachineData stateMachineData;

        private PlayableAnimPlayer player;
        private AnimBlackboard blackboard;
        private AnimStateData currentState;

        public AnimBlackboard Blackboard => blackboard;
        public string CurrentStateId => currentState != null ? currentState.stateId : string.Empty;

        private void Awake()
        {
            player = GetComponent<PlayableAnimPlayer>();
            blackboard = new AnimBlackboard();
        }

        private void Start()
        {
            if (stateMachineData == null)
            {
                Debug.LogError("StateMachineData is null.", this);
                enabled = false;
                return;
            }

            var entryState = stateMachineData.GetState(stateMachineData.entryStateId);
            if (entryState == null)
            {
                Debug.LogError($"Entry state not found: {stateMachineData.entryStateId}", this);
                enabled = false;
                return;
            }

            EnterStateImmediate(entryState);
        }

        private void Update()
        {
            EvaluateTransitions();
        }

        public void SetFloat(string key, float value)
        {
            blackboard.SetFloat(key, value);
        }

        public float GetFloat(string key)
        {
            return blackboard.GetFloat(key);
        }

        private void EvaluateTransitions()
        {
            if (currentState == null || currentState.transitions == null)
                return;

            for (int i = 0; i < currentState.transitions.Length; i++)
            {
                var transition = currentState.transitions[i];
                if (transition == null)
                    continue;

                if (!CheckTransition(transition))
                    continue;

                var nextState = stateMachineData.GetState(transition.toStateId);
                if (nextState == null)
                {
                    Debug.LogWarning($"Target state not found: {transition.toStateId}", this);
                    return;
                }

                if (nextState == currentState)
                    return;

                EnterStateCrossFade(nextState, transition.fadeDuration);
                return;
            }
        }

        private bool CheckTransition(AnimTransitionData transition)
        {
            if (transition.conditions == null || transition.conditions.Length == 0)
                return true;

            for (int i = 0; i < transition.conditions.Length; i++)
            {
                if (!CheckCondition(transition.conditions[i]))
                    return false;
            }

            return true;
        }

        private bool CheckCondition(AnimConditionData condition)
        {
            float actual = blackboard.GetFloat(condition.key);

            return condition.op switch
            {
                AnimCompareOp.Greater => actual > condition.value,
                AnimCompareOp.Less => actual < condition.value,
                AnimCompareOp.GreaterOrEqual => actual >= condition.value,
                AnimCompareOp.LessOrEqual => actual <= condition.value,
                AnimCompareOp.Equal => Mathf.Approximately(actual, condition.value),
                AnimCompareOp.NotEqual => !Mathf.Approximately(actual, condition.value),
                _ => false
            };
        }

        private void EnterStateImmediate(AnimStateData state)
        {
            currentState = state;
            player.PlayImmediate(state.clip, state.speed);
        }

        private void EnterStateCrossFade(AnimStateData state, float fadeDuration)
        {
            currentState = state;
            player.CrossFade(state.clip, fadeDuration, state.speed);
        }
    }
}