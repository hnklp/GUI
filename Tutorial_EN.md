
# ⛏ An app to manage our minecraft mob database

> [!CAUTION]
> This text was machine translated. We are not responsible for any harm caused to You, Your loved ones, The Universe™ or Your property.

### What is ORM?
ORM (Object-Relational Mapping), is a programming technique that facilitates database management in object-oriented languages. With ORM, a developer can manipulate data in a database directly using objects in a programming language without having to write complex SQL queries. This greatly simplifies and speeds up application development, improves code readability and project sustainability.

Entity Framework (EF) is a popular ORM framework for .NET that allows developers to work with databases.

### Approaches to EF programming
#### Code first
[Code-first](https://learn.microsoft.com/en-us/ef/ef6/modeling/designer/workflows/model-first) is an approach where you first create C# classes for your models and then use the schema you create to create a database. This way you can concentrate on designing the database model and EF will then take care of creating the database.

As we write above, EF will take care of creating the database or mapping it to an existing database based on the model you have created.

#### Database first

Database first is an approach to create data models based on an existing database. We create data models and the DbContext class based on an existing database schema. We start by creating the database including the tables, their properties, and the relationships between them.

Procedure:
- Create/connect the database.
- [Use EF](https://learn.microsoft.com/en-us/ef/ef6/modeling/designer/workflows/database-first), which will generate the necessary files.
- Use the generated files (Entity and DbContext classes).

### Compatibility
EF Core is compatible with **Microsoft SQL Server**, **SQLite**, **PostgreSQL**, **MySQL**, **MariaDB**, **SQL Server Compact**, **Cosmos DB**, **InMemory**.
We will use **SQLite** for simplicity.

## Creating a new project 

The first step is to create a new project in Visual Studio. We will choose WPF C# (Windows Presentation Foundation) as the project type. We will use .NET 8.

We will need to install several NuGet packages:
- Microsoft.EntityFrameworkCore.Sqlite
- Microsoft.EntityFrameworkCore.Design
- Bogus


## Model
When working with Entity Framework, the model is a key component. The model provides us with an abstraction over our database and defines how our entities are represented in our code.

Within the Entity Framework, a model is a set of classes that represent the entities in our database. These classes contain properties that correspond to the columns in the tables of our database, and define the relationships between these entities.

#### Attribute Obligation
An attribute is mandatory or not depending on whether the variable can contain null. 

#### Relationships between entities
Sample 1:N relationship

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

Now, back to our database...

We'll create a `Data` folder and in it a file called `mobs.cs`.
This file will contain the class definition for our `Mob` entity.

```csharp
public class Mob
    {
        public int MobId { get; set; }
        [Required]
        public string Name { get; set; }
        public DateTime DateOfCapture { get; set; } = DateTime.Now;
    }
```

## Data context

Before we prepare the user interface (GUI) for displaying data, we first need to connect several layers of our application. The first layer we need to prepare is the data context.

The data context is a fundamental part of the Entity Framework. It is a means of communicating with the database and provides a set of properties and methods for working with data. Why do we need it?

The data context allows us to work with entities (objects) and perform operations such as retrieving, saving, updating, and removing data from the database. It also allows us to define configurations for mapping between objects and tables in the database.

Now we create a file `MobsContext.cs` inside the Data folder and define our data context in it:

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

Now let's look at the ViewModel (view model) for our main window. But before we dive into the code, let's first take a look at the MVVM architecture and why we are using it in our case.

MVVM (Model-View-ViewModel) is an architectural pattern that is often used when developing GUI applications. This pattern separates the data presentation from the application logic and allows for easier testing, code management, and component reuse. It is often used in WPF projects.

Now we will create a ViewModel for our main window in the `MainWindowViewModel.cs` file:

Note, you must add `using EFCore.Data` or similar depending on your namespace and the folder you have the models in.

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

## GUI design

Now let's look at the user interface (GUI) design of our main window. 

The following code shows the `MainWindow.xaml` implementation of the main window, which contains a `DataGrid` for displaying data:

Here it is important to set xmlns:local and x:Class.

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

## Other essentials

The model is linked to the GUI, but we are still missing something. The following code shows the implementation of the logic for the main window `MainWindow.xaml.cs`:

Note that again, it must be `using EFCore.Data`.

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

The following code shows the implementation of the App class in the `App.xaml.cs` file:

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

## Creating a database

Use the following commands to create a database

```
dotnet tool install --global dotnet-ef
dotnet ef migrations add InitialCreate
dotnet ef database update
```

#### Database Migration

Database migrations are procedures that are used to update the database schema based on changes made to the Entity Framework model. When we make changes to the data model (such as adding a new table or changing columns), we must also update the database schema to match the new model. Database migrations allow us to manage this process of updating the database schema. We need to create a migration after each database change.

Great! Now that we've finished configuring and linking our layers, our application should be ready to go live for the first time.

## DataGrid

Now let's look at tweaking our DataGrid to give it a better look and functionality.

```csharp
<DataGrid x:Name="MobsGrid" ItemsSource="{Binding Mobs}" AutoGenerateColumns="False"
          IsReadOnly="False">
    <DataGrid.Columns>
        <DataGridTextColumn Header="ID" Binding="{Binding MobId}" />
        <DataGridTextColumn Header="Name" Binding="{Binding Name}" />
        <DataGridTextColumn Header="Date of Capture" Binding="{Binding DateOfCapture, StringFormat='d'}" />
    </DataGrid.Columns>
</DataGrid>
```

## Data generation

Well, wait, but we don't have any mobs yet. We're not gonna write it by hand. We'll have it generated.  To do this, we'll use the `God' library, which allows us to generate random data according to defined rules.

We'll create the data generator in a new Generator.cs file in the Generator folder. Here is the code for the generator:

```csharp
class DataGenerator
{
    public static IList<Mob> GenerateMobs(int count)
    {
        Faker<Mob> generator = new Faker<Mob>()
            .StrictMode(true)
            .RuleFor(x => x.MobId, f => f.IndexGlobal)
            .RuleFor(x => x.Name, f => f.Person.FirstName)
            .RuleFor(x => x.DateOfCapture, f => f.Person.DateOfBirth);

        return generator.Generate(count);
    }
}
```

We now implement the call to our data generator when the application is run in the `App.xaml.cs` file:

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

## Buttons

We have the data in the datagrid and we can even change it. It's just that we're not saving it yet. 

#### Independent work assignment:

Your task is to add buttons to the `MainWindow.xaml` user interface that will allow users to save changes made to the DataGrid.

- Save Button: Add a button named `Save` that will allow users to save changes made to the DataGrid.
- Magic Button: Add a button called `Magic Button` that does nothing for now.\
  
<details>
<summary>Solution</summary>

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
``csharp
public void SaveChanges()
{
    Context.SaveChanges();
}
```

Well done. We already have the `Save` button. But what is the second button for? We can use it to generate a new set of mobs.

`MainWindow.xaml.cs`:
``csharp
private void Button_Click(object sender, RoutedEventArgs e)
{
    ViewModel.DeleteAllMobs();
    ViewModel.GenerateMobs();
}
```

`MainWindowViewModel.cs`:
``csharp
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
</details>

## New models

Well, we still only have a list of our mobs with one model. That can be impractical. When we can't tell from the table if Francis is a chicken in a pen or a creeper that's taken up residence in the basement under our house.

#### Independent work assignment:

Your task is to add another model for mob types to the application so that we can assign a type to each "mob".

- Create a new Species class with properties for the species identifier (SpeciesId), name (Title) and hostility level (Hostility). The Hostility property will be the enum HostilityLevel.
- Create a HostilityLevel enum: Create a HostilityLevel enum with three values: Harmless, FightsBack and Hostile.
- Mob Update: Add a foreign key for species to the Mob model.
<details>
<summary>Solution</summary>

`Species.cs`:
``csharp
    public class Species
    {
        public int SpeciesId { get; set; }
        public string Title { get; set; }
        public HostilityLevel Hostility { get; set; }
        public virtual List<Mob>? Mobs { get; set; }
    }
```

`HostilityLevel.cs`:
``csharp
    public enum HostilityLevel
    {
        Harmless = 1,
        FightsBack = 2,
        Hostile = 3
    }
```

`Mobs.cs`:
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

After this step, we do the migration again because we changed the database.
</details>

## New models

Well, we still only have a list of our one-model mobs. That can be impractical. If we can't tell from the table if Francis is a chicken in a pen or a creeper that's taken up residence in the basement under our house.

#### Independent work assignment:

Your task is to add another model for mob species to the app so that we can assign a species to each "mob".

- Create a new Species class with properties for the species identifier (SpeciesId), name (Title) and hostility level (Hostility). The Hostility property will be the enum HostilityLevel.
- Create a HostilityLevel enum: Create a HostilityLevel enum with three values: Harmless, FightsBack and Hostile.
- Mob Update: Add a foreign key for species to the Mob model.
<details>
<summary>Solution</summary>

`Species.cs`:
```csharp
    public class Species
    {
        public int SpeciesId { get; set; }
        public string Title { get; set; }
        public HostilityLevel Hostility { get; set; }
        public virtual List<Mob>? Mobs { get; set; }
    }
```

`HostilityLevel.cs`:
```csharp
    public enum HostilityLevel
    {
        Harmless = 1,
        FightsBack = 2,
        Hostile = 3
    }
```

`Mobs.cs`:
```csharp
public třída Mob
{
    public int MobId { get; set; }
    public string Name { get; set; }
    public DateTime DateOfCapture { get; set; } = DateTime.Now;
    public int? SpeciesId { get; set; }
    public virtual Species? Species { get; set; }
}
```

Po tomto kroku uděláme znovu migraci, protože jsme změnili databázi.
</details>
