module Client.Types

open Shared
open System

type RowState = 
    | HasNormal
    | HasEdit

type [<CLIMutable>] UserModel =
    {
        Id        : UserId
        FirstName : string
        LastName  : string
        RowState  : RowState
    }
    static member Create (user : User) =
        {
            Id        = user.Id
            FirstName = user.FirstName
            LastName  = user.LastName
            RowState  = RowState.HasNormal
        }
    member this.toUser() =
        {
            Id        = this.Id
            FirstName = this.FirstName
            LastName  = this.LastName
        }

type ErrorMessage =
    {
        Message : string
        Id      : UserId
    }

type State =
    {
        Users    : Map<UserId, UserModel>
        NewUser  : User option
        Error    : ErrorMessage option
        PrevUser : UserModel option
    }

type Msg =
    | LoadUsers
    | UsersLoaded         of User list
    | LoadUsersFailure    of ErrorMessage
    | SetNewUserFirstName of string
    | SetNewUserLastName  of string
    | AddUser
    | AddUserFailed       of ErrorMessage
    | UserAdded           of User
    | UpdateUser          of UserModel
    | UpdateUserFailed    of ErrorMessage
    | UserUpdated         of User
    | DeleteUser          of UserId
    | DeleteUserFailure   of ErrorMessage
    | UserDeleted         of User
    | UpdateRowState      of UserId * RowState
    | UpdateRowName       of UserId * string * bool
