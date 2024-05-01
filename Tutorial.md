
# ⛏ Aplikace pro správu databáze našich minecraft mobů

### Co je vlastně ORM?
ORM (Object-Relational Mapping), je technika programování, která usnadňuje správu databází v objektově orientovaných jazycích. Díky ORM může vývojář manipulovat s daty v databázi přímo pomocí objektů v programovacím jazyku, aniž by musel psát složité SQL dotazy. Tím se značně zjednodušuje a zrychluje vývoj aplikací, zlepšuje čitelnost kódu a udržitelnost projektů.

Entity Framework (EF) je populární ORM framework pro .NET, který umožňuje vývojářům pracovat s databázemi.

### Přístupy k programování v EF
#### Code first
Code-first je přístup, kdy první vytvoříte třídy C# pro své modely a vytvořené schéma poté použijete pro vytvoření databáze. Tímto způsobem se můžete soustředit na návrh modelu databáze a EF se pak postará o vytvoření databáze.

Jak píšeme výše, EF se postará o vytvoření databáze nebo o mapování na již existující databázi, přičemž vychází z modelu, které jste vytvořili.

#### Database first

Database first je přístup, jak vytvořit datové modely na základě existující databáze. Datové modely a třídu DbContext vytváříme na základě existujícího schématu databáze. Začneme vytvořením databáze včetně tabulek, jejich vlastností a vztahů mezi nimi.

Postup:
- Vytvořit/připojit databázi.
- Použít EF, který vygeneruje potřebné soubory.
- Použít vygenerované soubory (třídy Entity a DbContext).

### Kompatibilita
EF Core je kompatibliní s **Microsoft SQL Server**, **SQLite**, **PostgreSQL**, **MySQL**, **MariaDB**, **SQL Server Compact**, **Cosmos DB**, **InMemory**.\
My budeme pro jednoduchost používat **SQLite**.

## Vytvoření nového projektu 

Prvním krokem je vytvoření nového projektu ve Visual Studiu. Zvolíme WPF C# (Windows Presentation Foundation) jako typ projektu. Budeme používat .NET 8.

Budeme potřebovat nainstalovat několik NuGet balíčků:
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
public class Tool
{
    public int Id { get; set; }
    public string Name { get; set; }
    public ICollection<Enchantment> Enchantments { get; set; }
}

public class Enchantment
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int ToolId { get; set; }
    public Tool Tool { get; set; }
}
```

A teď zpátky k naší databázi...

Vytvoříme složku `Data` a v ní soubor se jménem `mobs.cs`.
Tento soubor bude obsahovat definici třídy pro naši entitu `Mob`.

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

Nyní vytvoříme soubor `MobsContext.cs` uvnitř složky Data a v něm definujeme náš datový kontext:

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

## ViewModel

Nyní se podíváme na ViewModel (view model) pro naše hlavní okno. Ale než se ponoříme do kódu, pojďme se nejprve podívat na MVVM architekturu a proč ji používáme v našem případě.

MVVM (Model-View-ViewModel) je architektonický vzor, který je často používán při vývoji aplikací s grafickým uživatelským rozhraním. Tento vzor odděluje prezentaci dat od logiky aplikace a umožňuje snadnější testování, správu kódu a znovupoužití komponent. Používá se často u WPF projektů.

Nyní vytvoříme ViewModel pro naše hlavní okno v souboru `MainWindowViewModel.cs`:

Pozor, musí se přidat `using EFCore.Data` nebo obdobně podle názvu vašeho namespace a složky ve které máte modely.

```csharp
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

## Návrh GUI

Nyní se podíváme na návrh uživatelského rozhraní (GUI) našeho hlavního okna. 

Následující kód ukazuje implementaci hlavního okna `MainWindow.xaml`, která obsahuje `DataGrid` pro zobrazení dat:

Tady je důležité nastavit xmlns:local a x:Class.

```csharp
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
        <DataGrid x:Name="MobsGrid" ItemsSource="{Binding Mobs}"/>
    </Grid>
</Window>
```

## Další nezbytnosti

Model je spojený s GUI, ale ještě nám něco chybí. Následující kód ukazuje implementaci logiky pro hlavní okno `MainWindow.xaml.cs`:

Pozor, opět se musí `using EFCore.Data`.

```csharp
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

Následující kód ukazuje implementaci třídy App v souboru `App.xaml.cs`:

```csharp
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

