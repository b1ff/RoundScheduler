using System.ComponentModel;

namespace RoundScheduler.Model
{
    public abstract class BasePropertyChanged : INotifyPropertyChanged
    {
        protected void InvokePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
