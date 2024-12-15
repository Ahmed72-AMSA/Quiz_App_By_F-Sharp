namespace QuizApp.Forms

open System.Windows.Forms
open QuizApp.Model.WrittenQuestionStudent
open QuizApp.Model
open System.Drawing

module WrittenQuestionStudentForm =

    let createWrittenQuestionForm (switchToScoreBoard: unit -> unit): Form =
        let form = new Form(Text = "Written Questions", Width = 800, Height = 600)

        // Set a soft background color for the form
        form.BackColor <- System.Drawing.Color.FromArgb(245, 245, 245) // Light gray for a formal feel

        // Status label to show submission message
        let statusLabel = 
            new Label(Text = "", Top = 500, Left = 20, Width = 500, ForeColor = System.Drawing.Color.Green)
        statusLabel.Font <- new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Italic)

        // Submit Button with modern look
        let submitButton = 
            new Button(Text = "Submit", Top = 450, Left = 650, Width = 100, Height = 50)
        submitButton.Font <- new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold)
        submitButton.BackColor <- System.Drawing.Color.MidnightBlue
        submitButton.ForeColor <- System.Drawing.Color.White
        submitButton.FlatStyle <- FlatStyle.Flat
        submitButton.FlatAppearance.BorderSize <- 0
        submitButton.Cursor <- Cursors.Hand
        submitButton.FlatAppearance.BorderColor <- System.Drawing.Color.FromArgb(0, 0, 0, 0)
        submitButton.MouseEnter.Add(fun _ -> submitButton.BackColor <- System.Drawing.Color.DarkBlue)
        submitButton.MouseLeave.Add(fun _ -> submitButton.BackColor <- System.Drawing.Color.MidnightBlue)

        let writtenQuestions = loadWrittenQuestions ()
        let answersTextBoxes = new System.Collections.Generic.List<TextBox>()

        // Function to create a panel for each question
        let createQuestionPanel (question: Question) (index: int) =
            let panel = new Panel(Top = 20 + (index * 120), Left = 20, Width = 750, Height = 120)
            panel.BackColor <- System.Drawing.Color.White
            panel.Padding <- new Padding(15) // Soft padding for a more relaxed look

            let questionLabel = 
                new Label(Text = question.Question, Top = 5, Left = 10, Width = 600, Height = 40)
            questionLabel.Font <- new System.Drawing.Font("Segoe UI", 14F, FontStyle.Regular)
            questionLabel.ForeColor <- System.Drawing.Color.DarkSlateGray
            questionLabel.TextAlign <- ContentAlignment.TopLeft

            // Word Count Label styling
            let wordCountLabel = 
                new Label(Text = sprintf "Word Count Needed: %d" question.WordCount, Top = 50, Left = 10, Width = 300)
            wordCountLabel.Font <- new System.Drawing.Font("Segoe UI", 10F, FontStyle.Regular)
            wordCountLabel.ForeColor <- System.Drawing.Color.Gray
            wordCountLabel.TextAlign <- ContentAlignment.TopLeft

            // TextBox styling (without border and with a nice background color)
            let textBox = 
                new TextBox(Top = 70, Left = 10, Width = 700, Height = 40, Multiline = true)
            textBox.Font <- new System.Drawing.Font("Segoe UI", 12F)
            textBox.Padding <- new Padding(10)
            textBox.BorderStyle <- BorderStyle.None
            textBox.BackColor <- System.Drawing.Color.FromArgb(240, 240, 240)
            textBox.ForeColor <- System.Drawing.Color.DarkSlateGray
            textBox.ScrollBars <- ScrollBars.Vertical

            // Add components to panel
            panel.Controls.Add(questionLabel)
            panel.Controls.Add(wordCountLabel)
            panel.Controls.Add(textBox)

            answersTextBoxes.Add(textBox)
            panel

        // Function to validate if all questions are answered
        let validateAnswers () =
            answersTextBoxes |> Seq.forall (fun textBox -> textBox.Text.Length > 0)

        // Event handler for the Submit button
        submitButton.Click.Add(fun _ ->
            if validateAnswers() then
                let studentAnswers =
                    writtenQuestions
                    |> List.mapi (fun i q -> 
                        {
                            Question = q.Question
                            Answer = answersTextBoxes.[i].Text
                            Mark = 
                                if validateAnswerLength answersTextBoxes.[i].Text q.WordCount
                                   && answersTextBoxes.[i].Text = q.Answer then 
                                    q.Mark 
                                else 
                                    0
                        })

                let totalScore = processAnswers studentAnswers
                statusLabel.Text <- sprintf "Submitted! Total score: %d" totalScore
                statusLabel.ForeColor <- System.Drawing.Color.Green

                // Save the student's score after submission
                let userName = match LogIn.loggedInUserName with
                               | Some name -> name
                               | None -> "Unknown User"
                saveStudentScore userName totalScore
                form.Hide()
                switchToScoreBoard()  // Navigate to the scoreboard after submitting
            else
                statusLabel.Text <- "Answer all questions."
                statusLabel.ForeColor <- System.Drawing.Color.Red
        )

        // Add each question's panel to the form
        writtenQuestions |> List.iteri (fun index question -> 
            let panel = createQuestionPanel question index
            form.Controls.Add(panel)
        )

        // Add other controls to the form
        form.Controls.Add(statusLabel)
        form.Controls.Add(submitButton)

        form
