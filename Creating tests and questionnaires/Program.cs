using Microsoft.VisualBasic.FileIO;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Cryptography;

class Program
{

    static void Main(string[] args)
    {

        SelectRole();

    }

    static void SelectRole()
    {

        string pathregist = @"AllData\Авторизация.txt";
        string pathquestion = @"AllData\Вопросы.txt";
        string pathanswer = @"AllData\Ответы.txt";
        string pathuser = @"AllData\user";

        Regist regist = new Regist();
        regist = ReadRegist(regist, pathregist);

        List<string> questions = new List<string>();
        questions = ReadQuestions(questions, pathquestion);
        List<List<string>> question = new List<List<string>>();
        question = ReadQuestion(questions);
        List<int> answer = new List<int>();
        answer = ReadAnswer(answer, pathanswer);
        List<Users> users = new List<Users>();
        users = ReadUsers(users, pathuser);

        int point = 1;
        bool show = true;

        List<string> menu = new List<string>() { " Welcome, log in how? \n", " admin", " user", " exit", "\r\n Press the Enter key to select... " };

        while (show)
        {
            SelectRole(point, menu);

            switch (Console.ReadKey(true).Key)
            {

                case ConsoleKey.UpArrow:
                    point--;
                    point = SelectRole(point, menu);
                    break;
                case ConsoleKey.DownArrow:
                    point++;
                    point = SelectRole(point, menu);
                    break;
                case ConsoleKey.Enter:
                    show = false;
                    if (point == 1)
                    {
                        show = Admin(ref regist, pathregist, questions, question, pathquestion, answer, pathanswer, users, pathuser);
                    }
                    if (point == 2)
                    {
                        show = User(question, answer, pathuser, users);
                    }
                    if (point == 3)
                    {
                        show = Exit();
                    }
                    break;
                default:
                    break;
            }
        }

    }

    static int SelectRole(int point, List<string> menu)
    {

        Console.Clear();

        if (point == menu.Count - 1)
        {
            point = 1;
        }
        if (point == 0)
        {
            point += menu.Count - 2;
        }

        for (int i = 0; i < menu.Count; i++)
        {
            if (i == point && i != menu.Count - 1)
            {
                Console.ForegroundColor = ConsoleColor.Red;
            }
            Console.Write(menu[i]);

            Console.ResetColor();

            if (i != menu.Count - 1)
            {
                Console.WriteLine();
            }
        }

        return point;
    }

    static bool Exit()
    {

        int point = 1;
        bool show = true;

        List<string> menu = new List<string>() { "Would you like to get out? \n", " yes", " no", "\r\n Press the Enter key to select... " };
        while (show)
        {
            SelectRole(point, menu);

            switch (Console.ReadKey(true).Key)
            {
                case ConsoleKey.UpArrow:
                    point--;
                    point = SelectRole(point, menu);
                    break;
                case ConsoleKey.DownArrow:
                    point++;
                    point = SelectRole(point, menu);
                    break;
                case ConsoleKey.Enter:
                    show = false;
                    if (point == 1)
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                default:
                    break;
            }
        }

        return true;
    }

    struct Users
    {
        public string name;
        public double result;
    }

