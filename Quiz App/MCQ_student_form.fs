namespace QuizApp.Forms

open System
open System.Windows.Forms
open QuizApp.Model.MCQStudent
open QuizApp.Model
open QuizApp.Model.MCQ_teacher
open QuizApp.Model.LogIn
open QuizApp.Forms // Import the namespace for navigation to other forms

module MCQStudentForm =
    let createStudentMCQForm (switchToWrittenQuestion : unit -> unit):Form =
        let form = new Form(Text = "MCQ Questions", Width = 800, Height = 600)

        // UI Components
        let questionLabel = new Label(Text = "Select an answer:", Top = 20, Left = 20, Width = 200)
        let statusLabel = new Label(Text = "", Top = 500, Left = 20, Width = 500, ForeColor = System.Drawing.Color.Green)
        let submitButton = new Button(Text = "Submit", Top = 350, Left = 550, Width = 100)

        let questions = MCQStudent.loadQuestions()
        
        // Use List for mutable list of ComboBoxes
        let questionComboBoxes = new System.Collections.Generic.List<ComboBox>()

        let createQuestionComboBox (question: MCQ) (index: int) =
            let panel = new Panel(Top = 60 + (index * 120), Left = 20, Width = 750, Height = 50)

            let questionTextLabel = new Label(Text = question.Question, Top = 5, Left = 0, Width = 600, Height = 20)

            let comboBox = new ComboBox(Top = 25, Left = 0, Width = 200)
            
            comboBox.Items.AddRange(question.Options |> List.toArray |> Array.map box)  // Convert each string to 'obj'
            comboBox.SelectedIndex <- -1  // Set default to no selection
            panel.Controls.Add(questionTextLabel)
            panel.Controls.Add(comboBox)

            comboBox.SelectedIndexChanged.Add(fun _ -> 
                MCQStudent.updateAnswer question.Question (comboBox.SelectedItem.ToString())
            )

            panel, comboBox

        let validateAnswers () =
            questionComboBoxes |> Seq.forall (fun comboBox -> comboBox.SelectedIndex <> -1)

        // Function to calculate and display the score
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
            let panel, comboBox = createQuestionComboBox question index
            questionComboBoxes.Add(comboBox)
            form.Controls.Add(panel)
        )

        form.Controls.Add(questionLabel)
        form.Controls.Add(statusLabel)
        form.Controls.Add(submitButton)

        form
