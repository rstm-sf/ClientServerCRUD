module Server.App

open System
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Cors.Infrastructure
open Microsoft.AspNetCore.Hosting
open Microsoft.Extensions.Hosting
open Microsoft.Extensions.Logging
open Microsoft.Extensions.DependencyInjection
open Giraffe
open Server.DataModels
open Server.Handlers

// ---------------------------------
// Web app
// ---------------------------------

let webApp =
    choose [
        GET >=> choose [
            route "/api/users" >=> usersHandler ]
        POST >=> choose [
            route "/api/users" >=> userAddHandler ]
        PUT >=> choose [
            routef "/api/users/%i" userUpdateHandler ]
        DELETE >=> choose [
            routef "/api/users/%i" userDeleteHandler ]
        setStatusCode 404 >=> json "Not Found" ]

// ---------------------------------
// Error handler
// ---------------------------------

let errorHandler (ex : Exception) (logger : ILogger) =
    logger.LogError(ex, "An unhandled exception has occurred while executing the request.")
    clearResponse >=> setStatusCode 500 >=> json ex.Message

let configureCors (builder : CorsPolicyBuilder) =
    builder
        .WithOrigins("http://localhost:8080")
        .AllowAnyMethod()
        .AllowAnyHeader()
        |> ignore

let configureApp (app : IApplicationBuilder) =
    let env = app.ApplicationServices.GetService<IWebHostEnvironment>()
    (match env.IsDevelopment() with
    | true  ->
        app.UseDeveloperExceptionPage()
    | false ->
        app .UseGiraffeErrorHandler(errorHandler)
            .UseHttpsRedirection())
        .UseCors(configureCors)
        .UseGiraffe(webApp)

let configureServices (services : IServiceCollection) =
    services.AddDbContext<SmthContext>() |> ignore
    services.AddCors() |> ignore
    services.AddGiraffe() |> ignore

let configureLogging (builder : ILoggingBuilder) =
    builder.AddConsole()
           .AddDebug() |> ignore

[<EntryPoint>]
let main args =
    let host = Host.CreateDefaultBuilder(args)
                    .ConfigureWebHostDefaults(
                        fun webHostBuilder ->
                            webHostBuilder
                                .Configure(Action<IApplicationBuilder> configureApp)
                                .ConfigureServices(configureServices)
                                .ConfigureLogging(configureLogging)
                                |> ignore)
                    .Build()

    let scopeFactory = host.Services.GetService<IServiceScopeFactory>()
    using(scopeFactory.CreateScope()) ( fun scope ->
        let dbContext = scope.ServiceProvider.GetService<SmthContext>()
        SmthSeeder(dbContext).Seed()) |> ignore

    host.Run()
    0