    static List<Users> ReadUsers(List<Users> users, string pathn)
    {

        users.Clear();

        Users temp = new Users();
        List<string> names = new List<string>();
        List<double> result = new List<double>();

        string pathtemp = pathn;
        string[] allfolders = Directory.GetDirectories(pathn);

        foreach (string folder in allfolders)
        {
            string s = folder;
            s = s.Substring(folder.LastIndexOf(@"\") + 1);
            names.Add(s);

            pathtemp = pathn + @$"\{s}\Результат.txt";
            using (StreamReader sr = new StreamReader(pathtemp))
            {
                result.Add(double.Parse(sr.ReadLine().Replace("Result: ", "")));
            }

        }

        for (int i = 0; i < allfolders.Length; i++)
        {
            temp.name = names[i];
            temp.result = result[i];
            users.Add(temp);
        }

        return users;
    }


    static bool Admin(ref Regist regist, string path, List<string> questions, List<List<string>> question, string pathq, List<int> answer, string patha, List<Users> users, string pathn)
    {

        int point = 1;
        bool show = true;

        List<string> menu = new List<string>() { "Choose what you want to do... \n", " view the test report", " view the list of questions", " edit the list of questions with answers", " edit login details", " log out of your account", "\r\n Press the Enter key to select... " };
        if (Check(regist))
        {
            while (show)
            {
                SelectRole(point, menu);

                switch (Console.ReadKey(true).Key)
                {
                    case ConsoleKey.UpArrow:
                        point--;
                        point = SelectRole(point, menu);
                        break;
                    case ConsoleKey.DownArrow:
                        point++;
                        point = SelectRole(point, menu);
                        break;
                    case ConsoleKey.Enter:
                        show = false;
                        if (point == 1)
                        {
                            show = ViewTestReport(users, pathn);
                        }
                        if (point == 2)
                        {
                            show = ViewListQuestion(questions, question);
                        }
                        if (point == 3)
                        {
                            show = EditListQuestion(questions, question, pathq, answer, patha);
                        }
                        if (point == 4)
                        {
                            show = EditRegist(ref regist, path);
                        }
                        if (point == 5)
                        {
                            show = LogOut();
                        }
                        break;
                    default:
                        break;
                }
            }
        }

        return true;
    }

    static bool Check(Regist regist)
    {

        Regist temp = ReadCheck(regist);
        if (temp.login == regist.login && temp.password == regist.password)
        {
            return true;
        }
        else
        {
            if (CheckError())
            {
                return Check(regist);
            }
            else
            {
                return false;
            }
        }

    }

    static Regist ReadCheck(Regist temp)
    {

        int point = 1;

        List<string> menu = new List<string>() { "Enter your username and password... \n", " Enter your username: ", "\n Enter the password: ", "\r\n Press enter to continue... " };
        SelectRole(point, menu);
        Regist regist = new Regist();

        Console.ForegroundColor = ConsoleColor.Red;
        Console.SetCursorPosition(22, 2);

        regist.login = Console.ReadLine();

        menu[1] += regist.login;
        Console.ResetColor();

        point++;
        SelectRole(point, menu);

        Console.ForegroundColor = ConsoleColor.Red;
        Console.SetCursorPosition(21, 4);

        regist.password = Console.ReadLine();

        Console.ResetColor();

        return regist;
    }

    struct Regist
    {
        public string login;
        public string password;
    }

    static Regist ReadRegist(Regist regist, string path)
    {
        using (StreamReader sr = new StreamReader(path))
        {
            while (!sr.EndOfStream)
            {
                regist.login = sr.ReadLine();
                regist.password = sr.ReadLine();
            }
        }

        return regist;
    }

    static List<string> ReadQuestions(List<string> questions, string pathq)
    {
        questions.Clear();
        using (StreamReader sr = new StreamReader(pathq))
        {
            while (!sr.EndOfStream)
            {
                questions.Add(sr.ReadLine());
            }
        }

        return questions;
    }

    static List<List<string>> ReadQuestion(List<string> questions)
    {

        List<List<string>> question = new List<List<string>>();
        List<string> temp = new List<string>();

        int i = 0;
        question.Add(new List<string>());

        foreach (string s in questions)
        {
            if (s != "")
            {
                question[i].Add(s);
            }
            else
            {
                i++;
                question.Add(new List<string>());
            }
        }

        return question;
    }

    static List<int> ReadAnswer(List<int> answer, string patha)
    {

        answer.Clear();

        using (StreamReader sr = new StreamReader(patha))
        {
            while (!sr.EndOfStream)
            {
                string s = sr.ReadLine();
                s = s.Replace(s.Substring(0, s.IndexOf('.') + 2), "");
                answer.Add(int.Parse(s));
            }

        }

        return answer;
    }

    static void WriteQuestions(List<string> questions)
    {
        foreach (string s in questions)
        {
            Console.WriteLine(s);
        }
    }

    static int WriteQuestion(int point, List<List<string>> question)
    {
        Console.Clear();

        Console.WriteLine("Viewing questions... \n");
        if (point == question.Count)
        {
            point = 0;
        }
        if (point == -1)
        {
            point = question.Count - 1;
        }

        foreach (string s in question[point])
        {
            Console.WriteLine(s);
        }
        Console.WriteLine("\r\n Press the Enter key to select... ");

        return point;
    }

    static bool CheckError()
    {
        int point = 1;
        bool show = true;

        List<string> menu = new List<string>() { "Data entered incorrectly. Would you like to try again? \n", " yes", " no", "\r\n Press the Enter key to select... " };

        while (show)
        {
            SelectRole(point, menu);

            switch (Console.ReadKey(true).Key)
            {
                case ConsoleKey.UpArrow:
                    point--;
                    point = SelectRole(point, menu);
                    break;
                case ConsoleKey.DownArrow:
                    point++;
                    point = SelectRole(point, menu);
                    break;
                case ConsoleKey.Enter:
                    show = false;
                    if (point == 1)
                    {
                        return true;
                    }
                    if (point == 2)
                    {
                        return false;
                    }
                    break;
                default:
                    break;
            }
        }

        return true;
    }

    static bool ViewTestReport(List<Users> users, string pathn)
    {

        int point = 1;
        bool show = true;

        List<string> menu = new List<string>() { "View the test results... \n", " in ascending order", " in descending order", " alphabetically", " back", "\r\n Press the Enter key to select... " };

        while (show)
        {
            SelectRole(point, menu);

            switch (Console.ReadKey(true).Key)
            {
                case ConsoleKey.UpArrow:
                    point--;
                    point = SelectRole(point, menu);
                    break;
                case ConsoleKey.DownArrow:
                    point++;
                    point = SelectRole(point, menu);
                    break;
                case ConsoleKey.Enter:
                    show = false;
                    if (point == 1)
                    {
                        show = ViewTopUser(users);
                    }
                    if (point == 2)
                    {
                        show = ViewTopUserDescending(users);
                    }
                    if (point == 3)
                    {
                        show = ViewTopUserAlphabetically(users, pathn);
                    }
                    break;
                default:
                    break;
            }
        }

        return true;
    }

    static bool ViewTopUserDescending(List<Users> users)
    {

        Console.Clear();
        Console.WriteLine("Test results...\n");

        SwapDescending(users);

        for (int i = 0; i < users.Count; i++)
        {
            Console.Write($"{users[i].result,(5):f2}% - ");
            Console.WriteLine(users[i].name);
        }

        Console.WriteLine("\r\n Press the Enter key to select... ");

        bool show = true;
        while (show)
        {

            switch (Console.ReadKey(true).Key)
            {
                case ConsoleKey.Enter:
                    show = false;
                    break;
                default:
                    break;
            }
        }

        return true;
    }

    static void SwapDescending(List<Users> users)
    {
        for (int i = 0; i < users.Count; i++)
        {
            for (int j = i + 1; j < users.Count; j++)
            {

                if (users[i].result > users[j].result)
                {
                    Users temp = new Users();
                    temp = users[i];
                    users[i] = users[j];
                    users[j] = temp;
                }

            }
        }
    }

    static bool ViewTopUserAlphabetically(List<Users> users, string pathn)
    {

        Console.Clear();
        Console.WriteLine("Test results...\n");

        users = ReadUsers(users, pathn);

        for (int i = 0; i < users.Count; i++)
        {
            Console.Write($"{users[i].result,(5):f2}% - ");
            Console.WriteLine(users[i].name);
        }

        Console.WriteLine("\r\n Press the Enter key to select... ");

        bool show = true;
        while (show)
        {

            switch (Console.ReadKey(true).Key)
            {
                case ConsoleKey.Enter:
                    show = false;
                    break;
                default:
                    break;
            }
        }

        return true;
    }

    static bool EditListQuestion(List<string> questions, List<List<string>> question, string pathq, List<int> answer, string patha)
    {

        int point = 1;
        bool show = true;

        List<string> menu = new List<string>() { "Choose what you want to do... \n", " edit a question", " add a question", " delete a question", " back", "\r\n Press the Enter key to select... " };

        while (show)
        {
            SelectRole(point, menu);

            switch (Console.ReadKey(true).Key)
            {
                case ConsoleKey.UpArrow:
                    point--;
                    point = SelectRole(point, menu);
                    break;
                case ConsoleKey.DownArrow:
                    point++;
                    point = SelectRole(point, menu);
                    break;
                case ConsoleKey.Enter:
                    show = false;
                    if (point == 1)
                    {
                        show = EditQuestion(question, questions, pathq, answer, patha);
                    }
                    if (point == 2)
                    {
                        show = AddQuestion(question, questions, pathq, answer, patha);
                    }
                    if (point == 3)
                    {
                        show = DeleteQuestion(questions, question, pathq, answer, patha);
                    }
                    if (point == 4)
                    {
                        return true;
                    }
                    break;
                default:
                    break;
            }
        }

        return true;
    }






    static bool EditQuestion(List<List<string>> question, List<string> questions, string pathq, List<int> answer, string patha)
    {

        int point = 1;
        bool show = true;

        List<string> menu = new List<string>() { "Choose what you want to do... \n", " select a question to change by number", "  select a question to change by selecting from the list", " back", "\r\n Press the Enter key to select... " };

        while (show)
        {
            SelectRole(point, menu);

            switch (Console.ReadKey(true).Key)
            {
                case ConsoleKey.UpArrow:
                    point--;
                    point = SelectRole(point, menu);
                    break;
                case ConsoleKey.DownArrow:
                    point++;
                    point = SelectRole(point, menu);
                    break;
                case ConsoleKey.Enter:
                    show = false;
                    if (point == 1)
                    {
                        show = EditByNumber(question, questions, pathq, answer, patha);
                    }
                    if (point == 2)
                    {
                        show = EditBySelecting(question, questions, pathq, answer, patha);
                    }
                    if (point == 3)
                    {
                        return true;
                    }
                    break;
                default:
                    break;
            }
        }

        return true;
    }


    static bool EditByNumber(List<List<string>> question, List<string> questions, string pathq, List<int> answer, string patha)
    {

        List<string> menu = new List<string>() { "Would you like to change this question? \n", " yes", " no", "\r\n Press the Enter key to select... " };

        int number = EnterQuestion(question);
        if (number != -1)
        {
            int point = 1;
            bool show = true;

            while (show)
            {
                SelectRole(point, menu, number, question);

                switch (Console.ReadKey(true).Key)
                {
                    case ConsoleKey.UpArrow:
                        point--;
                        point = SelectRole(point, menu, number, question);
                        break;
                    case ConsoleKey.DownArrow:
                        point++;
                        point = SelectRole(point, menu, number, question);
                        break;
                    case ConsoleKey.Enter:
                        show = false;
                        if (point == 1)
                        {
                            show = EditQuestion(number, question, questions, pathq, answer, patha);
                        }
                        return true;
                    default:
                        break;
                }
            }
        }

        return true;
    }


    static bool EditBySelecting(List<List<string>> question, List<string> questions, string pathq, List<int> answer, string patha)
    {

        Console.Clear();

        int point = 0;
        bool show = true;

        while (show)
        {
            WriteQuestion(point, question);

            switch (Console.ReadKey(true).Key)
            {
                case ConsoleKey.LeftArrow:
                    point--;
                    point = WriteQuestion(point, question);
                    break;
                case ConsoleKey.RightArrow:
                    point++;
                    point = WriteQuestion(point, question);
                    break;
                case ConsoleKey.Enter:
                    show = false;
                    show = EditQuestion(point, question, questions, pathq, answer, patha);
                    return true;
                default:
                    break;
            }
        }

        return true;
    }

    static bool EditQuestion(int number, List<List<string>> question, List<string> questions, string pathq, List<int> answer, string patha)
    {

        int point = 1;
        bool show = true;

        List<string> menu = new List<string>() { "What do you want to change? \n", " change a question", " change the answer to the question", " change the correct answer option", " back", "\r\n Press the Enter key to select... " };

        while (show)
        {
            SelectRole(point, menu);

            switch (Console.ReadKey(true).Key)
            {
                case ConsoleKey.UpArrow:
                    point--;
                    point = SelectRole(point, menu);
                    break;
                case ConsoleKey.DownArrow:
                    point++;
                    point = SelectRole(point, menu);
                    break;
                case ConsoleKey.Enter:
                    show = false;
                    if (point == 1)
                    {
                        show = ChangeQuestion(question[number], number + 1);
                    }
                    if (point == 2)
                    {
                        show = ChangeAnswerQuestion(question[number]);
                    }
                    if (point == 3)
                    {
                        show = ChangeCorrectAnswer(answer, number, question[number]);
                    }
                    if (point == 4)
                    {
                        return true;
                    }
                    WriteListQuestions(question, pathq);
                    questions = ReadQuestions(questions, pathq);
                    WriteAnswer(answer, patha);
                    break;
                default:
                    break;
            }
        }

        return true;
    }

    static bool ChangeQuestion(List<string> quest, int number)
    {

        Console.Clear();

        List<string> temp = new List<string>();

        bool search = true;
        int ind = -1;

        while (search)
        {
            ind++;
            if (quest[ind].StartsWith("1)") == false)
            {
                search = true;
            }
            else
            {
                search = false;
            }


        }

        Console.WriteLine(" Old question: \n");
        for (int i = 0; i < ind; i++)
        {
            if (i == 0)
            {
                Console.WriteLine(" " + quest[i].Replace(quest[i].Substring(0, quest[i].IndexOf('.') + 2), ""));
            }
            else
            {
                Console.WriteLine(" " + quest[i]);
            }

        }

        Console.Write("\n Enter a new question: ");

        string s = Console.ReadLine();

        s = s.Replace(" ", "");
        string[] lines = s.Split('.');

        for (int i = 0; i < lines.Length; i++)
        {
            if (i != lines.Length - 1)
            {
                temp.Add(lines[i] + '.');
            }
            else
            {
                temp.Add(lines[i]);
            }
        }

        temp[0] = $"{number}. " + temp[0];

        for (int i = ind; i < quest.Count; i++)
        {
            temp.Add(quest[i]);
        }

        List<string> menu = new List<string>() { "Are you sure you want to change? \n", " yes", " no", "\r\n Press the Enter key to select... " };
        if (Warning(menu))
        {
            quest.Clear();
            quest.AddRange(temp);
        }

        return true;
    }

    static bool ChangeAnswerQuestion(List<string> quest)
    {
        Console.Clear();

        bool search = true;
        int ind = -1;

        while (search)
        {
            ind++;
            if (quest[ind].StartsWith("1)") == false)
            {
                search = true;
            }
            else
            {
                search = false;
            }
        }

        Console.WriteLine(" Your question: \n");
        for (int i = 0; i < quest.Count; i++)
        {
            if (i == 0)
            {
                Console.WriteLine(" " + quest[i].Replace(quest[i].Substring(0, quest[i].IndexOf('.') + 2), ""));
            }
            else
            {
                Console.WriteLine(" " + quest[i]);
            }

        }

        Console.Write("\nEnter the question number you want to change: ");
        int count = quest.Count - ind;
        int answ;
        while (!int.TryParse(Console.ReadLine(), out answ) || (answ < 1 || answ > count))
        {
            Console.Clear();

            Console.WriteLine(" Your question: \n");

            foreach (string s in quest)
            {
                Console.WriteLine(s);
            }

            Console.Write("\n Error! Enter the question number you want to change: ");
        }

        Console.Write("\n Enter a new version of the question: ");
        string str = Console.ReadLine();

        List<string> menu = new List<string>() { "Are you sure you want to change? \n", " yes", " no", "\r\n Press the Enter key to select... " };
        if (Warning(menu))
        {
            quest[ind - 1 + answ] = $"{answ}) " + str;
        }

        return true;
    }

    static bool ChangeCorrectAnswer(List<int> answer, int number, List<string> question)
    {

        Console.Clear();

        bool search = true;
        int ind = -1;

        while (search)
        {
            ind++;
            if (question[ind].StartsWith("1)") == false)
            {
                search = true;
            }
            else
            {
                search = false;
            }


        }

        int count = question.Count - ind;
        Console.WriteLine(" A variant of the old answer to the question: " + answer[number] + "\n");

        Console.Write("Enter a variant of the new answer to the question: ");
        int answ;
        while (!int.TryParse(Console.ReadLine(), out answ) || (answ < 1 || answ > count))
        {
            Console.Clear();

            Console.WriteLine(" A variant of the old answer to the question: " + answer + "\n");

            Console.Write("Enter a variant of the new answer to the question: ");
            Console.SetCursorPosition(32, 2);
        }

        List<string> menu = new List<string>() { "Are you sure you want to change? \n", " yes", " no", "\r\n Press the Enter key to select... " };
        if (Warning(menu))
        {
            answer[number] = answ;
        }

        return true;
    }

    static bool AddQuestion(List<List<string>> question, List<string> questions, string pathq, List<int> answer, string patha)
    {

        int point = 1;
        bool show = true;

        List<string> temp = AddQuestionRead(question.Count + 1);

        List<string> menu = new List<string>() { "Are you sure you want to add a question? \n", " yes", " no", "\r\n Press the Enter key to select... " };

        while (show)
        {
            SelectRole(point, menu);

            switch (Console.ReadKey(true).Key)
            {

                case ConsoleKey.UpArrow:
                    point--;
                    point = SelectRole(point, menu);
                    break;
                case ConsoleKey.DownArrow:
                    point++;
                    point = SelectRole(point, menu);
                    break;
                case ConsoleKey.Enter:
                    show = false;
                    if (point == 1)
                    {
                        AddQuestionInList(question, temp, pathq, answer, patha);
                        questions = ReadQuestions(questions, pathq);
                    }
                    return true;
                default:
                    break;
            }
        }

        return true;
    }

    static List<string> AddQuestionRead(int number)
    {

        Console.Clear();

        List<string> temp = new List<string>();

        Console.WriteLine("Enter the required data... \n");
        Console.WriteLine(" enter a question: \n");
        Console.WriteLine(" enter the number of responses: ");

        Console.SetCursorPosition(19, 2);

        string s = Console.ReadLine();
        string stemp = s;

        s = s.Replace(" ", "");
        string[] lines = s.Split('.');

        for (int i = 0; i < lines.Length; i++)
        {
            if (i != lines.Length - 1)
            {
                temp.Add(lines[i] + '.');
            }
            else
            {
                temp.Add(lines[i]);
            }
        }

        int n;
        Console.SetCursorPosition(32, 4);
        while (!int.TryParse(Console.ReadLine(), out n) || (n < 1 || n > 5))
        {
            Console.Clear();

            Console.Write("Incorrect data entered! ");
            Console.WriteLine("Enter the required data... \n");
            Console.WriteLine(" enter a question: " + stemp + "\n");
            Console.WriteLine(" enter the number of responses: ");

            Console.SetCursorPosition(32, 4);
        }

        int pos = 4;
        for (int i = 1; i <= n; i++)
        {
            pos += 2;
            Console.WriteLine($"\n enter {i} answer option: \n");
            Console.SetCursorPosition(25, pos);
            temp.Add($"{i}) " + Console.ReadLine());
        }
        temp[0] = temp[0].Insert(0, $"{number}. ");

        pos += 2;
        Console.WriteLine($"\n enter true answer option: \n");
        Console.SetCursorPosition(27, pos);

        int tr;
        while (!int.TryParse(Console.ReadLine(), out tr) || (tr < 1 || tr > n))
        {
            Console.Write("\nIncorrect data entered! ");
            Console.Write($" Enter true answer option: \n");
            pos += 2;
            Console.SetCursorPosition(51, pos);
        }
        temp.Add(Convert.ToString(tr));

        return temp;
    }

    static void AddQuestionInList(List<List<string>> question, List<string> temp, string pathq, List<int> answer, string patha)
    {

        answer.Add((int.Parse(temp[temp.Count - 1])));
        WriteAnswer(answer, patha);

        temp.RemoveAt(temp.Count - 1);

        question.Add(temp);
        WriteListQuestions(question, pathq);


    }

    static void WriteAnswer(List<int> answer, string patha)
    {
        using (StreamWriter sw = new StreamWriter(patha))
        {
            for (int i = 1; i <= answer.Count; i++)
            {
                sw.WriteLine($"{i}. " + answer[i - 1]);
            }
        }
    }

    static bool DeleteQuestion(List<string> questions, List<List<string>> question, string pathq, List<int> answer, string patha)
    {

        int point = 1;
        bool show = true;

        List<string> menu = new List<string>() { "Choose what you want to do... \n", " delete by question number", " delete by selecting from the list", " back", "\r\n Press the Enter key to select... " };

        while (show)
        {
            SelectRole(point, menu);

            switch (Console.ReadKey(true).Key)
            {

                case ConsoleKey.UpArrow:
                    point--;
                    point = SelectRole(point, menu);
                    break;
                case ConsoleKey.DownArrow:
                    point++;
                    point = SelectRole(point, menu);
                    break;
                case ConsoleKey.Enter:
                    show = false;
                    if (point == 1)
                    {
                        show = DeleteByNumber(question, pathq, answer, patha);
                    }
                    if (point == 2)
                    {
                        show = DeleteBySelecting(question, pathq, answer, patha);
                    }
                    if (point == 3)
                    {
                        return true;
                    }
                    questions = ReadQuestions(questions, pathq);
                    break;
                default:
                    break;
            }
        }

        return true;
    }


    static bool DeleteByNumber(List<List<string>> question, string pathq, List<int> answer, string patha)
    {

        int point = 1;
        bool show = true;

        List<string> menu = new List<string>() { "Would you like to delete a question? \n", " yes", " no", "\r\n Press the Enter key to select... " };

        int number = EnterQuestion(question);
        if (number == -1)
        {
            show = false;
            return true;
        }

        while (show)
        {

            SelectRole(point, menu, number, question);

            switch (Console.ReadKey(true).Key)
            {

                case ConsoleKey.UpArrow:
                    point--;
                    point = SelectRole(point, menu, number, question);
                    break;
                case ConsoleKey.DownArrow:
                    point++;
                    point = SelectRole(point, menu, number, question);
                    break;
                case ConsoleKey.Enter:
                    show = false;
                    if (point == 1)
                    {
                        DeleteQuestion(number, ref question, pathq, answer, patha);
                        return true;
                    }
                    if (point == 2)
                    {
                        return true;
                    }
                    break;
                default:
                    break;
            }
        }

        return true;
    }

    static int SelectRole(int point, List<string> menu, int number, List<List<string>> question)
    {

        Console.Clear();

        if (point == menu.Count - 1)
        {
            point = 1;
        }

        if (point == 0)
        {
            point += menu.Count - 2;
        }

        Console.WriteLine(point);

        for (int i = 0; i < menu.Count; i++)
        {
            if (i == 1)
            {
                WriteQuestions(question[number]);
                Console.WriteLine();
            }

            if (i == point && i != menu.Count - 1)
            {
                Console.ForegroundColor = ConsoleColor.Red;
            }
            Console.Write(menu[i]);

            Console.ResetColor();
            Console.WriteLine();
        }

        return point;
    }

    static int EnterQuestion(List<List<string>> question)
    {
        Console.Clear();

        Console.Write("\nEnter the question number: ");
        int number;
        while (!int.TryParse(Console.ReadLine(), out number) || (number > question.Count || number <= 0))
        {
            if (number > question.Count || number <= 0)
            {
                if (!CheckError())
                {
                    return -1;
                }
                else
                {
                    return EnterQuestion(question) - 1;
                }
            }

        }

        return number - 1;
    }

    static void DeleteQuestion(int number, ref List<List<string>> question, string pathq, List<int> answer, string patha)
    {

        List<List<string>> temp = new List<List<string>>();
        List<int> tempint = new List<int>();

        int size = -1;

        for (int i = 0; i < question.Count - 1; i++)
        {
            temp.Add(new List<string>());

            size++;
            if (i == number)
            {
                size++;
            }

            foreach (string s in question[size])
            {
                temp[i].Add(s);
            }

            tempint.Add(answer[size]);

            temp[i][0] = temp[i][0].Replace(temp[i][0].Substring(0, temp[i][0].IndexOf('.')), $"{i + 1}");
        }

        WriteListQuestions(temp, pathq);
        WriteAnswer(tempint, patha);

        question.Clear();
        question.AddRange(temp);

        answer.Clear();
        answer.AddRange(tempint);
    }

    static void WriteListQuestions(List<List<string>> question, string pathq)
    {

        using (StreamWriter sw = new StreamWriter(pathq))
        {
            for (int i = 0; i < question.Count; i++)
            {
                foreach (string s in question[i])
                {
                    sw.WriteLine(s);
                }
                if (i != question.Count - 1)
                {
                    sw.WriteLine();
                }
            }
        }

    }

    static bool DeleteBySelecting(List<List<string>> question, string pathq, List<int> answer, string patha)
    {

        Console.Clear();

        int point = 0;
        bool show = true;

        while (show)
        {

            WriteQuestion(point, question);

            switch (Console.ReadKey(true).Key)
            {
                case ConsoleKey.LeftArrow:
                    point--;
                    point = WriteQuestion(point, question);
                    break;
                case ConsoleKey.RightArrow:
                    point++;
                    point = WriteQuestion(point, question);
                    break;
                case ConsoleKey.Enter:
                    show = false;
                    List<string> menu = new List<string>() { "Would you like to delete a question? \n", " yes", " no", "\r\n Press the Enter key to select... " };
                    if (Warning(menu))
                    {
                        DeleteQuestion(point, ref question, pathq, answer, patha);
                    }
                    return true;
                default:
                    break;
            }
        }

        return true;

    }

    static bool Warning(List<string> menu)
    {

        int point = 1;
        bool show = true;

        while (show)
        {
            SelectRole(point, menu);

            switch (Console.ReadKey(true).Key)
            {

                case ConsoleKey.UpArrow:
                    point--;
                    point = SelectRole(point, menu);
                    break;
                case ConsoleKey.DownArrow:
                    point++;
                    point = SelectRole(point, menu);
                    break;
                case ConsoleKey.Enter:
                    show = false;
                    if (point == 1)
                    {
                        return true;
                    }
                    if (point == 2)
                    {
                        return false;
                    }
                    break;
                default:
                    break;
            }
        }

        return true;
    }

    static bool ViewListQuestion(List<string> questions, List<List<string>> question)
    {

        int point = 1;
        bool show = true;

        List<string> menu = new List<string>() { "Select a viewing option... \n", " the whole list of questions", " list of questions in turn", " back", "\r\n Press the Enter key to select... " };

        while (show)
        {
            SelectRole(point, menu);

            switch (Console.ReadKey(true).Key)
            {

                case ConsoleKey.UpArrow:
                    point--;
                    point = SelectRole(point, menu);
                    break;
                case ConsoleKey.DownArrow:
                    point++;
                    point = SelectRole(point, menu);
                    break;
                case ConsoleKey.Enter:
                    show = false;
                    if (point == 1)
                    {
                        show = WholeList(questions);
                    }
                    if (point == 2)
                    {
                        show = InTurnTheList(question);
                    }
                    if (point == 3)
                    {
                        return true;
                    }
                    break;
                default:
                    break;
            }
        }

        return true;
    }

    static bool WholeList(List<string> questions)
    {

        Console.Clear();

        bool show = true;

        Console.WriteLine("Viewing questions... \n");
        WriteQuestions(questions);
        Console.WriteLine("\r\n Press the Enter key to select... ");

        while (show)
        {

            switch (Console.ReadKey(true).Key)
            {
                case ConsoleKey.Enter:
                    show = false;
                    return true;
                default:
                    break;
            }
        }

        return true;
    }

    static bool InTurnTheList(List<List<string>> question)
    {

        Console.Clear();

        int point = 0;
        bool show = true;

        while (show)
        {

            WriteQuestion(point, question);

            switch (Console.ReadKey(true).Key)
            {
                case ConsoleKey.LeftArrow:
                    point--;
                    point = WriteQuestion(point, question);
                    break;
                case ConsoleKey.RightArrow:
                    point++;
                    point = WriteQuestion(point, question);
                    break;
                case ConsoleKey.Enter:
                    show = false;
                    return true;
                default:
                    break;
            }
        }

        return true;
    }

    static bool EditRegist(ref Regist regist, string path)
    {
        int point = 1;
        bool show = true;

        List<string> menu = new List<string>() { "Select the item you want to change... \n", " change login", " change password", " back", "\r\n Press the Enter key to select... " };

        while (show)
        {
            SelectRole(point, menu);

            switch (Console.ReadKey(true).Key)
            {

                case ConsoleKey.UpArrow:
                    point--;
                    point = SelectRole(point, menu);
                    break;
                case ConsoleKey.DownArrow:
                    point++;
                    point = SelectRole(point, menu);
                    break;
                case ConsoleKey.Enter:
                    show = false;
                    if (point == 3)
                    {
                        return true;
                    }
                    else
                    {
                        EditRegist(point, ref regist, path);
                        show = true;
                    }
                    break;
                default:
                    break;
            }
        }

        return true;
    }

    static void EditRegist(int point, ref Regist regist, string path)
    {

        Console.Clear();

        Console.WriteLine("Enter new data...\n");

        List<string> option = new List<string>() { " new login: ", " new password: " };

        Console.WriteLine(option[point - 1]);
        Console.WriteLine("\r\n Press the Enter key to select... ");

        Console.SetCursorPosition(option[point - 1].Length, 2);
        if (point == 1)
        {
            regist.login = Console.ReadLine();
        }
        else
        {
            regist.password = Console.ReadLine();
        }

        List<string> menu = new List<string>() { "Would you like to change your login details? \n", " yes", " no", "\r\n Press the Enter key to select... " };
        if (Warning(menu))
        {
            using (StreamWriter sw = new StreamWriter(path))
            {
                sw.WriteLine(regist.login);
                sw.WriteLine(regist.password);
            }
        }

    }

    static bool LogOut()
    {

        int point = 1;
        bool show = true;

        List<string> menu = new List<string>() { "Are you sure you want to log out of your account? \n", " yes", " no", "\r\n Press the Enter key to select... " };

        while (show)
        {
            SelectRole(point, menu);

            switch (Console.ReadKey(true).Key)
            {

                case ConsoleKey.UpArrow:
                    point--;
                    point = SelectRole(point, menu);
                    break;
                case ConsoleKey.DownArrow:
                    point++;
                    point = SelectRole(point, menu);
                    break;
                case ConsoleKey.Enter:
                    show = false;
                    if (point == 1)
                    {
                        return false;
                    }
                    if (point == 2)
                    {
                        return true;
                    }
                    break;
                default:
                    break;
            }
        }

        return true;
    }


    static bool User(List<List<string>> question, List<int> answer, string pathn, List<Users> users)
    {

        int point = 1;
        bool show = true;

        List<string> menu = new List<string>() { "What do you want to do? \n", " take the test", " view top users", " logout", "\r\n Press the Enter key to select... " };
        List<int> ansuser = new List<int>();

        while (show)
        {
            SelectRole(point, menu);

            switch (Console.ReadKey(true).Key)
            {

                case ConsoleKey.UpArrow:
                    point--;
                    point = SelectRole(point, menu);
                    break;
                case ConsoleKey.DownArrow:
                    point++;
                    point = SelectRole(point, menu);
                    break;
                case ConsoleKey.Enter:
                    show = false;
                    if (point == 1)
                    {
                        show = TakeTest(pathn, question, answer, ansuser, users);
                    }
                    if (point == 2)
                    {
                        show = ViewTopUser(users);
                    }
                    if (point == 3)
                    {
                        show = LogOut();
                    }
                    break;
                default:
                    break;
            }
        }

        return true;
    }

    static bool ViewTopUser(List<Users> users)
    {

        Console.Clear();
        Console.WriteLine("Test results...\n");

        Swap(users);

        for (int i = 0; i < users.Count; i++)
        {
            Console.Write($"{users[i].result,(5):f2}% - ");
            Console.WriteLine(users[i].name);
        }

        Console.WriteLine("\r\n Press the Enter key to select... ");

        bool show = true;
        while (show)
        {

            switch (Console.ReadKey(true).Key)
            {
                case ConsoleKey.Enter:
                    show = false;
                    break;
                default:
                    break;
            }
        }

        return true;
    }

    static bool ViewTopUser(List<Users> users, string names)
    {

        Console.Clear();
        Console.WriteLine("Test results...\n");

        Swap(users);

        for (int i = 0; i < users.Count; i++)
        {
            if (users[i].name == names)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write($"{users[i].result,(5):f2}% - ");
                Console.WriteLine(users[i].name);
                Console.ResetColor();
            }
            else
            {
                Console.Write($"{users[i].result,(5):f2}% - ");
                Console.WriteLine(users[i].name);
            }

        }

        Console.WriteLine("\r\n Press the Enter key to select... ");

        bool show = true;
        while (show)
        {

            switch (Console.ReadKey(true).Key)
            {
                case ConsoleKey.Enter:
                    show = false;
                    break;
                default:
                    break;
            }
        }

        return true;
    }

    static void Swap(List<Users> users)
    {
        for (int i = 0; i < users.Count; i++)
        {
            for (int j = i + 1; j < users.Count; j++)
            {

                if (users[i].result < users[j].result)
                {
                    Users temp = new Users();
                    temp = users[i];
                    users[i] = users[j];
                    users[j] = temp;
                }

            }
        }
    }

    static bool TakeTest(string pathn, List<List<string>> question, List<int> answer, List<int> ansuser, List<Users> users)
    {

        string pathu = pathn;
        if (EnterName(pathn, ref pathu))
        {

            int point = 1;
            bool show = true;

            ansuser = Test(question, pathu, ansuser);
            YourScore(answer, ansuser, pathu);
            ReadUsers(users, pathn);
            List<string> menu = new List<string>() { "Would you like to see your result among users? \n", " yes", " no", "\r\n Press the Enter key to select... " };

            while (show)
            {

                SelectRole(point, menu);

                switch (Console.ReadKey(true).Key)
                {

                    case ConsoleKey.UpArrow:
                        point--;
                        point = SelectRole(point, menu);
                        break;
                    case ConsoleKey.DownArrow:
                        point++;
                        point = SelectRole(point, menu);
                        break;
                    case ConsoleKey.Enter:
                        show = false;
                        if (point == 1)
                        {
                            string names = pathu.Replace(@"\Ответы.txt", "");
                            names = names.Substring(names.LastIndexOf(@"\") + 1);

                            return ViewTopUser(users, names);

                        }
                        return true;
                    default:
                        break;
                }
            }
        }


        return true;
    }

    static void YourScore(List<int> answer, List<int> ansuser, string pathu)
    {

        int score = 0;
        for (int i = 0; i < answer.Count; i++)
        {

            if (answer[i] == ansuser[i])
            {
                score++;
            }

        }

        double result = (score * 1.0 / answer.Count * 1.0) * 100;

        WriteYourResult(result);

        pathu = pathu.Replace("Ответы.txt", "Результат.txt");
        File.Create(pathu).Close();
        using (StreamWriter sw = new StreamWriter(pathu))
        {
            sw.WriteLine($"Result: {result:f2}");
        }

    }


    static void WriteYourResult(double result)
    {

        Console.Clear();

        Console.WriteLine("Your result was: " + $"{result:f2}%");

        if (result > 80)
        {
            Console.WriteLine("\nYou have a great outlook and skills, the profession of a programmer is definitely suitable for you");
        }
        else
        {
            if (result > 50)
            {
                Console.WriteLine("\nYou have a decent result, you should try harder to master this profession");
            }
            else
            {
                Console.WriteLine("\nYou should tighten up your knowledge and broaden your horizons in order to become a good programmer");
            }
        }

        Console.WriteLine("\r\n Press the Enter key to select... ");

        bool show = true;
        while (show)
        {

            switch (Console.ReadKey(true).Key)
            {
                case ConsoleKey.Enter:
                    show = false;
                    break;
                default:
                    break;
            }
        }

    }
    static List<int> Test(List<List<string>> question, string pathu, List<int> ansuser)
    {

        ansuser.Clear();

        for (int i = 0; i < question.Count; i++)
        {

            bool search = true;
            int ind = -1;

            while (search)
            {
                ind++;
                if (question[i][ind].StartsWith("1)") == false)
                {
                    search = true;
                }
                else
                {
                    search = false;
                }
            }

            int point = 1;
            bool show = true;

            while (show)
            {
                Test(point, question[i], ind);

                switch (Console.ReadKey(true).Key)
                {

                    case ConsoleKey.UpArrow:
                        point--;
                        point = Test(point, question[i], ind);
                        break;
                    case ConsoleKey.DownArrow:
                        point++;
                        point = Test(point, question[i], ind);
                        break;
                    case ConsoleKey.Enter:
                        show = false;
                        WriteAnswerUser(pathu, i + 1, point);
                        ansuser.Add(point);
                        break;
                    default:
                        break;
                }
            }
        }

        return ansuser;

    }

    static void WriteAnswerUser(string pathu, int numberanswer, int answer)
    {
        using (StreamWriter sw = new StreamWriter(pathu, true))
        {
            sw.WriteLine($"{numberanswer}. " + answer);
        }
    }

    static int Test(int point, List<string> question, int ind)
    {

        Console.Clear();

        if (point == question.Count - ind + 1)
        {
            point = 1;
        }

        if (point == 0)
        {
            point = question.Count - ind;
        }

        Console.WriteLine("Read the question carefully...\n");

        Console.WriteLine(point);

        for (int i = 0; i < ind; i++)
        {
            Console.WriteLine(question[i]);
        }

        for (int i = ind; i < question.Count; i++)
        {

            if (i == point + ind - 1)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(question[i]);
            }
            else
            {
                Console.WriteLine(question[i]);
            }
            Console.ResetColor();
        }

        Console.WriteLine("\r\n Press the Enter key to select... ");

        return point;
    }

    static bool EnterName(string pathn, ref string pathu)
    {

        Console.Clear();

        Console.Write(" Enter your nickname: ");
        string name = Console.ReadLine();

        if (FolderNames(name, pathn))
        {

            Directory.CreateDirectory($"{pathn}/{name}");
            pathu = pathn + @$"\{name}" + $@"\Ответы.txt";
            File.Create(pathu).Close();

        }
        else
        {
            List<string> menu = new List<string>() { "A user with that name already exists. Lead the name again?  \n", " yes", " no", "\r\n Press the Enter key to select... " };

            if (Warning(menu))
            {
                return EnterName(pathn, ref pathu);
            }
            else
            {
                return false;
            }
        }


        return true;
    }

    static bool FolderNames(string name, string pathn)
    {

        List<string> names = new List<string>();

        string[] allfolders = Directory.GetDirectories(pathn);

        foreach (string folder in allfolders)
        {
            string s = folder;

            s = s.Substring(folder.LastIndexOf(@"\") + 1);
            names.Add(s);
        }

        for (int i = 0; i < names.Count; i++)
        {
            if (names[i] == name)
            {

                return false;

            }
        }

        return true;
    }

}