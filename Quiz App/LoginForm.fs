namespace QuizApp.Forms

open System.Windows.Forms
open QuizApp.Model.LogIn
open QuizApp.Model
open System.Drawing

module LoginForm =
    let users = Signup.loadUsers()

    let createLoginForm (switchToSignup: unit -> unit) (switchToMCQTeacher: unit -> unit) (switchToStudentMCQ: unit -> unit): Form = 
        let form = new Form(Text = "Login Form", Width = 400, Height = 300)

        let nameLabel = new Label(Text = "Name:", Top = 20, Left = 30, Width = 80)
        let nameInput = new TextBox(Top = 20, Left = 120, Width = 200)

        let passwordLabel = new Label(Text = "Password:", Top = 60, Left = 30, Width = 80)
        let passwordInput = new TextBox(Top = 60, Left = 120, Width = 200, PasswordChar = '*')

        let loginButton = new Button(Text = "Login", Top = 100, Left = 120, Width = 80)
        let signupButton = new Button(Text = "Signup", Top = 100, Left = 220, Width = 80)

        let statusLabel = new Label(Text = "", Top = 150, Left = 30, Width = 300, ForeColor = Color.Red)

        // Helper function to handle login logic
        let handleLogin () =
            let name = nameInput.Text.Trim()
            let password = passwordInput.Text.Trim()

            if name = "" || password = "" then
                statusLabel.Text <- "Please fill out all fields."
            else
                LogIn.handleSignIn nameInput passwordInput statusLabel
                if statusLabel.Text.StartsWith("Login Successful!") then
                    match LogIn.loggedInUserName with
                    | Some userName ->
                        match users |> List.tryFind (fun user -> user.Name = userName) with
                        | Some user when user.Type = "teacher" ->
                            form.Hide()
                            switchToMCQTeacher()
                        | Some user when user.Type = "student" ->
                            form.Hide()
                            switchToStudentMCQ()
                        | _ ->
                            statusLabel.Text <- "Access restricted."
                    | None ->
                        statusLabel.Text <- "Error retrieving user information."

        // Event handlers
        loginButton.Click.Add(fun _ -> handleLogin())
        signupButton.Click.Add(fun _ -> 
            form.Hide()
            switchToSignup()
        )

        form.Controls.Add(nameLabel)
        form.Controls.Add(nameInput)
        form.Controls.Add(passwordLabel)
        form.Controls.Add(passwordInput)
        form.Controls.Add(loginButton)
        form.Controls.Add(signupButton)
        form.Controls.Add(statusLabel)

        form
