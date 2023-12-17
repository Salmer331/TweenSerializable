using UnityEditor;
using UnityEngine;

namespace SerializableTween
{
    [CustomEditor(typeof(SerializedSequence), true)]
    public class SerializedSequenceEditor : Editor
    {
        //private SerializedSequence t;

        SerializedProperty spInitialValues;
        SerializedProperty spTweens;
        SerializedProperty spTweenActions;
        SerializedProperty spLoopsCount;
        SerializedProperty spSequenceCurve;

        GUIStyle horizontalLine;
        GUIStyle injectionFieldStyle;
        //GUIStyle boxStyle;

        private bool foldoutInspectorInitials = false;
        private bool foldoutInspectorTweens = false;
        private bool foldoutInspectorActions = false;
        private void OnEnable()
        {
            //t = (SerializedSequence) target;

            spTweens = serializedObject.FindProperty("tweens");
            spTweenActions = serializedObject.FindProperty("tweenActions");
            spInitialValues = serializedObject.FindProperty("initialValues");
            spLoopsCount = serializedObject.FindProperty("loopsCount");
            spSequenceCurve = serializedObject.FindProperty("sequenceCurve");

            horizontalLine = new GUIStyle
            {
                normal = { background = EditorGUIUtility.whiteTexture },
                margin = new RectOffset(0, 0, 4, 4),
                fixedHeight = 1
            };

        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            OnGUIInitialValues();
            EditorGUILayout.Space(5);
            OnGUITweens();
            EditorGUILayout.Space(5);
            OnGUITweenActions();
            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("Sequence Settings", EditorStyles.boldLabel);
            EditorGUILayout.Space(5);
            EditorGUI.indentLevel += 1;
            EditorGUILayout.PropertyField(spLoopsCount);
            EditorGUILayout.PropertyField(spSequenceCurve);
            EditorGUI.indentLevel -= 1;
            serializedObject.ApplyModifiedProperties();
        }

