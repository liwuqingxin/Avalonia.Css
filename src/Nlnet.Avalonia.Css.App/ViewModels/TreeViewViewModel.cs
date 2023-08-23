using System;
using System.Collections.ObjectModel;
using System.Linq;
using Avalonia.Controls;

namespace Nlnet.Avalonia.Css.App
{
    public class TreeViewPageViewModel : NotifyPropertyChanged
    {
        private readonly Node _root;
        private SelectionMode _selectionMode;
        
        public ObservableCollection<Node> Items { get; }
        
        public ObservableCollection<Node> SelectedItems { get; }
        
        public SelectionMode SelectionMode
        {
            get => _selectionMode;
            set
            {
                SelectedItems.Clear();
                if (_selectionMode != value)
                {
                    _selectionMode = value;
                    OnPropertyChanged();
                }
            }
        }
        
        
        
        public TreeViewPageViewModel()
        {
            _root = new Node();

            Items = _root.Children;
            SelectedItems = new ObservableCollection<Node>();
        }

        public void SelectRandomItem()
        {
            var random = new Random();
            var depth = random.Next(4);
            var indexes = Enumerable.Range(0, depth).Select(x => random.Next(10));
            var node = _root;

            foreach (var i in indexes)
            {
                node = node.Children[i];
            }

            SelectedItems.Clear();
            SelectedItems.Add(node);
        }

        public class Node
        {
            private ObservableCollection<Node>? _children;
            private int _childIndex = 10;

            public Node()
            {
                Header = "Item";
            }

            private Node(Node parent, int index)
            {
                Parent = parent;
                Header = parent.Header + ' ' + index;
            }

            public Node? Parent { get; }
            
            public string Header { get; }
            
            public bool AreChildrenInitialized => _children != null;
            
            public ObservableCollection<Node> Children => _children ??= CreateChildren();
            
            public void AddItem() => Children.Add(new Node(this, _childIndex++));
            
            public void RemoveItem(Node child) => Children.Remove(child);
            
            public override string ToString() => Header;

            private ObservableCollection<Node> CreateChildren()
            {
                return new ObservableCollection<Node>(Enumerable.Range(0, 10).Select(i => new Node(this, i)));
            }
        }
    }
}
