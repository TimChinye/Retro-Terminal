using System.Collections.Generic;
using System;
using System.Reflection;
using System.Linq;
using System.Threading;
using System.Reflection.Emit;
using System.ComponentModel;

/*
    Things to note while marking this assignment:

    This code will run best in VS Code. Although it should work in other IDEs, all testing was done using VS code.
    This is because I developed it in VS Code. I did unit testing in VS, so I'm aware it also works fine there.

    In order to "show off", I decided to over-complicate things by making the whole computer monitor thing.
    This of course takes a long of space, and therefore I added a feature to allow the user to customize
    the size of the display. This included using Console.WindowWidth/Height to create a default display size.
    Errors will occur if the configured size is too small. For the Trinary Converter and ISBN verifier, it should
    run entirely fine, but for School Roster, errors may (will) occur on small displays. I could enforce a minimum
    display size that'd work for all programs, but I think it's more fun to have a ridiculously tiny display.

    However, some IDEs may have a Window Height/Width that isn't what's actually being shown, or they may run an
    older version of .NET/C# meaning certain features may not work. Such as the shorthand for Nullable `?`.
    So, code like "String?" or "??=" may not work if this code is being ran on an IDE that is not VSCode (the one
    used on the test device).

    I have a bad habit of not commenting my code properly, so if you'd like me to explain any of my code. Just
    ask me directly. Additionally, I usually avoid using `try..catch` because I believe using it over an if statement
    is bad programming. While I'm on the topic, in the code below I do some bad programming on purpose, solely to
    "show off". An example of this would be when I used goto with a label. I believe this is often frown upon for
    readability's sake, however each times I used it, I made sure to sue it in a way that makes sense. An example
    would be, try..catch, I used it to resolve the error from .Single() if there's no matches and a failed
    int.Parse() (even though TryParse() would've been the best option).

    In reference to the brief:

    Often there's an "ideal" or "conventional" way of doing this in programming.
    Quite a lot of the code below is done in a potentially unusual method for the sole reason of "showing off".

    This means I will be:
    - Purposefully overly complicating things.
    - Purposefully doing the same thing, using a different method.

    With that being said, the method chosen, although unconvential would still be considered "sensible and effective".

    When it comes to error handling, I have a concern, as in some parts of my code, instead of showing an error message.
    I just prevent the user from making the mistake in the first place. Is this fine? It should be. In my opinion it's
    better to idiot-proof than to show an error message, for user experience reasons.

    Final thing to note:

    After stress testing, I found that because the program needs to clear and re-write a lot of characters, if you make it
    do this a ton of times (like thousands of times), you'll notice a delay every time you make the program clear and write
    itself again. This can be reproduced easily but copying, say, a paragraph of text (like this one), and pasting it into
    the ISBN Verifier program about 10 times. After waiting a couple minutes for each key to be processed, by going back to
    the menu and navigating through the program selector. You'll notice, as you select different programs, that it takes a
    few hundred milliseconds longer than usual to clear and display the screen in order to select the target program.
*/

namespace Assignment
{
    public class Program
    {
        static void Main(string[] args)
        {
            Computer.PowerOn(true); // Builds frame for computer
            if (!Console.IsOutputRedirected) Computer.ConfigureDisplaySize(); // Allows user to customize the display for their console size.
            Computer.ProgramSelector(); // Allows user to choose and start a program.

            // For UX reasons I would add "ConfigureDisplaySize" to the program selector but the brief only mentioned
            // three items in the menu system.
        }

        public static class Computer
        {
            private static readonly string[] splashTexts = {
                "I may have tried too hard...",
                "Tim Chinye's PC",
                "/|\\ ATARI SM124",
                "I'm only going to write 5 of these.",
                "Nothing to see here, just a comSHUter.",
                "Designed & Developed by Tim",
                "Made with <3... kidding, I just want a good grade.",
                "I'm having too much fun with this.",
                "This going to take so long...",
            };
            private static string? splashText;
            private static string? screen;
            private static string? currentScreen;
            private static int displayWidth;
            private static int displayHeight;
            private static readonly int widthPadding = 5;
            private static readonly int heightPadding = 1;
            private static char indicator = 'O';
            private static void Build()
            {
                // Calculate the max size of computer while keeping a 4:3 ratio, if no specific size is set by user
                int width = 0, height = 0, maxWidth = Console.WindowWidth - 14, maxHeight = Console.WindowHeight - 14, perfectWidth, perfectHeight;

                perfectWidth = (int)((double)maxHeight / 41 * 136);
                perfectHeight = (int)((double)maxWidth / 136 * 41);

                // I'm basically grabbing the smallest side (width or height) and then check if the perfect (4:3)
                // "other side" would fit in the Console, if not check again but on the other way. An `else` is unnecessary here.
                if (perfectWidth <= maxWidth)
                {
                    width = perfectWidth;
                    height = maxHeight;
                }
                else if (perfectHeight <= maxHeight)
                {
                    width = maxWidth;
                    height = perfectHeight;
                }

                Build(width, height); // We've now specified a size, so actually build the thing.
            }

            private static void Build(int width = 68, int height = 21) // 136:41 is the ideal ratio (close to 4:3, which was the Atari SM124 display's aspect ratio)
            {
                int minWidth = widthPadding << 1; // `<< 1` shifts each bit by one, which is basically just doubling / "* 2".
                int minHeight = heightPadding << 1; // This is me going out my way to show off. I use "* 2" every other time.

                // The following is Recursion -> "Do not use recursion *to implement the menu system*" -> therefore, recursion here (building Computer screen) should be fine.
                if (width <= minWidth) width = minWidth + 1;
                if (height <= minHeight) height = minHeight + 1;
                if (width <= minWidth || height <= minHeight) Build(width, height);

                displayWidth = width;
                displayHeight = height;

                splashText ??= splashTexts[new Random().Next(0, splashTexts.Length - 1)]; // Takes random splash text, if one isn't already set.
                splashText = splashText.PadRight(displayWidth, ' ');

                // Build screen using the porivded width (height is used in the actually display).
                // Also I show the dimens so user can get response feedback when doing "ConfigureDisplaySize()".
                screen = $@"{$"{displayWidth} x {displayHeight}".PadRight(displayWidth + 10)}
  _____{string.Concat(Enumerable.Repeat("_", displayWidth))}_____  
 /     {string.Concat(Enumerable.Repeat(" ", displayWidth))}     \ 
|      {string.Concat(Enumerable.Repeat(" ", displayWidth))}      |
|      {string.Concat(Enumerable.Repeat("_", displayWidth))}      |
|     /{string.Concat(Enumerable.Repeat(" ", displayWidth))}\     |
Display:
|     \{string.Concat(Enumerable.Repeat("_", displayWidth))}/     |
|      {string.Concat(Enumerable.Repeat(" ", displayWidth))}      |
|     {splashText.Substring(0, displayWidth)} {indicator}     |
 \_____{string.Concat(Enumerable.Repeat("_", displayWidth))}_____/ 
   !___{string.Concat(Enumerable.Repeat("_", displayWidth))}___!   

{"Press CTRL+C to Force Stop".PadSides(displayWidth + 14)}"; // Custom Method to equally pad left and right (PadSides).

                string display = String.Join("\n", Enumerable.Repeat("", displayHeight).ToList().Select((item) => $"|     |{string.Concat(Enumerable.Repeat(" ", displayWidth))}|     |"));
                currentScreen = screen.Replace("\nDisplay:", $"{(displayHeight == 0 ? "" : $"\n{display}")}");

            }

