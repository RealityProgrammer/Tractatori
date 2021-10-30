using System;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

public class TractatoriToolPanelElement : VisualElement {
    private const int MainTabButton_Property = 0;
    private const int MainTabButton_Schematic = 1;

    private const int PropertyTabButton_Object = 0;
    private const int PropertyTabButton_Vector = 1;
    private const int PropertyTabButton_Boolean = 2;

    private StyleSheet myStyleSheet;
    private float resizeBorderThreshold = 5;

    private VisualElement resizer;

    private TabElement mainTabElement;

    public TractatoriToolPanelElement(VisualElement root) : base() {
        AddToClassList("tractatori-tool-panel");

        myStyleSheet = Resources.Load<StyleSheet>("ToolPanel");
        styleSheets.Add(myStyleSheet);

        RegisterCallback<CustomStyleResolvedEvent>(HandleCustomStyleResolved);

        SetupMainTabButtons();
    }

    void SetupMainTabButtons() {
        mainTabElement = new TabElement();
        mainTabElement.TabButtonContainer.style.height = 30;

        mainTabElement.RegisterTabButton(MainTabButton_Property, "main-tab-button-property", "Properties");
        mainTabElement.RegisterTabButton(MainTabButton_Schematic, "main-tab-button-schematic", "Schematic");

        SetupPropertyTabs();

        var container2 = mainTabElement.GetTabContentContainer(MainTabButton_Schematic);

        container2.Add(new Label("Schematic are currently being under developed"));

        Add(mainTabElement);
    }

    void SetupPropertyTabs() {
        var currentEditing = TractatoriGraphEditorWindow.CurrentEditingAsset;

        var propertyTab = new TabElement() {
            name = "property-tab",
        };
        propertyTab.TabButtonContainer.style.height = 21;

        var tab1 = propertyTab.RegisterTabButton(PropertyTabButton_Object, "property-tab-button-object", "Objects");
        tab1.Container.Add(new ScrollView(ScrollViewMode.Vertical));

        var tab2 = propertyTab.RegisterTabButton(PropertyTabButton_Vector, "property-tab-button-vector", "Vectors");
        tab2.Container.Add(new ScrollView(ScrollViewMode.Vertical));

        var tab3 = propertyTab.RegisterTabButton(PropertyTabButton_Boolean, "property-tab-button-boolean", "Booleans");
        tab3.Container.Add(new ScrollView(ScrollViewMode.Vertical));

        mainTabElement.GetTabContentContainer(MainTabButton_Property).Add(propertyTab);
    }

    public void InitializeAllBindablePropertyFields() {
        var currentEditing = TractatoriGraphEditorWindow.CurrentEditingAsset;

        var propertyTab = mainTabElement.GetTabContainer(MainTabButton_Property).Container.Q<TabElement>("property-tab");

        var objectContainer = propertyTab.GetTabContentContainer(PropertyTabButton_Object).Q<ScrollView>(className: "unity-scroll-view").contentContainer;
        foreach (var property in currentEditing.ObjectBindableProperties) {
            objectContainer.Add(CreateObjectPropertyElement(property));
        }

        var vectorContainer = propertyTab.GetTabContentContainer(PropertyTabButton_Vector).Q<ScrollView>(className: "unity-scroll-view").contentContainer;
        foreach (var property in currentEditing.VectorBindableProperties) {
            vectorContainer.Add(CreateVectorPropertyElement(property));
        }

        var booleanContainer = propertyTab.GetTabContentContainer(PropertyTabButton_Boolean).Q<ScrollView>(className: "unity-scroll-view").contentContainer;
        foreach (var property in currentEditing.BooleanBindableProperties) {
            booleanContainer.Add(CreateBooleanPropertyElement(property));
        }
    }

