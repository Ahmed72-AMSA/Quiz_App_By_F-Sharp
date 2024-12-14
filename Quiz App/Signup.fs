namespace QuizApp.Model

open System
open System.IO
open System.Text.Json
open System.Text.RegularExpressions
open System.Windows.Forms

module Signup =
    type User = { Email: string; Password: string; Name: string; Type: string }

    let jsonFilePath = @"D:\studying section\projects\Programming Language 3\Quiz App\Quiz App\users.json"

    let loadUsers () =
        if File.Exists(jsonFilePath) then
            let json = File.ReadAllText(jsonFilePath)
            JsonSerializer.Deserialize<User list>(json) |> List.ofSeq
        else
            []

    let saveUsers (users: User list) =
        try
            let json = JsonSerializer.Serialize(users)
            File.WriteAllText(jsonFilePath, json)
        with
        | ex -> 
            MessageBox.Show($"Error saving users: {ex.Message}") |> ignore

    let isValidEmail (email: string) =
        let emailPattern = @"^[^\s@]+@[^\s@]+\.[^\s@]+$"
        Regex.IsMatch(email, emailPattern)

    let isUnique (users: User list) email name =
        not (users |> List.exists (fun user -> user.Email = email || user.Name = name))

    let handleSignup (emailInput: TextBox) (passwordInput: TextBox) (nameInput: TextBox) (statusLabel: Label) =
        let users = loadUsers()
        let email = emailInput.Text
        let password = passwordInput.Text
        let name = nameInput.Text
        let userType = "student"

        if String.IsNullOrEmpty(email) || String.IsNullOrEmpty(password) || String.IsNullOrEmpty(name) then
            statusLabel.Text <- "All fields are required."
        elif not (isValidEmail email) then
            statusLabel.Text <- "Invalid email format."
        elif isUnique users email name then
            let newUser = { Email = email; Password = password; Name = name; Type = userType }
            saveUsers (newUser :: users)
            statusLabel.Text <- "Signup successful!"
            emailInput.Clear()
            passwordInput.Clear()
            nameInput.Clear()
        else
            statusLabel.Text <- "Email or name already exists."
