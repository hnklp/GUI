using Microsoft.EntityFrameworkCore;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using GUI.Data;

namespace GUI;

public class MainWindowViewModel : ObservableObject
{
    public ObservableCollection<Person> Customers { get; }

    public MainWindowViewModel(CustomerContext context)
    {
        context.Customers.Load();
        Customers = context.Customers.Local.ToObservableCollection();
    }

}
