using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NoWireAnim
{
    public class AnimBlackboard
    {
        private readonly Dictionary<string,float> _floats = new Dictionary<string,float>();
        public void SetFloat(string key, float value)
        {
            _floats[key] = value;
        }
        public float GetFloat(string key)
        {
            {
                return _floats.TryGetValue(key, out float value) ? value : 0f;
            }
        }
    }
}