﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Controls.Primitives;
using System.Collections.Specialized;
using System.Windows.Media.Animation;

namespace ChromeTabs
{
    /// <summary>
    /// Follow steps 1a or 1b and then 2 to use this custom control in a XAML file.
    ///
    /// Step 1a) Using this custom control in a XAML file that exists in the current project.
    /// Add this XmlNamespace attribute to the root element of the markup file where it is 
    /// to be used:
    ///
    ///     xmlns:MyNamespace="clr-namespace:ChromiumTabs"
    ///
    ///
    /// Step 1b) Using this custom control in a XAML file that exists in a different project.
    /// Add this XmlNamespace attribute to the root element of the markup file where it is 
    /// to be used:
    ///
    ///     xmlns:MyNamespace="clr-namespace:ChromiumTabs;assembly=ChromiumTabs"
    ///
    /// You will also need to add a project reference from the project where the XAML file lives
    /// to this project and Rebuild to avoid compilation errors:
    ///
    ///     Right click on the target project in the Solution Explorer and
    ///     "Add Reference"->"Projects"->[Select this project]
    ///
    ///
    /// Step 2)
    /// Go ahead and use your control in the XAML file.
    ///
    ///     <MyNamespace:CustomControl1/>
    ///
    /// </summary>
    public class ChromeTabControl : Selector
    {
        internal static readonly DependencyPropertyKey CanAddTabPropertyKey = DependencyProperty.RegisterReadOnly("CanAddTab", typeof(bool), typeof(ChromeTabControl), new PropertyMetadata(true));
        public static readonly DependencyProperty CanAddTabProperty = CanAddTabPropertyKey.DependencyProperty;
        public static readonly DependencyProperty SelectedContentProperty = DependencyProperty.Register("SelectedContent", typeof(object), typeof(ChromeTabControl), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));

        static ChromeTabControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ChromeTabControl), new FrameworkPropertyMetadata(typeof(ChromeTabControl)));
        }

        public void AddTab(object tab, bool select)
        {
            if (!CanAddTab)
            {
                return;
            }
            this.Items.Add(tab);
            if (select || this.Items.Count == 1)
            {
                this.SelectedIndex = this.Items.Count - 1;
            }
        }

        public bool CanAddTab
        {
            get { return (bool)GetValue(CanAddTabProperty); }
        }

        public void RemoveTab(object tab)
        {
            int selectedIndex = this.SelectedIndex;
            bool removedSelectedTab = false;
            ChromeTabItem removeItem = this.AsTabItem(tab);

            foreach (object item in this.Items)
            {

                ChromeTabItem tabItem = this.AsTabItem(item);

                if (tabItem != null && tabItem == removeItem)
                {
                    if (tabItem.Content == this.SelectedContent)
                    {
                        removedSelectedTab = true;
                    }
                    if (this.ObjectToContainer.ContainsKey(tab))
                    {
                        this.ObjectToContainer.Remove(tab);
                    }
                    this.Items.Remove(item);

                    if (ItemRemoved != null) { ItemRemoved(tabItem); }

                    break;
                }
            }

            if (removedSelectedTab && this.Items.Count > 0)
            {
                this.SelectedItem = this.Items[Math.Min(selectedIndex, this.Items.Count - 1)];
            }
            else if (removedSelectedTab)
            {
                this.SelectedItem = null;
                this.SelectedContent = null;
            }
        }

        public delegate void ItemRemovedEvent(object tab);
        public event ItemRemovedEvent ItemRemoved;

        public object SelectedContent
        {
            get { return (object)GetValue(SelectedContentProperty); }
            set { SetValue(SelectedContentProperty, value); }
        }

        internal int GetTabIndex(ChromeTabItem item)
        {
            for (int i = 0; i < this.Items.Count; i += 1)
            {
                ChromeTabItem tabItem = this.AsTabItem(this.Items[i]);
                if (tabItem == item)
                {
                    return i;
                }
            }
            return -1;
        }

        internal void ChangeSelectedItem(ChromeTabItem item)
        {
            int index = this.GetTabIndex(item);
            if (index > -1)
            {
                if (this.SelectedItem != null)
                {
                    Canvas.SetZIndex(this.AsTabItem(this.SelectedItem), 0);
                }
                this.SelectedIndex = index;
                Canvas.SetZIndex(item, 1001);
            }
        }

        internal void MoveTab(int fromIndex, int toIndex)
        {
            if (this.Items.Count == 0)
            {
                return;
            }
            object tab = this.Items[fromIndex];
            this.Items.RemoveAt(fromIndex);
            this.Items.Insert(toIndex, tab);
            for (int i = 0; i < this.Items.Count; i += 1)
            {
                this.AsTabItem(this.Items[i]).Margin = new Thickness(0);
            }
            this.SelectedItem = tab;
        }

        internal void SetCanAddTab(bool value)
        {
            SetValue(CanAddTabPropertyKey, value);
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new ChromeTabItem { Header = "New Tab" };
        }

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return (item is ChromeTabItem);
        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            bool somethingSelected = false;
            foreach (UIElement element in this.Items)
            {
                somethingSelected |= ChromeTabItem.GetIsSelected(element);
            }
            if (!somethingSelected)
            {
                this.SelectedIndex = 0;
            }
            KeyboardNavigation.SetIsTabStop(this, false);
        }

        protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e)
        {
            base.OnItemsChanged(e);
            if (e.OldItems != null)
            {
                foreach (object item in e.OldItems)
                {
                    if (ObjectToContainer.ContainsKey(item))
                    {
                        ObjectToContainer.Remove(item);
                    }
                }
            }
            this.SetChildrenZ();
        }

        protected override void OnSelectionChanged(SelectionChangedEventArgs e)
        {
            base.OnSelectionChanged(e);
            this.SetChildrenZ();
            if (e.AddedItems.Count == 0) { return; }
            foreach (UIElement element in this.Items)
            {
                if (element == e.AddedItems[0]) { continue; }
                ChromeTabItem.SetIsSelected(element, false);
            }
            ChromeTabItem item = this.AsTabItem(this.SelectedItem);
            this.SelectedContent = item != null ? item.Content : null;
        }

        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            base.PrepareContainerForItemOverride(element, item);
            if (element != item)
            {
                this.ObjectToContainer[item] = element;
                this.SetChildrenZ();
            }
        }

        private ChromeTabItem AsTabItem(object item)
        {
            ChromeTabItem tabItem = item as ChromeTabItem;
            if (tabItem == null && item != null && this.ObjectToContainer.ContainsKey(item))
            {
                tabItem = this.ObjectToContainer[item] as ChromeTabItem;
            }
            return tabItem;
        }

        private Dictionary<object, DependencyObject> ObjectToContainer
        {
            get
            {
                if (objectToContainerMap == null)
                {
                    objectToContainerMap = new Dictionary<object, DependencyObject>();
                }
                return objectToContainerMap;
            }
        }

        private void SetChildrenZ()
        {
            int zindex = this.Items.Count - 1;
            foreach (object element in this.Items)
            {
                ChromeTabItem tabItem = this.AsTabItem(element);
                if (tabItem == null) { continue; }
                if (ChromeTabItem.GetIsSelected(tabItem))
                {
                    Panel.SetZIndex(tabItem, this.Items.Count);
                }
                else
                {
                    Panel.SetZIndex(tabItem, zindex);
                }
                zindex -= 1;
            }
        }

        private Dictionary<object, DependencyObject> objectToContainerMap;
    }
}
