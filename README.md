# EntityFrameworkCore.LoggingExposure

Support to using EF6-like api in EntityFrameworkCore, the api looks like `context.ConfigureLogging(...)`.

In EF6, we can use `context.Database.Log` to get or display SQL Text easily, but this api removed in EF7 (named EFCore today). We have to write much more code by using [Microsoft.Extensions.Logging](https://docs.microsoft.com/en-us/ef/core/miscellaneous/logging). This project solved it, and thanks you, [David Browne](https://blogs.msdn.microsoft.com/dbrowne/2017/09/22/simple-logging-for-ef-core/).

## Install

via nuget:
```
Install-Package Alexinea.EntityFrameworkCore.LoggingExposure
```

via dotnet cli:
```
dotnet add package Alexinea.EntityFrameworkCore.LoggingExposure
```


## Usage

```
using (var db = new BloggingContext())
{
  db.ConfigureLogging( s => Console.WriteLine(s) );
  //. . .
}
```

```
using (var db = new BloggingContext())
{
  db.ConfigureLogging( s => Console.WriteLine(s) , (c,l) => l == LogLevel.Error || c == DbLoggerCategory.Query.Name);
  //. . .
}
```

```
using (var db = new BloggingContext())
{
  db.ConfigureLogging(s => Console.WriteLine(s), LoggingCategories.SQL);
  //. . .
}
```



## Thanks

So much thanks to [David Browne](https://blogs.msdn.microsoft.com/dbrowne/2017/09/22/simple-logging-for-ef-core/), all the achievements of this project belong to him. I'm just a porter...emmm.... copied codes and publish it. 😋