            // Overloading
            private static void ClearScreen() => ClearScreen(0);
            private static void ClearScreen(int yPos = 0)
            {
                DisplayText(String.Join("\n", Enumerable.Repeat(String.Concat(Enumerable.Repeat(' ', displayWidth - (widthPadding * 2))), displayHeight - (heightPadding * 2))), yPos);
            }

            // And again but more
            private static void DisplayText(string _string) => DisplayText(_string, 0);
            private static void DisplayText(string _string, int yPos = 0) => DisplayText(_string, 0, yPos);
            private static void DisplayText(string _string, int xPos = 0, int yPos = 0) => DisplayText(_string, xPos, yPos, true);
            private static void DisplayText(string _string, int xPos = 0, int yPos = 0, bool clearLine = true)
            {
                // Some IDEs (VSCode included) re-direct the logs, on VSCode they re-direct to a tab called "Output". This can be
                // changed in some settings to re-direct to the integrated Terminal. Usually these outputs are quites restricted,
                // meaning they don't allow you to input, or perhaps "Read()" works but "ReadKey()" won't, or it won't allow you
                // to use Clear(). I use the "IsOutputRedirect" field to allow me to prevent errors and show an error message.
                if (!Console.IsOutputRedirected) Console.Clear();
                if (screen is null || currentScreen is null) Console.WriteLine(_string); // A fail-safe, should never actually execute, also "is" keyword (although there is no difference between "is" and plain old "==" when using this with `null`).
                else
                {
                    // Setting initial pos for the new text.
                    int startXPos = xPos + 7 + widthPadding;
                    int startYPos = yPos + 0 + heightPadding;

                    // Firstly, I grab the display only (so, ignoring the body of the Computer screen), in rows.
                    List<string> displayRows = currentScreen.Split('\n').ToList().GetRange(6, displayHeight);

                    // I then split string provided in the parameters into it's respective lines.
                    _string.Split('\n').ToList().ForEach((nowrapString) => {
                        // Now, a line could go throw the computer screen if they're too long. So, we need to add a new line just before it goes pass with dispaly width.
                        List<string> wrappedStrings = nowrapString.WrapOverflow(displayWidth - (widthPadding * 2)); // Custom method to do just that ^^ (basically converts a CSS nowrap to a normal).

                        // Here we just replace each row (in the display we grabbed previously) with one with the newly wrapped text provided by the paramter.  
                        foreach (string wrappedString in wrappedStrings)
                        {
                            string fullLine = clearLine ? wrappedString.PadRight(displayWidth - (widthPadding * 2) - xPos) : wrappedString;
                            string ReplacementRow(int rowIndex) => displayRows[rowIndex].Substring(0, startXPos) + fullLine + displayRows[rowIndex].Substring(startXPos + fullLine.Length);

                            if (startYPos > displayRows.Count() - heightPadding) continue; // Using .Count() (Which is +1 of last index) accounts for the default bottom padding of screen

                            displayRows[startYPos] = ReplacementRow(startYPos++);
                        }
                    });

                    // Join it back together and send it to be displayed (we cleared screen at the top).
                    string display = String.Join('\n', displayRows);
                    currentScreen = screen.Replace("\nDisplay:", $"{(displayHeight == 0 ? "" : $"\n{display}")}");

                    Console.WriteLine(currentScreen);
                }
            }

            public static void PowerOn(bool animateCheck)
            {
                if (!Console.IsOutputRedirected) Computer.Build(); // Builds the computer
                else Computer.Build(68, 21);

                // Alternatively, I was going to do this boot-up animation using ConsoleColor (White, LightGray, Gray and Black)
                // But, it since the actually hexcode for each enum constant depends on the terminal itself, this may not look the best on
                // all terminals. In my case, "LightGray" and "White" look identical. For this reason, I used the brightness of ASCII chars.

                if (animateCheck) // Runs a bad simulation of the old Atari SM124 boot up screen
                {
                    string gradientString = """ '|V#"""; // Each character has different levels of brightness when bunched up.
                    int seconds = 3; // The amount of time for the boot up screen to be shown for. 3 seconds should be fine...

                    foreach (char c in gradientString.ToList())
                    {
                        // Basically, just fill up the entire screen with the same character.
                        DisplayText(string.Concat(Enumerable.Repeat(c, (displayWidth - (widthPadding * 2)) * (displayHeight - (heightPadding * 2)))));
                        Thread.Sleep(1 * (seconds * 1000 / gradientString.Length));
                    }
                }
                else
                {
                    ClearScreen(); // Actually shows the display that was built (DisplayText() does this, ClearScreen executes DisplayTex()).
                }
            }

            public static void ConfigureDisplaySize()
            {

                ConsoleKeyInfo keyinfo;
                do
                {
                    // This code is kinda self-explanatory.
                    Build(displayWidth, displayHeight);

                    DisplayText("Choose your screen size\n(Use the Arrow Keys then press 'Enter')", 0);
                    keyinfo = Console.ReadKey(true);

                    if (keyinfo.Key == ConsoleKey.UpArrow) displayHeight--;
                    if (keyinfo.Key == ConsoleKey.DownArrow) displayHeight++;
                    if (keyinfo.Key == ConsoleKey.LeftArrow) displayWidth--;
                    if (keyinfo.Key == ConsoleKey.RightArrow) displayWidth++;

                    indicator = keyinfo.Key switch // Switch expression to change indicator at the bottom of PC (... the old Atari SM124 couldn't do this lol, it was just a solid green LED).
                    {
                        ConsoleKey.UpArrow => '^',
                        ConsoleKey.DownArrow => 'v',
                        ConsoleKey.LeftArrow => '<',
                        ConsoleKey.RightArrow => '>',
                        _ => indicator
                    };
                } while (keyinfo.Key != ConsoleKey.Enter);

                indicator = 'O';
                Build(displayWidth, displayHeight); // Build the thing with the new specs.
            }

