using System.Windows.Media;

namespace BabelIm.Controls.PivotControl {
    internal delegate void ScrollCompletedEventHandler(object sender, ScrollCompletedEventArgs e);

    internal class ScrollCompletedEventArgs {
        public int SelectedIndex;
    }

    internal struct ScrollHost {
        public double Padding;
        public double Speed;
        public TranslateTransform Transform;
        public double Width;

        public void Reset() {
            Width = 0.0;
            Speed = 1.0;
        }
    }
}