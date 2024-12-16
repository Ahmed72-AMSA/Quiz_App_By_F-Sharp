namespace QuizApp.Model

open System
open System.IO
open System.Text.Json
open QuizApp.Model.MCQ_teacher

module MCQStudent =
    // Define file paths for questions and scores
    let questionsFilePath = "Questions.json"
    let scoresFilePath = "MCQ_Scores.json"

    // Record to represent a score entry
    type UserScore = { Name: string; Score: int }

    // Load questions from the Questions.json file
    let loadQuestions () =
        if File.Exists(questionsFilePath) then
            let json = File.ReadAllText(questionsFilePath)
            JsonSerializer.Deserialize<MCQ list>(json)
        else
            []

    // Mutable map to store the selected answers
    let mutable selectedAnswers = Map.empty<string, string>

    // Update the selected answer for a question
    let updateAnswer questionText answer =
        selectedAnswers <- selectedAnswers.Add(questionText, answer)

    // Calculate the score based on selected answers
    let calculateScore (questions: MCQ list) =
        questions |> List.fold (fun acc q ->
            if selectedAnswers |> Map.tryFind q.Question = Some q.CorrectAnswer then
                acc + 1
            else
                acc) 0

    // Save the score to the MCQ_Scores.json file
    let saveScoreToFile userName score =
        let userScoreData = { Name = userName; Score = score }

        let scores =
            if File.Exists(scoresFilePath) then
                let json = File.ReadAllText(scoresFilePath)
                JsonSerializer.Deserialize<List<UserScore>>(json) // Deserialize to List<UserScore> instead of FSharpList
            else
                []

        let updatedScores = userScoreData :: scores

        let json = JsonSerializer.Serialize(updatedScores, JsonSerializerOptions(WriteIndented = true))
        File.WriteAllText(scoresFilePath, json)
