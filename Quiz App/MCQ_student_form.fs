namespace QuizApp.Forms

open System
open System.Windows.Forms
open QuizApp.Model.MCQStudent
open QuizApp.Model
open QuizApp.Model.MCQ_teacher
open QuizApp.Model.LogIn
open QuizApp.Forms // Import the namespace for navigation to other forms

module MCQStudentForm =
  let createStudentMCQForm (switchToWrittenQuestion: unit -> unit): Form =
        let form = new Form(Text = "MCQ Questions", Width = 1000, Height = 800)

        let scrollPanel = new Panel(Top = 20, Left = 20, Width = 940, Height = 650)
        scrollPanel.AutoScroll <- true  

        let statusLabel = new Label(Text = "", Top = 720, Left = 20, Width = 500, ForeColor = System.Drawing.Color.Green)
        statusLabel.Font <- new System.Drawing.Font("Arial", 12F)

        let submitButton = new Button(Text = "Submit", Width = 120, Height = 50)
        submitButton.Font <- new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold)
        submitButton.BackColor <- System.Drawing.Color.White
        submitButton.ForeColor <- System.Drawing.Color.MidnightBlue
        submitButton.FlatStyle <- FlatStyle.Flat
        submitButton.FlatAppearance.BorderSize <- 0
        submitButton.Top <- 700
        submitButton.Left <- (form.ClientSize.Width - submitButton.Width) - 40

        let questions = MCQStudent.loadQuestions()

        let questionComboBoxes = new System.Collections.Generic.List<ComboBox>()

        // Function to create UI for each question
        let createQuestionComboBox (index: int) (question: MCQ) =
            let yPosition = 10 + (index * 100)

            let questionTextLabel = new Label(Text = question.Question, Top = yPosition, Left = 10, Width = 900, Height = 40)
            questionTextLabel.Font <- new System.Drawing.Font("Arial", 12F)
            questionTextLabel.ForeColor <- System.Drawing.Color.DarkSlateGray

            let comboBox = new ComboBox(Top = yPosition + 40, Left = 10, Width = 300)
            comboBox.Items.AddRange(question.Options |> List.toArray |> Array.map box)
            comboBox.SelectedIndex <- -1
            comboBox.Font <- new System.Drawing.Font("Arial", 10F)
            comboBox.ForeColor <- System.Drawing.Color.DarkSlateGray
            comboBox.DropDownStyle <- ComboBoxStyle.DropDownList

            comboBox.SelectedIndexChanged.Add(fun _ ->
                MCQStudent.updateAnswer question.Question (comboBox.SelectedItem.ToString())
            )

            scrollPanel.Controls.Add(questionTextLabel)
            scrollPanel.Controls.Add(comboBox)
            questionComboBoxes.Add(comboBox)

        let validateAnswers () =
            questionComboBoxes |> Seq.forall (fun comboBox -> comboBox.SelectedIndex <> -1)

        submitButton.Click.Add(fun _ ->
            if validateAnswers() then
                let score = MCQStudent.calculateScore questions
                let userName =
                    match LogIn.loggedInUserName with
                    | Some name -> name
                    | None -> "Unknown User"
                MCQStudent.saveScoreToFile userName score
                statusLabel.Text <- "Quiz Submitted!"
                statusLabel.ForeColor <- System.Drawing.Color.Green
                form.Hide()
                switchToWrittenQuestion()
            else
                statusLabel.Text <- "Please answer all questions."
                statusLabel.ForeColor <- System.Drawing.Color.Red
        )

        // Add questions to the scrollable panel
        questions |> List.iteri createQuestionComboBox

        form.Controls.Add(scrollPanel)
        form.Controls.Add(statusLabel)
        form.Controls.Add(submitButton)

        form.StartPosition <- FormStartPosition.CenterScreen
        form.FormBorderStyle <- FormBorderStyle.FixedDialog
        form.MaximizeBox <- false
        form.MinimizeBox <- false

        form