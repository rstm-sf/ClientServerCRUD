module Client.Server

open Client.Types
open Elmish
open Shared
open Thoth
open Thoth.Fetch
open Thoth.Json

let loadUsers () =
    let load () = Fetch.get<unit, User list> ("/api/users", caseStrategy = CaseStrategy.CamelCase)
    Cmd.OfPromise.either load () UsersLoaded (fun (ex : exn) -> AddUserFailed {Message = ex.Message; Id = -2})

let addUser (user : User) =
    let newUser x = Fetch.post<User, User> ("/api/users", x, caseStrategy = CaseStrategy.CamelCase)
    Cmd.OfPromise.either newUser user UserAdded (fun (ex : exn) -> AddUserFailed {Message = ex.Message; Id = -1})

let updateUser (user : UserModel) =
    let url = "/api/users/" + user.Id.ToString()
    let update (x : User) = Fetch.put<User, User> (url, x, caseStrategy = CaseStrategy.CamelCase)
    Cmd.OfPromise.either update (user.toUser()) UserUpdated (fun (ex : exn) -> AddUserFailed {Message = ex.Message; Id = user.Id})

let deleteUser (id : UserId) =
    let url = "/api/users/" + id.ToString()
    let delete() = Fetch.delete<unit, User> (url, caseStrategy = CaseStrategy.CamelCase)
    Cmd.OfPromise.either delete () UserDeleted (fun (ex : exn) -> AddUserFailed {Message = ex.Message; Id = id})
