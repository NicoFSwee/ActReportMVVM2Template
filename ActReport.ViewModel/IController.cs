using System;
using System.Collections.Generic;
using System.Text;

namespace ActReport.ViewModel
{
    public interface IController
    {
        void ShowWindow(BaseViewModel baseViewModel);
        void CloseWindow(BaseViewModel baseViewModel);
    }
}
