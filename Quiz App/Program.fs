namespace QuizApp

open System.Windows.Forms
open QuizApp.Forms

module Program =

    [<System.STAThread>]
    do
        Application.EnableVisualStyles()
        Application.SetCompatibleTextRenderingDefault(false)

        let mutable currentForm: Form = null

        // Function to switch to the MCQ Teacher form
        let switchToMCQTeacher () =
            let mcqTeacherForm = MCQForm.createMCQForm() // Assuming you have an MCQTeacherForm module
            currentForm <- mcqTeacherForm
            mcqTeacherForm.Show()

        // Function to switch to the Scoreboard form
        let switchToScoreBoard () =
            // Create and show the ScoreBoard form here
            let scoreBoardForm = scoreBoardForm.createScoreBoardForm()  // Assuming you have the scoreBoardForm module
            currentForm <- scoreBoardForm
            scoreBoardForm.Show()

        let switchToWrittenQuestion () =
            // Call the function to switch to the written question form here
            let writtenQuestionForm = WrittenQuestionStudentForm.createWrittenQuestionForm switchToScoreBoard
            currentForm <- writtenQuestionForm
            writtenQuestionForm.Show()

        let switchToStudentMCQ () =
            let studentMCQForm = MCQStudentForm.createStudentMCQForm switchToWrittenQuestion
            currentForm <- studentMCQForm
            studentMCQForm.Show()

        // Function to switch to the Signup form
        let rec switchToSignup () =
            let signupForm = SignupForm.createSignupForm switchToLogin
            currentForm <- signupForm
            signupForm.Show()

        // Function to switch to the Login form
        and switchToLogin () = 
            let loginForm = LoginForm.createLoginForm switchToSignup switchToMCQTeacher switchToStudentMCQ
            currentForm <- loginForm
            loginForm.Show()

        // Now we pass all three callbacks to createLoginForm
        let initialForm = LoginForm.createLoginForm switchToSignup switchToMCQTeacher switchToStudentMCQ
        currentForm <- initialForm
        Application.Run(initialForm)
