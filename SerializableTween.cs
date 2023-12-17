using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace SerializableTween
{
    public enum AnimationTargetType
    {
        None = 0,
        Transform = 1 << 0,
        Image = 1 << 1,
        CanvasGroup = 1 << 2
        //Transform = 1 << 3,
        //Transform = 1 << 4,
    };

    public enum AnimationType
    {
        Move,
        Scale,
        Fade,
        Rotate
    };

    [System.Serializable]
    public class SerializableTweenAction
    {
        public float injectionTime = default;
        public UnityEvent unityEvent;
    }
    
    
    [System.Serializable]
    public class SerializableTween
    {
        public Behaviour target;
        public AnimationType animationType;
        public AnimationTargetType animationTargetType;
        [Space]
        public float injectionTime = default;
        public float duration = default;
        public AnimationCurve easingCurve = default;
        //Fade values
        public float finalFadeValue = 0f;
        //Transform values
        public Vector3 finalVector3;
        public bool localValues;

        public Tween GetTweenAnimation()
        {
            switch (animationType)
            {
                case AnimationType.Fade: return GetFadeAnimation();
                case AnimationType.Move: return GetMoveAnimation();
                case AnimationType.Scale: return GetScaleAnimation();
                case AnimationType.Rotate: return GetRotateAnimation();
            }
            return null;
        }

        Tween GetFadeAnimation()
        {
            if (animationTargetType == AnimationTargetType.Image)
            {
                var t = GetImage(target);
                return t.DOFade(finalFadeValue, duration).SetEase(easingCurve);
            }
            else
            {
                var t = GetCanvasGroup(target);
                return t.DOFade(finalFadeValue, duration).SetEase(easingCurve);
            }
        }
        Tween GetMoveAnimation()
        {
            var t = GetTransform(target);
            if (localValues)
            {
                return t.DOLocalMove(finalVector3, duration).SetEase(easingCurve);
            }
            else
            {
                return t.DOMove(finalVector3, duration).SetEase(easingCurve);
            }
        }
        Tween GetScaleAnimation()
        {
            var t = GetTransform(target);
            return t.DOScale(finalVector3, duration).SetEase(easingCurve);
        }
        Tween GetRotateAnimation()
        {
            var t = GetTransform(target);
            if (localValues)
            {
                return t.DOLocalRotate(finalVector3, duration).SetEase(easingCurve);
            }
            else
            {
                return t.DORotate(finalVector3, duration).SetEase(easingCurve);
            }
        }

        static TJ CastTo<TJ>(Behaviour obj) where TJ: Behaviour
        {
            return obj as TJ;
        }
        static Image GetImage(Behaviour target)
        {
            return CastTo<Image>(target);
        }
        static CanvasGroup GetCanvasGroup(Behaviour target)
        {
            return CastTo<CanvasGroup>(target);
        }
        static Transform GetTransform(Behaviour target)
        {
            return target.transform;
        }
    }

    [System.Serializable]
    public class SerializableInitialValues
    {
        public Behaviour target;
        public float initialFadeValue = 0f;
        public Vector3 initialPosition = Vector3.zero;
        public Vector3 initialScale = Vector3.one;
        public Vector3 initialRotation = Vector3.zero;
        public bool localValues = true;
        public AnimationTargetType animationTargetType;

        public bool doFade = false;
        public bool doPos = false;
        public bool doScale = false;
        public bool doRotate = false;

        public void SetInitialValues()
        {
            if (doFade)
            {
                if (animationTargetType == AnimationTargetType.Image)
                {
                    var img = GetImage(target);
                    img.color = new Color(img.color.r, img.color.g, img.color.b, initialFadeValue);
                }
                if (animationTargetType == AnimationTargetType.CanvasGroup)
                {
                    var canvas = GetCanvasGroup(target);
                    canvas.alpha = initialFadeValue;
                }
            }

            var trns = GetTransform(target);
            if (doPos)
            {
                if (localValues)
                {
                    trns.localPosition = initialPosition;
                }
                else
                {
                    trns.position = initialPosition;
                }
            }
            if (doRotate)
            {
                if (localValues)
                {
                    trns.localRotation = Quaternion.Euler(initialRotation);
                }
                else
                {
                    trns.rotation = Quaternion.Euler(initialRotation);
                }
            }
            if(doScale)
                trns.localScale = initialScale;
        }

        static TJ CastTo<TJ>(Behaviour obj) where TJ: Behaviour
        {
            return obj as TJ;
        }
        static Image GetImage(Behaviour target)
        {
            return CastTo<Image>(target);
        }
        static CanvasGroup GetCanvasGroup(Behaviour target)
        {
            return CastTo<CanvasGroup>(target);
        }
        static Transform GetTransform(Behaviour target)
        {
            return target.transform;
        }
    }
}

