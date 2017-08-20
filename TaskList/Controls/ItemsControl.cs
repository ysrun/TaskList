using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Xamarin.Forms;
namespace TaskList.Controls
{
    /// <summary>
    /// ItemsControl 風 View
    /// </summary>
    public class ItemsControl : ContentView
    {
        /// <summary>
        /// ItemsPanel
        /// </summary>
        private Layout<View> itemsPanel = null;

        /// <summary>
        /// ItemsPanel CLR プロパティ
        /// </summary>
        public Layout<View> ItemsPanel
        {
            get { return this.itemsPanel; }
            set { this.itemsPanel = value; }
        }

        #region ItemsSource

        /// <summary>
        /// ItemsSource BindableProperty
        /// </summary>
        public static readonly BindableProperty ItemsSourceProperty = BindableProperty.Create<ItemsControl, IEnumerable>(
            p => p.ItemsSource,
            new ObservableCollection<object>(),
            BindingMode.OneWay,
            null,
            OnItemsSourceChanged);

        /// <summary>
        /// ItemsSource CLR プロパティ
        /// </summary>
        public IEnumerable ItemsSource
        {
            get { return (IEnumerable)this.GetValue(ItemsSourceProperty); }
            set { this.SetValue(ItemsSourceProperty, value); }
        }

        /// <summary>
        /// ItemsSource 変更イベントハンドラ
        /// </summary>
        /// <param name="bindable">BindableObject</param>
        /// <param name="oldValue">古い値</param>
        /// <param name="newValue">新しい値</param>
        private static void OnItemsSourceChanged(BindableObject bindable, IEnumerable oldValue, IEnumerable newValue)
        {
            var control = bindable as ItemsControl;
            if (control == null)
            {
                return;
            }

            var oldCollection = oldValue as INotifyCollectionChanged;
            if (oldCollection != null)
            {
                oldCollection.CollectionChanged -= control.OnCollectionChanged;
            }

            if (newValue == null)
            {
                return;
            }

            control.ItemsPanel.Children.Clear();

            foreach (var item in newValue)
            {
                var content = control.ItemTemplate.CreateContent();
                View view;
                var cell = content as ViewCell;
                if (cell != null)
                {
                    view = cell.View;
                }
                else
                {
                    view = (View)content;
                }

                view.BindingContext = item;
                control.ItemsPanel.Children.Add(view);
            }

            var newCollection = newValue as INotifyCollectionChanged;
            if (newCollection != null)
            {
                newCollection.CollectionChanged += control.OnCollectionChanged;
            }

            control.UpdateChildrenLayout();
            control.InvalidateLayout();
        }

        #endregion //ItemsSource

        #region ItemTemplate

        /// <summary>
        /// ItemTemplate BindableProperty
        /// </summary>
        public static readonly BindableProperty ItemTemplateProperty = BindableProperty.Create<ItemsControl, DataTemplate>(
            p => p.ItemTemplate,
            default(DataTemplate));

        /// <summary>
        /// ItemTemplate CLR プロパティ
        /// </summary>
        public DataTemplate ItemTemplate
        {
            get { return (DataTemplate)this.GetValue(ItemTemplateProperty); }
            set { this.SetValue(ItemTemplateProperty, value); }
        }

        #endregion //ItemTemplate

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ItemsControl()
        {
            this.itemsPanel = new StackLayout() { Orientation = StackOrientation.Horizontal };
            this.Content = this.itemsPanel;
        }

        /// <summary>
        /// Items の変更イベントハンドラ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
            {
                this.ItemsPanel.Children.RemoveAt(e.OldStartingIndex);
                this.UpdateChildrenLayout();
                this.InvalidateLayout();
            }

            var collection = this.ItemsSource as ObservableCollection<object>;
            if (e.NewItems == null || collection == null)
            {
                return;
            }
            foreach (var item in e.NewItems)
            {
                var content = this.ItemTemplate.CreateContent();

                View view;
                var cell = content as ViewCell;
                if (cell != null)
                {
                    view = cell.View;
                }
                else
                {
                    view = (View)content;
                }

                view.BindingContext = item;
                this.ItemsPanel.Children.Insert(collection.IndexOf(item), view);
            }

            this.UpdateChildrenLayout();
            this.InvalidateLayout();
        }
    }
}