## Vytvoření databáze

Pomocí následujících příkazů vytvoříme databázi

```
dotnet tool install --global dotnet-ef
dotnet ef migrations add InitialCreate
dotnet ef database update
```

#### Migrace databáze

Migrace databáze jsou postupy, které slouží k aktualizaci schématu databáze na základě změn provedených v modelu Entity Frameworku. Když provedeme změny v datovém modelu (například přidání nové tabulky nebo změna sloupců), musíme také aktualizovat schéma databáze tak, aby odpovídalo novému modelu. Migrace databáze nám umožňují spravovat tento proces aktualizace schématu databáze. Je potřeba vztvářet migraci po každé změně databáze.

Skvěle! Teď, když jsme dokončili konfiguraci a propojení našich vrstev, by měla být naše aplikace připravena k prvnímu spuštění.

## DataGrid

Nyní se podíváme na vyladění našeho DataGridu, abychom mu poskytli lepší vzhled a funkčnost.

```csharp
<DataGrid x:Name="MobsGrid"  ItemsSource="{Binding Mobs}" AutoGenerateColumns="False"
          IsReadOnly="False">
    <DataGrid.Columns>
        <DataGridTextColumn Header="ID" Binding="{Binding MobId}"
        <DataGridTextColumn Header="Name" Binding="{Binding Name}" />
        <DataGridTextColumn Header="Date of Capture" Binding="{Binding DateOfCapture, StringFormat='d'}" />
    </DataGrid.Columns>
</DataGrid>
```

## Generování dat

No, počkat, ale my nemáme zatím žádné moby. Nebudeme to psát ručně. Necháme si to vygenerovat.  K tomu použijeme knihovnu `Bogus`, která umožňuje generovat náhodná data podle definovaných pravidel.

Vytvoříme generátor dat v novém souboru Generator.cs ve složce Generator. Zde je kód generátoru:

```csharp
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

Nyní implementujeme volání našeho generátoru dat při spuštění aplikace v souboru `App.xaml.cs`:

```csharp
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

## Tlačítka

Máme data v datagridu a dokonce je můžeme měnit. Jenomže se nám to zatím neukládá. 

#### Zadání samostatné práce

Vaším úkolem je přidat tlačítka do uživatelského rozhraní `MainWindow.xaml`, která umožní uživatelům ukládat změny provedené v DataGridu.

Tlačítko pro ukládání: Přidejte tlačítko s názvem `Save`, které umožní uživatelům ukládat změny provedené v DataGridu.
Magické tlačítko: Přidejte tlačítko s názvem `Magic Button`, které zatím nedělá nic.

#### Řešení:

`MainWindow.xaml`:

```csharp
 <Button x:Name="SaveButton" Content="Save" HorizontalAlignment="Left" Margin="690,2,0,0" VerticalAlignment="Top" Width="90" Click="SaveButton_Click"/>
 
 <Button x:Name="Button" Content="Magic Button" HorizontalAlignment="Left" Margin="585,2,0,0" VerticalAlignment="Top" Width="90" Click="Button_Click"/>
```

`MainWindow.xaml.cs`:
```csharp
private void SaveButton_Click(object sender, RoutedEventArgs e)
{
    ViewModel.SaveChanges();
}

private void Button_Click(object sender, RoutedEventArgs e)
{

}
```

`MainWindowViewModel.cs`:
```csharp
public void SaveChanges()
{
    Context.SaveChanges();
}
```

Výborně. Tlačítko `Save`

Na co je ale to druhé tlačítko?
Můžeme pomocí něj vygenerovat novou sadu mobů

MainWindow.xaml.cs
```csharp
private void Button_Click(object sender, RoutedEventArgs e)
{
    ViewModel.DeleteAllMobs();
    ViewModel.GenerateMobs();
}
```

MainWindowViewModel.cs
```csharp
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
```csharp
    public class Species
    {
        public int SpeciesId { get; set; }
        public string Title { get; set; }
        public HostilityLevel Hostility { get; set; }

        public virtual List<Mob>? Mobs { get; set; }
    }
```

HostilityLevel.cs
```csharp
    public enum HostilityLevel
    {
        Harmless = 1,
        FightsBack = 2,
        Hostile = 3
    }
```
Změníme i moba

Mobs.cs
```csharp
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
