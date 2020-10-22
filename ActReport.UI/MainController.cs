using ActReport.ViewModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace ActReport.UI
{
    public class MainController : IController
    {
        private Dictionary<BaseViewModel, Window> _openWindows;

        public MainController()
        {
            _openWindows = new Dictionary<BaseViewModel, Window>();
        }

        public void CloseWindow(BaseViewModel baseViewModel)
        {
            if(_openWindows.ContainsKey(baseViewModel))
            {
                _openWindows[baseViewModel].Close();
                _openWindows.Remove(baseViewModel);
            }
        }

        public void ShowWindow(BaseViewModel baseViewModel)
        {
            Window window = baseViewModel switch
            {
                null => throw new ArgumentNullException(nameof(baseViewModel)),

                EmployeeViewModel _ => new MainWindow(),
                ActivityViewModel _ => new ActivityWindow(),
                NewEditActivityViewModel _ => new NewEditActivityWindow(),

                _ => throw new InvalidOperationException($"Unknown ViewModel of type '{baseViewModel}'")
            };

            _openWindows.Add(baseViewModel, window);

            window.DataContext = baseViewModel;
            window.ShowDialog();
        }
    }
}
