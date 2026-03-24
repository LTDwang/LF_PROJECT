using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NoWireAnim
{
    public enum AnimCompareOp
    {
        Greater,
        Less,
        GreaterOrEqual,
        LessOrEqual,
        Equal,
        NotEqual,
    }
    [Serializable]
    public class AnimConditionData
    {
        public string key;
        public AnimCompareOp op;
        public float value;
    }
    [Serializable]
    public class AnimTransitionData
    {
        public string toStateId;
        public float fadeDuration = 0.15f;
        public AnimConditionData[] conditions;
    }
}
