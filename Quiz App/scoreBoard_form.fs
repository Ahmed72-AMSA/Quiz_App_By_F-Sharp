namespace QuizApp.Forms

open System
open System.Windows.Forms
open System.Drawing
open QuizApp.Model.LogIn
open QuizApp.Model

module scoreBoardForm =

    // Define UI components
    let createScoreBoardForm () =
        let form = new Form(Text = "Quiz App", Width = 400, Height = 300, BackColor = Color.LightGray)

        // Header Label
        let headerLabel = new Label(Text = "Score Board", Location = Point(50, 10), Width = 300, Font = new Font("Arial", 16.0f, FontStyle.Bold), TextAlign = ContentAlignment.MiddleCenter)

        // User Info Label
        let labelUserName = new Label(Text = "User: Not Logged In", Location = Point(50, 60), Width = 300, Font = new Font("Arial", 10.0f))

        // Score Info Label
        let labelScore = new Label(Text = "Score: 0/0", Location = Point(50, 110), Width = 300, Font = new Font("Arial", 10.0f), ForeColor = Color.DarkGreen)

        // Show Score Button
        let buttonShowScore = new Button(Text = "Show Score", Location = Point(125, 170), Width = 150, BackColor = Color.LightBlue, FlatStyle = FlatStyle.Flat)
        buttonShowScore.FlatAppearance.BorderSize <- 0

        // Footer Note
        let footerLabel = new Label(Text = "Click 'Show Score' to update your results", Location = Point(50, 230), Width = 300, Font = new Font("Arial", 8.0f), ForeColor = Color.DarkGray, TextAlign = ContentAlignment.MiddleCenter)

        // Event handler for showing the score
        buttonShowScore.Click.Add(fun _ ->
            match loggedInUserName with
            | Some userName ->
                // Retrieve the score and full marks from the FinalScoreCalculator module
                let (realScore, fullMarks) = FinalScoreCalculator.calculateFinalScore ()
                labelUserName.Text <- sprintf "User: %s" userName
                labelScore.Text <- sprintf "Score: %d/%d" realScore fullMarks

                // Save the final score to a JSON file
                FinalScoreCalculator.saveFinalScore ()
            | None ->
                labelUserName.Text <- "No user logged in."
                labelScore.Text <- "Score: 0/0"
        )

        // Add controls to the form
        form.Controls.Add(headerLabel)
        form.Controls.Add(labelUserName)
        form.Controls.Add(labelScore)
        form.Controls.Add(buttonShowScore)
        form.Controls.Add(footerLabel)

        form
