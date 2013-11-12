using System;
using System.Collections;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Microsoft.Phone.Controls;

namespace RefreshableListBoxSample
{
    [TemplatePart(Name = InnerSelectorName, Type = typeof(LongListSelector))]
    public class RefreshableListBox : Control
    {
        #region Implementation Fields

        private const string InnerSelectorName = "InnerSelector";

        private LongListSelector _innerSelector = null;
        private LongListSelectorLayoutMode _layoutMode = LongListSelectorLayoutMode.List;
        private object _selectedItem;

        private ViewportControl _viewportControl;
        private DataTemplate _prepareRefreshDataTemplate;
        private DataTemplate _refreshingDataTemplate;
        private bool _isTopOnce;
        private bool _isRefreshing;

        #endregion

        #region Dependency Properties

        /// <summary>
        ///     Gets or sets the size used when displaying an item in the RefreshableListBox.
        /// </summary>
        public Size GridCellSize
        {
            get { return (Size)GetValue(GridCellSizeProperty); }
            set { SetValue(GridCellSizeProperty, value); }
        }

        /// <summary>
        /// Identifies the RefreshableListBox.GridCellSize dependency property.
        /// </summary>
        public static readonly DependencyProperty GridCellSizeProperty =
            DependencyProperty.Register("GridCellSize", typeof(Size), typeof(RefreshableListBox), new PropertyMetadata(Size.Empty));

        /// <summary>
        ///     Gets or sets the template for the group footer in the RefreshableListBox.
        /// </summary>
        public DataTemplate GroupFooterTemplate
        {
            get { return (DataTemplate)GetValue(GroupFooterTemplateProperty); }
            set { SetValue(GroupFooterTemplateProperty, value); }
        }

        /// <summary>
        ///     Identifies the RefreshableListBox.GroupFooterTemplate dependency property.
        /// </summary>
        public static readonly DependencyProperty GroupFooterTemplateProperty =
            DependencyProperty.Register("GroupFooterTemplate", typeof(DataTemplate), typeof(RefreshableListBox), new PropertyMetadata(null));

        /// <summary>
        ///    Gets or sets the template for the group header in the RefreshableListBox.
        /// </summary>
        public DataTemplate GroupHeaderTemplate
        {
            get { return (DataTemplate)GetValue(GroupHeaderTemplateProperty); }
            set { SetValue(GroupHeaderTemplateProperty, value); }
        }

