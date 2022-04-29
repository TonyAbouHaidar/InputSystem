#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UIElements;

namespace UnityEngine.InputSystem.Editor
{
    /// <summary>
    /// A list view to display the action maps of the currently opened input actions asset.
    /// </summary>
    internal class ActionMapsView : ViewBase<ActionMapsView.ViewState>
    {
        public ActionMapsView(VisualElement root, StateContainer stateContainer)
            : base(stateContainer)
        {
            m_Root = root;

            m_ListView = m_Root?.Q<ListView>("action-maps-list-view");
            m_ListView.selectionType = UIElements.SelectionType.Single;
            m_ListView.selectionChanged += _ => SelectActionMap();

            m_ListView.bindItem = (element, i) =>
            {
                var treeViewItem = (InputActionsTreeViewItem)element;
                treeViewItem.label.text = (string)m_ListView.itemsSource[i];
                treeViewItem.EditTextFinished += ChangeActionMapName;
            };
            m_ListView.makeItem = () => new InputActionsTreeViewItem();
            m_ListView.unbindItem = (element, i) =>
            {
                ((InputActionsTreeViewItem)element).EditTextFinished -= ChangeActionMapName;
            };

            CreateSelector(s => new ViewStateCollection<string>(Selectors.GetActionMapNames(s)),
                (actionMapNames, state) => new ViewState(Selectors.GetSelectedActionMap(state), actionMapNames));
        }

        private Button addActionMapButton => m_Root?.Q<Button>("add-new-action-map-button");

        public override void RedrawUI(ViewState viewState)
        {
            m_ListView.itemsSource = viewState.actionMapNames?.ToList() ?? new List<string>();
            addActionMapButton.clicked += ShowAddActionMapWindow;
        }

        public override void DestroyView()
        {
            addActionMapButton.clicked -= ShowAddActionMapWindow;
        }

        private void ChangeActionMapName(string newName)
        {
            Dispatch(Commands.ChangeActionMapName(newName));
        }

        private void SelectActionMap()
        {
            Dispatch(Commands.SelectActionMap((string)m_ListView.selectedItem));
        }

        private void ShowAddActionMapWindow()
        {
        }

        private readonly VisualElement m_Root;
        private ListView m_ListView;

        internal class ViewState
        {
            public SerializedInputActionMap selectedActionMap;
            public IEnumerable<string> actionMapNames;

            public ViewState(SerializedInputActionMap selectedActionMap, IEnumerable<string> actionMapNames)
            {
                this.selectedActionMap = selectedActionMap;
                this.actionMapNames = actionMapNames;
            }
        }
    }
}

#endif
