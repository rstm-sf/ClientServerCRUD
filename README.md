# Client-Server-CRUD 

Simple client-server CRUD application written in F#. Usage:
- [Elmish](https://elmish.github.io/elmish/) for Elm-style SPA.
- [Fable](https://fable.io/) for compiling F# code to JS.
- [Bulma](https://bulma.io/) as the CSS framework.
- [Giraffe](https://giraffe.wiki/) as the web server.
- [EFCore.FSharp](https://github.com/efcore/EFCore.FSharp) as the ORM.
- [SQLite](https://www.sqlite.org/index.html) as the storage.

## Install pre-requisites

You'll need to install the following pre-requisites in order to build applications

* The [.NET SDK](https://dotnet.microsoft.com/download/dotnet) 5.0 or higher.
* [npm](https://nodejs.org/en/download/) package manager.
* [Node LTS](https://nodejs.org/en/download/).

## Starting the application

Start the server:

```ps1
cd src\Server\
dotnet watch run
```

Start the client:

```ps1
npm install
npm start
```

Open a browser to `http://localhost:8080` to view the site.