        /// <summary>
        ///    Identifies the RefreshableListBox.GroupHeaderTemplate dependency property.
        /// </summary>
        public static readonly DependencyProperty GroupHeaderTemplateProperty =
            DependencyProperty.Register("GroupHeaderTemplate", typeof(DataTemplate), typeof(RefreshableListBox), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets a value that indicates whether to hide empty groups in the RefreshableListBox.
        /// </summary>
        public bool HideEmptyGroups
        {
            get { return (bool)GetValue(HideEmptyGroupsProperty); }
            set { SetValue(HideEmptyGroupsProperty, value); }
        }

        /// <summary>
        /// Identifies the RefreshableListBox.HideEmptyGroups  dependency property.
        /// </summary>
        public static readonly DependencyProperty HideEmptyGroupsProperty =
            DependencyProperty.Register("HideEmptyGroups", typeof(bool), typeof(RefreshableListBox), new PropertyMetadata(false));

        /// <summary>
        /// Gets or sets a value that indicates whether grouping is enabled in the RefreshableListBox.
        /// </summary>
        public bool IsGroupingEnabled
        {
            get { return (bool)GetValue(IsGroupingEnabledProperty); }
            set { SetValue(IsGroupingEnabledProperty, value); }
        }

        /// <summary>
        /// Identifies the RefreshableListBox.IsGroupingEnabled dependency property.
        /// </summary>
        public static readonly DependencyProperty IsGroupingEnabledProperty =
            DependencyProperty.Register("IsGroupingEnabled", typeof(bool), typeof(RefreshableListBox), new PropertyMetadata(false));

        /// <summary>
        /// Gets or sets a collection used to generate the content of the RefreshableListBox.
        /// </summary>
        public IList ItemsSource
        {
            get { return (IList)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        /// <summary>
        /// Identifies the RefreshableListBox.ItemsSource dependency property.
        /// </summary>
        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register("ItemsSource", typeof(IList), typeof(RefreshableListBox), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the template for the items in the items view.
        /// </summary>
        public DataTemplate ItemTemplate
        {
            get { return (DataTemplate)GetValue(ItemTemplateProperty); }
            set { SetValue(ItemTemplateProperty, value); }
        }

        /// <summary>
        /// Identifies the RefreshableListBox.ItemTemplate dependency property.
        /// </summary>
        public static readonly DependencyProperty ItemTemplateProperty =
            DependencyProperty.Register("ItemTemplate", typeof(DataTemplate), typeof(RefreshableListBox), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the System.Windows.Style for jump list in the RefreshableListBox.
        /// </summary>
        public Style JumpListStyle
        {
            get { return (Style)GetValue(JumpListStyleProperty); }
            set { SetValue(JumpListStyleProperty, value); }
        }

        /// <summary>
        /// Identifies the RefreshableListBox.JumpListStyle dependency property.
        /// </summary>
        public static readonly DependencyProperty JumpListStyleProperty =
            DependencyProperty.Register("JumpListStyle", typeof(Style), typeof(RefreshableListBox), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets a value that specifies if the RefreshableListBox
        /// is in a list mode or grid mode from the Microsoft.Phone.Controls.LongListSelectorLayoutMode
        /// enum.
        /// </summary>
        public LongListSelectorLayoutMode LayoutMode
        {
            get { return _layoutMode; }
            set
            {
                _layoutMode = value;
                if (_innerSelector != null)
                {
                    _innerSelector.LayoutMode = value;
                }
            }
        }

        public object SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                _selectedItem = value;
                if (_innerSelector != null)
                {
                    _innerSelector.SelectedItem = value;
                }
            }
        }

        #endregion

        public RefreshableListBox()
        {
            this.DefaultStyleKey = typeof(RefreshableListBox);
        }

        #region Methods

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (_innerSelector != null)
            {
                _innerSelector.ItemRealized -= _innerSelector_ItemRealized;
                _innerSelector.ItemUnrealized -= _innerSelector_ItemUnrealized;
                _innerSelector.JumpListClosed -= _innerSelector_JumpListClosed;
                _innerSelector.JumpListOpening -= _innerSelector_JumpListOpening;
                _innerSelector.ManipulationStateChanged -= _innerSelector_ManipulationStateChanged;
                _innerSelector.PropertyChanged -= _innerSelector_PropertyChanged;
                _innerSelector.SelectionChanged -= _innerSelector_SelectionChanged;
                _innerSelector.MouseMove -= _innerSelector_MouseMove;
                _innerSelector.MouseLeave -= _innerSelector_MouseLeave;
            }
            _innerSelector = this.GetTemplateChild(InnerSelectorName) as LongListSelector;
            if (_innerSelector != null)
            {
                _innerSelector.LayoutMode = LayoutMode;
                _innerSelector.SelectedItem = SelectedItem;
                _innerSelector.ItemRealized += _innerSelector_ItemRealized;
                _innerSelector.ItemUnrealized += _innerSelector_ItemUnrealized;
                _innerSelector.JumpListClosed += _innerSelector_JumpListClosed;
                _innerSelector.JumpListOpening += _innerSelector_JumpListOpening;
                _innerSelector.ManipulationStateChanged += _innerSelector_ManipulationStateChanged;
                _innerSelector.PropertyChanged += _innerSelector_PropertyChanged;
                _innerSelector.SelectionChanged += _innerSelector_SelectionChanged;
                _innerSelector.MouseMove += _innerSelector_MouseMove;
                _innerSelector.MouseLeave += _innerSelector_MouseLeave;

                _viewportControl = _innerSelector.GetFirstLogicalChildByType<ViewportControl>(true);
                _prepareRefreshDataTemplate = _innerSelector.Resources["PrepareRefreshDataTemplate"] as DataTemplate;
                _refreshingDataTemplate = _innerSelector.Resources["RefreshingDataTemplate"] as DataTemplate;
            }
        }

        /// <summary>
        /// Scrolls to a specified item in the RefreshableListBox.
        /// </summary>
        /// <param name="item">The list item to scroll to.</param>
        public void ScrollTo(object item)
        {
            if (_innerSelector != null)
            {
                _innerSelector.ScrollTo(item);
            }
        }

        public void HideRefreshPanel()
        {
            if (this._innerSelector != null)
            {
                this._isRefreshing = false;
                this.Dispatcher.BeginInvoke(() =>
                {
                    this._innerSelector.ListHeaderTemplate = null;
                });
            }
        }

        #endregion

        #region Events

        /// <summary>
        ///    Occurs when a new item is realized.
        /// </summary>
        public event EventHandler<ItemRealizationEventArgs> ItemRealized;
        private void OnItemRealized(ItemRealizationEventArgs eventArgs)
        {
            var handler = this.ItemRealized;
            if (handler != null)
            {
                handler(this, eventArgs);
            }
        }

        /// <summary>
        ///    Occurs when an item in the Microsoft.Phone.Controls.LongListMultiSelector is unrealized.
        /// </summary>
        public event EventHandler<ItemRealizationEventArgs> ItemUnrealized;
        private void OnItemUnrealized(ItemRealizationEventArgs eventArgs)
        {
            var handler = this.ItemUnrealized;
            if (handler != null)
            {
                handler(this, eventArgs);
            }
        }

        /// <summary>
        ///    Occurs when the jump list is closed.
        /// </summary>
        public event EventHandler JumpListClosed;
        private void OnJumpListClosed(EventArgs eventArgs)
        {
            var handler = this.JumpListClosed;
            if (handler != null)
            {
                handler(this, eventArgs);
            }
        }

        /// <summary>
        ///    Occurs when a jump list is opened.
        /// </summary>
        public event EventHandler JumpListOpening;
        private void OnJumpListOpening(EventArgs eventArgs)
        {
            var handler = this.JumpListOpening;
            if (handler != null)
            {
                handler(this, eventArgs);
            }
        }

        /// <summary>
        ///    Occurs when Microsoft.Phone.Controls.ManipulationState changes.
        /// </summary>
        public event EventHandler ManipulationStateChanged;
        private void OnManipulationStateChanged(EventArgs eventArgs)
        {
            var handler = this.ManipulationStateChanged;
            if (handler != null)
            {
                handler(this, eventArgs);
            }
        }

        /// <summary>
        ///    Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(PropertyChangedEventArgs eventArgs)
        {
            var handler = this.PropertyChanged;
            if (handler != null)
            {
                handler(this, eventArgs);
            }
        }

        /// <summary>
        ///    Occurs when the currently selected item changes.
        /// </summary>
        public event SelectionChangedEventHandler SelectionChanged;
        private void OnSelectionChanged(SelectionChangedEventArgs eventArgs)
        {
            var handler = this.SelectionChanged;
            if (handler != null)
            {
                handler(this, eventArgs);
            }
        }

        void _innerSelector_ItemRealized(object sender, ItemRealizationEventArgs e)
        {
            this.OnItemRealized(e);
        }

        void _innerSelector_ItemUnrealized(object sender, ItemRealizationEventArgs e)
        {
            this.OnItemUnrealized(e);
        }

        void _innerSelector_JumpListClosed(object sender, EventArgs e)
        {
            this.OnJumpListClosed(e);
        }

        void _innerSelector_JumpListOpening(object sender, EventArgs e)
        {
            this.OnJumpListOpening(e);
        }

        void _innerSelector_ManipulationStateChanged(object sender, EventArgs e)
        {
            this.OnManipulationStateChanged(e);
        }

        void _innerSelector_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            this.OnPropertyChanged(e);
        }

        void _innerSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.OnSelectionChanged(e);
        }

        void _innerSelector_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            //在此处判断是否第一次到达顶部
            if (this.ItemsSource != null && this.ItemsSource.Count > 0 &&
                _viewportControl.Bounds.Top == _viewportControl.Viewport.Top && _innerSelector.ListHeaderTemplate == null)
            {
                if (!this._isTopOnce)
                {
                    this._isTopOnce = true;
                    _innerSelector.ListHeaderTemplate = this._prepareRefreshDataTemplate;
                }
                return;
            }
        }

        void _innerSelector_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            //再此到顶
            if (!this._isRefreshing && this._isTopOnce)
            {
                if (_viewportControl.Bounds.Top == _viewportControl.Viewport.Top)
                {
                    this._isRefreshing = true;
                    _innerSelector.ListHeaderTemplate = this._refreshingDataTemplate;
                    this.OnRefreshTriggered();
                }
                else
                {
                    _innerSelector.ListHeaderTemplate = null;
                }
            }
            this._isTopOnce = false;
        }

        public event EventHandler RefreshTriggered;
        private void OnRefreshTriggered()
        {
            var handler = this.RefreshTriggered;
            if (handler != null)
            {
                handler(this, new EventArgs());
            }
        }

        #endregion
    }
}
