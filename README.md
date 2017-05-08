# TarkOrm.NET
It's a Prototype of a simple micro ORM system inspired in Dapper, I'm slowly developing some ideas by myself, anyone who'd like to contribute is welcome to share your own ideas, fork or pull a suggestion/fix/implementation/etc. 

My meta is to make something more interesting than Dapper, since I think that some dapper interfaces doesn't sound natural or doesn't sound so natural as a repository.

My meta is also including some automatic basic queries through the entities and some basic extensible repository ideas, let's see where will it take us.

It can be mapped using DataAnnotation.Schema attributes and I hope I'm going to make a basic MappingConfiguration class in the future.

#### Do not consider it as release version, I still expect to refector it some times till it get in a good shape.

# Methods

These components are being used in the following usage examples

#### Country Table
```sql
CREATE TABLE [dbo].[Country](
    [CountryID] [int] NOT NULL,
    [CountryXpto] [varchar](2) NOT NULL,
    [Name] [varchar](150) NOT NULL,
    [ContinentID] [int] NOT NULL,
    [FlagB64] [varchar](4000) NULL,
    [CurrencyID] [int] NULL,
)
```

#### Country Class
```csharp
public class Country
{
    [Key]        
    public int CountryID { get; set; }
  
    [Column("CountryXpto")]
    public string CountryCode { get; set; }
  
    public string Name { get; set; }
  
    public int ContinentID { get; set; }
  
    public string FlagB64{ get; set; }
  
    public int CurrencyID { get; set; }
}   
```

### Querying a list from a mapped entity

```csharp
public virtual IEnumerable<T> GetAll<T>
```

Example of usage:

```csharp
TarkDataAccess tarkDataAcess = new TarkDataAccess("connectionStringName");
IEnumerable<Country> lista = tarkDataAcess.GetAll<Country>();
```

### Selecting an item from a mapped entity

```csharp
public virtual T GetById<T>(params object[] keyValues)
```

Example of usage:

```csharp
TarkDataAccess tarkDataAcess = new TarkDataAccess("connectionStringName");
Country country = tarkDataAcess.GetById<Country>(10);
```

# First benchmarks 

This benchmarks were performed on a I7 4970k, 16gb ram and running SQL Server 2008 on a SSD Disk.

### Over 250 entities

![alt tag](https://raw.githubusercontent.com/juniortarcisio/TarkOrm.NET/master/benchmarkCountry.png)


### Over 50.000 entities

![alt tag](https://raw.githubusercontent.com/juniortarcisio/TarkOrm.NET/master/benchmarkCity.png)

