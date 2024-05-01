
# ⛏ Aplikace pro správu databáze našich minecraft mobů

... povídání o EF


## Vytvoření nového projektu 

Prvním krokem je vytvoření nového projektu ve Visual Studiu. Zvolíme WPF (Windows Presentation Foundation) jako typ projektu. Budeme používat .NET 8.

Budeme potřebovat nainstalovat několik balíčků:
- Microsoft.EntityFrameworkCore.Sqlite
- Microsoft.EntityFrameworkCore.Design
- Bogus


## Model
Při práci s Entity Frameworkem je model klíčovou součástí. Model nám poskytuje abstrakci nad naší databází a definuje, jak jsou naše entity reprezentovány v našem kódu.

V rámci Entity Frameworku je model soubor tříd, které představují entity v naší databázi. Tyto třídy obsahují vlastnosti, které odpovídají sloupcům v tabulkách naší databáze, a definují vztahy mezi těmito entitami.

#### Povinnost atributů
Atribut je povinný a nebo ne podle toho, zda proměnná může obsahovat null. 

#### Vztahy mezi entitami
Ukázka vztahu 1:N

```csharp
public class Author
{
    public int AuthorId { get; set; }
    public string Name { get; set; }
    
    // Navigační vlastnost pro knihy tohoto autora
    public ICollection<Book> Books { get; set; }
}

public class Book
{
    public int BookId { get; set; }
    public string Title { get; set; }
    
    // Cizí klíč pro spojení s autorem
    public int AuthorId { get; set; }
    public Author Author { get; set; }
}
```

A teď zpátky k naší databázi...

Vytvoříme složku "Data" a v ní soubor se jménem "mobs.cs".
Tento soubor bude obsahovat definici třídy pro naši entitu "Mob".

```csharp
public class Mob
    {
        public int MobId { get; set; }
        [Required]
        public string Name { get; set; }
        public DateTime DateOfCapture { get; set; } = DateTime.Now;
    }
```

## Datový kontext

Předtím než připravíme uživatelské rozhraní (GUI) pro zobrazení dat, je potřeba nejdříve propojit několik vrstev naší aplikace. První vrstvou, kterou musíme připravit, je datový kontext.

Datový kontext je základní součástí Entity Frameworku. Jedná se o prostředek pro komunikaci s databází a poskytuje sadu vlastností a metod pro práci s daty. Proč ho potřebujeme?

Datový kontext nám umožňuje pracovat s entitami (objekty) a provádět operace jako je načítání, ukládání, aktualizace a odstraňování dat z databáze. Také nám umožňuje definovat konfigurace pro mapování mezi objekty a tabulkami v databázi.

Nyní vytvoříme soubor MobsContext.cs uvnitř složky Data a v něm definujeme náš datový kontext:

```csharp
public class MobsContext : DbContext
{
    public const string FileName = "data.db";

    public MobsContext()
        : this(new DbContextOptionsBuilder<MobsContext>().UseSqlite($"Data Source={FileName}").Options)
    { }

    public MobsContext(DbContextOptions options)
    : base(options)
    { }

    public DbSet<Mob> Mobs => Set<Mob>();
}
```

Pokračuje s viewmodelem
Něco málo k MVVM architektuře...
Vytvoříme pro to soubor MainWindowViewModel.cs
Pozor na import dat

```
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

    private void Mobs_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
{
    CollectionViewSource.GetDefaultView(Mobs).Refresh();
}
}
```

Teď už se vrhneme na to GUI 
Vysvětlit datagrid, ukázat co umí a různý atributy
To spíš později teď to ještě nepůjde spustit
Pozor na local a class
```
<Window x:Class="EFCore.MainWindow"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
        xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
        xmlns:local="clr-namespace:EFCore"
        mc:Ignorable="d"
        
        Title="MainWindow" Height="450" Width="800"
        
        d:DataContext="{d:DesignInstance Type={x:Type local:MainWindowViewModel}}">
    
    <Grid>
        <DataGrid x:Name="MobsGrid"  ItemsSource="{Binding Mobs}"/>
    </Grid>
</Window>
```


Model je spojený s GUI, ale ještě nám něco chybí. 

MainWindow.xaml.cs
Opět import dat 

