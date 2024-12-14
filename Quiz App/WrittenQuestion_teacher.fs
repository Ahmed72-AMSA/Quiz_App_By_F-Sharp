namespace QuizApp.Model

open System
open System.Text.Json
open System.IO

module WrittenQuestionTeacher = 

    type Question = { 
        Question: string
        Answer: string
        Mark: int 
        WordCount: int
    }

    let loadSavedQuestions () =
        let filePath = @"D:\studying section\projects\Programming Language 3\Quiz App\Quiz App\written_questions.json"
        
        if File.Exists(filePath) then
            try
                let json = File.ReadAllText(filePath)
                JsonSerializer.Deserialize<Question list>(json)
            with
            | :? JsonException -> []
        else
            []

    let saveQuestions (questions: Question list) =
        let filePath = @"D:\studying section\projects\Programming Language 3\Quiz App\Quiz App\written_questions.json"
        let json = JsonSerializer.Serialize(questions)
        File.WriteAllText(filePath, json)
