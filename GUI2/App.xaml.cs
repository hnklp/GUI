using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Windows;
using EFCore.Data;
using EFCore.Generator;
using System.Reflection;

namespace EFCore
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {

            using(var context= new MobsContext())
            {
                context.Database.Migrate();

                context.Add(new Species
                {
                    Title = "Creeper",
                    Hostility = HostilityLevel.Hostile,
                });
                context.Add(new Species
                {
                    Title = "Zombie",
                    Hostility = HostilityLevel.Hostile,
                });
                context.SaveChanges();

                if (!context.Mobs.Any())
                {
                    foreach (var mob in DataGenerator.GenerateMobs(20))
                    {
                        mob.MobId = 0;
                        context.Mobs.Add(mob);
                    }
                    context.SaveChanges();
                }
            }

            base.OnStartup(e);
        }
    }
}
