# Installation

EasyCosmos is in pre-release and is available as a NuGet package.

# What's this

EasyCosmos is a migration script management tool for Azure's Cosmos DB using c#. This package enables projects to create and run these migration scripts. It will also track what has already been run and therefore not run those again. This makes no assumptions about how you trigger these migrations and therefore can be added to any c# project that supports .Net Standard 2+

There is also information available about each migration as well as the overall migrations that have been run.

# Creating a migration script

You can either have EasyCosmos automatically pick up the migration scripts to run by following the naming convention or you can choose to optionally provide EasyCosmos with a list of migrations in order for them to be run.

If you decide to go down the automatic detection route then you will need to create your migration scripts beginning with the migration number in a 4 digit format e.g. 0001. Because c# doesn't allow classes to start with a number you will need to start with an underscore and then delimit with another underscore for the name. 

```csharp
class _0001_Migration : CosmosMigration
  {
          protected override Task Migrate()
      {
              return Task.CompletedTask;
      }
  }   
```
 
If you decide not to have EasyCosmos automatically detect the migration scripts then you can decide to call your scripts whatever you please.

# Running migration scripts
You will need to create a MigrationRunner with the Cosmos database name, the collection name, the url to the database, and the primary key to the database)


```csharp
var runner = new MigrationRunner(databaseName, collectionName, dbUrl, primaryKey);
```
```csharp
// if you followed the automatic detection naming convention
await runner.ApplyMigrations(GetType().Assembly)
```
```csharp
// if you opted to not follow this convention then use
await runner.ApplyMigrations(new[] {typeof(AMigration)});
```         
