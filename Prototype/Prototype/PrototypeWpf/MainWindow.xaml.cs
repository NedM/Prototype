using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PrototypeWpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public string[] comboOptions = {"Yes", "No"};
        public bool[] boolOptions = {true, false};
        private ProtoClass myClass = new ProtoClass();

        public MainWindow()
        {
            InitializeComponent();

            DataContext = myClass;

            ProtoCombo.IsEditable = false;
            ProtoCombo.IsTextSearchEnabled = false;
            //ProtoCombo.ItemsSource = comboOptions;
            //ProtoCombo.ItemsSource = boolOptions;
            ProtoCombo.DataContext = myClass;
            ProtoCombo.ItemsSource = myClass.YesNoOptions;
            ProtoCombo.SelectedIndex = 0;


            Binding yesNoBinding = new Binding("SelectedOption")
            {
                Mode = BindingMode.TwoWay,
                Source = myClass.SelectedOption,
                Converter = new StringToBoolConverter(),
                //Converter = new BoolToStringConverter(),
            };

            ProtoCombo.SetBinding(ComboBox.SelectedItemProperty, yesNoBinding);
        }

        private void ProtoCombo_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DisplayLabel.Content = string.Format("Value is {0}/{1}", e.AddedItems.Count > 0 ? e.AddedItems[0] : "NULL", myClass.SelectedOption);
        }
    }

    internal class ProtoClass : INotifyPropertyChanged
    {
        public IEnumerable<string> YesNoOptions
        {
            get { return new string[] {"Yes", "No"}; }

        }

        private bool _selection;
        public bool SelectedOption
        { 
            get { return _selection; }
            set
            {
                _selection = value;
                OnPropertyChanged("SelectedOption");
            }
        }

        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }

    internal class StringToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool result = ((string)value).Equals("Yes", StringComparison.CurrentCultureIgnoreCase);
            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string result = ((bool)value) ? "Yes" : "No";
            return result;
        }
    }

    internal class BoolToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return ((bool)value) ? "Yes" : "No";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return ((string)value).Equals("Yes", StringComparison.CurrentCultureIgnoreCase);
        }
    }
}
