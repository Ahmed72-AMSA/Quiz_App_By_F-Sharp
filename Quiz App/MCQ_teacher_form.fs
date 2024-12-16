namespace QuizApp.Forms

open System
open System.Windows.Forms
open System.Text.Json
open System.IO
open QuizApp.Model.MCQ_teacher

module MCQForm =

    let createMCQForm () =
        let form = new Form(Text = "Create MCQ", Width = 900, Height = 700)

        // UI Components
        let headerLabel = new Label(Text = "MCQ Creation Form", Top = 10, Left = 300, Width = 300, Font = new System.Drawing.Font("Arial", 14.0f, System.Drawing.FontStyle.Bold), TextAlign = System.Drawing.ContentAlignment.MiddleCenter)

        let questionGroupBox = new GroupBox(Text = "Add Question", Top = 50, Left = 20, Width = 820, Height = 200)
        let questionLabel = new Label(Text = "Question:", Top = 30, Left = 20, Width = 100)
        let questionInput = new TextBox(Top = 30, Left = 120, Width = 600)

        let optionsLabel = new Label(Text = "Options (comma separated):", Top = 70, Left = 20, Width = 200)
        let optionsInput = new TextBox(Top = 70, Left = 220, Width = 500)

        let correctAnswerLabel = new Label(Text = "Correct Answer:", Top = 110, Left = 20, Width = 150)
        let correctAnswerInput = new TextBox(Top = 110, Left = 220, Width = 500)

        let saveButton = new Button(Text = "Save Question", Top = 150, Left = 220, Width = 150)
        let finishButton = new Button(Text = "Finish and Save All", Top = 150, Left = 400, Width = 180)

        questionGroupBox.Controls.AddRange([| questionLabel; questionInput; optionsLabel; optionsInput; correctAnswerLabel; correctAnswerInput; saveButton; finishButton |])

        let questionListBoxGroup = new GroupBox(Text = "Current Session Questions", Top = 260, Left = 20, Width = 400, Height = 250)
        let questionListBox = new ListBox(Top = 30, Left = 20, Width = 350, Height = 180)
        let removeButton = new Button(Text = "Remove Selected Question", Top = 220, Left = 120, Width = 180)
        questionListBoxGroup.Controls.AddRange([| questionListBox; removeButton |])

        let jsonQuestionListBoxGroup = new GroupBox(Text = "Previously Saved Questions", Top = 260, Left = 440, Width = 400, Height = 250)
        let jsonQuestionListBox = new ListBox(Top = 30, Left = 20, Width = 350, Height = 180)
        jsonQuestionListBox.Font <- new System.Drawing.Font("Arial", 10.0f)
        let removeFromJSONButton = new Button(Text = "Remove from JSON", Top = 220, Left = 120, Width = 180)
        jsonQuestionListBoxGroup.Controls.AddRange([| jsonQuestionListBox; removeFromJSONButton |])

        let actionsGroupBox = new GroupBox(Text = "Actions", Top = 530, Left = 20, Width = 820, Height = 100)
        let displayAllButton = new Button(Text = "Display All Questions", Top = 30, Left = 50, Width = 200)
        let writtenQuestionButton = new Button(Text = "Go to Written Questions", Top = 30, Left = 300, Width = 200)
        actionsGroupBox.Controls.AddRange([| displayAllButton; writtenQuestionButton |])

        let statusLabel = new Label(Text = "", Top = 640, Left = 20, Width = 820, ForeColor = System.Drawing.Color.Green, TextAlign = System.Drawing.ContentAlignment.MiddleCenter)

        // Events
        let mutable questions = System.Collections.Generic.HashSet<MCQ>()

        let loadQuestions () =
            let filePath = "Questions.json"
            if File.Exists(filePath) then
                let json = File.ReadAllText(filePath)
                let loadedQuestions = JsonSerializer.Deserialize<MCQ list>(json)
                loadedQuestions |> List.iter (fun q -> questions.Add(q) |> ignore)
                jsonQuestionListBox.Items.Clear()
                loadedQuestions |> List.iter (fun q -> jsonQuestionListBox.Items.Add(q.Question) |> ignore)

        saveButton.Click.Add(fun _ -> 
            let questionText = questionInput.Text
            let options = optionsInput.Text.Split(',') |> List.ofArray |> List.map (fun s -> s.Trim())
            let correctAnswer = correctAnswerInput.Text

            if String.IsNullOrEmpty(questionText) || options.IsEmpty || String.IsNullOrEmpty(correctAnswer) then
                statusLabel.Text <- "All fields must be filled out."
                statusLabel.ForeColor <- System.Drawing.Color.Red
            elif options.Length < 2 then
                statusLabel.Text <- "Please provide at least two options."
                statusLabel.ForeColor <- System.Drawing.Color.Red
            elif not (options |> List.contains correctAnswer) then
                statusLabel.Text <- "Correct answer must be one of the options."
                statusLabel.ForeColor <- System.Drawing.Color.Red
            elif options |> List.distinct |> List.length <> options.Length then
                statusLabel.Text <- "Options must be unique."
                statusLabel.ForeColor <- System.Drawing.Color.Red
            elif questions |> Seq.exists (fun q -> q.Question = questionText) then
                statusLabel.Text <- "This question already exists in the session."
                statusLabel.ForeColor <- System.Drawing.Color.Red
            else
                let mcq = { Question = questionText; Options = options; CorrectAnswer = correctAnswer }
                questions.Add(mcq) |> ignore
                questionListBox.Items.Add(questionText) |> ignore
                questionInput.Clear()
                optionsInput.Clear()
                correctAnswerInput.Clear()
                statusLabel.Text <- "Question added successfully!"
                statusLabel.ForeColor <- System.Drawing.Color.Green
        )

        finishButton.Click.Add(fun _ -> 
            if questions.Count = 0 then
                statusLabel.Text <- "No questions to save. Add some questions first!"
                statusLabel.ForeColor <- System.Drawing.Color.Red
            else
                let filePath = "Questions.json"
                let currentSessionQuestions = questions |> Seq.toList
                let json = JsonSerializer.Serialize(currentSessionQuestions |> List.rev)
                File.WriteAllText(filePath, json)
                questions.Clear()
                questionListBox.Items.Clear()
                jsonQuestionListBox.Items.Clear()
                statusLabel.Text <- "All questions saved to 'Questions.json'!"
                statusLabel.ForeColor <- System.Drawing.Color.Green
        )

        removeButton.Click.Add(fun _ -> 
            let selectedQuestion = questionListBox.SelectedItem
            match selectedQuestion with
            | null -> 
                statusLabel.Text <- "No question selected to remove."
                statusLabel.ForeColor <- System.Drawing.Color.Red
            | _ -> 
                let questionText = selectedQuestion.ToString()
                let questionToRemove = questions |> Seq.tryFind (fun q -> q.Question = questionText)
                match questionToRemove with
                | Some(mcq) -> 
                    questions.Remove(mcq) |> ignore
                    questionListBox.Items.Remove(selectedQuestion) |> ignore
                    statusLabel.Text <- "Question removed from the current session."
                    statusLabel.ForeColor <- System.Drawing.Color.Green
                | None -> 
                    statusLabel.Text <- "Selected question not found in the list."
                    statusLabel.ForeColor <- System.Drawing.Color.Red
        )

        removeFromJSONButton.Click.Add(fun _ -> 
            let selectedQuestion = jsonQuestionListBox.SelectedItem
            match selectedQuestion with
            | null -> 
                statusLabel.Text <- "No question selected to remove from JSON."
                statusLabel.ForeColor <- System.Drawing.Color.Red
            | _ -> 
                let questionText = selectedQuestion.ToString()
                let filePath = @"D:\studying section\projects\Programming Language 3\Quiz App\Quiz App\Questions.json"
                if File.Exists(filePath) then
                    let json = File.ReadAllText(filePath)
                    let questionsInJSON = JsonSerializer.Deserialize<MCQ list>(json)
                    let questionToRemove = questionsInJSON |> List.tryFind (fun q -> q.Question = questionText)
                    match questionToRemove with
                    | Some(mcq) -> 
                        questions.Remove(mcq) |> ignore
                        let updatedQuestions = questionsInJSON |> List.filter (fun q -> q.Question <> questionText)
                        let updatedJson = JsonSerializer.Serialize(updatedQuestions)
                        File.WriteAllText(filePath, updatedJson)
                        jsonQuestionListBox.Items.Clear()
                        loadQuestions()
                        statusLabel.Text <- "Question removed from both session and JSON!"
                        statusLabel.ForeColor <- System.Drawing.Color.Green
                    | None -> 
                        statusLabel.Text <- "Selected question not found"
                        statusLabel.ForeColor <- System.Drawing.Color.Red
                else
                    statusLabel.Text <- "Questions.json file does not exist."
                    statusLabel.ForeColor <- System.Drawing.Color.Red
        )

        displayAllButton.Click.Add(fun _ -> 
            jsonQuestionListBox.Items.Clear()
            loadQuestions()
        )

        writtenQuestionButton.Click.Add(fun _ -> 
            let writtenForm = WrittenTeacherQuestionForm.createExamForm()
            writtenForm.ShowDialog() |> ignore
            form.Show()
        )

        // Adding Controls to the Form
        form.Controls.Add(headerLabel)
        form.Controls.Add(questionGroupBox)
        form.Controls.Add(questionListBoxGroup)
        form.Controls.Add(jsonQuestionListBoxGroup)
        form.Controls.Add(actionsGroupBox)
        form.Controls.Add(statusLabel)

        form