            public static Dictionary<String, SchoolRoster.Student> allStudents = new Dictionary<String, SchoolRoster.Student>(); // Stored student database outside of SR class, so data is stored whilst program isn't running (also because SchoolRoster isn't static).

            public static void ProgramSelector()
            {
                ClearScreen();

                int index = 0;

                // All the inner classes inside the Computer class, are the three programs the brief requires.
                // Everything else is a method. Allowing me to just get all the classes, and loop through them.
                // As a menu system.
                Type[] programs = typeof(Computer).GetNestedTypes();
                Type selectedProgram = programs[index];

                ConsoleKeyInfo keyinfo = new ConsoleKeyInfo();
                do
                {
                    // Action delegate to execute code with parameters, using lambda expression
                    Action<Type, int> DisplayOptions = (program, index) => {
                        string selectedIndicator = Object.Equals(programs[index], selectedProgram) ? "> " : "";
                        string? programName = programs[index].GetMethod("GetName")?.Invoke(Activator.CreateInstance(programs[index]), null)?.ToString();  // Calls the GetName() method
                        int position = programs[index].Name == "PowerOff" ? displayHeight - (heightPadding * 2) - 1 : index * 2;

                        DisplayText(selectedIndicator + programName, position);
                    };

                    for (int i = 0; i < programs.Length; i++) // Inline for loop
                        DisplayOptions(programs[i], i);

                    if (!Console.IsOutputRedirected) keyinfo = Console.ReadKey(true); // Using ReadKey instead of ReadLine for better UX.
                    else
                    {
                        ClearScreen();
                        DisplayText("No console found, output redirected.".PadSides(displayWidth - (widthPadding * 2)), (displayHeight - (heightPadding * 2) - 1) / 2);
                        DisplayText("Use regular terminal.".PadSides(displayWidth - (widthPadding * 2)), ((displayHeight - (heightPadding * 2) - 1) / 2) + 1);
                        break;
                    }

                    // Handle each key input
                    if (keyinfo.Key == ConsoleKey.DownArrow) index = (index + 1 < programs.Length) ? ++index : 0;
                    else if (keyinfo.Key == ConsoleKey.UpArrow) index = (index - 1 >= 0) ? --index : programs.Length - 1;

                    selectedProgram = programs[index];
                }
                while (keyinfo.Key != ConsoleKey.Enter);

                if (keyinfo.Equals(default)) return;
                selectedProgram.GetMethod("Start")?.Invoke(Activator.CreateInstance(selectedProgram), null)?.ToString();  // Calls the Start() method
            }

            public class TrinaryConverter : BaseProgram
            {
                public override string GetName() => "Trinary Converter";

                /*
                    Input:          Expected Output:    Actual Output:                      Resolved Output:
                    1               1                   "The decimal value is: 1"           N/A
                    2               2                   "The decimal value is: 2"           N/A
                    10              3                   "The decimal value is: 3"           N/A
                    112             14                  "The decimal value is: 14"          N/A
                    1122000120      32091               "The decimal value is: 32091"       N/A
                    7               Error               "Error: 7 is not used im base 3!"   N/A
                    A               Error               "Invalid character!"                N/A
                    ! (shift+1)     Error               !                                   "Invalid character!"
                    " (shift+2)     Error               "                                   "Invalid character!"

                    A unit test will be provided, in addition to this, upon submission. Unit testing was done using xUnit as opposed to MSTest.
                    Unit testing was done only for Trinary Converter, due to the following sentence from the brief:
                    "For a better grade you should implement unit tests for this problem." ('this' referring to Trinary Converter).

                    I believe the program has been tested to a suitable level.
                */

                private int index = 0;
                private string input = "";
                private string output = "";
                private ConsoleKeyInfo keyinfo;
                public void Start()
                {
                    ClearScreen();
                    DisplayText("Input a ternary value:");

                    do
                    {
                        DisplayText(((index == 0) ? "> " : "") + this.input, 1);
                        DisplayText(((index == 1) ? "> " : "") + "Back to Menu", displayHeight - (heightPadding * 2) - 1);

                        keyinfo = Console.ReadKey(true);

                        // Handle each key input
                        if (keyinfo.Key == ConsoleKey.DownArrow && index < 1) index++;
                        else if (keyinfo.Key == ConsoleKey.UpArrow && index > 0) index--;

                        if (index == 1) continue;

                        // Keeping code easy to read with the use of methods, inside inner class to not mix in with the slightly
                        // different "UpdateInputField()"s used in other programs.
                        UpdateInputField();
                    }
                    while (keyinfo.Key != ConsoleKey.Enter || (index == 0 && this.input.Length == 0));

                    if (index == 0)
                    {
                        // Calculate the answer instantly.
                        this.output = "The decimal value is: " + CalculateConversion(this.input, out List<string> equationParts);

                        // Even though answer is already received, we show user how the answer was received.
                        if (equationParts.Count() > 1) AnimateCalculation(equationParts);

                        ClearScreen(3);
                        DisplayText(this.output + "\n\nPress any key to go again.", 3);

                        Console.ReadKey(true); // Using boolean parameter in .ReadKey() to hide input
                        new TrinaryConverter().Start();
                    }
                    else ProgramSelector();
                }

                public double CalculateConversion(string input, out List<string> equationParts)
                {
                    // Calculate the answer instantly.
                    List<string> tempEquationParts = new List<string>(); // Could've used .ToCharArray() but, .Cast<>() exists too
                    double result = input.Cast<char>().Select((digit, i) => { // Using select over a for loop for compactness + I can use it for the animating later on
                        double part = ((int)digit - '0') * Math.Pow(3, input.Length - (++i)); // Trinary conversion in one line
                        tempEquationParts.Add(i == 1 ? "" + part : $" + {part}"); // Add the parts to list, to later be animated 
                        return part;
                    }).Aggregate((a, b) => a + b); // Could've used .Sum() but I wanted to show that I know how .Aggregate() works

                    equationParts = tempEquationParts;
                    return result;
                }

                private void AnimateCalculation(List<string> equationParts)
                {
                    // This slowly shows the calculation being done.
                    string equation = "Calculating: ";
                    foreach (string part in equationParts)
                    {
                        equation += part;
                        DisplayText(equation, 0, 3);
                        Thread.Sleep(1000);
                    }
                }

