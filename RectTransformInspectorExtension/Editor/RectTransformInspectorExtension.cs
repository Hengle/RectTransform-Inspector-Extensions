﻿using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Hont
{
    [CustomEditor(typeof(RectTransform))]
    public class RectTransformInspectorExtension : Editor
    {
        Editor mOldEditor;

        float mCacheDeltaSizeRatio;

        bool mIsLockedScalingRatio;
        bool mIsBoundEditorUpdate;


        void OnDestroy()
        {
            EditorApplication.update -= onEditorApplicationUpdate;
        }

        public override void OnInspectorGUI()
        {
            var editorAssembly = Assembly.GetAssembly(typeof(Editor));
            var type = editorAssembly.GetType("UnityEditor.RectTransformEditor");

            if (mOldEditor == null)
                mOldEditor = CreateEditor(target, type);

            mOldEditor.OnInspectorGUI();

            var lineStyle = new GUIStyle();
            lineStyle.normal.background = EditorGUIUtility.whiteTexture;
            lineStyle.stretchWidth = true;
            lineStyle.margin = new RectOffset(0, 0, 7, 7);

            var c = GUI.color;
            var p = GUILayoutUtility.GetRect(GUIContent.none, lineStyle, GUILayout.Height(1));
            p.width -= 70;
            if (Event.current.type == EventType.Repaint)
            {
                GUI.color = EditorGUIUtility.isProSkin ?
                    new Color(0.157f, 0.157f, 0.157f) : new Color(0.5f, 0.5f, 0.5f);
                lineStyle.Draw(p, false, false, false, false);
            }

            EditorGUI.LabelField(new Rect(p.xMax, p.y - 7, 70, 20), "Extensions");
            GUI.color = c;

            var concertTarget = target as RectTransform;

            EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
            EditorGUILayout.LabelField("Reset");
            if (GUILayout.Button("P"))
            {
                Undo.RecordObject(concertTarget.transform, GetType().FullName);

                concertTarget.localPosition = Vector3.zero;
            }
            if (GUILayout.Button("D"))
            {
                Undo.RecordObject(concertTarget.transform, GetType().FullName);

                concertTarget.sizeDelta = Vector2.zero;
            }
            if (GUILayout.Button("R"))
            {
                Undo.RecordObject(concertTarget.transform, GetType().FullName);

                concertTarget.localRotation = Quaternion.identity;
            }
            if (GUILayout.Button("S"))
            {
                Undo.RecordObject(concertTarget.transform, GetType().FullName);

                concertTarget.localScale = Vector3.one;
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
            EditorGUILayout.LabelField("Locked Scaling Ratio");

            var rect = EditorGUILayout.GetControlRect(false);
            var xMax = rect.xMax;
            rect.xMin = xMax - Mathf.Min(149, rect.width);
            mIsLockedScalingRatio = GUI.Toggle(rect, mIsLockedScalingRatio, "Locked", EditorStyles.miniButton);

            if (mIsLockedScalingRatio)
            {
                if (!mIsBoundEditorUpdate)
                {
                    EditorApplication.update -= onEditorApplicationUpdate;
                    EditorApplication.update += onEditorApplicationUpdate;

                    mCacheDeltaSizeRatio = (concertTarget.sizeDelta.y / concertTarget.sizeDelta.x + 0.00001f);
                }
            }
            else
            {
                if (mIsBoundEditorUpdate)
                {
                    EditorApplication.update -= onEditorApplicationUpdate;
                    mIsBoundEditorUpdate = false;
                }
            }

            EditorGUILayout.EndHorizontal();
        }

        void onEditorApplicationUpdate()
        {
            mIsBoundEditorUpdate = true;

            var concertTarget = target as RectTransform;
            var y = concertTarget.sizeDelta.x * mCacheDeltaSizeRatio;
            concertTarget.sizeDelta = new Vector2(concertTarget.sizeDelta.x, y);
        }
    }
}
