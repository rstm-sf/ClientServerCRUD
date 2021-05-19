module App

open Client
open Elmish
open Elmish.React
open Elmish.Debug
open Elmish.HMR

Program.mkProgram 
    State.initialState
    State.update
    View.render
#if DEBUG
|> Program.withConsoleTrace
#endif
|> Program.withReactBatched "elmish-app"
#if DEBUG
|> Program.withDebugger
#endif
|> Program.run
