namespace QuizApp.Model

open System
open System.IO
open System.Text.Json
open QuizApp.Model.MCQStudent

module WrittenQuestionStudent =

    type Question = {
        Question: string
        Answer: string
        Mark: int
        WordCount: int
    }

    type StudentAnswer = {
        Question: string
        Answer: string
        Mark: int
    }

    let questionsFilePath = "written_questions.json"
    let scoresFilePath = "Written_Scores.json"

    // Load written questions
    let loadWrittenQuestions () =
        if File.Exists(questionsFilePath) then
            let json = File.ReadAllText(questionsFilePath)
            JsonSerializer.Deserialize<Question list>(json)
        else
            []

    // Save student scores
    let saveStudentScore userName score =
        let scores =
            if File.Exists(scoresFilePath) then
                let json = File.ReadAllText(scoresFilePath)
                JsonSerializer.Deserialize<List<UserScore>>(json)
            else
                []

        let updatedScores = { Name = userName; Score = score } :: scores
        let json = JsonSerializer.Serialize(updatedScores, JsonSerializerOptions(WriteIndented = true))
        File.WriteAllText(scoresFilePath, json)

    // Validate the answer length
    let validateAnswerLength (answer: string) (wordCount: int) =
        let actualWordCount = answer.Split([|' '; '\n'; '\t'|], StringSplitOptions.RemoveEmptyEntries).Length
        actualWordCount >= wordCount

    // Process answers and calculate total score
    let processAnswers (answers: StudentAnswer list) =
        let questions = loadWrittenQuestions ()
        let score =
            answers
            |> List.sumBy (fun a ->
                match questions |> List.tryFind (fun q -> q.Question = a.Question) with
                | Some q when validateAnswerLength a.Answer q.WordCount && a.Answer = q.Answer -> q.Mark
                | _ -> 0)
        score


