using System.ComponentModel;

namespace ActReport.ViewModel
{
  public class BaseViewModel : INotifyPropertyChanged
  {
        protected IController _controller;

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName)
    {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

        public BaseViewModel(IController controller)
        {
            _controller = controller;
        }
  }
}