        void OnGUIInitialValues()
        {
            EditorGUILayout.LabelField("Initial Values:", EditorStyles.boldLabel);
            foldoutInspectorInitials = EditorGUILayout.BeginFoldoutHeaderGroup(foldoutInspectorInitials, $"Objects List: Count {spInitialValues.arraySize}");
            if (foldoutInspectorInitials)
            {
                Show<SerializableInitialValues>(spInitialValues, horizontalLine, GUI.skin.window);
                HorizontalLine(horizontalLine, Color.gray);
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Add"))
            {
                spInitialValues.InsertArrayElementAtIndex(spInitialValues.arraySize);
            }
            if (GUILayout.Button("Remove"))
            {
                if (spInitialValues.arraySize == 0) return;
                spInitialValues.DeleteArrayElementAtIndex(spInitialValues.arraySize-1);
                serializedObject.ApplyModifiedProperties();
                return;
            }
            EditorGUILayout.EndHorizontal();
        }
        void OnGUITweens()
        {
            EditorGUILayout.LabelField("Tweens:", EditorStyles.boldLabel);
            foldoutInspectorTweens = EditorGUILayout.BeginFoldoutHeaderGroup(foldoutInspectorTweens, $"Tweens List: Count {spTweens.arraySize}");
            if (foldoutInspectorTweens)
            {
                Show<SerializableTween>(spTweens, horizontalLine, GUI.skin.window);
                HorizontalLine(horizontalLine, Color.gray);
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Add"))
            {
                spTweens.InsertArrayElementAtIndex(spTweens.arraySize);
            }
            if (GUILayout.Button("Remove"))
            {
                if (spTweens.arraySize == 0) return;
                spTweens.DeleteArrayElementAtIndex(spTweens.arraySize-1);
                serializedObject.ApplyModifiedProperties();
                return;
            }
            EditorGUILayout.EndHorizontal();
        }
        void OnGUITweenActions()
        {
            EditorGUILayout.LabelField("Tween Actions:", EditorStyles.boldLabel);
            foldoutInspectorActions = EditorGUILayout.BeginFoldoutHeaderGroup(foldoutInspectorActions, $"Tween Actions List: Count {spTweenActions.arraySize}");
            if (foldoutInspectorActions)
            {
                Show<SerializableTweenAction>(spTweenActions, horizontalLine, GUI.skin.window);
                HorizontalLine(horizontalLine, Color.gray);
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Add"))
            {
                spTweenActions.InsertArrayElementAtIndex(spTweenActions.arraySize);
            }

            if (GUILayout.Button("Remove"))
            {
                if (spTweenActions.arraySize == 0) return;
                spTweenActions.DeleteArrayElementAtIndex(spTweenActions.arraySize-1);
                serializedObject.ApplyModifiedProperties();
                return;
            }
            EditorGUILayout.EndHorizontal();
        }


        static void Show <T>(SerializedProperty list, GUIStyle lineSeparator, GUIStyle boxStyle) {
            EditorGUI.indentLevel += 1;
            for (int i = 0; i < list.arraySize; i++)
            {
                HorizontalLine(lineSeparator, Color.gray);
                var item = list.GetArrayElementAtIndex(i);
                Rect rect = EditorGUILayout.BeginHorizontal();
                GUI.Box(rect, GUIContent.none, boxStyle);
                if (typeof(SerializableTween).IsAssignableFrom(typeof(T)))
                {
                    var injectTime = item.FindPropertyRelative("injectionTime");
                    injectTime.floatValue = EditorGUILayout.FloatField(injectTime.floatValue, GUILayout.Width(80));
                    ShowTweenSerialization(list.GetArrayElementAtIndex(i));
                }
                if (typeof(SerializableTweenAction).IsAssignableFrom(typeof(T)))
                {
                    var injectTime = item.FindPropertyRelative("injectionTime");
                    injectTime.floatValue = EditorGUILayout.FloatField(injectTime.floatValue, GUILayout.Width(80));
                    ShowActionSerialization(list.GetArrayElementAtIndex(i));
                }
                if (typeof(SerializableInitialValues).IsAssignableFrom(typeof(T)))
                {
                    ShowInitialValues(list.GetArrayElementAtIndex(i));
                }

                EditorGUILayout.EndHorizontal();

                if (GUILayout.Button("Remove"))
                {
                    list.DeleteArrayElementAtIndex(i);
                }
            }
            EditorGUI.indentLevel -= 1;
        }
        static void HorizontalLine(GUIStyle horizontalLine, Color color) {
            var c = GUI.color;
            GUI.color = color;
            GUILayout.Box(GUIContent.none, horizontalLine );
            GUI.color = c;
        }

        #region Action GUI
        static void ShowActionSerialization(SerializedProperty prop)
        {
            var spAction = prop.FindPropertyRelative("unityEvent");
            EditorGUILayout.BeginVertical();
            EditorGUILayout.PropertyField(spAction);
            EditorGUILayout.EndVertical();
        }
        #endregion

        #region Initial Values
        static void ShowInitialValues(SerializedProperty prop)
        {
            var spTarget = prop.FindPropertyRelative("target");
            var spTargetType = prop.FindPropertyRelative("animationTargetType");

            var spFadeInitial = prop.FindPropertyRelative("initialFadeValue");
            var spVectorTransform = prop.FindPropertyRelative("initialPosition");
            var spVectorScale = prop.FindPropertyRelative("initialScale");
            var spVectorRotate = prop.FindPropertyRelative("initialRotation");

            var spLocalVal = prop.FindPropertyRelative("localValues");

            var spDoFade = prop.FindPropertyRelative("doFade");
            var spDoPos = prop.FindPropertyRelative("doPos");
            var spDoScale = prop.FindPropertyRelative("doScale");
            var spDoRotate = prop.FindPropertyRelative("doRotate");

            EditorGUILayout.BeginVertical();
            EditorGUILayout.PropertyField(spTarget);
            EditorGUILayout.Space(10);

            if(spTarget.objectReferenceValue == null) {
                EditorGUILayout.EndVertical();
                return;
            }

            var _canFade = false;
            if (spTarget.objectReferenceValue.GetType().IsAssignableFrom(typeof(UnityEngine.UI.Image)))
            {
                spTargetType.intValue = (int) AnimationTargetType.Image;
                _canFade = true;
            }
            if (spTarget.objectReferenceValue.GetType().IsAssignableFrom(typeof(CanvasGroup)))
            {
                spTargetType.intValue = (int) AnimationTargetType.CanvasGroup;
                _canFade = true;
            }

            if (_canFade)
            {
                EditorGUILayout.PropertyField(spDoFade);
                if (spDoFade.boolValue)
                {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(spFadeInitial);
                    EditorGUI.indentLevel--;
                }
            }
            else spDoFade.boolValue = false;

            EditorGUILayout.Space(10);
            EditorGUILayout.PropertyField(spLocalVal);
            EditorGUILayout.Space(5);
            EditorGUILayout.PropertyField(spDoPos);
            if (spDoPos.boolValue)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(spVectorTransform);
                EditorGUI.indentLevel--;
            }

            EditorGUILayout.PropertyField(spDoScale);
            if (spDoScale.boolValue)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(spVectorScale);
                EditorGUI.indentLevel--;
            }

            EditorGUILayout.PropertyField(spDoRotate);
            if (spDoRotate.boolValue)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(spVectorRotate);
                EditorGUI.indentLevel--;
            }

            EditorGUILayout.EndVertical();
        }
        #endregion

