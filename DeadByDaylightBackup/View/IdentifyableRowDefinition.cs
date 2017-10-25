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
using DeadByDaylightBackup.Data;
using DeadByDaylightBackup.Interface;
using DeadByDaylightBackup.Utility;
using DeadByDaylightBackup.View;

namespace DeadByDaylightBackup.View
{
    public abstract class IdentifyableRowDefinition<T> : RowDefinition, IDisposable where T : Identifyable
    {

        public const int IMaxHeight = 40;
        public T Identity
        {
            get;
            private set;
        }

        protected IdentifyableRowDefinition(T input)
        {
            Identity = input;
        }

        public void Dispose()
        {
            Dispose(true);
        }
        protected abstract void Dispose(bool final);

        public int RowNumber
        {
            get
            {
                return (int)this.GetValue(Grid.RowProperty);
            }
            set
            {
                SetValue(Grid.RowProperty, value);
                SetRow(value);
            }
        }
        protected abstract void SetRow(int value);

    }
}
