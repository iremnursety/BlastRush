using System;
using UnityEngine;

namespace GridSystem
{
    public class TileChildAnimationController : MonoBehaviour
    {
        public AnimationCurve animationCurve;

        public void Animation()
        {
            var passed = 0f;
            var animationTime = 1f;
            var targetPos = Vector3.zero;
            while (passed < animationTime)
            {
                passed += Time.deltaTime;
                var normalized = passed / animationTime;
                var rate = animationCurve.Evaluate(normalized);
                Vector3.LerpUnclamped(transform.localPosition, targetPos, rate);
            }
            transform.localPosition = Vector3.zero;
        }   
    }
}