                private void UpdateInputField()
                {
                    switch (keyinfo.Key) // Swith statement using fall-thrugh behaviour
                    {
                        case ConsoleKey.Backspace when this.input.Length > 0: // Using case guard "when" instead of an if statement
                            this.input = this.input.Remove(this.input.Length - 1);
                            goto case ConsoleKey.Enter; // Empty output using goto statement
                        case ConsoleKey.D0:
                        case ConsoleKey.D1:
                        case ConsoleKey.D2:
                            if (keyinfo.Modifiers != ConsoleModifiers.None) goto default;
                            this.input += keyinfo.KeyChar;
                            goto case ConsoleKey.Enter; // Empty output
                        case ConsoleKey.D3:
                        case ConsoleKey.D4:
                        case ConsoleKey.D5:
                        case ConsoleKey.D6:
                        case ConsoleKey.D7:
                        case ConsoleKey.D8:
                        case ConsoleKey.D9:
                            if (keyinfo.Modifiers != ConsoleModifiers.None) goto default;
                            this.output = $"Error: {keyinfo.KeyChar} is not used im base 3!";
                            break;
                        case ConsoleKey.UpArrow:
                        case ConsoleKey.DownArrow:
                        case ConsoleKey.Enter:
                            this.output = String.Empty;
                            break;
                        default:
                            this.output = "Invalid character!";
                            break;
                    }

                    DisplayText(this.output, 3);
                }
            }

            public class SchoolRoster : BaseProgram
            {
                public override string GetName() => "School Roster";

                /*
                    I did thooughly test this, however I won't be providing a test plan as it would be unnecessary.

                    ".. you are asked to provide a test plan for at least one of your solutions" ("at least one")
                    "Provide a test plan for the Trinary Converter problem."

                    Since I've already done a test plan the the Trinary Converter, it would be unnecessary to do so for this one.
                */

                /* To save time testing the code below, you can use the following methods.

                    I'm aware going through the conversation again and again takes some time. But... it's fun :D

                    AddStudent("Aimee", 2);

                    AddStudent("Blair", 2);
                    AddStudent("James", 2);
                    AddStudent("Paul", 2);

                    AddStudent("Chelsea", 3);
                    AddStudent("Logan", 7);

                    AddStudent("Jennifer", 4);
                    AddStudent("Kareem", 6);
                    AddStudent("Chris", 4);
                    AddStudent("Claire", 3);
                */

                public struct Student
                { // Structs are more closer to DB entities than a class, it also isn't commonly used therefore I'm showing off again
                    public string firstName;
                    public int form;
                    /*
                        I could make a struct (entity) for this, but it would be unnecessary so I'll leave it as an int.
                        Benefit of struct would be to include, for example the year groups's timetable, the subject's being
                        taught in the year groups, teachers, etc. But we only need the form number for this asm.
                    */

                    public Student(string providedFirstName, int providedForm)
                    {
                        /*
                            The brief says:
                            "Note that our students have just one name and those names are unique within a form"
                            Therefore, I'll be using the firstName as the primary key as opposed to a UUID-type student ID
                            Additionally, since students have "one name" this implies they don't have a last name either.
                            ... even though that's incredibly unrealistic.
                        */

                        // id = Guid.NewGuid();
                        firstName = providedFirstName;
                        // lastName = providedLastName;
                        form = providedForm;
                    }
                }

                private string convo = "";
                private string input = "";
                private string output = "";

                private string name = "You";
                private string childName = "Them";
                private string studentName = "Someone";

