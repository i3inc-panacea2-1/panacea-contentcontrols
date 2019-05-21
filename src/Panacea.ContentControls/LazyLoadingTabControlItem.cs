using System.Windows;
using System.Windows.Controls;

namespace Panacea.ContentControls
{
    public class LazyLoadingTabControlItem : Control
    {
        static LazyLoadingTabControlItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(LazyLoadingTabControlItem), new FrameworkPropertyMetadata(typeof(LazyLoadingTabControlItem)));
        }
    }
}
