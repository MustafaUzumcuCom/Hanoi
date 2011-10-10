using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace BabelIm.Controls.PivotControl {
    [ContentProperty("Content")]
    public class PivotItem
        : ContentControl {
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register(
                "Title",
                typeof (object),
                typeof (PivotItem),
                new PropertyMetadata(OnTitleChanged));

        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register(
                "Header",
                typeof (object),
                typeof (PivotItem),
                new PropertyMetadata(OnHeaderChanged));

        public PivotItem() {
            DefaultStyleKey = typeof (PivotItem);
        }

        public object Title {
            get { return GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        public object Header {
            get { return GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }

        public override void OnApplyTemplate() {
            base.OnApplyTemplate();
        }

        private static void OnTitleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            var ctrl = (PivotItem) d;
            ctrl.OnTitleChanged(e.OldValue, e.NewValue);
        }

        protected virtual void OnTitleChanged(object oldHeader, object newHeader) {
        }

        private static void OnHeaderChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            var ctrl = (PivotItem) d;
            ctrl.OnHeaderChanged(e.OldValue, e.NewValue);
        }

        protected virtual void OnHeaderChanged(object oldHeader, object newHeader) {
        }
        }
}