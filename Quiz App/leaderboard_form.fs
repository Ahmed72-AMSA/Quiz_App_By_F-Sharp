namespace QuizApp.Forms

open System
open System.Drawing
open System.Windows.Forms
open QuizApp.Model

module LeaderBoardForm =

    let createLeaderBoardForm (): Form =
        let form = new Form(Text = "Leaderboard", Width = 400, Height = 500, StartPosition = FormStartPosition.CenterScreen, BackColor = Color.LightBlue)
        form.Font <- new Font("Arial", 12.0f, FontStyle.Regular)

        try
            // Load and process the students
            let filePath = @"D:\studying section\projects\Programming Language 3\Quiz App\Quiz App\Final_Scores.json"
            let students = Leaderboard.loadStudentsFromFile filePath

            // Sort students by percentage in descending order
            let sortedStudents = students |> List.sortByDescending (fun student -> student.Percentage)


            // Create a TableLayoutPanel for better alignment
            let table = new TableLayoutPanel(Dock = DockStyle.Fill, AutoSize = true, Padding = Padding(10))
            table.ColumnCount <- 2
            table.RowCount <- sortedStudents.Length + 1
            table.CellBorderStyle <- TableLayoutPanelCellBorderStyle.Single
            table.BackColor <- Color.White

            // Add a header row
            let nameHeader = new Label(Text = "Name", TextAlign = ContentAlignment.MiddleCenter, Font = new Font("Arial", 12.0f, FontStyle.Bold), ForeColor = Color.DarkBlue, AutoSize = true)
            let scoreHeader = new Label(Text = "Score (%)", TextAlign = ContentAlignment.MiddleCenter, Font = new Font("Arial", 12.0f, FontStyle.Bold), ForeColor = Color.DarkBlue, AutoSize = true)
            table.Controls.Add(nameHeader, 0, 0)
            table.Controls.Add(scoreHeader, 1, 0)

            // Populate the leaderboard data
            sortedStudents
            |> List.iteri (fun i student ->
                let nameLabel = new Label(Text = student.Name, TextAlign = ContentAlignment.MiddleLeft, Font = new Font("Arial", 10.0f), ForeColor = Color.Black, AutoSize = true)
                let scoreLabel = new Label(Text = sprintf "%.2f%%" student.Percentage, TextAlign = ContentAlignment.MiddleRight, Font = new Font("Arial", 10.0f), ForeColor = Color.Black, AutoSize = true)
                table.Controls.Add(nameLabel, 0, i + 1)
                table.Controls.Add(scoreLabel, 1, i + 1)
            )

            // Add the table to the form
            form.Controls.Add(table)

        with
        | ex ->
            // Display error message if loading fails
            let errorLabel = new Label(Text = $"Error: {ex.Message}", Font = new Font("Arial", 10.0f, FontStyle.Bold), ForeColor = Color.Red, AutoSize = true, Dock = DockStyle.Fill)
            form.Controls.Add(errorLabel)

        // Return the form
        form
