using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Media;
using System.Text.RegularExpressions;
using System.Windows.Data;
using IPMaskedTextBox.Annotations;

namespace IPmaskedtextbox
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class IPMaskedTextBox : UserControl, INotifyPropertyChanged
    {
        private const string ErrorMessage = "Please specify a value between 0 and 255.";

        private const string IpRegex =
            @"^(25[0-5]|2[0-4]\d|[0-1]\d{2}|[1-9]?\d)\.(25[0-5]|2[0-4]\d|[0-1]\d{2}|[1-9]?\d)\.(25[0-5]|2[0-4]\d|[0-1]\d{2}|[1-9]?\d)\.(25[0-5]|2[0-4]\d|[0-1]\d{2}|[1-9]?\d)$";
        public static readonly DependencyProperty IPProperty = DependencyProperty.Register(
            "IP", typeof(string), typeof(IPMaskedTextBox), new PropertyMetadata(default(string), IPPropertyChanged));

        private static bool _doCallBacked = true;
        private static bool _updateIP = true;
        private string _first;
        private string _second;
        private string _third;
        private string _fourth;

        private static void IPPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!_doCallBacked) return;
            Regex regex=new Regex(IpRegex);
            if(!regex.IsMatch((string)e.NewValue))
            {
                //MessageBox.Show("error ip");
                return;
            }
            var str = ((string) e.NewValue).Split('.');
            if (str.Length == 4)
            {
                _updateIP = false;
                for (int i = 0; i < 4; i++)
                {
                    switch (i)
                    {
                        case 0:
                            ((IPMaskedTextBox) (d)).First = str[0];
                            break;
                        case 1:
                            ((IPMaskedTextBox) (d)).Second = str[1];
                            break;
                        case 2:
                            ((IPMaskedTextBox) (d)).Third = str[2];
                            break;
                        case 3:
                            ((IPMaskedTextBox) (d)).Fourth = str[3];
                            break;
                    }
                }

                _updateIP = true;
            }
        }

        private void UpdateIP(int index, string ip, string value)
        {
            if (!_updateIP) return;

            BindingExpression mes = GetBindingExpression(IPProperty);
            if (mes == null) return;
            _doCallBacked = false;

            if (string.IsNullOrEmpty(ip))
            {
                ip = "...";
            }

            var ips = ip.Split('.');
            ips[index] = value;
            var s = mes.GetType().GetProperty("ResolvedSourcePropertyName").GetValue(mes, null);
            var type = mes.DataItem.GetType();
            var bindProperty = type.GetProperty((string) s);
            bindProperty.SetValue(mes.DataItem, string.Join(".", ips), null);
            _doCallBacked = true;
        }

        public string First
        {
            get { return _first; }
            set
            {
                if (_first != value)
                {
                    _first = value;
                    OnPropertyChanged("First");
                    UpdateIP(0, IP, value);
                }
            }
        }

        public string Second
        {
            get { return _second; }
            set
            {
                if (_second != value)
                {
                    _second = value;
                    OnPropertyChanged("Second");
                    UpdateIP(1, IP, value);
                }
            }
        }

        public string Third
        {
            get { return _third; }
            set
            {
                if (_third != value)
                {
                    _third = value;
                    OnPropertyChanged("Third");
                    UpdateIP(2, IP, value);
                }
            }
        }

        public string Fourth
        {
            get { return _fourth; }
            set
            {
                if (_fourth != value)
                {
                    _fourth = value;
                    OnPropertyChanged("Fourth");
                    UpdateIP(3, IP, value);
                }
            }
        }

        public string IP
        {
            get { return (string) GetValue(IPProperty); }
            set { SetValue(IPProperty, value); }
        }

        public IPMaskedTextBox()
        {
            InitializeComponent();
            var bind1 = new Binding();
            bind1.Path = new PropertyPath("First");
            bind1.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            bind1.Mode = BindingMode.TwoWay;
            firstBox.SetBinding(TextBox.TextProperty, bind1);
            firstBox.DataContext = this;

            var bind2 = new Binding();
            bind2.Path = new PropertyPath("Second");
            bind2.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            secondBox.SetBinding(TextBox.TextProperty, bind2);
            secondBox.DataContext = this;

            var bind3 = new Binding();
            bind3.Path = new PropertyPath("Third");
            bind3.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            thirdBox.SetBinding(TextBox.TextProperty, bind3);
            thirdBox.DataContext = this;

            var bind4 = new Binding();
            bind4.Path = new PropertyPath("Fourth");
            bind4.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            fourthBox.SetBinding(TextBox.TextProperty, bind4);
            fourthBox.DataContext = this;
        }


        #region private methods

        private void jumpRight(TextBox rightNeighborBox, KeyEventArgs e)
        {
            rightNeighborBox.Focus();
            rightNeighborBox.CaretIndex = 0;
            e.Handled = true;
        }

        private void jumpLeft(TextBox leftNeighborBox, KeyEventArgs e)
        {
            leftNeighborBox.Focus();
            if (leftNeighborBox.Text != "")
            {
                leftNeighborBox.CaretIndex = leftNeighborBox.Text.Length;
            }

            e.Handled = true;
        }

        //checks for backspace, arrow and decimal key presses and jumps boxes if needed.
        //returns true when key was matched, false if not.
        private bool checkJumpRight(TextBox currentBox, TextBox rightNeighborBox, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Right:
                    if (currentBox.CaretIndex == currentBox.Text.Length || currentBox.Text == "")
                    {
                        jumpRight(rightNeighborBox, e);
                    }

                    return true;
                case Key.OemPeriod:
                case Key.Decimal:
                case Key.Space:
                    jumpRight(rightNeighborBox, e);
                    rightNeighborBox.SelectAll();
                    return true;
                default:
                    return false;
            }
        }

        private bool checkJumpLeft(TextBox currentBox, TextBox leftNeighborBox, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Left:
                    if (currentBox.CaretIndex == 0 || currentBox.Text == "")
                    {
                        jumpLeft(leftNeighborBox, e);
                    }

                    return true;
                case Key.Back:
                    if ((currentBox.CaretIndex == 0 || currentBox.Text == "") && currentBox.SelectionLength == 0)
                    {
                        jumpLeft(leftNeighborBox, e);
                    }

                    return true;
                default:
                    return false;
            }
        }

        //discards non digits, prepares IPMaskedBox for textchange.
        private void handleTextInput(TextBox currentBox, TextBox rightNeighborBox, TextCompositionEventArgs e)
        {
            if (!char.IsDigit(Convert.ToChar(e.Text)))
            {
                e.Handled = true;
                SystemSounds.Beep.Play();
                return;
            }

            if (currentBox.Text.Length == 3 && currentBox.SelectionLength == 0)
            {
                e.Handled = true;
                SystemSounds.Beep.Play();
                if (currentBox != fourthBox)
                {
                    rightNeighborBox.Focus();
                    rightNeighborBox.SelectAll();
                }
            }
        }

        //checks whether textbox content > 255 when 3 characters have been entered.
        //clears if > 255, switches to next textbox otherwise 
        private void handleTextChange(TextBox currentBox, TextBox rightNeighborBox)
        {
            if (currentBox.Text.Length == 3)
            {
                try
                {
                    Convert.ToByte(currentBox.Text);

                }
                catch (Exception exception) when (exception is FormatException || exception is OverflowException)
                {
                    currentBox.Clear();
                    currentBox.Focus();
                    SystemSounds.Beep.Play();
                    MessageBox.Show(ErrorMessage, "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (currentBox.CaretIndex != 2 && currentBox != fourthBox)
                {
                    rightNeighborBox.CaretIndex = rightNeighborBox.Text.Length;
                    rightNeighborBox.SelectAll();
                    rightNeighborBox.Focus();
                }
            }
        }

        #endregion

        #region Events

        //jump right, left or stay. 
        private void firstByte_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            checkJumpRight(firstBox, secondBox, e);
        }

        private void secondByte_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (checkJumpRight(secondBox, thirdBox, e))
                return;

            checkJumpLeft(secondBox, firstBox, e);
        }

        private void thirdByte_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (checkJumpRight(thirdBox, fourthBox, e))
                return;

            checkJumpLeft(thirdBox, secondBox, e);
        }

        private void fourthByte_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            checkJumpLeft(fourthBox, thirdBox, e);

            if (e.Key == Key.Space)
            {
                SystemSounds.Beep.Play();
                e.Handled = true;
            }
        }


        //discards non digits, prepares IPMaskedBox for textchange.
        private void firstByte_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            handleTextInput(firstBox, secondBox, e);
        }

        private void secondByte_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            handleTextInput(secondBox, thirdBox, e);
        }

        private void thirdByte_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            handleTextInput(thirdBox, fourthBox, e);
        }

        private void fourthByte_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            handleTextInput(fourthBox, fourthBox, e); //pass fourthbyte twice because no right neighboring box.
        }


        //checks whether textbox content > 255 when 3 characters have been entered.
        //clears if > 255, switches to next textbox otherwise 
        private void firstByte_TextChanged(object sender, TextChangedEventArgs e)
        {
            handleTextChange(firstBox, secondBox);
        }

        private void secondByte_TextChanged(object sender, TextChangedEventArgs e)
        {
            handleTextChange(secondBox, thirdBox);
        }

        private void thirdByte_TextChanged(object sender, TextChangedEventArgs e)
        {
            handleTextChange(thirdBox, fourthBox);
        }

        private void fourthByte_TextChanged(object sender, TextChangedEventArgs e)
        {
            handleTextChange(fourthBox, fourthBox);
        }

        #endregion

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