                private ConsoleKeyInfo keyinfo;
                public void Start()
                {
                    /* General guide of the conversation:
                     *
                     *   Stranger: Hey, I'm the head teacher!
                     *   Head teacher: And you are?
                     *
                     *   You:
                     *   -> My name is {Input Name}. DONE
                     *
                     *   Head teacher: Ah yes, Mx. {Input Name}.
                     *   Head teacher: So, why did you request for this meeting?
                     *
                     *   {Input Name}:
                     *   |-> I'd like to enroll a child. DONE
                     *   -> I have a question. DONE
                     *
                     *   Head teacher:
                     *   Oh certaintly, would you like to know about a specific student?
                     *
                     *   {Input Name}:
                     *   |-> Yes, I'd like to ask about {Student Name}}. DONE
                     *   -> No, I actually want a list of student names. DONE
                     *
                     *   Head teacher:
                     *   Sure thing, we don't care about privacy here!
                     *   Are you interested in a specific year group?
                     *
                     *   {Input Name}:
                     *   |-> Yes, I'd like to year {#}. NOT DONE
                     *   |-> No, Can I get a list of all the students? NOT DONE
                     *
                     *   -----
                     *
                     *   Head teacher:
                     *   Hopefully that information helped you, is there anything else?
                     *
                     *   Loop back.
                     *
                     */

                    // AddStudent("Jennifer", 4);
                    // AddStudent("Kareem", 6);
                    // AddStudent("Chris", 4);
                    // AddStudent("Claire", 3);

                    ClearScreen();

                    string[] questions = {
                    "Stranger: Hey, I'm the head teacher!\nHead Teacher: And you are?",
                    "Head Teacher:\nAh yes, {name}. It's a plesaure to meet you.\nSo, why did you request for this meeting?",
                    "Head Teacher: Oh certaintly, would you like to know about a specific student?",
                    "Head Teacher:\nSure thing, we don't care about privacy here!\nAre you interested in a specific year group?",
                    "Head Teacher:\nHopefully that information helped you.\nIs there anything else I can help you with?",
                    "Head Teacher:\nOf course, even as a guardian, you can enroll your child.\nWhat's their name?",
                    "Head Teacher:\n{childName} did you say? That's a nice name!\nWhat year group do you intend them to join?",
                    "Head Teacher:\nSure thing, I'll add them into the school database immediately.\nIs there anything else I can help you with?",
                    "Head Teacher:\n{studentName}.. {studentName}.. ah, here they are!\nAccording to the database, {studentName} is in year {yearGroup}.\nnIs there anything else I can help you with?",
                    "Head Teacher:\nWell, you see.. {studentName}'s file is marked as confidential... for some reason...\nIs there anything else I can help you with?",
                    "Head Teacher:\nYes, I know they're your child but- look, I don't want anyone to get hurt.\nIs there anything else I can help you with?",
                    "Head Teacher:\nHere's a list of names of all students in year {yearGroup}:\n{listOfNames}\n\nIs there anything else I can help you with?",
                    "Head Teacher:\nHere's a list of names of all students in the school:\n{listOfNames}\n\nIs there anything else I can help you with?"
                };

                    string[][] answers = {[
                    "My name is ___."
                ], [
                    "I'd like to enroll a child.",
                    "I have a question."
                ], [
                    "Yes, I'd like to ask about ___.",
                    "No, I actually want a list of student names."
                ], [
                    "Yes, tell me about year ___.",
                    "No, Can I get a list of all the students?"
                ], [
                    "Yes.",
                    "No, that's all. Thank you."
                ], [
                    "Their name is ___."
                ], [
                    "I'd like {childName} to join year ___."
                ], [
                    "Yes.",
                    "No, that's all. Thank you."
                ], [
                    "Yes, can I have more information on {studentName}?",
                    "No, that's all. Thank you."
                ], [
                    "But {studentName} is MY child?",
                    "No, that's all. Thank you."
                ], [
                    "Yes, what data are you holding on my child?!",
                    "No... that'll be all... for now."
                ], [
                    "Yes.",
                    "No, that's all. Thank you."
                ], [
                    "Yes.",
                    "No, that's all. Thank you."
                ]};

                    int currentQ = 0;
                    int currentA = 0;
                    string nextQ = String.Empty;
                    
                    SendText(questions[currentQ]);

                    int index = 1;
                    do
                    {

                        // Most things here are code I've already used before but in slightly different ways, also kinda common sense
                        // The code below involves two while loops, the first one for the Left and Right options menu between
                        // Back to Menu and Respond, the second involves all arrow keys for used for switching between the two
                        // aforementioned and also for the potential answers.
                        int maxYPos = displayHeight - (heightPadding * 2) - 1;
                        int answerCount = answers[currentQ].Length - 1;

                        ClearScreen(maxYPos - answerCount - 1);
                        SendText("", 3 + answerCount, false);
                        if (nextQ != "" && keyinfo.Key == ConsoleKey.Enter) SendText(nextQ);

                        Computer.DisplayText(((index == 0) ? "> " : "") + "Back to Menu".PadRight(15), 0, maxYPos, false); // I use "Computer." to make it clear, which method we're using
                        Computer.DisplayText(("Respond" + ((index == 1) ? " <" : "")).PadLeft(displayWidth - (widthPadding * 2) - 15), 15, maxYPos);

                        keyinfo = Console.ReadKey(true);

                        if (keyinfo.Key == ConsoleKey.LeftArrow && index > 0) index--;
                        else if (keyinfo.Key == ConsoleKey.RightArrow && index < 1) index++;

                        if (index == 0 || keyinfo.Key != ConsoleKey.Enter) continue;

                        do
                        {
                            if (this.input == "") ClearScreen(maxYPos - answerCount - 1);
                            SendText("", 3 + answerCount, false);
                            Computer.DisplayText(((index == 0) ? "> " : "") + "Back to Menu".PadRight(15), 0, maxYPos, false);

                            // This shows the potential answers
                            if (this.input == "") for (int i = 0; i <= answerCount; i++) // Inline if and for loop statements? You don't see that often.
                                    Computer.DisplayText((answers[currentQ][i] + ((index == 1 && currentA == i) ? " <" : "")).PadLeft(displayWidth - (widthPadding * 2) - 15), 15, maxYPos - (answerCount - i));

                            keyinfo = Console.ReadKey(true);

                            // Prevents being able to change answers when there's only one answer available.
                            if (answerCount == 0 && (keyinfo.Key == ConsoleKey.UpArrow || keyinfo.Key == ConsoleKey.DownArrow)) continue;

                            // Deals with key inputs for selecting answers & back to menu
                            int oldA = currentA, oldIndex = index;
                            if (index == 1 && keyinfo.Key == ConsoleKey.UpArrow && currentA > 0) currentA--;
                            else if (index == 1 && keyinfo.Key == ConsoleKey.DownArrow && currentA < 1) currentA++;
                            else if (keyinfo.Key == ConsoleKey.LeftArrow && index > 0) index--;
                            else if (keyinfo.Key == ConsoleKey.RightArrow && index < 1) index++;

                            if (oldA != currentA | oldIndex != index) continue; // Unnecessary use of bitwise OR operator

                            if (index == 0)
                            {
                                // Makes when on "Back to Menu" no input fields are updated
                                if (keyinfo.Key != ConsoleKey.Enter) continue;
                                else break;
                            }
                            else
                            {
                                if (keyinfo.Key != ConsoleKey.Enter)
                                {
                                    if (keyinfo.Key == ConsoleKey.RightArrow) continue;

                                    List<ConsoleKey> acceptedControlKeys = new List<ConsoleKey> { ConsoleKey.Backspace, ConsoleKey.Enter, ConsoleKey.Spacebar };
                                    int maxLength = displayWidth - (widthPadding * 2) - ((answerCount - currentA) == 0 ? 15 : 0);

                                    // Using Func delegate
                                    Func<int, bool> UpdateInputField = delegate(int limit) {
                                        if (keyinfo.Key == ConsoleKey.Backspace)
                                        {
                                            if (this.input.Length > 0) this.input = this.input.Remove(this.input.Length - 1);
                                            else return true;
                                        }
                                        else if (!Char.IsControl(keyinfo.KeyChar) && (limit == -1 || (limit != -1 && this.input.Length < limit))) this.input += keyinfo.KeyChar;

                                        return false;
                                    };

                                    switch (currentQ)
                                    { // The inputs accepted in some answers are different to others, I deal with that here:
                                        case 0: /* Stranger: Hey, I'm the head teacher!\nHead Teacher: And you are? */
                                        case 2: /* Head Teacher:\nOh certaintly, would you like to know about a specific student? */
                                        case 5: /* Head Teacher:\nOf course, even as a guardian, you can enroll your child. What's their name? */

                                            // The "break" used below doesn't break the switch statement, it breaks the overarching do..while loop.
                                            // Therefore, the if statement with the method returning a boolean `UpdateInputField()`, below, although may look useless, it isn't.

                                            if (currentA != 0) break;

                                            // Limit to alphabet characters only
                                            if ((keyinfo.Key < ConsoleKey.A || keyinfo.Key > ConsoleKey.Z) && !acceptedControlKeys.Contains(keyinfo.Key)) break;
                                            if (UpdateInputField(-1)) break; // Method returns boolean value

                                            break;
                                        case 3: /* Head Teacher:\nSure thing, we don't care about privacy here!\nAre you interested in a specific year group? */
                                        case 6: /* Head Teacher:\n{name} did you say? That's a nice name!\nWhat year group do you intend them to join? */
                                            if (currentA != 0) break;

                                            // Limit to digit characters only
                                            if ((!Char.IsDigit(keyinfo.KeyChar) && !acceptedControlKeys.SkipLast(1).Contains(keyinfo.Key)) || keyinfo.Modifiers != ConsoleModifiers.None) break;
                                            if (UpdateInputField(2)) break;

                                            break;
                                    }

                                    string response = answers[currentQ][0].Replace("___", this.input) + " <";
                                    if (response.Length < maxLength - ((answerCount - currentA) == 0 ? 2 : -1)) Computer.DisplayText(response.PadLeft(maxLength), (answerCount - currentA) == 0 ? 15 : 0, maxYPos - (answerCount - currentA));
                                    else Computer.DisplayText("... < ", displayWidth - (widthPadding * 2) - 5, maxYPos - (answerCount - currentA)); // Ellipses `...` to deal with overflow.

                                    if (keyinfo.Key != ConsoleKey.Enter) continue;
                                }
                                else
                                {
                                    bool endChat = false;
                                    this.output = name + ": " + answers[currentQ][currentA];

                                    switch (currentQ)
                                    {

                                        case 0:

                                            if (true) /* "My name is ___. */
                                            {
                                                name = this.input != "" ? Char.ToUpper(this.input[0]) + String.Concat(this.input.Skip(1))?.ToLower() : name;
                                                this.output = "You: " + answers[currentQ][currentA].Replace("___", name);

                                                currentQ = 1;
                                                nextQ = questions[currentQ].Replace("{name}", name);
                                            }

                                            break;
                                        case 1:

                                            if (currentA == 0) /* I'd like to enroll a child. */
                                            {
                                                currentQ = 5;
                                                nextQ = questions[currentQ];
                                            }
                                            else if (currentA == 1) /* I have a question. */
                                            {
                                                currentQ = 2;
                                                nextQ = questions[currentQ];
                                            }

                                            break;
                                        case 2:

                                            if (currentA == 0) /* Yes, I'd like to ask about ___. */
                                            {
                                                studentName = this.input != "" ? Char.ToUpper(this.input[0]) + String.Concat(this.input.Skip(1))?.ToLower() : studentName;
                                                this.output = name + ": " + answers[currentQ][currentA].Replace("___", studentName);

                                                if (GetStudent(studentName, out Student? student))
                                                {
                                                    Student existingStudent = student == null ? new Student() : (Student)student; // Converting nullable to a non-null
                                                    int yearGroup = existingStudent.form;

                                                    currentQ = 8;
                                                    nextQ = questions[currentQ].Replace("{studentName}", studentName).Replace("{yearGroup}", yearGroup.ToString());
                                                    answers[currentQ][0] = answers[currentQ][0].Replace("{studentName}", studentName);
                                                }
                                                else
                                                {
                                                    SendText("Head Teacher: I could not find a student called \"" + studentName + "\".\nHow do you spell it?");

                                                    currentA = 0;
                                                    this.input = String.Empty;
                                                    this.output = String.Empty;

                                                    continue;
                                                };

                                            }
                                            else if (currentA == 1) /* No, I actually want a list of student names. */
                                            {
                                                currentQ = 3;
                                                nextQ = questions[currentQ];
                                            }

                                            break;
                                        case 3:

                                            if (currentA == 0) /* Yes, tell me about year ___. */
                                            {
                                                if (this.input == "")
                                                {
                                                    currentA = 0;
                                                    this.input = String.Empty;
                                                    this.output = String.Empty;

                                                    continue;
                                                }
                                                else
                                                {
                                                    int yearGroup = int.Parse(this.input);
                                                    this.output = name + ": " + answers[currentQ][currentA].Replace("___", yearGroup.ToString());

                                                    string listOfNames = String.Join("\n", ListStudents(yearGroup).Select((student) => "- " + student.firstName));
                                                    if (listOfNames.Length == 0) listOfNames = "- None.";

                                                    currentQ = 11;
                                                    nextQ = questions[currentQ].Replace("{yearGroup}", yearGroup.ToString()).Replace("{listOfNames}", listOfNames);
                                                }
                                            }
                                            else if (currentA == 1) /* No, Can I get a list of all the students? */
                                            {
                                                string listOfNames = String.Join("\n", ListStudents().Select((student) => "- " + student.firstName + " from Year " + student.form));
                                                if (listOfNames.Length == 0) listOfNames = "- None.";

                                                currentQ = 12;
                                                nextQ = questions[currentQ].Replace("{listOfNames}", listOfNames);
                                            }

                                            break;
                                        case 4:

                                            if (currentA == 0) /* Yes. */
                                            {
                                                currentQ = 1;
                                                nextQ = "Ah, what is it you'd like to talk about?";
                                            }
                                            else if (currentA == 1) /* No, that's all. Thank you. */
                                            {
                                                endChat = true;
                                            }

                                            break;
                                        case 5:

                                            if (true) /* "Their name is ___. */
                                            {
                                                childName = this.input != "" ? Char.ToUpper(this.input[0]) + String.Concat(this.input.Skip(1))?.ToLower() : childName;
                                                this.output = name + ": " + answers[currentQ][currentA].Replace("___", childName).Replace("{childName}", childName);

                                                currentQ = 6;
                                                nextQ = questions[currentQ].Replace("{childName}", childName);
                                                answers[currentQ][0] = answers[currentQ][0].Replace("{childName}", childName);
                                            }

                                            break;
                                        case 6:

                                            if (true) /* "I'd like {childName} to join year ___. */
                                            {
                                                if (this.input == "")
                                                {
                                                    currentA = 0;
                                                    this.input = String.Empty;
                                                    this.output = String.Empty;

                                                    continue;
                                                }
                                                else
                                                {
                                                    int yearGroup = int.Parse(this.input);
                                                    this.output = name + ": " + answers[currentQ][currentA].Replace("___", yearGroup.ToString()).Replace("{childName}", childName);

                                                    AddStudent(childName, yearGroup);

                                                    currentQ = 7;
                                                    nextQ = questions[currentQ];
                                                }
                                            }

                                            break;
                                        case 7:

                                            if (currentA == 0) /* Yes. */
                                            {
                                                currentQ = 1;
                                                nextQ = "Head Teacher: What do you need help with?";
                                            }
                                            else if (currentA == 1) /* No, that's all. Thank you. */
                                            {
                                                endChat = true;
                                            }

                                            break;
                                        case 8:

                                            if (currentA == 0) /* Can I have more information on {studentName}? */
                                            {
                                                this.output = name + ": " + answers[currentQ][currentA].Replace("{studentName}", studentName);

                                                currentQ = 9;
                                                nextQ = questions[currentQ].Replace("{studentName}", studentName);
                                                answers[currentQ][0] = answers[currentQ][0].Replace("{studentName}", studentName);
                                            }
                                            else if (currentA == 1) /* No, that's all. Thank you. */
                                            {
                                                endChat = true;
                                            }

                                            break;
                                        case 9:

                                            if (currentA == 0) /* But {studentName} is MY child? */
                                            {
                                                this.output = name + ": " + answers[currentQ][currentA].Replace("{studentName}", studentName);

                                                currentQ = 10;
                                                nextQ = questions[currentQ];
                                            }
                                            else if (currentA == 1) /* No, that's all. Thank you. */
                                            {
                                                endChat = true;
                                            }

                                            break;
                                        case 10:

                                            if (currentA == 0) /* Yes, what data are you holding on my child?! */
                                            {
                                                currentQ = 1;
                                                nextQ = "Head Teacher:\nSecurity!!!";
                                                Thread.Sleep(2500);

                                                endChat = true;
                                            }
                                            else if (currentA == 1) /* No... that'll be all... for now. */
                                            {
                                                endChat = true;
                                            }

                                            break;
                                        case 11:

                                            if (currentA == 0) /* Yes. */
                                            {
                                                currentQ = 1;
                                                nextQ = "Head Teacher: What do you need help with?";
                                            }
                                            else if (currentA == 1) /* No, that's all. Thank you. */
                                            {
                                                endChat = true;
                                            }

                                            break;
                                        case 12:

                                            if (currentA == 0) /* Yes. */
                                            {
                                                currentQ = 1;
                                                nextQ = "Head Teacher: What do you need help with?";
                                            }
                                            else if (currentA == 1) /* No, that's all. Thank you. */
                                            {
                                                endChat = true;
                                            }

                                            break;
                                    }

                                    SendText(this.output);

                                    if (endChat)
                                    {
                                        Thread.Sleep(1000);
                                        goto End_School_Roster_Program;
                                    }

                                    currentA = 0;
                                    this.input = String.Empty;
                                    this.output = String.Empty;
                                }
                            }
                        }
                        while (keyinfo.Key != ConsoleKey.Enter);
                    }
                    while (keyinfo.Key != ConsoleKey.Enter || index == 1);

                    End_School_Roster_Program:
                    ProgramSelector();
                }

