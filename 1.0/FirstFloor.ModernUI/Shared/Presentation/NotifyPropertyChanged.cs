using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;

namespace FirstFloor.ModernUI.Presentation
{
    /// <summary>
    ///     The base implementation of the INotifyPropertyChanged contract.
    /// </summary>
    public abstract class NotifyPropertyChanged
        : DependencyObject, INotifyPropertyChanged
    {
        /// <summary>
        ///     Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        ///     Raises the PropertyChanged event.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        protected virtual void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

#if !NET4
        /// <summary>
        ///     Updates specified value, and raises the <see cref="PropertyChanged" /> event when the value has changed.
        /// </summary>
        /// <typeparam name="T">The type of the value.</typeparam>
        /// <param name="storage">The current stored value</param>
        /// <param name="value">The new value</param>
        /// <param name="propertyName">The optional property name, automatically set to caller member name when not set.</param>
        /// <returns>Indicates whether the value has changed.</returns>
        protected bool Set<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (!Equals(storage, value))
            {
                storage = value;
                OnPropertyChanged(propertyName);
                return true;
            }
            return false;
        }
#endif
    }
}