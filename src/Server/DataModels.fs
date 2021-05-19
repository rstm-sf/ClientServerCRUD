module Server.DataModels

open System.ComponentModel.DataAnnotations
open Microsoft.EntityFrameworkCore
open FSharp.Control.Tasks

[<CLIMutable>]
type Message =
    {
        Text : string
    }

[<AllowNullLiteral>]
type DbUser() =
    [<Key>]
    member val Id = 0 with get, set
    member val FirstName = "" with get, set
    member val LastName = "" with get, set


type SmthContext =
    inherit DbContext
    
    new() = { inherit DbContext }
    new(options : DbContextOptions<SmthContext>) = { inherit DbContext(options) }

    [<DefaultValue>] val mutable private users : DbSet<DbUser>
    member x.Users with get() = x.users and set v = x.users <- v

    override _.OnConfiguring (options: DbContextOptionsBuilder) =
        if options.IsConfigured = false then
            options.UseSqlite("Data Source=smth.db") |> ignore


type SmthSeeder (ctx : SmthContext) =
    member this.Seed() =
        task {
            let! _ = ctx.Database.EnsureDeletedAsync()
            let! _ = ctx.Database.EnsureCreatedAsync()
            let! _ = ctx.Users.AddRangeAsync([|
                DbUser (FirstName = "Cassie",   LastName = "Jacobson")
                DbUser (FirstName = "Alana",    LastName = "Williams")
                DbUser (FirstName = "Eliezer",  LastName = "Wade")
                DbUser (FirstName = "Cannon",   LastName = "Mcintosh")
                DbUser (FirstName = "Rene",     LastName = "Raymond")
                DbUser (FirstName = "Leslie",   LastName = "Giles")
                DbUser (FirstName = "Cristina", LastName = "Joseph") |])
            let! _ = ctx.SaveChangesAsync()
            return ()
        }
