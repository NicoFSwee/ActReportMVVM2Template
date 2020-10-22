using ActReport.Core.Contracts;
using ActReport.Core.Entities;
using ActReport.Persistence;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace ActReport.ViewModel
{
    public class ActivityViewModel : BaseViewModel
    {
        private Employee _employee;
        private ObservableCollection<Activity> _activities;
        private Activity _selectedActivity;

        public Activity SelectedActivity 
        {
            get => _selectedActivity;
            set
            {
                _selectedActivity = value;
                OnPropertyChanged(nameof(SelectedActivity));
            }
        }

        public ObservableCollection<Activity> Activities
        {
            get => _activities;
            set
            {
                _activities = value;
                OnPropertyChanged(nameof(Activities));
            }
        }

        public string FullName => $"{_employee.FirstName} {_employee.LastName}";

        public ActivityViewModel(IController controller, Employee employee) : base(controller)
        {
            _employee = employee;
            using IUnitOfWork unitOfWork = new UnitOfWork();
            Activities = new ObservableCollection<Activity>(unitOfWork.ActivityRepository.Get(
                filter: x => x.Employee.Id == employee.Id,
                orderBy: coll => coll.OrderBy(activity => activity.Date).ThenBy(activity => activity.StartTime)));
            LoadActivites();
        }

        private void LoadActivites()
        {
            using(IUnitOfWork uow = new UnitOfWork())
            {
                Activities = new ObservableCollection<Activity>(
                    uow.ActivityRepository.Get(filter: p => p.Employee_Id == _employee.Id,
                    orderBy: coll => coll.OrderBy(act => act.Date).ThenBy(act => act.StartTime)));
            }
            Activities.CollectionChanged += Activities_CollectionChanged;
        }

        private void Activities_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if(e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove)
            {
                using (UnitOfWork uow = new UnitOfWork())
                {
                    foreach(var item in e.OldItems)
                    {
                        uow.ActivityRepository.Delete((item as Activity).Id);
                    }
                    uow.Save();
                }
            }
        }

        private ICommand _cmdNewActivity;
        public ICommand CmdNewActivity
        {
            get
            {
                if(_cmdNewActivity == null)
                {
                    _cmdNewActivity = new RelayCommand(
                        execute: _ => 
                        {
                            _controller.CloseWindow(this);
                            _controller.ShowWindow(new NewEditActivityViewModel(_controller, _employee));
                        },
                        canExecute: _ => _employee != null);
                }
                return _cmdNewActivity;
            }
        }

        private ICommand _cmdDeleteActivity;
        public ICommand CmdDeleteActivity
        {
            get
            {
                if(_cmdDeleteActivity == null)
                {
                    _cmdDeleteActivity = new RelayCommand(
                        execute: _ =>
                        {
                            using (IUnitOfWork uow = new UnitOfWork())
                            {
                                uow.ActivityRepository.Delete(SelectedActivity);
                                uow.Save();
                            }
                            LoadActivites();
                        },
                        canExecute: _ => SelectedActivity != null);
                        
                }
                return _cmdDeleteActivity;
            }
        }

        private ICommand _cmdEditActivity;
        public ICommand CmdEditActivity
        {
            get
            {
                if(_cmdEditActivity == null)
                {
                    _cmdEditActivity = new RelayCommand(
                        execute: _ => 
                        {
                            _controller.CloseWindow(this);
                            _controller.ShowWindow(new NewEditActivityViewModel(_controller, _employee, SelectedActivity));
                        },
                        canExecute: _ => SelectedActivity != null);
                }
                return _cmdEditActivity;
            }
        }

        private ICommand _cmdClose;
        public ICommand CmdClose
        {
            get
            {
                if(_cmdClose == null)
                {
                    _cmdClose = new RelayCommand(
                        execute: _ => _controller.CloseWindow(this),
                        canExecute: _ => true);   
                }
                return _cmdClose;
            }
        }
    }
}