                private void SendText(string _string) => SendText(_string, 3); // "Send" "Text" refers to sending SMS text messages to another user, whereas "Display" "Text" refers to literally showing alphabetic characters on a screen
                private void SendText(string _string, int marginBottom = 3) => SendText(_string, marginBottom, true);
                private void SendText(string _string, int marginBottom = 3, bool newLine = true) // Using ` = ` to set the default value for the parameters
                {
                    // This is basically the same as DisplayText() but some differences to make it "scroll to end".
                    convo += ((convo == "" || !newLine) ? "" : "\n\n") + _string;
                    _string = convo;

                    if (!Console.IsOutputRedirected) Console.Clear();
                    if (screen is null || currentScreen is null) Console.WriteLine(_string);
                    else
                    {
                        int startXPos = 7 + widthPadding;
                        int startYPos = 0 + heightPadding;

                        List<string> displayRows = currentScreen.Split('\n').ToList().GetRange(6, displayHeight);

                        // In order to not get rid of the "Back to Menu" and "Respond" we set a margin that we don't overlap
                        // To do this, I "shrink" displayRows by basically taking the last few rows out of the equation below
                        // and it back after all of that's been done. Therefore it stays unaffected by "scroll to end"
                        // or in other words, it doesn't move up with the convo.
                        List<string> lastDisplayRows = displayRows.TakeLast(marginBottom).ToList();
                        displayRows.RemoveRange(displayRows.Count() - marginBottom, marginBottom);

                        _string.Split('\n').ToList().ForEach((nowrapString) => {
                            List<string> wrappedStrings = nowrapString.WrapOverflow(displayWidth - (widthPadding * 2));

                            foreach (string wrappedString in wrappedStrings)
                            {
                                string fullLine = wrappedString.PadRight(displayWidth - (widthPadding * 2));
                                // Inline function
                                string ReplacementRow(int rowIndex) => displayRows[rowIndex].Substring(0, startXPos) + fullLine + displayRows[rowIndex].Substring(startXPos + fullLine.Length);

                                if (startYPos > displayRows.Count() - 1) // Using .Count() (Which is +1 of last index) accounts for the default bottom padding of screen
                                {
                                    // If conversation is too big, remove the first row and append a new one at the bottom.
                                    displayRows.Insert(displayRows.Count(), ReplacementRow(startYPos - 1));
                                    displayRows.RemoveAt(heightPadding);
                                    continue;
                                }

                                // Like normal, just append a new line at the bottom
                                displayRows[startYPos] = ReplacementRow(startYPos++);
                            }
                        });

                        // Add the respond, answwers and back to menu buttons back
                        displayRows.AddRange(lastDisplayRows);

                        string display = String.Join('\n', displayRows);
                        currentScreen = screen.Replace("\nDisplay:", $"{(displayHeight == 0 ? "" : $"\n{display}")}");

                        Console.WriteLine(currentScreen);
                    }
                }

