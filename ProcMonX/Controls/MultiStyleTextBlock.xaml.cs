using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ProcMonX.Controls {
    /// <summary>
    /// Interaction logic for MultiStyleTextBlock.xaml
    /// </summary>
    public partial class MultiStyleTextBlock {
        public MultiStyleTextBlock() {
            InitializeComponent();
        }

        public string Separator {
            get { return (string)GetValue(SeparatorProperty); }
            set { SetValue(SeparatorProperty, value); }
        }

        public static readonly DependencyProperty SeparatorProperty =
            DependencyProperty.Register(nameof(Separator), typeof(string), typeof(MultiStyleTextBlock), new PropertyMetadata(";;"));


        public string Text {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register(nameof(Text), typeof(string), typeof(MultiStyleTextBlock), new PropertyMetadata(null, (s, e) => ((MultiStyleTextBlock)s).OnTextChanged(e)));

        private void OnTextChanged(DependencyPropertyChangedEventArgs e) {
            _textBlock.Inlines.Clear();
            if (e.NewValue == null)
                return;

            var substrings = ((string)e.NewValue).Split(new string[] { Separator }, StringSplitOptions.RemoveEmptyEntries);
            bool normal = true;
            foreach (var str in substrings) {
                var run = new Run(str) { FontWeight = normal ? FontWeights.Normal : FontWeights.Bold };
                if (!string.IsNullOrWhiteSpace(str))
                    normal = !normal;
                _textBlock.Inlines.Add(run);
            }
        }
    }
}
