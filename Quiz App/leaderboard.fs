namespace QuizApp.Model

open System
open System.IO
open System.Text.Json

// Define a record type to represent student data
type Student = {
    Name: string
    RealScore: int
    FullMarks: int
    Percentage: float
}

module Leaderboard =

    // Function to load data from a JSON file and calculate percentage for each student
    let loadStudentsFromFile filePath =
        try
            let json = File.ReadAllText(filePath)
            let students = JsonSerializer.Deserialize<Student list>(json)
            
            // Calculate percentage for each student
            students |> List.map (fun student -> 
                { student with 
                    Percentage = float student.RealScore / float student.FullMarks * 100.0 
                })
        with
        | ex -> 
            printfn "Error: %s" ex.Message
            []  // Return an empty list if there's an error

    // Function to generate a leaderboard string sorted by percentage
    let generateLeaderboardString students =
        students
        |> List.sortByDescending (fun student -> student.Percentage) // Sort by percentage descending
        |> List.mapi (fun i student -> 
            sprintf "%d. %s - %.2f%%" (i + 1) student.Name student.Percentage)
        |> String.concat Environment.NewLine