    VisualElement CreateObjectPropertyElement(ObjectBindableProperty property) {
        var drawer = BindablePropertyDrawerCollector.CreateDrawer(property);
        drawer.Initialize();

        var actionButtonContainer = new VisualElement() {
            name = "action-button-container",
        };
        actionButtonContainer.style.flexDirection = FlexDirection.RowReverse;

        var addToGraphButton = new Button();
        addToGraphButton.Add(new Image() {
            image = Resources.Load<Texture2D>("Icons/AddPropertyToGraphIcon"),
        });
        addToGraphButton.clicked += () => {
            RequestCreateBindableNode<ObjectBindableProperty, ObjectBindablePropertyNode>(property);
        };

        actionButtonContainer.Add(addToGraphButton);

        var deletePropertyIcon = new Button();
        deletePropertyIcon.Add(new Image() {
            image = Resources.Load<Texture2D>("Icons/DeletePropertyIcon"),
        });
        deletePropertyIcon.clicked += () => {
            drawer.RemoveFromHierarchy();
            TractatoriGraphEditorWindow.CurrentEditingAsset.ObjectBindableProperties.Remove(property);
        };
        actionButtonContainer.Add(deletePropertyIcon);

        drawer.style.borderBottomWidth = 1;
        drawer.style.borderBottomColor = Color.gray;
        actionButtonContainer.style.marginBottom = 5;
        drawer.Add(actionButtonContainer);

        return drawer;
    }
    VisualElement CreateVectorPropertyElement(MVectorBindableProperty property) {
        var drawer = BindablePropertyDrawerCollector.CreateDrawer(property);
        drawer.Initialize();

        var actionButtonContainer = new VisualElement() {
            name = "action-button-container",
        };
        actionButtonContainer.style.flexDirection = FlexDirection.RowReverse;

        var addToGraphButton = new Button();
        addToGraphButton.Add(new Image() {
            image = Resources.Load<Texture2D>("Icons/AddPropertyToGraphIcon"),
        });
        addToGraphButton.clicked += () => {
            RequestCreateBindableNode<MVectorBindableProperty, VectorBindablePropertyNode>(property);
        };

        actionButtonContainer.Add(addToGraphButton);

        var deletePropertyIcon = new Button();
        deletePropertyIcon.Add(new Image() {
            image = Resources.Load<Texture2D>("Icons/DeletePropertyIcon"),
        });
        deletePropertyIcon.clicked += () => {
            drawer.RemoveFromHierarchy();
            TractatoriGraphEditorWindow.CurrentEditingAsset.VectorBindableProperties.Remove(property);
        };
        actionButtonContainer.Add(deletePropertyIcon);

        actionButtonContainer.style.marginBottom = 5;
        drawer.Add(actionButtonContainer);

        drawer.style.borderBottomWidth = 1;
        drawer.style.borderBottomColor = Color.gray;

        return drawer;
    }
    VisualElement CreateBooleanPropertyElement(BooleanBindableProperty property) {
        var drawer = BindablePropertyDrawerCollector.CreateDrawer(property);
        drawer.Initialize();

        var actionButtonContainer = new VisualElement() {
            name = "action-button-container",
        };
        actionButtonContainer.style.flexDirection = FlexDirection.RowReverse;

        var addToGraphButton = new Button();
        addToGraphButton.Add(new Image() {
            image = Resources.Load<Texture2D>("Icons/AddPropertyToGraphIcon"),
        });
        addToGraphButton.clicked += () => {
            var wndPos = TractatoriGraphEditorWindow.WindowInstance.position;

            var req = new ConstantNodeCreationRequest();
            BaseBindablePropertyNode nodeInstance = ScriptableObject.CreateInstance<Boolean4BindablePropertyNode>();

            nodeInstance.Name = property.Name;
            req.ExistInstance = nodeInstance;

            req.Position = TractatoriGraphEditorWindow.WindowInstance.GraphView.viewTransform.matrix.inverse.MultiplyPoint(new Vector2(wndPos.x + wndPos.width / 2, wndPos.y + wndPos.height / 2));

            TractatoriGraphEditorWindow.WindowInstance.GraphView.NodeEmitter.EnqueueRequest(req);
        };

        actionButtonContainer.Add(addToGraphButton);

        var deletePropertyIcon = new Button();
        deletePropertyIcon.Add(new Image() {
            image = Resources.Load<Texture2D>("Icons/DeletePropertyIcon"),
        });
        deletePropertyIcon.clicked += () => {
            drawer.RemoveFromHierarchy();
            TractatoriGraphEditorWindow.CurrentEditingAsset.BooleanBindableProperties.Remove(property);
        };
        actionButtonContainer.Add(deletePropertyIcon);

        actionButtonContainer.style.marginBottom = 5;
        drawer.Add(actionButtonContainer);

        drawer.style.borderBottomWidth = 1;
        drawer.style.borderBottomColor = Color.gray;

        return drawer;
    }

    void RequestCreateBindableNode<TProperty, TNode>(TProperty property) where TProperty : BaseBindableProperty where TNode : BaseBindablePropertyNode {
        var wndPos = TractatoriGraphEditorWindow.WindowInstance.position;

        var req = new ConstantNodeCreationRequest();
        BaseBindablePropertyNode nodeInstance = ScriptableObject.CreateInstance<TNode>();

        nodeInstance.Name = property.Name;
        req.ExistInstance = nodeInstance;

        req.Position = TractatoriGraphEditorWindow.WindowInstance.GraphView.viewTransform.matrix.inverse.MultiplyPoint(new Vector2(wndPos.x + wndPos.width / 2, wndPos.y + wndPos.height / 2));

        TractatoriGraphEditorWindow.WindowInstance.GraphView.NodeEmitter.EnqueueRequest(req);
    }

    bool dragActive;
    void HandleBeginResizeDrag(MouseDownEvent evt) {
        if (dragActive) {
            evt.StopImmediatePropagation();
            return;
        }

        if (evt.button == 0) {
            if (Mathf.Abs(evt.mousePosition.x - style.width.value.value) <= resizeBorderThreshold) {
                dragActive = true;
                resizer.CaptureMouse();
                evt.StopPropagation();
            }
        }
    }
    void HandleResizeDrag(MouseMoveEvent evt) {
        if (dragActive || resizer.HasMouseCapture()) {
            style.width = Mathf.Clamp(evt.mousePosition.x, parent.resolvedStyle.width * 0.1f, parent.resolvedStyle.width * 0.25f);

            evt.StopPropagation();
        }
    }
    void HandleEndResizeDrag(MouseUpEvent evt) {
        if (dragActive) {
            dragActive = false;
            resizer.ReleaseMouse();
            evt.StopPropagation();
        }
    }

    static CustomStyleProperty<Texture2D> _resizeWindowThreshold = new CustomStyleProperty<Texture2D>("--resizer-texture");
    void HandleCustomStyleResolved(CustomStyleResolvedEvent evt) {
        evt.customStyle.TryGetValue(_resizeWindowThreshold, out var resizerTexture);
        // resizer.style.backgroundImage = resizerTexture;
    }
}
