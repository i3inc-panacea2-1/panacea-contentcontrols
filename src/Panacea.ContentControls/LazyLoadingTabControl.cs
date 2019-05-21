using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Globalization;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using Panacea.Controls;
using Panacea.Models;
using System.Collections.ObjectModel;

namespace Panacea.ContentControls
{
/// <summary>
/// Follow steps 1a or 1b and then 2 to use this custom control in a XAML file.
///
/// Step 1a) Using this custom control in a XAML file that exists in the current project.
/// Add this XmlNamespace attribute to the root element of the markup file where it is 
/// to be used:
///
///     xmlns:MyNamespace="clr-namespace:PanaceaLib.Controls"
///
///
/// Step 1b) Using this custom control in a XAML file that exists in a different project.
/// Add this XmlNamespace attribute to the root element of the markup file where it is 
/// to be used:
///
///     xmlns:MyNamespace="clr-namespace:PanaceaLib.Controls;assembly=PanaceaLib.Controls"
///
/// You will also need to add a project reference from the project where the XAML file lives
/// to this project and Rebuild to avoid compilation errors:
///
///     Right click on the target project in the Solution Explorer and
///     "Add Reference"->"Projects"->[Browse to and select this project]
///
///
/// Step 2)
/// Go ahead and use your control in the XAML file.
///
///     <MyNamespace:LazyLoadingTabControl/>
///
/// </summary>
public class LazyLoadingTabControl : Control
{
    static LazyLoadingTabControl()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(LazyLoadingTabControl), new FrameworkPropertyMetadata(typeof(LazyLoadingTabControl)));
    }

    #region dependency properties

    #region MaterialIcon
    public string MaterialIcon
    {
        get { return (string)GetValue(MaterialIconProperty); }
        set { SetValue(MaterialIconProperty, value); }
    }

    // Using a DependencyProperty as the backing store for Icon.  This enables animation, styling, binding, etc...
    private static readonly DependencyProperty MaterialIconProperty =
        DependencyProperty.Register("MaterialIcon", typeof(string), typeof(LazyLoadingTabControl), new PropertyMetadata("none"));
    #endregion

    #region Title
    public string Title
    {
        get { return (string)GetValue(TitleProperty); }
        set { SetValue(TitleProperty, value); }
    }

    // Using a DependencyProperty as the backing store for Title.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty TitleProperty =
        DependencyProperty.Register("Title", typeof(string), typeof(LazyLoadingTabControl), new PropertyMetadata(null));
    #endregion

    #region Color
    public Brush Color
    {
        get { return (Brush)GetValue(ColorProperty); }
        set { SetValue(ColorProperty, value); }
    }

    // Using a DependencyProperty as the backing store for Color.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty ColorProperty =
        DependencyProperty.Register("Color", typeof(Brush), typeof(LazyLoadingTabControl), new PropertyMetadata(Brushes.Red));
    #endregion

    #region SearchCommand
    public ICommand SearchCommand
    {
        get { return (ICommand)GetValue(SearchCommandProperty); }
        set { SetValue(SearchCommandProperty, value); }
    }

    // Using a DependencyProperty as the backing store for SearchCommand.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty SearchCommandProperty =
        DependencyProperty.Register("SearchCommand", typeof(ICommand), typeof(LazyLoadingTabControl), new PropertyMetadata(null));
    #endregion

    #region CategoryRequestedCommand
    public ICommand CategoryRequestedCommand
    {
        get { return (ICommand)GetValue(CategoryRequestedCommandProperty); }
        set { SetValue(CategoryRequestedCommandProperty, value); }
    }

    // Using a DependencyProperty as the backing store for CategoryRequestedCommand.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty CategoryRequestedCommandProperty =
        DependencyProperty.Register("CategoryRequestedCommand", typeof(ICommand), typeof(LazyLoadingTabControl), new PropertyMetadata(null));
    #endregion

    #region ImageVisibility
    public Visibility ImageVisibility
    {
        get { return (Visibility)GetValue(ImageVisibilityProperty); }
        set { SetValue(ImageVisibilityProperty, value); }
    }
    public static readonly DependencyProperty ImageVisibilityProperty =
        DependencyProperty.Register("ImageVisibility",
            typeof(Visibility),
            typeof(LazyLoadingTabControl),
            new FrameworkPropertyMetadata(System.Windows.Visibility.Visible));
    #endregion

    #region NameVisibility
    public Visibility NameVisibility
    {
        get { return (Visibility)GetValue(NameVisibilityProperty); }
        set { SetValue(NameVisibilityProperty, value); }
    }
    public static readonly DependencyProperty NameVisibilityProperty =
        DependencyProperty.Register("NameVisibility",
            typeof(Visibility),
            typeof(LazyLoadingTabControl),
            new FrameworkPropertyMetadata(System.Windows.Visibility.Visible));
    #endregion

    #region IsSearchable
    public bool IsSearchable
    {
        get { return (bool)GetValue(IsSearchableProperty); }
        set { SetValue(IsSearchableProperty, value); }
    }
    public static readonly DependencyProperty IsSearchableProperty =
        DependencyProperty.Register("IsSearchable",
            typeof(bool),
            typeof(LazyLoadingTabControl),
            new FrameworkPropertyMetadata(true));
    #endregion

    #region RefreshCommand
    public ICommand RefreshCommand
    {
        get { return (ICommand)GetValue(RefreshCommandProperty); }
        protected set { SetValue(RefreshCommandProperty, value); }
    }

    // Using a DependencyProperty as the backing store for RefreshCommand.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty RefreshCommandProperty =
        DependencyProperty.Register("RefreshCommand", typeof(ICommand), typeof(LazyLoadingTabControl), new PropertyMetadata(null));
    #endregion

    #region ItemProvider
    public ILazyItemProvider ItemProvider
    {
        get { return (ILazyItemProvider)GetValue(ItemProviderProperty); }
        set { SetValue(ItemProviderProperty, value); }
    }

    // Using a DependencyProperty as the backing store for Provider.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty ItemProviderProperty =
        DependencyProperty.Register("ItemProvider", typeof(ILazyItemProvider), typeof(LazyLoadingTabControl), new PropertyMetadata(null, OnItemProviderChanged));

    private static async void OnItemProviderChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var control = d as LazyLoadingTabControl;
        var oldProvider = e.OldValue as ILazyItemProvider;
        await control.RefreshCategories();
    }
    #endregion

    #region Ratio
    public double Ratio
    {
        get { return (double)GetValue(RatioProperty); }
        set { SetValue(RatioProperty, value); }
    }

    public static readonly DependencyProperty RatioProperty = DependencyProperty.Register("Ratio",
        typeof(double), typeof(LazyLoadingTabControl), new PropertyMetadata(1.5d));
    #endregion

    #region ThumbnailExtraTemplate
    public DataTemplate ThumbnailExtraTemplate
    {
        get { return (DataTemplate)GetValue(ThumbnailExtraTemplateProperty); }
        set { SetValue(ThumbnailExtraTemplateProperty, value); }
    }
    public static readonly DependencyProperty ThumbnailExtraTemplateProperty =
        DependencyProperty.Register("ThumbnailExtraTemplate",
            typeof(DataTemplate),
            typeof(LazyLoadingTabControl),
            new FrameworkPropertyMetadata(null));
    #endregion


    public ICommand OpenItemCommand
    {
        get { return (ICommand)GetValue(OpenItemCommandProperty); }
        set { SetValue(OpenItemCommandProperty, value); }
    }

    // Using a DependencyProperty as the backing store for OpenItemCommand.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty OpenItemCommandProperty =
        DependencyProperty.Register("OpenItemCommand", typeof(ICommand), typeof(LazyLoadingTabControl), new PropertyMetadata(null));



    public int Columns
    {
        get { return (int)GetValue(ColumnsProperty); }
        set { SetValue(ColumnsProperty, value); }
    }

    // Using a DependencyProperty as the backing store for Columns.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty ColumnsProperty =
        DependencyProperty.Register("Columns", typeof(int), typeof(LazyLoadingTabControl), new PropertyMetadata(4));


    #endregion deps

    public LazyLoadingTabControl()
    {
        RefreshCommand = new RelayCommand(async (args) => await Refresh());
    }

    CancellationTokenSource _cts;
    private bool _busy = false;
    ScrollViewer _scrollviewer;
    TextBox _textbox;

    public override void OnApplyTemplate()
    {
        base.OnApplyTemplate();
        _textbox = Template.FindName("PART_search", this) as TextBox;
        _textbox.PreviewKeyDown += text_PreviewKeyDown;
        var itemsControl = Template.FindName("PART_itemscontainer", this) as ItemsControl;
        if (itemsControl == null) return;
        itemsControl.ApplyTemplate();
        _scrollviewer = (itemsControl.Template.FindName("PART_scrollviewer", itemsControl) as ScrollViewer);

        //_scrollviewer.ScrollChanged += LazyLoadingTabControl_ScrollChanged;

    }

    private async void Provider_Refreshed(object sender, EventArgs e)
    {
        await RefreshCategories();
    }

    private void LazyLoadingTabControl_Loaded(object sender, RoutedEventArgs e)
    {

    }

    Task Refresh()
    {
            return Task.CompletedTask;
    }

    async Task RefreshCategories()
    {

        try
        {
            await ItemProvider.Initialize();
        }
        catch
        {
            //TODO: HostWindow.ThemeManager.Toast(new Translator("core").Translate("An error occured. Please, try again later."));
        }
    }

    ServerGroupItem _searchCategory = new ServerGroupItem() { Name = "Search results..." };

    private async void text_PreviewKeyDown(object sender, KeyEventArgs e)
    {
        _cts?.Cancel();
        _cts = new CancellationTokenSource();
        try
        {
            await Task.Delay(800, _cts.Token);
        }
        catch
        {
            return;
        }


    }
    private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
    {

    }

    private void UIElement_OnMouseDown(object sender, MouseButtonEventArgs e)
    {
        (sender as UniformGrid).Focus();
    }

    private async Task DoWhileBusy(Func<Task> action)
    {
        if (_busy) return;
        _busy = true;
        try
        {
            await action();
        }
        finally
        {
            _busy = false;
        }
    }

    private Task<T> DoWhileBusy<T>(Func<Task<T>> action)
    {
        if (_busy) return Task.FromResult(default(T));
        _busy = true;
        try
        {
            return action();
        }
        finally
        {
            _busy = false;
        }
    }

