using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;
using UnityEngine.UIElements;

public class TabElement : VisualElement {
    public class TabContainer {
        public Button TabButton { get; private set; }
        public VisualElement Container { get; private set; }

        private TabContainer(Button btn, VisualElement container) {
            TabButton = btn;
            Container = container;
        }

        public static TabContainer Create(TabElement parent, Button btn) {
            var container = new VisualElement() {
                name = "tab-content-container",
            };
            parent.Add(container);

            return new TabContainer(btn, container);
        }
    }

    public VisualElement TabButtonContainer { get; private set; }

    private Dictionary<int, TabContainer> _tabButtons;
    private int active = -1;

    public ReadOnlyDictionary<int, TabContainer> TabButtons {
        get => new ReadOnlyDictionary<int, TabContainer>(_tabButtons);
    }

    public TabElement() : this(Resources.Load<StyleSheet>("StandardTabElement")) { }

    private StyleSheet myStyleSheet;

    public TabElement(StyleSheet styleSheet) {
        _tabButtons = new Dictionary<int, TabContainer>();

        TabButtonContainer = new VisualElement() {
            name = "tab-button-container"
        };

        myStyleSheet = styleSheet;
        styleSheets.Add(myStyleSheet);

        Add(TabButtonContainer);
    }

    public TabContainer RegisterTabButton(int id, string name, string text) {
        if (id < 0) {
            Debug.LogWarning("Tab Element cannot contains button with negative ID value");
            return null;
        }

        var btn = new Button() {
            text = text,
            name = name
        };

        if (_tabButtons.Count == 0) {
            active = id;
            btn.AddToClassList("selected-tab-button");
        } else {
            btn.AddToClassList("deselected-tab-button");
        }

        if (_tabButtons.ContainsKey(id)) {
            Debug.LogError("Tab Element already contains button id of " + id);
            return null;
        } else {
            var tab = TabContainer.Create(this, btn);
            tab.Container.style.display = _tabButtons.Count == 0 ? DisplayStyle.Flex : DisplayStyle.None;
            tab.Container.name += "-" + id;

            btn.clicked += () => {
                Switch(id);
            };

            _tabButtons.Add(id, tab);
            TabButtonContainer.Add(btn);

            return tab;
        }
    }

    public TabContainer GetTabContainer(int id) {
        return _tabButtons[id];
    }

    public bool TryGetTabContainer(int id, out TabContainer output) {
        return _tabButtons.TryGetValue(id, out output);
    }

    public bool Switch(int id) {
        if (active == id) return false;

        if (_tabButtons.TryGetValue(id, out var container)) {
            if (_tabButtons.TryGetValue(active, out var old)) {
                old.TabButton.RemoveFromClassList("selected-tab-button");
                old.TabButton.AddToClassList("deselected-tab-button");

                old.Container.style.display = DisplayStyle.None;
            }

            active = id;

            container.TabButton.RemoveFromClassList("deselected-tab-button");
            container.TabButton.AddToClassList("selected-tab-button");
            container.Container.style.display = DisplayStyle.Flex;

            return true;
        }

        Debug.LogWarning("Cannot switch to tab button with ID of " + id + " as the ID was null");
        return false;
    }

    public VisualElement GetTabContentContainer(int id) {
        if (_tabButtons.TryGetValue(id, out var container)) {
            return container.Container;
        }

        return null;
    }
}