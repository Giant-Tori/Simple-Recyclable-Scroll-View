using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEngine;
using UnityEngine.UI;

namespace Tori.UI
{
    [CustomEditor(typeof(OptimizedScrollRect), true)]
    [CanEditMultipleObjects]
    public class OptimizedScrollRectEditor : Editor
    {
        SerializedProperty m_Content;
        SerializedProperty m_MovementType;
        SerializedProperty m_Elasticity;
        SerializedProperty m_Inertia;
        SerializedProperty m_DecelerationRate;
        SerializedProperty m_ScrollSensitivity;
        SerializedProperty m_Viewport;
        SerializedProperty m_OnValueChanged;
        SerializedProperty m_Horizontal;
        SerializedProperty m_Vertical;

        //inherited
        SerializedProperty _slotPrefab;
        SerializedProperty _verticalPadding;
        SerializedProperty _horizontalPadding;
        SerializedProperty _gridCount;
        

        AnimBool m_ShowElasticity;
        AnimBool m_ShowDecelerationRate;

        OptimizedScrollRect _script;
        private bool _isHorizontal = false;

        protected virtual void OnEnable()
        {
            _script = (OptimizedScrollRect)target;
            m_Content = serializedObject.FindProperty("m_Content");
            m_Horizontal = serializedObject.FindProperty("m_Horizontal");
            m_Vertical = serializedObject.FindProperty("m_Vertical");
            m_MovementType = serializedObject.FindProperty("m_MovementType");
            m_Elasticity = serializedObject.FindProperty("m_Elasticity");
            m_Inertia = serializedObject.FindProperty("m_Inertia");
            m_DecelerationRate = serializedObject.FindProperty("m_DecelerationRate");
            m_ScrollSensitivity = serializedObject.FindProperty("m_ScrollSensitivity");
            m_Viewport = serializedObject.FindProperty("m_Viewport");
            m_OnValueChanged = serializedObject.FindProperty("m_OnValueChanged");

            //Inherited
            _slotPrefab = serializedObject.FindProperty("_slotPrefab");
            _verticalPadding = serializedObject.FindProperty("_verticalPadding");
            _horizontalPadding = serializedObject.FindProperty("_horizontalPadding");
            _gridCount = serializedObject.FindProperty("_gridCount");

            m_ShowElasticity = new AnimBool(Repaint);
            m_ShowDecelerationRate = new AnimBool(Repaint);
            SetAnimBools(true);

            if(m_Horizontal.boolValue)
            {
                _isHorizontal = true;
            }
            else
            {
                _isHorizontal = false;
            }
        }

        protected virtual void OnDisable()
        {
            m_ShowElasticity.valueChanged.RemoveListener(Repaint);
            m_ShowDecelerationRate.valueChanged.RemoveListener(Repaint);
        }

        void SetAnimBools(bool instant)
        {
            SetAnimBool(m_ShowElasticity, !m_MovementType.hasMultipleDifferentValues && m_MovementType.enumValueIndex == (int)ScrollRect.MovementType.Elastic, instant);
            SetAnimBool(m_ShowDecelerationRate, !m_Inertia.hasMultipleDifferentValues && m_Inertia.boolValue, instant);
        }

        void SetAnimBool(AnimBool a, bool value, bool instant)
        {
            if (instant)
                a.value = value;
            else
                a.target = value;
        }

        public override void OnInspectorGUI()
        {
            SetAnimBools(false);
            serializedObject.Update();

            if (_isHorizontal)
            {
                m_Horizontal.boolValue = true;
                m_Vertical.boolValue = false;
            }
            else
            {
                m_Horizontal.boolValue = false;
                m_Vertical.boolValue = true;
            }

            EditorGUILayout.PropertyField(m_Viewport);
            EditorGUILayout.PropertyField(m_Content);
            EditorGUILayout.PropertyField(m_Horizontal);
            EditorGUILayout.PropertyField(m_Vertical);
            EditorGUILayout.PropertyField(_slotPrefab);
            EditorGUILayout.PropertyField(_verticalPadding);
            EditorGUILayout.PropertyField(_horizontalPadding);
            EditorGUILayout.PropertyField(_gridCount);
            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(m_MovementType);
            if (EditorGUILayout.BeginFadeGroup(m_ShowElasticity.faded))
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(m_Elasticity);
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndFadeGroup();

            EditorGUILayout.PropertyField(m_Inertia);
            if (EditorGUILayout.BeginFadeGroup(m_ShowDecelerationRate.faded))
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(m_DecelerationRate);
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndFadeGroup();

            EditorGUILayout.PropertyField(m_ScrollSensitivity);

            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(m_OnValueChanged);

            if(m_Horizontal.boolValue != _isHorizontal)
            {
                m_Vertical.boolValue = !m_Horizontal.boolValue;
                _isHorizontal = m_Horizontal.boolValue;
            }
            else if(m_Vertical.boolValue != !_isHorizontal)
            {
                m_Horizontal.boolValue = !m_Vertical.boolValue;
                _isHorizontal = !m_Vertical.boolValue;
            }


            serializedObject.ApplyModifiedProperties();
        }
    }
}
