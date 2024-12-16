namespace QuizApp.Model

open System.Windows.Forms
open QuizApp.Model.Signup
open System
open System.Text.Json
open System.IO

// Define the FinalScore type
type FinalScore = {
    Name: string
    RealScore: int
    FullMarks: int
}

module LogIn =
    let mutable loggedInUserName: string option = None
    let mutable loggedInUserScore: int option = None

    // Define file paths for user data and final scores
    let usersFilePath = "Users.json"
    let finalScoresFilePath = "Final_Scores.json"

    // Function to calculate the percentage score
    let calculatePercentageScore (realScore: int) (fullMarks: int) =
        if fullMarks > 0 then
            (float realScore / float fullMarks) * 100.0
        else
            0.0

    // Load users from JSON file
    let loadUsers () =
        if File.Exists(usersFilePath) then
            let json = File.ReadAllText(usersFilePath)
            JsonSerializer.Deserialize<List<User>>(json)  // Deserialize into a list of users
        else
            []  // Return an empty list if the file does not exist

    // Load final scores from the final_scores.json file
    let loadFinalScores () =
        if File.Exists(finalScoresFilePath) then
            let json = File.ReadAllText(finalScoresFilePath)
            JsonSerializer.Deserialize<List<FinalScore>>(json)  // Deserialize into a list of final scores
        else
            []  // Return an empty list if the file does not exist

    // Handle the user sign-in
    let handleSignIn (nameInput: TextBox) (passwordInput: TextBox) (statusLabel: Label) =
        let users = loadUsers()
        let finalScores = loadFinalScores()

        let name = nameInput.Text.Trim().ToLowerInvariant()
        let password = passwordInput.Text.Trim()

        if String.IsNullOrEmpty(name) || String.IsNullOrEmpty(password) then
            statusLabel.Text <- "Username and password are required."
        else
            // Find user by name (case-insensitive match)
            match users |> List.tryFind (fun user -> user.Name.Trim().ToLowerInvariant() = name) with
            | Some user when user.Password = password ->
                // Check if the user has already taken the quiz by looking for their score
                match finalScores |> List.tryFind (fun score -> score.Name.Trim().ToLowerInvariant() = name) with
                | Some score ->
                    // If a score is found, display it and prevent further login
                    let percentage = calculatePercentageScore score.RealScore score.FullMarks
                    statusLabel.Text <- sprintf "You have already taken the quiz. Your score: %.2f%%" percentage
                    nameInput.Clear()
                    passwordInput.Clear()
                    loggedInUserName <- Some user.Name
                    loggedInUserScore <- Some (int percentage)  
                | None ->
                    // If no score is found, proceed with login and allow quiz attempt
                    statusLabel.Text <- "Login Successful! Welcome! You can now take the quiz."
                    nameInput.Clear()
                    passwordInput.Clear()
                    loggedInUserName <- Some user.Name
                    loggedInUserScore <- Some 0  // Set initial score as 0 until quiz is taken
            | Some _ ->
                // Password mismatch
                statusLabel.Text <- "Invalid username or password."
            | None ->
                // If no user is found with this name
                statusLabel.Text <- "Invalid username or password."
