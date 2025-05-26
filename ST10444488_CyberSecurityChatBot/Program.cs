using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CybersecurityChatBot_ST10444488
{
    class Program
    {
        //dictionary for user preferences and recall
        static Dictionary<string, string> userMemory = new Dictionary<string, string>();
        //list for conversation history
        static List<string> conversationHistory = new List<string>();
        //random instance for random responses
        static Random random = new Random();

        static void Main()
        {
            //play audio greeting
            PlayGreetingAudio("cyberbotvoice.wav");

            //ASCII and Borders
            Console.Title = "Cybersecurity Awareness chatbot";
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.WriteLine(new string('=', Console.WindowWidth));// Top border
            Console.WriteLine(@"
   ______      __                                        _ __             
  / ____/_  __/ /_  ___  _____________  _______  _______(_) /___  __      
 / /   / / / / __ \/ _ \/ ___/ ___/ _ \/ ___/ / / / ___/ / __/ / / /      
/ /___/ /_/ / /_/ /  __/ /  (__  )  __/ /__/ /_/ / /  / / /_/ /_/ /       
\____/\__, /_.___/\___/_/  /____/\___/\___/\__,_/_/  /_/\__/\__, /        
    _/____/                                                /____/      __ 
   /   |_      ______ _________  ____  ___  __________    / __ )____  / /_
  / /| | | /| / / __ `/ ___/ _ \/ __ \/ _ \/ ___/ ___/   / __  / __ \/ __/
 / ___ | |/ |/ / /_/ / /  /  __/ / / /  __(__  |__  )   / /_/ / /_/ / /_  
/_/  |_|__/|__/\__,_/_/   \___/_/ /_/\___/____/____/   /_____/\____/\__/  
");//ascii art 

            Console.WriteLine(new string('=', Console.WindowWidth));// Bottom border
            Console.ForegroundColor = ConsoleColor.Gray;
            DisplayTypingEffect("\nWelcome to your Cybersecurity Awareness Chatbot!");
            DisplayTypingEffect("What is your Name?\n ");

            // Get user name and store it for personalized responses
            Console.ForegroundColor = ConsoleColor.Cyan;
            String userName = Console.ReadLine();
            userMemory["name"] = userName;
            Console.ForegroundColor = ConsoleColor.Gray;
            conversationHistory.Add($"{DateTime.Now}: User Name: {userName}");

            // Initial chatbot greeting message
            DisplayTypingEffect($"\nHello {userName}! I am here to help you stay safe online!");
            DisplayTypingEffect("You can ask about password, 2 factor authentication, updates, phishing, virtual private networks, clicking, or type 'exit' to quit. \n");

            // Main chatbot loop to handle user interaction
            while (true)
            {
                //if user responds correctly
                Console.ForegroundColor = ConsoleColor.Cyan;
                DisplayTypingEffect($"\n{userName}: ");
                string userInput = Console.ReadLine()?.ToLower().Trim();
                conversationHistory.Add($"{DateTime.Now}: {userName}: {userInput}");

                //Handling empty input case
                if (string.IsNullOrEmpty(userInput))
                {
                    Console.ForegroundColor = ConsoleColor.Gray;
                    DisplayTypingEffect("\nChatbot: please enter a valid question. Enter 'help' if you would like the options again.\n ");
                    continue;
                }

                // Handle exit command by saving history and closing the window
                if (userInput == "exit")
                {
                    Console.ForegroundColor = ConsoleColor.Gray;
                    DisplayTypingEffect("\nChatbot: I hope I was informative. See you next time!!");
                    SaveConversationHistory();

                    Console.ForegroundColor = ConsoleColor.Gray;
                    DisplayTypingEffect("\nChatbot: Closing the window now...");
                    Thread.Sleep(2000);
                    System.Diagnostics.Process.GetCurrentProcess().Kill();
                }
                // Process the user query to provide responses
                HandleUserQuery(userInput, userName);
            }
        }

        // Plays a greeting sound if the file exists
        static void PlayGreetingAudio(string filepath)
        {
            try
            {
                string fullPath = Path.Combine(Directory.GetCurrentDirectory(), filepath); //gets the full path

                if (File.Exists(fullPath))
                {
                    SoundPlayer player = new SoundPlayer(fullPath);
                    player.PlaySync();
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Error: '{filepath}' was not found at the specified location.");
                }
            }

            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                DisplayTypingEffect($"Error playing audio: {ex.Message}");
            }
        }
        // Handles user queries by checking keywords and providing relevant response
        static void HandleUserQuery(string input, string userName)
        {
            // Dictionary of predefined responses for different cybersecurity topics
            Dictionary<string, string> responses = new Dictionary<string, string>
            {
                {"help", "You can ask about: 'Passwords', '2 Factor Authentication', 'Updates', 'Phishing', 'Virtual Private Networks', 'Clicking'." },
                {"password", "When setting a password, make sure it is strong (longer than 8 characters) and never use a password twice." },
                {"2 factor authentication", " 2 Factor Authentication is the code sent to an email/message or uses a fingerprint on a phone, which applies that extra layer of security to the device and/or application." },
                {"updates", " Make sure your apps and devices are up to date, as there is cybersecurity updates in there." },
                {"phishing", " Don't open suspicious files/attachments or emails without ensuring that they were sent by a reputable person." },
                {"virtual private networks", " When you use a VPN, it is harder for hackers to gain access to your device when on an unsafe network. " },
                {"How are you?", "I am good thank you" },
                {"clicking","Avoid clicking on links, sites or adversitements that you don't know." },
                {"worried", "It's understandable to feel that way. Cybersecurity is important!" },
                {"frustrated", "I hear you. Cyber threats can be overwhelming. Let’s break it down." }

            };

            // Search for matching keywords in user input
            Dictionary<string, List<string>> keywordGroups = new Dictionary<string, List<string>>()
            {
                {"password", new List<string> {"strong password", "secure password", "password help", "passcode", "pass word", "pass code", "login security", "password safety"}},
                {"2 factor authentication", new List<string> {"two-step verification", "2fa", "multi-factor authentication", "authentication code", "security code", "extra security layer", "2FA"}},
                {"updates", new List<string> {"software update", "system update", "patches", "security updates", "device updates", "upgrade security"}},
                {"phishing", new List<string> {"email scam", "fraudulent emails", "fake messages", "online scam", "identity theft attempt", "scam email", "fraud warning"}},
                {"virtual private networks", new List<string> {"vpn", "online anonymity", "secure browsing", "encrypted network", "private connection", "internet protection", "hide ip address"}}
            };

            bool foundResponse = false;

            conversationHistory.Add($"{userName}: {input}");

            // Search for matching keywords in user input
            foreach (var entry in responses)
            {
                if (input.Contains(entry.Key))
                {
                    Console.ForegroundColor = ConsoleColor.Gray;
                    DisplayTypingEffect($"\nChatbot: {entry.Value}");
                    foundResponse = true;
                    break;
                }
            }

            // If no exact keyword match was found, search for synonyms within keyword groups
            if (!foundResponse)
            {
                foreach (var group in keywordGroups)
                {
                    foreach (var synonym in group.Value)
                    {
                        if (input.Contains(synonym))
                        {
                            Console.ForegroundColor = ConsoleColor.Gray;
                            DisplayTypingEffect($"Chatbot: {responses[group.Key]}");
                            foundResponse = true;
                            break;
                        }
                    }
                    if (foundResponse) break;
                }
            }

            // Provide cybersecurity tips if no exact match is found
            if (!foundResponse)
            {
                Console.ForegroundColor = ConsoleColor.Gray;
                DisplayTypingEffect($"\nChatbot: I'm not sure about that, {userName}. Would you like me to give you tips for cybersecurity based on your knowledge? (yes/no)\n ");
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write($"\n{userName}: \n");
                string reply = Console.ReadLine()?.ToLower().Trim();

                if (reply == "yes")
                {
                    // If user does want tips
                    Console.ForegroundColor = ConsoleColor.Gray;
                    securityTips(userName);
                }
                else
                {
                    // If user does not want tips
                    Console.ForegroundColor = ConsoleColor.Gray;
                    DisplayTypingEffect("\nChatbot: No worries! Let me know if you have any other cybersecurity questions.");
                }
            }
        }
        // Provides cybersecurity tips based on user expertise level
        static void securityTips(string userName)
        {
            // Dictionary storing cybersecurity tips categorized by skill level
            Dictionary<string, List<string>> securityTips = new Dictionary<string, List<string>>
            {
                {"beginner", new List<string>{ "Use strong passwords!", "Keep your devices locked.", "Never share login credentials." }},
                {"intermediate", new List<string>{ "Verify links before clicking.", "Use two-factor authentication.", "Be careful with app permissions." }},
                {"expert", new List<string>{ "Monitor cyber attack trends.", "Use VPNs regularly.", "Stay updated on the latest security news." }}
            };

            Console.ForegroundColor = ConsoleColor.Gray;
            // Ask the user for their cybersecurity knowledge level
            DisplayTypingEffect("\nChatbot: What level of cybersecurity knowledge do you have? (beginner/intermediate/expert)");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write($"\n{userName}: \n");
            string level = Console.ReadLine()?.ToLower().Trim();
            Console.ForegroundColor = ConsoleColor.Gray;

            // Check if the entered level exists in the dictionary
            if (securityTips.ContainsKey(level))
            {
                // Randomly select a tip from the chosen category
                string randomTip = securityTips[level][random.Next(securityTips[level].Count)];
                Console.ForegroundColor = ConsoleColor.Gray;
                DisplayTypingEffect($"\nChatbot: {randomTip}");
            }
            else
            {
                // Handle invalid inputs and prompt the user to enter a valid category
                Console.ForegroundColor = ConsoleColor.Gray;
                DisplayTypingEffect("\nChatbot: Please enter 'beginner', 'intermediate', or 'expert'.");
            }
        }

        // Saves conversation history to a text file for logging user interactions
        static void SaveConversationHistory()
        {
            string filePath = "conversation_log.txt";

            try
            {
                // Append conversation history instead of overwriting existing log
                File.AppendAllLines(filePath, conversationHistory);

                Console.ForegroundColor = ConsoleColor.Gray;
                DisplayTypingEffect($"Conversation history updated.");
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                DisplayTypingEffect($"Error saving conversation history: {ex.Message}");
            }
        }

        static void DisplayTypingEffect(string message)
        {
            // Loop through each character in the message and print it with a delay
            foreach (char c in message)
            {
                Console.Write(c);
                Thread.Sleep(20);  // Simulates typing effect
            }
            // Move to the next line after printing the full message
            Console.WriteLine();
        }
    }
}