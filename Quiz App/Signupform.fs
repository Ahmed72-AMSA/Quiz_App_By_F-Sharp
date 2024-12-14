namespace QuizApp.Forms

open System.Windows.Forms
open QuizApp.Model.Signup

module SignupForm =
    let createSignupForm (switchToLogin: unit -> unit): Form = 
        let form = new Form(Text = "Signup Form", Width = 400, Height = 300)

        let emailLabel = new Label(Text = "Email:", Top = 20, Left = 30, Width = 80)
        let emailInput = new TextBox(Top = 20, Left = 120, Width = 200)

        let passwordLabel = new Label(Text = "Password:", Top = 60, Left = 30, Width = 80)
        let passwordInput = new TextBox(Top = 60, Left = 120, Width = 200, PasswordChar = '*')

        let nameLabel = new Label(Text = "Name:", Top = 100, Left = 30, Width = 80)
        let nameInput = new TextBox(Top = 100, Left = 120, Width = 200)

        let signupButton = new Button(Text = "Signup", Top = 140, Left = 120, Width = 80)
        let loginButton = new Button(Text = "Login", Top = 140, Left = 220, Width = 80) 
        let statusLabel = new Label(Text = "", Top = 180, Left = 30, Width = 300, ForeColor = System.Drawing.Color.Red)

        signupButton.Click.Add(fun _ -> 
            handleSignup emailInput passwordInput nameInput statusLabel
        )

        loginButton.Click.Add(fun _ -> 
            form.Hide()         // Hide the current signup form
            switchToLogin()     // Call the function to show the login form
        )

        form.Controls.Add(emailLabel)
        form.Controls.Add(emailInput)
        form.Controls.Add(passwordLabel)
        form.Controls.Add(passwordInput)
        form.Controls.Add(nameLabel)
        form.Controls.Add(nameInput)
        form.Controls.Add(signupButton)
        form.Controls.Add(loginButton)
        form.Controls.Add(statusLabel)

        form
