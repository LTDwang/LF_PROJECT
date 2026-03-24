using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NoWireAnim
{
    [CreateAssetMenu(fileName = "NoWireAnim/Machine")]
    public class AnimStateMachineData: ScriptableObject
    {
        public string entryStateId;
        public AnimStateData[] states;

        public AnimStateData GetState(string stateId)
        {
            if (states == null || string.IsNullOrEmpty(stateId))
            {
                return null;
            }
            for (int i = 0; i < states.Length; i++)
            {
                if (states[i] != null && states[i].stateId == stateId)
                {
                    return states[i];
                }
            }
            return null;
        }
    }
}
