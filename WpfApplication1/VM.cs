using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace WpfApplication1
{
    class VM:ViewModelBase
    {
        private string _ip;

        public VM()
        {
            IP = "1.23.4.22";
            Command=new RelayCommand(ExecuteCommand);
        }

        public RelayCommand Command { get; }

        private void ExecuteCommand()
        {
            if (IP == "1.2.3.4")
            {
                IP = "2.3.4.5";
            }
            else
            {
                IP = "1.2.3.4";
            }
        }

        public string IP
        {
            set { Set(ref _ip , value); }
            get { return _ip; }
        }
    }

    class MyClass
    {
        
    }
}
