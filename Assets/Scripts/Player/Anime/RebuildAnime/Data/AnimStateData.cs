using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NoWireAnim
{
    [CreateAssetMenu(menuName = "NoWireAnim/AnimState")]
    public class AnimStateData:ScriptableObject
    {
        public string stateId;
        public AnimationClip clip;
        public float speed = 1.0f;
        public AnimTransitionData[] transitions;
    }
}
