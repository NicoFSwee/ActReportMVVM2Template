using ActReport.ViewModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace ActReport.UI
{
  /// <summary>
  /// Interaction logic for App.xaml
  /// </summary>
  public partial class App : Application
  {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            IController controller = new MainController();
            controller.ShowWindow(new EmployeeViewModel(controller));
        }
  }
}
