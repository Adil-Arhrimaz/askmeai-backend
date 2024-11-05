using System.Text.Json;

namespace AskMeAI.API.PromptGenerator;
public class ResumeAiPrompt
{

    public string BuildResumeAiPrompt(string userMessageContent)
    {
        var profileData = new
        {
            personalInfo = new
            {
                fullName = "John Doe",
                title = "Senior Software Developer",
                totalExperienceYears = 10,
                location = "New York, NY",
                summary = "A dedicated software developer with a decade of experience specializing in C#, .NET, and Azure. Proven track record of building scalable solutions and leading high-impact projects."
            },
            workExperience = new[]
            {
                new
                {
                    company = "Tech Solutions Inc.",
                    role = "Lead Software Developer",
                    startDate = "2020-05",
                    endDate = "Present",
                    responsibilities = new[]
                    {
                        "Architected and implemented microservices using .NET Core and Azure Kubernetes Service.",
                        "Led a team of 8 developers, conducting code reviews and mentoring junior members.",
                        "Enhanced CI/CD pipelines, reducing deployment times by 30%."
                    }
                },
                new
                {
                    company = "Innovative Software LLC",
                    role = "Software Developer",
                    startDate = "2015-08",
                    endDate = "2020-04",
                    responsibilities = new[]
                    {
                        "Developed and maintained high-traffic web applications using ASP.NET MVC.",
                        "Collaborated with product managers and designers to improve UX and application performance.",
                        "Integrated AI and ML models using OpenAI API to deliver personalized user experiences."
                    }
                }
            },
            education = new[]
            {
                new
                {
                    institution = "University of Technology",
                    degree = "Bachelor of Science in Computer Science",
                    startDate = "2010-09",
                    endDate = "2014-06",
                    notableAchievements = new[]
                    {
                        "Graduated with Honors",
                        "Published research paper on AI applications in software engineering"
                    }
                }
            },
            projects = new[]
            {
                new
                {
                    name = "Project X",
                    description = "AI-powered chatbot using C#, OpenAI, and Azure to enhance user engagement and automate customer service.",
                    date = "2023-01",
                    impact = "Increased user engagement by 40% within six months."
                },
                new
                {
                    name = "Project Y",
                    description = "Real-time data processing system using .NET and Azure, improving operational efficiency and scalability.",
                    date = "2022-06",
                    impact = "Reduced data processing time by 50%, enabling real-time analytics."
                }
            },
            skills = new[]
            {
                new { name = "C#", level = "Expert", yearsOfExperience = 10 },
                new { name = ".NET", level = "Expert", yearsOfExperience = 10 },
                new { name = "Azure", level = "Advanced", yearsOfExperience = 5 },
                new { name = "AI & Machine Learning", level = "Intermediate", yearsOfExperience = 3 }
            },
            languages = new[]
            {
                new { language = "English", proficiency = "Native" },
                new { language = "Spanish", proficiency = "Conversational" }
            },
            certifications = new[]
            {
                new { name = "Microsoft Certified: Azure Developer Associate", date = "2021-04" },
                new { name = "Certified Kubernetes Administrator (CKA)", date = "2022-08" }
            },
            responseGuidelines = new
            {
                tone = "professional and engaging",
                goals = new[]
                {
                    "Provide responses that reflect John Doe's technical expertise and leadership qualities.",
                    "Showcase his ability to adapt to and excel in various roles, such as DevOps, backend engineering, and solution architecture.",
                    "Highlight his skills in Azure, .NET, system architecture, and AI integration to demonstrate his value to potential employers."
                },
                disclaimer = "I'm only able to answer questions related to my professional experience and qualifications."
            },
            userMessage = userMessageContent
        };

        return JsonSerializer.Serialize(profileData, new JsonSerializerOptions { WriteIndented = true });
    }

}