                public Student AddStudent(string providedFirstName, int providedForm)
                {
                    // Creates a student (if not already a thing) and adds them to the database
                    // I could've use a HashSet for this, but I also having used a Dictionary yet, so I went for the latter.
                    // This allows me to also use a try..catch (haven't used it yet) when getting the student later.
                    Student newStudent;
                    if (GetStudent(providedFirstName, out Student? existingStudent))
                    {
                        newStudent = existingStudent == null ? new Student() : (Student)existingStudent;
                    }
                    else
                    {
                        newStudent = new Student(providedFirstName, providedForm);
                        allStudents.Add(newStudent.firstName, newStudent);
                    }

                    return newStudent;
                }

                public bool GetStudent(string providedFirstName, out Nullable<Student> student)
                {
                    // or... return allStudents[providedFirstName];

                    try
                    {
                        // Gets a student, the long way - haven't used .Single() yet.
                        student = allStudents.Values.Single((student) => student.firstName == providedFirstName);
                        return true;
                    }
                    catch
                    {
                        student = null;
                        return false;
                    }
                }

                public List<Student> ListStudents()
                {
                    // Using some methods I haven't shown that I know how to use yet
                    return allStudents.Values.OrderBy((student) => student.form).ThenBy((student) => student.firstName).ToList();
                }

                public List<Student> ListStudents(int providedForm)
                {
                    // Same again
                    return allStudents.Values.Where((student) => student.form == providedForm).ToList();
                }

            }

            public class ISBNVerifier : BaseProgram
            {

                // Inline method, I did this while overloading the DisplayText() as well
                public override string GetName() => "ISBN Verifier";

                /*
                    I did thooughly test this, however I won't be providing a test plan as it would be unnecessary.

                    ".. you are asked to provide a test plan for at least one of your solutions" ("at least one")
                    "Provide a test plan for the Trinary Converter problem."

                    Since I've already done a test plan the the Trinary Converter, it would be unnecessary to do so for this one.
                */


                /* To save time testing, you can copy and paste the following.

                    A brief warning, if the copy and paste includes a "new line", it will be percieved as an "Enter" and thus will immediately check for validity.
                    This is intended behaviour, as it makes copying and pasting easier, but may catch the user off guard if they intend to copy, paste, edit and then press enter.

                    3-598-21508-8 - Valid
                    3-598-21507-X - Valid
                    3-598-21508-9 - Invalid
                    3-598-21507-A - Invalid
                    3-598-2X507-9 - Invalid

                */

