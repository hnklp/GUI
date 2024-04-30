using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Data;
using EFCore.Data;

namespace EFCore
{
    public class MainWindowViewModel
    {
        public ObservableCollection<Mob> Mobs { get; } = new();
        public MobsContext Context { get; }

        public MainWindowViewModel(MobsContext context)
        {
            BindingOperations.EnableCollectionSynchronization(Mobs, new object());
            Context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task LoadData()
        {
            await foreach(var mob in Context.Mobs.AsAsyncEnumerable().ConfigureAwait(false))
            {
                //Just simulating a slow query
                await Task.Delay(100).ConfigureAwait(false);
                Mobs.Add(mob);
            }
        }
    }
}
