using GUI.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Toolkit.Mvvm.Messaging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace GUI;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    [STAThread]
    public static void Main(string[] args)
    {
        using IHost host = CreateHostBuilder(args).Build();
        host.Start();

        App app = new();
        app.InitializeComponent();

        using (var scope = host.Services.CreateScope())
        using (var dbContext = scope.ServiceProvider.GetRequiredService<CustomerContext>())
        {
            dbContext.Database.Migrate();

            Task.Run(async () => await PopulateDatabaseAsync(dbContext)).Wait();
        }

        app.MainWindow = host.Services.GetRequiredService<MainWindow>();
        app.MainWindow.Visibility = Visibility.Visible;
        app.Run();
    }

    private static async Task PopulateDatabaseAsync(CustomerContext context)
    {
        if (await context.Customers.AnyAsync())
        {
            return;
        }

        //foreach (var p in testdata.data.generatepeople(200))
        //{
        //    person person = new()
        //    {
        //        firstname = p.firstname,
        //        lastname = p.lastname,
        //        dateofbirth = p.dob
        //    };
        //    person.orders = new list<order>
        //    {
        //        new order
        //        {
        //            submittedat = p.dob
        //        },
        //        new order(),
        //    };
        //    context.customers.add(person);
        //}
        //context.savechanges();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
    Host.CreateDefaultBuilder(args)
        .ConfigureAppConfiguration((hostBuilderContext, configurationBuilder) =>
        {

        })
        .ConfigureServices((hostContext, services) =>
        {
            services.AddSingleton<MainWindow>();
            services.AddSingleton<MainWindowViewModel>();
            services.AddSingleton<WeakReferenceMessenger>();
            services.AddSingleton<IMessenger, WeakReferenceMessenger>(provider => provider.GetRequiredService<WeakReferenceMessenger>());

            services.AddSingleton<Dispatcher>(_ => Current.Dispatcher);

            services.AddDbContext<CustomerContext>(
                options =>
                {
                    options.UseSqlite("Data Source=customers.db");
                });
        });
}