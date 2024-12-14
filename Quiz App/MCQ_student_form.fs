namespace QuizApp.Forms

open System
open System.Windows.Forms
open QuizApp.Model.MCQStudent
open QuizApp.Model
open QuizApp.Model.MCQ_teacher
open QuizApp.Model.LogIn
open QuizApp.Forms // Import the namespace for navigation to other forms

module MCQStudentForm =
    let createStudentMCQForm (switchToWrittenQuestion : unit -> unit): Form =
        let form = new Form(Text = "MCQ Questions", Width = 1000, Height = 800)  // Increased form size

        // UI Components
        let questionLabel = new Label(Text = "Select an answer:", Top = 20, Left = 20, Width = 250)
        questionLabel.Font <- new System.Drawing.Font("Arial", 14F)
        
        let statusLabel = new Label(Text = "", Top = 720, Left = 20, Width = 500, ForeColor = System.Drawing.Color.Green)
        statusLabel.Font <- new System.Drawing.Font("Arial", 12F)

        let submitButton = new Button(Text = "Submit", Width = 120, Height = 50)
        submitButton.Font <- new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold)
        submitButton.BackColor <- System.Drawing.Color.White
        submitButton.ForeColor <- System.Drawing.Color.MidnightBlue
        submitButton.FlatStyle <- FlatStyle.Flat
        submitButton.FlatAppearance.BorderSize <- 0
        submitButton.Top <- 700 // Position it at the bottom of the form
        submitButton.Left <- (form.ClientSize.Width - submitButton.Width)  // Center the button

        let questions = MCQStudent.loadQuestions()
        


        let questionComboBoxes = new System.Collections.Generic.List<ComboBox>()

        let createQuestionComboBox (question: MCQ) (index: int) =
            let questionTextLabel = new Label(Text = question.Question, Top = 60 + (index * 120), Left = 20, Width = 950, Height = 40)
            questionTextLabel.Font <- new System.Drawing.Font("Arial", 12F)
            questionTextLabel.ForeColor <- System.Drawing.Color.DarkSlateGray

            let comboBox = new ComboBox(Top = 100 + (index * 120), Left = 20, Width = 300)
            comboBox.Items.AddRange(question.Options |> List.toArray |> Array.map box)  
            comboBox.SelectedIndex <- -1  
            comboBox.Font <- new System.Drawing.Font("Arial", 10F)
            comboBox.ForeColor <- System.Drawing.Color.DarkSlateGray
            comboBox.DropDownStyle <- ComboBoxStyle.DropDownList

            comboBox.SelectedIndexChanged.Add(fun _ -> 
                MCQStudent.updateAnswer question.Question (comboBox.SelectedItem.ToString())
            )

            form.Controls.Add(questionTextLabel)
            form.Controls.Add(comboBox)

            questionComboBoxes.Add(comboBox)

        let validateAnswers () =
            questionComboBoxes |> Seq.forall (fun comboBox -> comboBox.SelectedIndex <> -1)


        submitButton.Click.Add(fun _ -> 
            if validateAnswers() then
                let score = MCQStudent.calculateScore questions
                let userName = match LogIn.loggedInUserName with
                               | Some name -> name
                               | None -> "Unknown User"
                MCQStudent.saveScoreToFile userName score
                statusLabel.Text <- sprintf "Quiz Submitted!" 
                statusLabel.ForeColor <- System.Drawing.Color.Green
                form.Hide() 
                switchToWrittenQuestion() 
            else
                statusLabel.Text <- "Please answer all questions."
                statusLabel.ForeColor <- System.Drawing.Color.Red
        )

        questions |> List.iteri (fun index question -> 
            createQuestionComboBox question index
        )

        form.Controls.Add(questionLabel)
        form.Controls.Add(statusLabel)
        form.Controls.Add(submitButton)

        form.StartPosition <- FormStartPosition.CenterScreen
        form.FormBorderStyle <- FormBorderStyle.None  
        form.MaximizeBox <- true 
        form.MinimizeBox <- true  // Disable minimize box
        form.ShowIcon <- false
        form.Width <- 1000  
        form.Height <- 800  

        form
