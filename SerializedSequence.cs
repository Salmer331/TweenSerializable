using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace SerializableTween
{
    public class SerializedSequence : MonoBehaviour//; where T: MonoBehaviour
    {
        public List<SerializableInitialValues> initialValues = default;
        public List<SerializableTween> tweens = default;
        public List<SerializableTweenAction> tweenActions = default;

        public int loopsCount = 0;
        public AnimationCurve sequenceCurve = default;

        private Sequence innerSequence = default;

        public void SetInitialValues()
        {
            foreach (var values in initialValues)
            {
                values.SetInitialValues();
            }
        }
        public Sequence GetAnimation()
        {
            StopAnimation();
            //SetInitialValues();

            var s = DOTween.Sequence();
            foreach (var tween in tweens)
            {
                s.Insert(tween.injectionTime, tween.GetTweenAnimation());
            }

            foreach (var tweenAction in tweenActions)
            {
                s.InsertCallback(tweenAction.injectionTime, tweenAction.unityEvent.Invoke);
            }

            s.SetEase(sequenceCurve);
            s.SetLoops(loopsCount);

            innerSequence = s;
            return innerSequence;
        }
        public void StopAnimation()
        {
            innerSequence?.Kill(false);
        }
    }
}