                private string input = "";
                private string output = "";
                private ConsoleKeyInfo keyinfo;
                public void Start()
                {
                    ClearScreen();
                    DisplayText("Input your ISBN-10:");

                    int index = 0;
                    do
                    {
                        DisplayText(((index == 0) ? "> " : "") + this.input, 1);
                        DisplayText(((index == 1) ? "> " : "") + "Back to Menu", displayHeight - (heightPadding * 2) - 1);

                        keyinfo = Console.ReadKey(true);

                        // Handle each key input
                        if (keyinfo.Key == ConsoleKey.DownArrow && index < 1) index++;
                        else if (keyinfo.Key == ConsoleKey.UpArrow && index > 0) index--;

                        if (index == 1) continue;

                        UpdateInputField();
                    }
                    while (keyinfo.Key != ConsoleKey.Enter || (index == 0 && this.input.Length == 0));

                    if (index == 0)
                    {
                        this.input = this.input.Replace("-", "");

                        // Using and creating arrays, .ToCharArray(), .All(), .Any(), Skip(), .Take() and .Sum()
                        bool[] verifyConditions = new bool[4] {
                        this.input.Length == 10,
                        this.input.ToCharArray().SkipLast(1).All((digit) => char.IsDigit(digit)),
                        this.input.TakeLast(1).Any((digit) => digit == 'X' || char.IsDigit(digit)),
                        this.input.Select((digit, index) => (index, digit)).Sum((character) => {
                            if (character.digit == 'X') return 10;

                            // I believe any good program wouldn't have a try..catch in the first place.
                            // An update comment for context: I did ISBN before the School Roster. Hence the comment above ^^ (I end up unnecessarily using try..catch twice).
                            try
                            {
                                return int.Parse(character.ToTuple().Item2.ToString()) * (10 - character.index);
                            }
                            catch (FormatException) // Since you literally can't input an invalid character (such as "A"), this error will never be thrown.
                            {
                                ClearScreen(3);
                                this.output = $"Couldn't convert '{character.digit}' to int.\n\nPress any key to go again.";
                                DisplayText(this.output, 3);

                                Console.ReadKey(true);
                                new ISBNVerifier().Start();
                                return 0;
                            }
                        }) % 11 == 0 // returns boolean because of "=="
                    };
                        // I did it like this so I can show off and add an animation, similar to the ternary converter.

                        this.output = "The ISBN-10 is: " + (verifyConditions[0] & verifyConditions[1] & verifyConditions[2] & verifyConditions[3] ? "Valid" : "Invalid"); // Unnecessary use of bitwise AND operator

                        if (verifyConditions.First() == true) AnimateCalculation(verifyConditions);

                        ClearScreen(3);
                        DisplayText(this.output + "\n\nPress any key to go again.", 3);

                        Console.ReadKey(true);
                        new ISBNVerifier().Start();
                    }
                    else ProgramSelector();
                }

                private void AnimateCalculation(bool[] verifyConditions)
                {
                    string equation = "Validating...\n";

                    for (int i = 0; i < verifyConditions.Count(); i++)
                    {
                        string conditionName = i switch
                        {
                            0 => "ISBN Length",
                            1 => "First 9 characters",
                            2 => "Last character",
                            3 => "Final check",
                            _ => ""
                        };

                        equation += "\n" + conditionName + " is " + (verifyConditions[i] ? "Valid" : "Invalid");
                        DisplayText(equation, 0, 3);
                        Thread.Sleep(2500);
                    }
                }

                private void UpdateInputField()
                {

                    List<ConsoleKey> acceptedKeys = new List<ConsoleKey> { ConsoleKey.X, ConsoleKey.OemMinus, ConsoleKey.Backspace, ConsoleKey.Enter, ConsoleKey.UpArrow };
                    if ((char.IsDigit(keyinfo.KeyChar) || acceptedKeys.Contains(keyinfo.Key)) && keyinfo.Modifiers == ConsoleModifiers.None)
                    {
                        this.output = String.Empty;
                        if (keyinfo.Key == ConsoleKey.UpArrow) goto Display_Input;
                        if (keyinfo.Key == ConsoleKey.Enter) goto Display_Input;

                        if (keyinfo.Key == ConsoleKey.Backspace && this.input.Length > 0) this.input = this.input.Remove(this.input.Length - 1);
                        if (keyinfo.Key != ConsoleKey.Backspace) this.input += Char.ToUpper(keyinfo.KeyChar);
                    }
                    else if (!Char.IsControl(keyinfo.KeyChar))
                    {
                        this.output = $"You can't put '{Char.ToUpper(keyinfo.KeyChar)}' here.";
                    }
                    else this.output = $"Invalid character!";

                    Display_Input:
                    DisplayText(this.output, 3);
                }
            }

            public class PowerOff : BaseProgram
            {
                public override string GetName() => "Power Off";
                public void Start()
                {
                    Computer.ClearScreen();
                    DisplayText("Turned Off".PadSides(displayWidth - (widthPadding * 2)), (displayHeight - (heightPadding * 2) - 1) / 2);
                    // Environment.Exit(0); is not necessary because I allowed "control paths to naturally converge at the end of the main method"
                }
            }

        }

        public abstract class BaseProgram // This is purposely not a nested class of the Computer class
        {
            public virtual string GetName() => "Unknown Program";
        }
    }
}

namespace System
{
    public static class StringExtensions
    {
        public static string PadSides(this string _string, int totalWidth)
        {
            int spaces = totalWidth - _string.Length;
            int padLeft = spaces / 2 + _string.Length;
            return _string.PadLeft(padLeft).PadRight(totalWidth);
        }
        
        public static string PadSides(this string _string, int totalWidth, char paddingChar) // unused overload, simply just to make it identically to the PadLeft/Right which also has two overloads
        {
            int spaces = totalWidth - _string.Length;
            int padLeft = spaces / 2 + _string.Length;
            return _string.PadLeft(padLeft, paddingChar).PadRight(totalWidth, paddingChar);
        }

        public static List<string> WrapOverflow(this string _string, int maxCharactersPerLine)
        {
            // Truncate to max length:
            // _string.Substring(0, maxCharactersPerLine)

            int quotient = Math.DivRem(_string.Length, maxCharactersPerLine, out int remainder); // Gets both the quotient and remainder from division
            List<string> lines = Enumerable.Repeat("", quotient + (remainder == 0 ? 0 : 1)).ToList();
            if (quotient == 0 && remainder == 0) lines.Add("");

            lines = lines.Select((item, i) => {
                return lines.Count() == (i + 1) ? _string.Substring(i * maxCharactersPerLine, _string.Length - (i * maxCharactersPerLine)) : _string.Substring(i * maxCharactersPerLine, maxCharactersPerLine);
            }).ToList();
            
            return lines;
        }
    }
} 