        #region Tween GUI
        static void ShowTweenSerialization(SerializedProperty prop)
        {
            var spTarget = prop.FindPropertyRelative("target");
            var spTargetAnimation = prop.FindPropertyRelative("animationType");
            var spTargetType = prop.FindPropertyRelative("animationTargetType");

            var spInjectionTime = prop.FindPropertyRelative("injectionTime");
            var spAnimationCurve = prop.FindPropertyRelative("easingCurve");
            var spDuration = prop.FindPropertyRelative("duration");
            //Image or Canvas
            var spFadeFinal = prop.FindPropertyRelative("finalFadeValue");
            //Transform
            var spVectorFinal = prop.FindPropertyRelative("finalVector3");
            var spLocalVal = prop.FindPropertyRelative("localValues");

            EditorGUILayout.BeginVertical();
            EditorGUILayout.PropertyField(spTarget);
            if (spTarget.objectReferenceValue != null)
            {
                EditorGUILayout.PropertyField(spTargetAnimation);
                EditorGUILayout.Space(5);
                EditorGUILayout.PropertyField(spDuration);
                EditorGUILayout.PropertyField(spAnimationCurve);
                EditorGUILayout.Space(10);

                if (spTargetAnimation.intValue == (int) AnimationType.Move)
                {
                    spTargetType.intValue = (int) AnimationTargetType.Transform;
                    EditorGUILayout.PropertyField(spLocalVal);
                    EditorGUILayout.PropertyField(spVectorFinal);
                }
                else if (spTargetAnimation.intValue == (int) AnimationType.Scale)
                {
                    spTargetType.intValue = (int) AnimationTargetType.Transform;
                    EditorGUILayout.PropertyField(spVectorFinal);
                }
                else if (spTargetAnimation.intValue == (int) AnimationType.Rotate)
                {
                    spTargetType.intValue = (int) AnimationTargetType.Transform;
                    EditorGUILayout.PropertyField(spLocalVal);
                    EditorGUILayout.PropertyField(spVectorFinal);
                }
                else if
                (
                    !spTarget.objectReferenceValue.GetType().IsAssignableFrom(typeof(UnityEngine.UI.Image)) &&
                    !spTarget.objectReferenceValue.GetType().IsAssignableFrom(typeof(CanvasGroup)) &&
                    spTargetAnimation.intValue == (int) AnimationType.Fade)
                {
                    spTargetAnimation.intValue = (int) AnimationType.Move;
                }
                else if (spTarget.objectReferenceValue.GetType().IsAssignableFrom(typeof(UnityEngine.UI.Image)))
                {
                    spTargetType.intValue = (int) AnimationTargetType.Image;
                    EditorGUILayout.PropertyField(spFadeFinal);
                }
                else if (spTarget.objectReferenceValue.GetType().IsAssignableFrom(typeof(CanvasGroup)))
                {
                    spTargetType.intValue = (int) AnimationTargetType.CanvasGroup;
                    EditorGUILayout.PropertyField(spFadeFinal);
                }
            }
            EditorGUILayout.EndVertical();
        }
        static System.Type GetPropertyType(SerializedProperty property)
        {
            try
            {
                System.Type parentType = property.serializedObject.targetObject.GetType();
                var fiTarget = parentType.GetField("target");
                return fiTarget.GetValue(property.serializedObject.targetObject).GetType();
            }
            catch
            {
                return null;
            }
        }
        #endregion
    }
}
