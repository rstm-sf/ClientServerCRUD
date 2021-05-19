module Server.Handlers

open Microsoft.AspNetCore.Http
open Microsoft.Extensions.Logging
open FSharp.Control.Tasks
open Giraffe
open Shared
open Server.DataModels

let toApi (user : DbUser) =
    {Id = user.Id; FirstName = user.FirstName; LastName = user.LastName}

let usersHandler : HttpHandler =
    fun (next : HttpFunc) (ctx : HttpContext) -> 
        task {
            try
                use dbContext = ctx.GetService<SmthContext>()
                let result = dbContext.Users
                             |> Seq.map toApi
                             |> json
                return! Successful.ok result next ctx
            with
            | ex ->
                let logger = ctx.GetLogger<ILogger>()
                logger.LogError($"Ops, something went wrong: {ex}")
                return! ServerErrors.internalError (json "Ops, something went wrong") next ctx
        }

let userAddHandler : HttpHandler = 
    fun (next : HttpFunc) (ctx : HttpContext) -> 
        task {
            try
                use dbContext = ctx.GetService<SmthContext>()
                let! userRequest = ctx.BindJsonAsync<User>()
                let newUser = userRequest |> fun x -> DbUser(FirstName = x.FirstName, LastName = x.LastName) 
                let! _ = dbContext.AddAsync(newUser)
                let! count = dbContext.SaveChangesAsync()
                let result =
                    if count >= 1 then
                         newUser |> toApi |> json |> Successful.ok
                    else "User not added" |> json |> RequestErrors.badRequest
                return! result next ctx
            with
            | ex ->
                let logger = ctx.GetLogger<ILogger>()
                logger.LogError($"User not added: {ex}")
                return! ServerErrors.internalError (json "Ops, something went wrong") next ctx
        }

let userUpdateHandler (id : int) : HttpHandler =
    fun (next : HttpFunc) (ctx : HttpContext) -> 
        task {
            try
                let! userRequest = ctx.BindJsonAsync<User>()
                use dbContext = ctx.GetService<SmthContext>()
                let! user = dbContext.Users.FindAsync(id)
                user.FirstName <- userRequest.FirstName
                user.LastName <- userRequest.LastName
                let! count = dbContext.SaveChangesAsync()
                let result =
                    if count >= 1 then
                         Successful.ok (user |> toApi |> json)
                    else RequestErrors.badRequest (json "User not updated")
                return! result next ctx
            with
            | ex ->
                let logger = ctx.GetLogger<ILogger>()
                logger.LogError($"User not deleted: {ex}")
                return! RequestErrors.badRequest (json "User not updated") next ctx
        }

let userDeleteHandler (id : int) : HttpHandler =
    fun (next : HttpFunc) (ctx : HttpContext) -> 
        task {
            try
                use dbContext = ctx.GetService<SmthContext>()
                let! user = dbContext.Users.FindAsync(id)
                dbContext.Users.Remove(user) |> ignore
                let! count = dbContext.SaveChangesAsync()
                let result =
                    if count >= 1 then
                         Successful.ok (json user)
                    else RequestErrors.badRequest (json "User not deleted")
                return! result next ctx
            with
            | ex ->
                let logger = ctx.GetLogger<ILogger>()
                logger.LogError($"User not deleted: {ex}")
                return! RequestErrors.badRequest (json "User not deleted") next ctx
        }
