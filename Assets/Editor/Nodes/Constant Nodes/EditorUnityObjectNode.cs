using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEditor;

public class EditorUnityObjectNode : BaseEditorConstantNode
{
    public ObjectField Field { get; private set; }

    public EditorUnityObjectNode(UnityObjectNode node) : base() {
        UnderlyingRuntimeNode = node;
    }

    public override void Initialize() {
        title = "Unity Object Node";

        base.Initialize();
    }

    public override void InitializeFields(VisualElement contents) {
        var value = ((UnityObjectNode)UnderlyingRuntimeNode).Value;

        if (value != null) {
            Field = new ObjectField();
            Field.objectType = typeof(UnityEngine.Object);
            Field.allowSceneObjects = false;
            Field.SetValueWithoutNotify(value);

            Field.RegisterValueChangedCallback(ObjectFieldModifyCallback);

            Field.Q(className: "unity-object-field-display").style.maxWidth = new Length(175, LengthUnit.Pixel);

            contents.Add(Field);

            DeleteDetecting.Subscribe(this);
        } else {
            Field = new ObjectField();
            Field.objectType = typeof(UnityEngine.Object);
            Field.allowSceneObjects = false;

            Field.RegisterValueChangedCallback(ObjectFieldModifyCallback);

            Field.Q(className: "unity-object-field-display").style.maxWidth = new Length(175, LengthUnit.Pixel);

            contents.Add(Field);
        }
    }

    void ObjectFieldModifyCallback(ChangeEvent<Object> evt) {
        ((UnityObjectNode)UnderlyingRuntimeNode).Value = evt.newValue;
        DeleteDetecting.Change(this, evt);
    }

    private class DeleteDetecting : UnityEditor.AssetModificationProcessor {
        public static Dictionary<UnityEngine.Object, List<EditorUnityObjectNode>> SubscriptionList { get; private set; } = new Dictionary<Object, List<EditorUnityObjectNode>>();

        private static List<EditorUnityObjectNode> EnsureListNotNull(Object asset) {
            if (SubscriptionList.TryGetValue(asset, out var l)) {
                return l;
            } else {
                l = new List<EditorUnityObjectNode>(4);

                SubscriptionList.Add(asset, l);
                return l;
            }
        }

        public static void Subscribe(EditorUnityObjectNode node) {
            var asset = ((UnityObjectNode)node.UnderlyingRuntimeNode).Value;

            EnsureListNotNull(asset).Add(node);
        }

        public static void Change(EditorUnityObjectNode node, ChangeEvent<Object> change) {
            if (change.previousValue != null) {
                EnsureListNotNull(change.previousValue).Remove(node);
            }

            EnsureListNotNull(change.newValue).Add(node);
        }

        static AssetDeleteResult OnWillDeleteAsset(string path, RemoveAssetOptions opt) {
            var asset = AssetDatabase.LoadAssetAtPath<Object>(path);

            if (SubscriptionList.TryGetValue(asset, out var list)) {
                foreach (var node in list) {
                    node.Field.SetValueWithoutNotify(null);
                }
            }

            SubscriptionList.Remove(asset);

            return AssetDeleteResult.DidNotDelete;
        }
    }
}
