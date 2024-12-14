namespace QuizApp.Forms

open System
open System.Windows.Forms
open System.Drawing
open QuizApp.Model.WrittenQuestionTeacher

module WrittenTeacherQuestionForm =
    // Function to switch to the Leaderboard form
    let switchToLeaderboard () =
        // Assuming LeaderboardForm is a valid form module
        let leaderboardForm = LeaderBoardForm.createLeaderBoardForm() 
        leaderboardForm.Show()

    let createExamForm (): Form = 
        let form = new Form(Text = "Exam Application", Width = 500, Height = 600, StartPosition = FormStartPosition.CenterScreen, BackColor = Color.LightGray)

        let headerLabel = new Label(Text = "Exam Question Manager", Top = 10, Left = 150, Width = 200, Font = new Font("Arial", 14.0f, FontStyle.Bold), ForeColor = Color.Navy)

        let questionLabel = new Label(Text = "Question:", Top = 60, Left = 20, Width = 100, Font = new Font("Arial", 10.0f))
        let questionTextBox = new TextBox(Top = 60, Left = 130, Width = 320)

        let noteLabel = new Label(Text = "Note: Enter question name to delete if you wish", Top = 90, Left = 130, Width = 320, Font = new Font("Arial", 9.0f, FontStyle.Italic), ForeColor = Color.DarkRed)

        let answerLabel = new Label(Text = "Answer:", Top = 130, Left = 20, Width = 100, Font = new Font("Arial", 10.0f))
        let answerTextBox = new TextBox(Top = 130, Left = 130, Width = 320)

        let markLabel = new Label(Text = "Mark:", Top = 170, Left = 20, Width = 100, Font = new Font("Arial", 10.0f))
        let markTextBox = new TextBox(Top = 170, Left = 130, Width = 320)

        let wordCountLabel = new Label(Text = "Word Count:", Top = 210, Left = 20, Width = 100, Font = new Font("Arial", 10.0f))
        let wordCountTextBox = new TextBox(Top = 210, Left = 130, Width = 320)

        let saveButton = new Button(Text = "Save", Top = 270, Left = 130, Width = 100, BackColor = Color.LightBlue, Font = new Font("Arial", 10.0f))
        let showButton = new Button(Text = "Show All", Top = 270, Left = 250, Width = 100, BackColor = Color.LightGreen, Font = new Font("Arial", 10.0f))
        let deleteButton = new Button(Text = "Delete", Top = 320, Left = 130, Width = 100, BackColor = Color.LightCoral, Font = new Font("Arial", 10.0f))
        let leaderboardButton = new Button(Text = "Scores", Top = 320, Left = 250, Width = 100, BackColor = Color.LightYellow, Font = new Font("Arial", 10.0f))

        // Add controls to the form
        form.Controls.AddRange([| headerLabel; questionLabel; questionTextBox; noteLabel; answerLabel; answerTextBox; markLabel; markTextBox; wordCountLabel; wordCountTextBox; saveButton; showButton; deleteButton; leaderboardButton |])

        // Save button click event
        saveButton.Click.Add(fun _ -> 
            let question = questionTextBox.Text
            let answer = answerTextBox.Text
            let markText = markTextBox.Text
            let wordCountText = wordCountTextBox.Text

            if String.IsNullOrWhiteSpace(question) || String.IsNullOrWhiteSpace(answer) || String.IsNullOrWhiteSpace(markText) || String.IsNullOrWhiteSpace(wordCountText) then
                MessageBox.Show("Please fill in all fields.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error) |> ignore
            else
                let mark =
                    match Int32.TryParse(markText) with
                    | (true, value) -> Some value
                    | _ -> 
                        MessageBox.Show("Please enter a valid mark.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error) |> ignore
                        None
                 
                let wordCount =
                    match Int32.TryParse(wordCountText) with
                    | (true, value) -> Some value
                    | _ -> 
                        MessageBox.Show("Please enter a valid word count.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error) |> ignore
                        None

                match mark, wordCount with
                | Some validMark, Some validWordCount -> 
                    let questionData = { Question = question; Answer = answer; Mark = validMark; WordCount = validWordCount }
                    let existingQuestions = loadSavedQuestions ()

                    if existingQuestions |> List.exists (fun q -> q.Question = question) then
                        MessageBox.Show("This question already exists.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error) |> ignore
                    else
                        let updatedQuestions = existingQuestions @ [questionData]
                        saveQuestions updatedQuestions
                        MessageBox.Show("Question saved successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information) |> ignore
                | _ -> ()
        )

        // Show All button click event
        showButton.Click.Add(fun _ -> 
            let questions = loadSavedQuestions ()
            if List.isEmpty questions then
                MessageBox.Show("No saved questions.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information) |> ignore
            else
                let questionText = questions |> List.map (fun q -> $"Question: {q.Question}\nAnswer: {q.Answer}\nMark: {q.Mark}\nWord Count: {q.WordCount}") |> String.concat "\n\n"
                MessageBox.Show(questionText, "Saved Questions", MessageBoxButtons.OK, MessageBoxIcon.Information) |> ignore
        )

        // Delete button click event
        deleteButton.Click.Add(fun _ -> 
            let question = questionTextBox.Text

            if String.IsNullOrWhiteSpace(question) then
                MessageBox.Show("Please enter the question text to delete.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error) |> ignore
            else
                let existingQuestions = loadSavedQuestions ()
                let updatedQuestions = existingQuestions |> List.filter (fun q -> q.Question <> question)

                if List.length updatedQuestions = List.length existingQuestions then
                    MessageBox.Show("Question not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error) |> ignore
                else
                    saveQuestions updatedQuestions
                    MessageBox.Show("Question deleted successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information) |> ignore
        )

        // Leaderboard button click event
        leaderboardButton.Click.Add(fun _ -> 
            switchToLeaderboard ()
        )

        form
