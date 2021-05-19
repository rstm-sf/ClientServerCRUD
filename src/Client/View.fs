module Client.View

open Browser.Types
open Shared
open Client.Types
open Fable.React
open Fable.React.Props

let widthButton = 100

let isDangerId (error : ErrorMessage option) (id : UserId) =
    match error with
    | Some x -> x.Id = id
    | None _ -> false

let divFieldUser =
    div [ ClassName "field is-grouped is-grouped-centered" ]
let divInputClass =
    div [ ClassName "control is-medium" ]

let errorMessage (error : string) =
    [ divFieldUser
        [ span [ Style [ Color "red" ] ]
               [ str error ]] ]

let renderButton (strButton : string) (color : string) (clickEventFunc : MouseEvent -> unit) =
    let classname = sprintf "button is-medium %s" color
    [ button [ ClassName classname
               Style [ Width widthButton ]
               OnClick clickEventFunc ]
             [ str strButton ] ]

let renderInputNameReadonly (name : string) =
    [ input [ ReadOnly true
              ClassName "input is-medium"
              Value name ] ]

let renderInputName (placeHolder : string) (value : string) (changeEventFunc : Event -> unit) (isDanger : bool) =
    [ input [ ClassName ("input is-medium" + if isDanger then " is-danger" else "")
              Placeholder placeHolder
              DefaultValue value
              Value value
              OnChange changeEventFunc] ]


let renderUser (user: UserModel) (error : ErrorMessage option) (dispatch : Msg -> unit) =
    let isDanger = isDangerId error user.Id
    let firstInput =
        if user.RowState = RowState.HasEdit then
             renderInputName "" user.FirstName (fun ev -> (user.Id, ev.Value, true) |> UpdateRowName |> dispatch) isDanger
        else renderInputNameReadonly user.FirstName
    let lastInput =
        if user.RowState = RowState.HasEdit then
             renderInputName "" user.LastName (fun ev -> (user.Id, ev.Value, false) |> UpdateRowName |> dispatch) isDanger
        else renderInputNameReadonly user.LastName
    let firstButton =
        if user.RowState = RowState.HasEdit then
             renderButton "Cancel" "is-link"    (fun _ -> (user.Id, RowState.HasNormal) |> UpdateRowState |> dispatch)
        else renderButton "Edit"   "is-warning" (fun _ -> (user.Id, RowState.HasEdit)   |> UpdateRowState |> dispatch)
    let secondButton =
        if user.RowState = RowState.HasEdit then
             renderButton "Update" "is-danger" (fun _ -> user    |> UpdateUser |> dispatch)
        else renderButton "Delete" "is-danger" (fun _ -> user.Id |> DeleteUser |> dispatch)
    divFieldUser
        [
            divInputClass firstInput
            divInputClass lastInput
            divInputClass firstButton
            divInputClass secondButton ]


let addUser (state: State) (dispatch : Msg -> unit) =
    let isDanger = isDangerId state.Error -1
    let textValue = defaultArg state.NewUser {Id=0; FirstName = ""; LastName = ""}
    let firstInput =
        renderInputName "First Name" textValue.FirstName (fun ev -> ev.Value |> SetNewUserFirstName |> dispatch) isDanger
    let lastInput =
        renderInputName "Last Name"  textValue.LastName  (fun ev -> ev.Value |> SetNewUserLastName  |> dispatch) isDanger
    divFieldUser
        [
            divInputClass firstInput
            divInputClass lastInput
            divInputClass
                [ button [ ClassName "button is-medium is-success"
                           Style [ Width (2 * widthButton + 10) ]
                           OnClick (fun _ -> dispatch AddUser) ]
                         [ str "Add" ] ] ]

let render (state: State) (dispatch : Msg -> unit) =
    div
        [ Style [ Padding 20 ] ]
        [ yield! (if state.Error.IsSome then (errorMessage state.Error.Value.Message) else [])
          yield div [ ClassName "box" ] [ addUser state dispatch ]
          yield div
                [ ClassName "box" ]
                [ yield! state.Users |> List.map (fun user -> renderUser user state.Error dispatch) ] ]
