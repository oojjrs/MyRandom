#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Assets.Sources.Scripts.Editor
{
    public class EditorControl
    {
        public ClientBridgeInterface ClientBridge { get; }
        private Dictionary<object, bool> FoldoutCache { get; } = new();
        // 이름 컨벤션에 어긋나지만 일관성을 위해 맞추었다.
        private bool GUIenabled { get; set; }
        private Vector2 ScrollPosition { get; set; }
        private Dictionary<string, string> TextCache { get; } = new();
        private Dictionary<string, Texture2D> TextureCache { get; } = new();
        private Dictionary<object, bool> ToggleGroupCache { get; } = new();

        public EditorControl(ClientBridgeInterface clientBridge)
        {
            ClientBridge = clientBridge;
        }

        public void Button(string title, Action action)
        {
            if (GUILayout.Button(title))
                action?.Invoke();
        }

        public void DrawAll<T>(IEnumerable<T> values, Func<T, object> getName, Action<T> drawBody)
        {
            FoldoutBox("Entries", () =>
            {
                foreach (var value in values)
                {
                    FoldoutBox(getName?.Invoke(value)?.ToString() ?? value.ToString(), () =>
                    {
                        drawBody?.Invoke(value);
                    });
                }
            });
        }

        public bool Foldout(object key, string content)
        {
            if (FoldoutCache.ContainsKey(key) == false)
                FoldoutCache[key] = ClientBridge.GetDebugFlag("Foldout." + key);

            var open = EditorGUILayout.Foldout(FoldoutCache[key], content, true);
            if (open != FoldoutCache[key])
            {
                FoldoutCache[key] = open;
                ClientBridge.SetDebugFlag("Foldout." + key, open);
            }

            return open;
        }

        public void FoldoutBox(string content, Action action)
        {
            if (Foldout(content, content))
                VerticalBox(action);
        }

        public void FoldoutBox(object key, string content, Action action)
        {
            if (Foldout(key, content))
                VerticalBox(action);
        }

        public string GetText(string name)
        {
            TextCache.TryGetValue(name, out var text);
            return text;
        }

        public Texture2D GetTexture(string name)
        {
            TextureCache.TryGetValue(name, out var texture);
            return texture;
        }

        // 함수명이 왜 이러냐고 묻지 마라
        public void GUICall(Action action)
        {
            PushGUISettings();
            action?.Invoke();
            PopGUISettings();
        }

        public void Horizontal(Action action)
        {
            EditorGUILayout.BeginHorizontal();
            action?.Invoke();
            EditorGUILayout.EndHorizontal();
        }

        public void InfoBox(string message)
        {
            EditorGUILayout.HelpBox(message, MessageType.Info);
        }

        public void LabelField(string label)
        {
            EditorGUILayout.LabelField(label);
        }

        public void LabelField(string label, string content)
        {
            EditorGUILayout.LabelField(label, content);
        }

        // 이름 컨벤션에 어긋나지만 일관성을 위해 맞추었다.
        public void PopGUISettings()
        {
            GUI.enabled = GUIenabled;
        }

        // 이름 컨벤션에 어긋나지만 일관성을 위해 맞추었다.
        public void PushGUISettings()
        {
            GUIenabled = GUI.enabled;
        }

        public void ScrollView(Action action)
        {
            ScrollPosition = EditorGUILayout.BeginScrollView(ScrollPosition);
            action?.Invoke();
            EditorGUILayout.EndScrollView();
        }

        public void SelectEnum<T>(string name, string label, T defValue) where T : Enum
        {
            var value = ClientBridge.GetDebugEnum(name, defValue);
            var newValue = (T)EditorGUILayout.EnumPopup(label, value);
            if (newValue.Equals(value) == false)
                ClientBridge.SetDebugEnum(name, newValue);
        }

        public void TextField(string name)
        {
            TextCache.TryGetValue(name, out var text);

            var ret = EditorGUILayout.TextField(name, text);
            if (ret != text)
                TextCache[name] = ret;
        }

        public void TextFieldClear(string name)
        {
            TextCache.Remove(name);
        }

        public void TextureField(string name)
        {
            Vertical(() =>
            {
                LabelField(name);

                TextureCache.TryGetValue(name, out var texture);

                var ret = (Texture2D)EditorGUILayout.ObjectField(texture, typeof(Texture2D), false, GUILayout.Width(70), GUILayout.Height(70));
                if (ret != texture)
                    TextureCache[name] = ret;
            });
        }

        public void TextureFieldClear(string name)
        {
            TextureCache.Remove(name);
        }

        public void ToggleFlag(string name, string label)
        {
            var value = ClientBridge.GetDebugFlag(name);
            var newValue = EditorGUILayout.Toggle(label, value);
            if (newValue != value)
                ClientBridge.SetDebugFlag(name, newValue);
        }

        public void ToggleFlag(string name, string label, Action<bool> action)
        {
            var value = ClientBridge.GetDebugFlag(name);
            var newValue = EditorGUILayout.Toggle(label, value);
            if (newValue != value)
            {
                ClientBridge.SetDebugFlag(name, newValue);

                action?.Invoke(newValue);
            }
        }

        public void ToggleGroup(object key, string label, Action action)
        {
            if (ToggleGroupCache.ContainsKey(key) == false)
                ToggleGroupCache[key] = ClientBridge.GetDebugFlag("ToggleGroup." + key);

            var toggle = EditorGUILayout.BeginToggleGroup(label, ToggleGroupCache[key]);
            if (toggle != ToggleGroupCache[key])
            {
                ToggleGroupCache[key] = toggle;
                ClientBridge.SetDebugFlag("ToggleGroup." + key, toggle);
            }

            action?.Invoke();
            EditorGUILayout.EndToggleGroup();
        }

        public void Vertical(Action action)
        {
            EditorGUILayout.BeginVertical();
            action?.Invoke();
            EditorGUILayout.EndVertical();
        }

        public void VerticalBox(Action action)
        {
            EditorGUILayout.BeginVertical("Box");
            action?.Invoke();
            EditorGUILayout.EndVertical();
        }
    }
}
#endif
