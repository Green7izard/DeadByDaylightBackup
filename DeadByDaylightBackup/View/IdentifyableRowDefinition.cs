using DeadByDaylightBackup.Data;
using System.Windows.Controls;

namespace DeadByDaylightBackup.View
{
    /// <summary>
    /// Class that is the base for grid rows of specific types!
    /// </summary>
    /// <typeparam name="T">Type of Identifiyable</typeparam>
    public abstract class IdentifyableRowDefinition<T> : RowDefinition where T : Identifyable
    {
        /// <summary>
        /// Max hight of the Rows and its Contents
        /// </summary>
        public const double MaxRowHeight = 30;

        /// <summary>
        /// The identity of the Row
        /// </summary>
        public T Identity
        {
            get;
        }

        /// <summary>
        /// Create the row
        /// </summary>
        /// <param name="input">The identifyable at the base</param>
        protected IdentifyableRowDefinition(T input)
        {
            Identity = input;
            MaxHeight = MaxRowHeight;
        }

        /// <summary>
        /// RowNumber in the Datagrid!
        /// </summary>
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

        /// <summary>
        /// Abstract function to update rowNumbers
        /// </summary>
        /// <param name="value">The new rownumber to set</param>
        protected abstract void SetRow(int value);
    }
}
