namespace SFX.FSharp

module System =

    open SFX.FSharp.ROP
    open SFX.CSharp.System.Infrastructure

    module Assembly =
        let private service = AssemblyService()
        /// Retrieves the path of the current executable
        let getExeFilePath() = service.GetExeFilePath().ToFSharpResult() |> toResult

    module Base64 =
        let private service = Base64Service()

        /// Converts the provided byte array to a Base64 encoded string
        let toBase64 x = service.ToBase64String(x).ToFSharpResult() |> toResult
        /// Convets the provided Base64 encoded string to a byte array
        let fromBase64 x = service.FromBase64String(x).ToFSharpResult() |> toResult

    module IO =
        let private service = FileSystemService()

        type FilePath = {Value: string}
        /// Creates a FilePath
        let createFilePath x = {Value = x}
        let private fromFilePath (x: SFX.CSharp.System.Model.FilePath) =
            {Value = x.Value}
        let private toFilePath x = 
            let result = SFX.CSharp.System.Model.FilePath()
            result.Value <- x.Value
            result
        type FolderPath = {Value: string}
        /// Creates a FolderPath
        let createFolderPath x = {Value = x}
        let private fromFolderPath (x: SFX.CSharp.System.Model.FolderPath) =
            {Value = x.Value}
        let private toFolderPath x = 
            let result = SFX.CSharp.System.Model.FolderPath()
            result.Value <- x.Value
            result

        /// Checks whether a folder exists
        let folderExists folder = service.FolderExists(folder |> toFolderPath).ToFSharpResult() |> toResult
        /// Creates a new folder
        let createFolder folder = 
            match service.CreateFolder(folder |> toFolderPath).ToFSharpResult() |> toResult with
            | Success _ -> () |> succeed
            | Failure exn -> exn |> fail
        /// Clears the provided folder
        let clearFolder folder = 
            match service.ClearFolder(folder |> toFolderPath).ToFSharpResult() |> toResult with
            | Success _ -> () |> succeed
            | Failure exn -> exn |> fail

        /// Checks whether a file exists
        let fileExits file = service.FileExists(file |> toFilePath).ToFSharpResult() |> toResult
        /// Gets all files in the given folder
        let getFiles folder = 
            match service.GetFiles(folder |> toFolderPath).ToFSharpResult() |> toResult with
            | Success result -> result |> Seq.map fromFilePath |> succeed
            | Failure exn -> exn |> fail

        /// Represents binary or textual data in a file
        type Content =
        /// Binary data
        | B of byte array
        /// Textual data
        | S of string
        /// Creates a file and writes the provided content to it
        let createFile file content =
            let run() =
                match content with
                | B data -> service.CreateFile(file, data).ToFSharpResult() |> toResult
                | S data -> service.CreateFile(file, data).ToFSharpResult() |> toResult
            match run() with
            | Success _ -> () |> succeed
            | Failure exn -> exn |> fail
        /// Creates a file and writes the provided content to it
        let createFileAsync file content =
            let run() =
                match content with
                | B data -> (service.CreateFileAsync(file, data) |> Async.AwaitTask |> Async.RunSynchronously).ToFSharpResult() |> toResult
                | S data -> (service.CreateFileAsync(file, data) |> Async.AwaitTask |> Async.RunSynchronously).ToFSharpResult() |> toResult
            match run() with
            | Success _ -> () |> succeed
            | Failure exn -> exn |> fail

        /// Deletes the provided file
        let deleteFile file =
            match service.DeleteFile(file |> toFilePath).ToFSharpResult() |> toResult with
            | Success _ -> () |> succeed
            | Failure exn -> exn |> fail

        /// Reads the binary content in the file denoted
        let readBinaryContent file = (service.ReadFileBinaryContent (file |> toFilePath)).ToFSharpResult() |> toResult
        /// Reads the textual content in the file denoted
        let readStringContent file = (service.ReadFileStringContent (file |> toFilePath)).ToFSharpResult() |> toResult
        /// Reads the binary content in the file denoted
        let readBinaryContentAsync file = (service.ReadFileBinaryContentAsync (file |> toFilePath) |> Async.AwaitTask |> Async.RunSynchronously).ToFSharpResult() |> toResult
        /// Reads the textual content in the file denoted
        let readStringContentAsync file = (service.ReadFileStringContentAsync (file |> toFilePath) |> Async.AwaitTask |> Async.RunSynchronously).ToFSharpResult() |> toResult

    module SecureString =

        let private service = SecureStringService()

        /// Converts a string to a SecureString
        let toSecureString x = service.ToSecureString(x).ToFSharpResult() |> toResult
        /// Converts a SecureString to a string
        let toInsecureString x = service.ToInsecureString(x).ToFSharpResult() |> toResult