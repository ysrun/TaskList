using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace TaskList.BaseClass
{
    public class PropertyChangeBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public void RaisePropertyChanged([CallerMemberName] string PropertyName = null)
        {
            if(PropertyChanged != null)
                PropertyChanged(this,new PropertyChangedEventArgs(PropertyName));
        }
    }
}
