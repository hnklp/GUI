using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Threading.Tasks;
using System.Windows.Data;
using EFCore.Data;
using EFCore.Generator;
using Microsoft.EntityFrameworkCore;

namespace EFCore
{
    public class MainWindowViewModel
    {
        public ObservableCollection<Mob> Mobs { get; } = new();
        public MobsContext Context { get; }

        public MainWindowViewModel(MobsContext context)
        {
            Context = context ?? throw new ArgumentNullException(nameof(context));
            Mobs.CollectionChanged += Mobs_CollectionChanged;
        }


        public void LoadData()
        {
            foreach(var mob in Context.Mobs)
            {
                Mobs.Add(mob);
            }
        }

        public void SaveChanges()
        {
            Context.SaveChanges();
        }

        private void Mobs_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            CollectionViewSource.GetDefaultView(Mobs).Refresh();
        }

        public void DeleteAllMobs()
        {
            foreach (var mob in Mobs)
            {
                Context.Mobs.Remove(mob);
            }
            Mobs.Clear();
            Context.SaveChanges();
        }

        public void GenerateMobs()
        {
            foreach (var mob in DataGenerator.GenerateMobs(20))
            {
                mob.MobId = 0;
                Context.Mobs.Add(mob);
            }
            Context.SaveChanges();
            LoadData();
        }
    }
}
