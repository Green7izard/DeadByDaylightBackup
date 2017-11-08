using DeadByDaylightBackup.Data;
using System;
using System.Windows.Controls;

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