#if DEBUG
    ~LazyLoadingTabControl()
    {
        Console.WriteLine("~Lazy");
    }
#endif
}

internal class HeightRelationalToParentConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var height = (double)value;
        var ratio = double.Parse(parameter.ToString());
        return height * ratio;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

internal class LazyDesignTimeData
{
    public List<ServerGroupItem> Categories
    {
        get
        {
            return new List<ServerGroupItem>()
                {
                    new ServerGroupItem(){Name = "Test", Id="1"},
                    new ServerGroupItem(){Name = "Test 2", Id="2"},
                    new ServerGroupItem(){Name = "Test 3", Id="3"}
                };
        }
    }

    public ObservableCollection<ObservableCollection<ServerItem>> ItemRows
    {
        get
        {
            return new ObservableCollection<ObservableCollection<ServerItem>>
                {
                    new ObservableCollection<ServerItem>(){
                        new ServerItem() {Name = "Test", Id="1", ImgThumbnail = new Thumbnail(){ Image="pack://application:,,,/PanaceaLib;component/Resources/Images/broken-link.png" } },
                        new ServerItem() {Name = "Test", Id="1", ImgThumbnail = new Thumbnail(){ Image="pack://application:,,,/PanaceaLib;component/Resources/Images/broken-link.png" }},
                        new ServerItem() {Name = "Test", Id="1", ImgThumbnail = new Thumbnail(){ Image="pack://application:,,,/PanaceaLib;component/Resources/Images/broken-link.png" }},
                        new ServerItem() {Name = "Test", Id="1", ImgThumbnail = new Thumbnail(){ Image="pack://application:,,,/PanaceaLib;component/Resources/Images/broken-link.png" }}
                    }

                };
        }
    }

    public Brush Color { get => new SolidColorBrush(System.Windows.Media.Color.FromRgb(208, 73, 62)); }

    public string Title { get => "Demo"; }

    public string Icon { get => "access_alarm"; set { } }



}
}