using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class TestWindow : EditorWindow
{
    [MenuItem("RealityProgrammer/Test/Test Window")]
    static void Initialize() {
        var wnd = GetWindow<TestWindow>();
    }

    bool initiate = false;
    float subWndWidth;

    private void OnEnable() {
        wantsMouseMove = true;
    }

    static class InternalGUIStyle {
        public static GUIStyle DefaultButton;

        static InternalGUIStyle() {
            DefaultButton = new GUIStyle();
        }
    }

    private void OnGUI() {
        if (!initiate) {
            initiate = true;
            subWndWidth = position.width * 0.2f;
        }

        DoSubWindow();
    }

    const float resizeWindowThreshold = 4;
    void DoSubWindow() {
        var wndRect = new Rect(0, 0, subWndWidth, position.height);

        GUI.Box(wndRect, "", GUI.skin.window);

        var evt = Event.current;
        int id = GUIUtility.GetControlID(FocusType.Passive);

        if (GUIUtility.hotControl != id) {
            EditorGUIUtility.AddCursorRect(new Rect(subWndWidth - resizeWindowThreshold, 0, resizeWindowThreshold * 2, position.height), MouseCursor.ResizeHorizontal);
        } else {
            EditorGUIUtility.AddCursorRect(position, MouseCursor.ResizeHorizontal);
        }

        switch (evt.GetTypeForControl(id)) {
            case EventType.MouseDown:
                if (evt.button == 0 && GUIUtility.hotControl == 0) {
                    if (Mathf.Abs(evt.mousePosition.x - subWndWidth) <= resizeWindowThreshold) {
                        GUIUtility.hotControl = id;
                        evt.Use();
                    }
                }
                break;

            case EventType.MouseDrag:
                if (GUIUtility.hotControl == id) {
                    subWndWidth = Mathf.Clamp(evt.mousePosition.x, position.width * 0.1f, position.width * 0.25f);
                    Repaint();

                    evt.Use();
                }
                break;

            case EventType.MouseUp:
                if (evt.button == 0 && GUIUtility.hotControl != 0) {
                    GUIUtility.hotControl = 0;
                    evt.Use();
                }
                break;
        }

        GUILayout.BeginArea(wndRect);

        if (GUI.Button(EditorGUILayout.GetControlRect(), "E")) {

        }

        GUILayout.EndArea();
    }
}