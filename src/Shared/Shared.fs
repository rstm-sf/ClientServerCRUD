namespace Shared

type UserId = int

type [<CLIMutable>] User =
    {
        Id : UserId
        FirstName : string
        LastName : string
    }