```
public partial class MainWindow : Window
{
    private MainWindowViewModel ViewModel { get; }

    public MainWindow()
    {
        InitializeComponent();
        ViewModel = new MainWindowViewModel(new MobsContext());
        DataContext = ViewModel;

        Loaded += MainWindow_Loaded;
    }

    private void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
        ViewModel.LoadData();
    }
}
```

App.xaml.cs

```
public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        
        using(var context= new MobsContext())
        {
            context.Database.Migrate();
        }

        base.OnStartup(e);
    }
}
```


Teď už to doufám bude spustitelné. 
Teď si předvedeme ten datagrid a co všechno umí

Vytvoříme databázi a řekneme si něco o migracích

No, počkat, ale my nemáme zatím žádné moby.
Nebudeme to psát ručně. Necháme si to vygenerovat.

Složka Generator
Soubor Generator.cs
import dat

```
class DataGenerator
{
    public static IList<Mob> GenerateMobs(int count)
    {
        Faker<Mob> generator = new Faker<Mob>()
            .StrictMode(true)
            .RuleFor(x => x.MobId, f => f.IndexGlobal)
            .RuleFor(x => x.Name, f => f.Person.FirstName)
            .RuleFor(x => x.DateOfCapture, f => f.Person.DateOfBirth)

        return generator.Generate(count);
    }
}
```

Generator budeme volat v App.xaml.cs

Přidáme 

```
if (!context.Mobs.Any())
{
    foreach (var mob in DataGenerator.GenerateMobs(20))
    {
        mob.MobId = 0;
        context.Mobs.Add(mob);
    }
    context.SaveChanges();
}
```

Máme data v datagridu a dokonce je můžeme měnit. Jenomže se nám to neukládá. 
Přidáme tlačítko na ukládání.

Asi samostatné cvičení přidat tlačítka 

MainWindow.xaml
```
 <Button x:Name="SaveButton" Content="Save" HorizontalAlignment="Left" Margin="690,2,0,0" VerticalAlignment="Top" Width="90" Click="SaveButton_Click"/>
 
 <Button x:Name="Button" Content="Magic Button" HorizontalAlignment="Left" Margin="585,2,0,0" VerticalAlignment="Top" Width="90" Click="Button_Click"/>
```

MainWindow.xaml.cs
```
private void SaveButton_Click(object sender, RoutedEventArgs e)
{
    ViewModel.SaveChanges();
}

private void Button_Click(object sender, RoutedEventArgs e)
{

}
```

MainWindowViewModel.cs
```
public void SaveChanges()
{
    Context.SaveChanges();
}
```

Teď se nám to už i ukládá.

Na co je ale to druhé tlačítko?
Můžeme pomocí něj vygenerovat novou sadu mobů

MainWindow.xaml.cs
```
private void Button_Click(object sender, RoutedEventArgs e)
{
    ViewModel.DeleteAllMobs();
    ViewModel.GenerateMobs();
}
```

MainWindowViewModel.cs
```
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
```

Výborně. máme i tlačítko

No, ale pořád máme jen seznam našich mobů s jedním modelem. To může být nepraktické. Když z tabulky nepoznáme, jestli je František slepice v ohrádce nebo creeper, co se nám usídlil ve sklepě pod domem.

Z tohoto důvodu přidáme další model pro druh moba

samostatné cvičení pro tvoření modelů

Výsledek je


Species.cs
```
    public class Species
    {
        public int SpeciesId { get; set; }
        public string Title { get; set; }
        public HostilityLevel Hostility { get; set; }

        public virtual List<Mob>? Mobs { get; set; }
    }
```

HostilityLevel.cs
```
    public enum HostilityLevel
    {
        Harmless = 1,
        FightsBack = 2,
        Hostile = 3
    }
```
Změníme i moba

Mobs.cs
```
public class Mob
{
    public int MobId { get; set; }
    public string Name { get; set; }
    public DateTime DateOfCapture { get; set; } = DateTime.Now;
    public int? SpeciesId { get; set; }
    public virtual Species? Species { get; set; }
}
```

Uděláme migraci a úpravu zbytku kódu, aby nám fungovaly nové modely. Plnění databáze

Jestli nám zbyde čas, tak si budeme hrát s tlačítkem
