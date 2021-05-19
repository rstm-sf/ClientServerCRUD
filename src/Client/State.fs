module Client.State

open Client.Types
open Elmish
open Shared 

let initialState() = 
    let initState =
        {
            Users    = Map.empty
            NewUser  = None
            Error    = None
            PrevUser = None
        }

    initState, Cmd.ofMsg LoadUsers

let findUserIndexById (users : UserModel array) (id : UserId) =
    users |> Array.findIndex (fun x -> x.Id = id)

let update (msg: Msg) (prevState: State) = 
    match msg with
    | SetNewUserFirstName name ->
        let nextState = prevState.NewUser
                        |> Option.map (fun x -> {x with FirstName = name})
                        |> Option.defaultValue { Id = 0; FirstName = name; LastName = "" }
                        |> fun x -> { prevState with NewUser = Some x; Error = None }
        nextState, Cmd.none

    | SetNewUserLastName name ->
        let nextState = prevState.NewUser
                        |> Option.map (fun x -> {x with LastName = name})
                        |> Option.defaultValue { Id = 0; FirstName = ""; LastName = name }
                        |> fun x -> { prevState with NewUser = Some x; Error = None }
        nextState, Cmd.none

    | LoadUsers ->
        prevState, Server.loadUsers()

    | AddUser ->
        match prevState.NewUser with
        | None -> prevState, Cmd.none
        | Some user -> prevState, Server.addUser user

    | UserAdded user ->
        let nextUsers = prevState.Users |> Map.add user.Id (UserModel.Create user)
        let nextState = { prevState with Users = nextUsers; NewUser = None; Error = None }
        nextState, Cmd.none
            
    | UsersLoaded users ->
        let nextUsers = users |> List.map (fun x -> x.Id, UserModel.Create x) |> Map.ofList
        let nextState = { prevState with Users = nextUsers; Error = None}
        nextState, Cmd.none
    
    | UpdateUser user ->
        prevState, Server.updateUser user
    
    | UserUpdated user ->
        let nextUsers = prevState.Users |> Map.add user.Id (UserModel.Create user)
        let nextState = { prevState with Users = nextUsers; Error = None }
        nextState, Cmd.none

    | DeleteUser userId ->
        prevState, Server.deleteUser userId

    | UserDeleted user ->
        let nextUsers = prevState.Users |> Map.remove user.Id
        let nextState = { prevState with Users = nextUsers; Error = None }
        nextState, Cmd.none

    | UpdateRowState (id : UserId, state : RowState) ->
        let getUpdatedUser (user : UserModel) =
            if state = RowState.HasEdit then
                 Some user, { user with RowState = state }
            else None, prevState.PrevUser.Value

        let prevUser, newUser = getUpdatedUser prevState.Users.[id]

        let nextUsers = prevState.Users |> Map.add id newUser
        let nextState = { prevState with Users = nextUsers; Error = None; PrevUser = prevUser }
        nextState, Cmd.none

    | UpdateRowName (id : UserId, name : string, isFirstName : bool) ->
        let userUpdated (x : UserModel) =
            if isFirstName then
                 { x with FirstName = name }
            else { x with LastName = name }

        let userUpdated = userUpdated prevState.Users.[id]

        let nextUsers = prevState.Users |> Map.add id userUpdated
        let nextState = { prevState with Users = nextUsers; Error = None }
        nextState, Cmd.none

    | LoadUsersFailure er ->
        let nextState = { prevState with Error = Some er }
        nextState, Cmd.none

    | AddUserFailed er ->
        let nextState = { prevState with Error = Some er }
        nextState, Cmd.none

    | UpdateUserFailed er | DeleteUserFailure er ->
        let nextState = { prevState with Error = Some er }
        nextState, Cmd.none
