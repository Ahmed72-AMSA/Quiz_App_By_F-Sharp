namespace QuizApp.Model

open System
open System.IO
open System.Text.Json
open QuizApp.Model.MCQStudent
open QuizApp.Model.WrittenQuestionStudent
open QuizApp.Model.LogIn
open System.Windows.Forms
open MCQ_teacher

module FinalScoreCalculator =

    // Define file paths for MCQ and Written question scores
    let mcqScoreFilePath = @"D:\studying section\projects\Programming Language 3\Quiz App\Quiz App\MCQ_Scores.json"
    let writtenScoreFilePath = @"D:\studying section\projects\Programming Language 3\Quiz App\Quiz App\Written_Scores.json"
    let mcqQuestionFilePath = @"D:\studying section\projects\Programming Language 3\Quiz App\Quiz App\Questions.json"
    let writtenQuestionFilePath = @"D:\studying section\projects\Programming Language 3\Quiz App\Quiz App\written_questions.json"
    let finalScoreFilePath = @"D:\studying section\projects\Programming Language 3\Quiz App\Quiz App\Final_Scores.json"  

    type WrittenScore = { Name: string; Score: int }

    // Define a type to store final score data
    type FinalScore = { Name: string; RealScore: int; FullMarks: int }

    let loadMCQQuestions () =
        if File.Exists(mcqQuestionFilePath) then
            let json = File.ReadAllText(mcqQuestionFilePath)
            JsonSerializer.Deserialize<List<MCQ>>(json)
        else
            []

    let loadWrittenQuestions () =
        if File.Exists(writtenQuestionFilePath) then
            let json = File.ReadAllText(writtenQuestionFilePath)
            JsonSerializer.Deserialize<List<WrittenQuestionStudent.Question>>(json)
        else
            []

    // Calculate total MCQ marks
    let calculateTotalMCQMarks () =
        let mcqQuestions = loadMCQQuestions()
        List.length mcqQuestions  // Each question is worth 1 mark

    // Calculate total written marks (sum of marks for each question)
    let calculateTotalWrittenMarks () =
        let writtenQuestions = loadWrittenQuestions()
        writtenQuestions |> List.sumBy (fun q -> q.Mark)

    let getMCQScoreForUser userName =
        if File.Exists(mcqScoreFilePath) then
            let json = File.ReadAllText(mcqScoreFilePath)
            let mcqScores = JsonSerializer.Deserialize<List<MCQStudent.UserScore>>(json)
            match mcqScores |> List.tryFind (fun score -> score.Name = userName) with
            | Some score -> score.Score
            | None -> 0  
        else
            0  

    let getWrittenScoreForUser userName =
        if File.Exists(writtenScoreFilePath) then
            let json = File.ReadAllText(writtenScoreFilePath)
            let writtenScores = JsonSerializer.Deserialize<List<WrittenScore>>(json)
            match writtenScores |> List.tryFind (fun score -> score.Name = userName) with
            | Some score -> score.Score
            | None -> 0  
        else
            0  

    let calculateFinalScore () =
        match loggedInUserName with
        | Some userName -> 
            let mcqMarks = calculateTotalMCQMarks()  // Total possible marks from MCQ questions
            let writtenMarks = calculateTotalWrittenMarks()  // Total possible marks from written questions

            let mcqScore = getMCQScoreForUser userName
            let writtenScore = getWrittenScoreForUser userName

            let realScore = mcqScore + writtenScore
            let fullMarks = mcqMarks + writtenMarks

            (realScore, fullMarks)
        | None -> 
            // If no user is logged in, return 0 score and 0 full marks
            (0, 0)

    // Display the final score in a form
    let displayFinalScore () =
        match loggedInUserName with
        | Some userName -> 
            let (realScore, fullMarks) = calculateFinalScore()
            (userName, realScore, fullMarks)
        | None -> 
            ("No user logged in", 0, 0)

    // Save the final score to a JSON file
    let saveFinalScore () =
        match loggedInUserName with
        | Some userName -> 
            let (realScore, fullMarks) = calculateFinalScore()
            let finalScore = { Name = userName; RealScore = realScore; FullMarks = fullMarks }

            // Load existing final scores, if any
            let existingScores =
                if File.Exists(finalScoreFilePath) then
                    let json = File.ReadAllText(finalScoreFilePath)
                    JsonSerializer.Deserialize<List<FinalScore>>(json)
                else
                    []

            // Add the new final score to the list
            let updatedScores = existingScores @ [finalScore]

            // Serialize the updated list and write it to the file
            let json = JsonSerializer.Serialize(updatedScores, JsonSerializerOptions(WriteIndented = true))
            File.WriteAllText(finalScoreFilePath, json)
        | None -> 
            // If no user is logged in, we don't save the score
            ()
