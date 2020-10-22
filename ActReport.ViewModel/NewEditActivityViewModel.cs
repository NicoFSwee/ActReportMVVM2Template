using ActReport.Core.Contracts;
using ActReport.Core.Entities;
using ActReport.Persistence;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace ActReport.ViewModel
{
    public class NewEditActivityViewModel : BaseViewModel
    {
        private Employee _employee;
        private Activity _activity;
        private DateTime _date;
        private DateTime _startTime;
        private DateTime _endTime;
        private string _activityText;

        public string HeaderText { get; set; }

        public NewEditActivityViewModel(IController controller, Employee employee, Activity activity = null) : base(controller)
        {
            _controller = controller;
            _employee = employee;

            if (activity == null)
            {  
                Date = DateTime.Now;
                StartTime = DateTime.Now;
                EndTime = StartTime.AddHours(1);
                HeaderText = "Neue Tätigkeit anlegen für:";
            }
            else
            {
                Date = activity.Date;
                StartTime = activity.StartTime;
                EndTime = activity.EndTime;
                ActivityText = activity.ActivityText;
                _activity = activity;
                HeaderText = "Tätigkeit bearbeiten von:";
            }
        }

        public string FullName => $"{_employee.FirstName} {_employee.LastName}";

        public DateTime Date
        {
            get => _date;
            set
            {
                _date = value;
                OnPropertyChanged(nameof(Date));
            }
        }
        public DateTime StartTime
        {
            get => _startTime;
            set
            {
                _startTime = value;
                OnPropertyChanged(nameof(StartTime));
            }
        }
        public DateTime EndTime
        {
            get => _endTime;
            set
            {
                _endTime = value;
                OnPropertyChanged(nameof(EndTime));
            }
        }

        public string ActivityText
        {
            get => _activityText;
            set
            {
                _activityText = value;
                OnPropertyChanged(nameof(ActivityText));
            }
        }

        private ICommand _cmdSaveActivity;
        public ICommand CmdSaveActivity
        {
            get
            {
                if(_cmdSaveActivity == null && _activity == null)
                {
                    _cmdSaveActivity = new RelayCommand(
                        execute: _ =>
                        {
                            using IUnitOfWork uow = new UnitOfWork();
                            Activity newActivity = new Activity()
                            {
                                Date = Date,
                                StartTime = StartTime,
                                EndTime = EndTime,
                                ActivityText = ActivityText,
                                Employee_Id = _employee.Id
                            };
                            uow.ActivityRepository.Insert(newActivity);
                            uow.Save();

                            _controller.CloseWindow(this);
                            _controller.ShowWindow(new ActivityViewModel(_controller, _employee));
                        },
                        canExecute: _ => _employee != null);
                }
                else if(_cmdSaveActivity == null && _activity != null)
                {
                    _cmdSaveActivity = new RelayCommand(
                        execute: _ =>
                        {
                            using IUnitOfWork uow = new UnitOfWork();
                            _activity.Date = Date;
                            _activity.StartTime = StartTime;
                            _activity.EndTime = EndTime;
                            _activity.ActivityText = ActivityText;
                            uow.ActivityRepository.Update(_activity);
                            uow.Save();

                            _controller.CloseWindow(this);
                            _controller.ShowWindow(new ActivityViewModel(_controller, _employee));
                        },
                        canExecute: _ => _employee != null);
                }
                return _cmdSaveActivity;
            }
        }

        private ICommand _cmdCancleOperation;
        public ICommand CmdCancleOperation
        {
            get
            {
                if(_cmdCancleOperation == null)
                {
                    _cmdCancleOperation = new RelayCommand(
                        execute: _ =>
                        {
                            _controller.CloseWindow(this);
                            _controller.ShowWindow(new ActivityViewModel(_controller, _employee));
                        },
                        canExecute: _ => true);
                }
                return _cmdCancleOperation;
            }
        }
    }
}
