using RetroTerminal.App.Services;
using RetroTerminal.Logic.Interfaces;
using RetroTerminal.Logic.Models;

namespace RetroTerminal.App.Applications;

public class SchoolRosterApp : IApplication
{
    private readonly IDisplayService _display;
    private readonly ISchoolRoster _roster;
    public string Name => "School Roster";

    private string _convo = "";
    private string _input = "";
    private string _name = "You";
    private string _childName = "Them";
    private string _studentName = "Someone";

    public SchoolRosterApp(IDisplayService display, ISchoolRoster roster)
    {
        _display = display; _roster = roster;
    }

    public Task Run()
    {
        _convo = ""; 
        _input = "";
        
        _display.ClearContent();

        string[] questions = {
            "Stranger: Hey, I'm the head teacher!\nHead Teacher: And you are?",
            "Head Teacher:\nAh yes, {name}. It's a plesaure to meet you.\nSo, why did you request for this meeting?",
            "Head Teacher: Oh certaintly, would you like to know about a specific student?",
            "Head Teacher:\nSure thing, we don't care about privacy here!\nAre you interested in a specific year group?",
            "Head Teacher:\nHopefully that information helped you.\nIs there anything else I can help you with?",
            "Head Teacher:\nOf course, even as a guardian, you can enroll your child.\nWhat's their name?",
            "Head Teacher:\n{childName} did you say? That's a nice name!\nWhat year group do you intend them to join?",
            "Head Teacher:\nSure thing, I'll add them into the school database immediately.\nIs there anything else I can help you with?",
            "Head Teacher:\n{studentName}.. {studentName}.. ah, here they are!\nAccording to the database, {studentName} is in year {yearGroup}.\nIs there anything else I can help you with?",
            "Head Teacher:\nWell, you see.. {studentName}'s file is marked as confidential... for some reason...\nIs there anything else I can help you with?",
            "Head Teacher:\nYes, I know they're your child but- look, I don't want anyone to get hurt.\nIs there anything else I can help you with?",
            "Head Teacher:\nHere's a list of names of all students in year {yearGroup}:\n{listOfNames}\n\nIs there anything else I can help you with?",
            "Head Teacher:\nHere's a list of names of all students in the school:\n{listOfNames}\n\nIs there anything else I can help you with?"
        };

        string[][] answers = {
            new[] { "My name is ___." },
            new[] { "I'd like to enroll a child.", "I have a question." },
            new[] { "Yes, I'd like to ask about ___.", "No, I actually want a list of student names." },
            new[] { "Yes, tell me about year ___.", "No, Can I get a list of all the students?" },
            new[] { "Yes.", "No, that's all. Thank you." },
            new[] { "Their name is ___." },
            new[] { "I'd like {childName} to join year ___." },
            new[] { "Yes.", "No, that's all. Thank you." },
            new[] { "Yes, can I have more information on {studentName}?", "No, that's all. Thank you." },
            new[] { "But {studentName} is MY child?", "No, that's all. Thank you." },
            new[] { "Yes, what data are you holding on my child?!", "No... that'll be all... for now." },
            new[] { "Yes.", "No, that's all. Thank you." },
            new[] { "Yes.", "No, that's all. Thank you." }
        };

        int currentQ = 0;
        int currentA = 0;
        string nextQ = "";

        SendText(questions[currentQ]);

        int index = 1;
        do 
        {
            int maxYPos = _display.DisplayHeight - 3; 
            int answerCount = answers[currentQ].Length - 1;

            _display.ClearContent();
            SendText("", 3 + answerCount); 
            if (nextQ != "") { SendText(nextQ); nextQ = ""; }

            _display.Write(((index == 0) ? "> " : "") + "Back to Menu".PadRight(15), 0, maxYPos);
            int padWidth = Math.Max(0, _display.DisplayWidth - 10 - 15);
            _display.Write(("Respond" + ((index == 1) ? " <" : "")).PadLeft(padWidth), 15, maxYPos);

            var keyinfo = _display.ReadKey();

            if (keyinfo.Key == ConsoleKey.LeftArrow && index > 0) index--;
            else if (keyinfo.Key == ConsoleKey.RightArrow && index < 1) index++;

            if (index == 0 || keyinfo.Key != ConsoleKey.Enter) 
            {
                if (index == 0 && keyinfo.Key == ConsoleKey.Enter) break;
                continue;
            }

            // INNER LOOP
            do
            {
                for(int i = maxYPos - answerCount - 1; i < _display.DisplayHeight - 2; i++) 
                    _display.Write("", 0, i);
                
                _display.WriteScrollable(_convo, 3 + answerCount);
                _display.Write(((index == 0) ? "> " : "") + "Back to Menu".PadRight(15), 0, maxYPos, false);

                if (_input != "")
                {
                    string response = answers[currentQ][0].Replace("___", _input) + " <";
                    int maxLength = _display.DisplayWidth - 10 - ((answerCount - currentA) == 0 ? 15 : 0);
                    
                    if (response.Length < maxLength - ((answerCount - currentA) == 0 ? 2 : -1)) 
                        _display.Write(response.PadLeft(maxLength), (answerCount - currentA) == 0 ? 15 : 0, maxYPos - (answerCount - currentA));
                    else 
                    {
                        // FIX: Changed -5 to -15 to account for X-offset + padding
                        int overflowPad = Math.Max(0, _display.DisplayWidth - 10 - 15);
                        _display.Write("... < ".PadLeft(overflowPad), 15, maxYPos - (answerCount - currentA));
                    }
                }
                else
                {
                    for (int i = 0; i <= answerCount; i++)
                    {
                        string ans = answers[currentQ][i].Replace("{childName}", _childName).Replace("{studentName}", _studentName);
                        _display.Write((ans + (index == 1 && currentA == i ? " <" : "")).PadLeft(Math.Max(0, _display.DisplayWidth - 10 - 15)), 15, maxYPos - (answerCount - i));
                    }
                }

                keyinfo = _display.ReadKey();

                if (answerCount == 0 && (keyinfo.Key == ConsoleKey.UpArrow || keyinfo.Key == ConsoleKey.DownArrow)) continue;

                if (index == 1 && keyinfo.Key == ConsoleKey.UpArrow && currentA > 0) currentA--;
                else if (index == 1 && keyinfo.Key == ConsoleKey.DownArrow && currentA < answerCount) currentA++;
                else if (keyinfo.Key == ConsoleKey.LeftArrow && index > 0) index--;
                else if (keyinfo.Key == ConsoleKey.RightArrow && index < 1) index++;

                if (keyinfo.Key == ConsoleKey.LeftArrow || keyinfo.Key == ConsoleKey.RightArrow)
                {
                    if (index == 0) break; 
                    continue; 
                }

                if (index == 1)
                {
                    if (keyinfo.Key != ConsoleKey.Enter)
                    {
                        if (keyinfo.Key == ConsoleKey.RightArrow) continue;
                        
                        List<ConsoleKey> acceptedControlKeys = new List<ConsoleKey> { ConsoleKey.Backspace, ConsoleKey.Enter, ConsoleKey.Spacebar };
                        
                        bool isAlphaQ = (currentQ == 0 || currentQ == 2 || currentQ == 5);
                        bool isDigitQ = (currentQ == 3 || currentQ == 6);

                        if (isAlphaQ && currentA != 0) { }
                        else if (isDigitQ && currentA != 0) { }
                        else 
                        {
                            if (keyinfo.Key == ConsoleKey.Backspace)
                            {
                                if (_input.Length > 0) _input = _input[..^1];
                            }
                            else if (!char.IsControl(keyinfo.KeyChar))
                            {
                                if (isAlphaQ && !char.IsLetter(keyinfo.KeyChar) && keyinfo.Key != ConsoleKey.Spacebar) {} 
                                else if (isDigitQ && !char.IsDigit(keyinfo.KeyChar)) {}
                                else 
                                {
                                    if (isDigitQ && _input.Length >= 2) {} 
                                    else _input += keyinfo.KeyChar;
                                }
                            }
                        }
                        continue;
                    }
                    else 
                    {
                        bool endChat = false;
                        string finalAns = answers[currentQ][currentA].Replace("___", _input).Replace("{childName}", _childName).Replace("{studentName}", _studentName);
                        _convo += (_convo == "" ? "" : "\n\n") + _name + ": " + finalAns;
                        SendText(""); 

                        if (currentQ == 0) { _name = _input == "" ? _name : _input; currentQ = 1; }
                        else if (currentQ == 1) currentQ = currentA == 0 ? 5 : 2;
                        else if (currentQ == 2) {
                            if (currentA == 0) {
                                _studentName = _input == "" ? _studentName : _input;
                                if (_roster.GetStudent(_studentName, out var s) && s!=null) {
                                    currentQ = 8; questions[8] = questions[8].Replace("{studentName}", _studentName).Replace("{yearGroup}", s.Form.ToString());
                                    answers[8][0] = answers[8][0].Replace("{studentName}", _studentName);
                                } else {
                                    _convo += "\n\nHead Teacher: I couldn't find them."; SendText("");
                                    currentA = 0; _input = ""; continue;
                                }
                            } else currentQ = 3;
                        }
                        else if (currentQ == 3) {
                            if (currentA == 0) {
                                if (_input == "") { currentA = 0; _input = ""; continue; }
                                int y = int.Parse(_input);
                                var l = string.Join(", ", _roster.GetStudentsByForm(y).Select(s=>s.FirstName));
                                if(l=="") l="None.";
                                currentQ = 11; questions[11] = questions[11].Replace("{yearGroup}", y.ToString()).Replace("{listOfNames}", "- " + l);
                            } else {
                                var l = string.Join("\n", _roster.GetAllStudentsSorted().Select(s => $"- {s.FirstName} (Yr {s.Form})"));
                                if(l=="") l="None.";
                                currentQ = 12; questions[12] = questions[12].Replace("{listOfNames}", l);
                            }
                        }
                        else if (currentQ == 5) { _childName = _input == "" ? _childName : _input; currentQ = 6; answers[6][0] = answers[6][0].Replace("{childName}", _childName); }
                        else if (currentQ == 6) {
                             if (_input == "") { currentA = 0; _input = ""; continue; }
                             _roster.AddStudent(_childName, int.Parse(_input));
                             currentQ = 7;
                        }
                        else if (new[] {4,7,8,9,10,11,12}.Contains(currentQ)) {
                            if (currentA == 0) currentQ = 1; else endChat = true;
                        }

                        if (endChat) 
                        {
                            Thread.Sleep(1000); 
                            return Task.CompletedTask;
                        }

                        nextQ = questions[currentQ].Replace("{name}", _name).Replace("{childName}", _childName).Replace("{studentName}", _studentName);
                        currentA = 0; _input = "";
                        break; 
                    }
                }
            } while (keyinfo.Key != ConsoleKey.Enter);

        } while (index == 1 || index == 0); 

        return Task.CompletedTask;
    }

    private void SendText(string text, int marginBottom = 3)
    {
        if (text != "") _convo += (_convo == "" ? "" : "\n\n") + text;
        _display.WriteScrollable(_convo, marginBottom);
    }
}