namespace QuizApp.Model

open System.Collections.Generic


module MCQ_teacher =
 type MCQ = {
    Question: string
    Options: string list
    CorrectAnswer: string
  }
