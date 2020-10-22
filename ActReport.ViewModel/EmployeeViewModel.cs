using ActReport.Core.Contracts;
using ActReport.Core.Entities;
using ActReport.Persistence;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace ActReport.ViewModel
{
  public class EmployeeViewModel : BaseViewModel
  {
    private string _firstName; // Eingabefeld Vorname
    private string _lastName;  // Eingabefeld Nachname
    private Employee _selectedEmployee; // Aktuell ausgewählter Mitarbeiter
    private ObservableCollection<Employee> _employees; // Liste aller Mitarbeiter

    public string FirstName
    {
      get => _firstName;
      set
      {
        _firstName = value;
        OnPropertyChanged(nameof(FirstName));
      }
    }

    public string LastName
    {
      get => _lastName;
      set
      {
        _lastName = value;
        OnPropertyChanged(nameof(LastName));
      }
    }

    public Employee SelectedEmployee
    {
      get => _selectedEmployee;
      set
      {
        _selectedEmployee = value;
        FirstName = _selectedEmployee?.FirstName;
        LastName = _selectedEmployee?.LastName;
        OnPropertyChanged(nameof(SelectedEmployee));
      }
    }

    public ObservableCollection<Employee> Employees
    {
      get => _employees;
      set
      {
        _employees = value;
        OnPropertyChanged(nameof(Employees));
      }
    }

    public EmployeeViewModel(IController controller) : base(controller)
    {
      LoadEmployees();
    }

    private void LoadEmployees()
    {
      using IUnitOfWork uow = new UnitOfWork();
      var employees = uow.EmployeeRepository
        .Get(orderBy: (coll) => coll.OrderBy(emp => emp.LastName))
        .ToList();

      Employees = new ObservableCollection<Employee>(employees);
    }

    // Commands
    private ICommand _cmdSaveChanges;
    public ICommand CmdSaveChanges
    {
      get
      {
        if (_cmdSaveChanges == null)
        {
          _cmdSaveChanges = new RelayCommand(
            execute: _ =>
            {
              using IUnitOfWork uow = new UnitOfWork();
              _selectedEmployee.FirstName = _firstName;
              _selectedEmployee.LastName = _lastName;
              uow.EmployeeRepository.Update(_selectedEmployee);
              uow.Save();

              LoadEmployees();
            },
            canExecute: _ => _selectedEmployee != null);
        }

        return _cmdSaveChanges;
      }
      set { _cmdSaveChanges = value; }
    }

        private ICommand _cmdEditActivities;
        public ICommand CmdEditActivities
        {
            get
            {
                if(_cmdEditActivities == null)
                {
                    _cmdEditActivities = new RelayCommand(
                        execute: _ => _controller.ShowWindow(new ActivityViewModel(_controller, SelectedEmployee)),
                        canExecute: _ => SelectedEmployee != null);
                }
                return _cmdEditActivities;
            }
        }
  }
}
