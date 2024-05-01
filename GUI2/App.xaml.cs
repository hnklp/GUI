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

                addPresetSpecies(context);

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

        public static void addPresetSpecies(MobsContext context)
        {

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

            context.Add(new Species
            {
                Title = "Player",
                Hostility = HostilityLevel.FightsBack,
            });

            context.Add(new Species
            {
                Title = "Giant",
                Hostility = HostilityLevel.Hostile,
            });

            context.Add(new Species
            {
                Title = "Snow Golem",
                Hostility = HostilityLevel.Harmless,
            });

            context.Add(new Species
            {
                Title = "The Killer Bunny",
                Hostility = HostilityLevel.Hostile,
            });

            context.Add(new Species
            {
                Title = "Ocelot",
                Hostility = HostilityLevel.Harmless,
            });

            context.Add(new Species
            {
                Title = "Mob",
                Hostility = HostilityLevel.Harmless,
            });

            context.Add(new Species
            {
                Title = "Pink Wither",
                Hostility = HostilityLevel.Harmless,
            });

            context.Add(new Species
            {
                Title = "Spider Jockey",
                Hostility = HostilityLevel.Hostile,
            });

            context.SaveChanges();
        }
    }
}
