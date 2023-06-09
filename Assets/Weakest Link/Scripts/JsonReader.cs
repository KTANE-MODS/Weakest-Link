﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Newtonsoft.Json;

public class JsonReader : MonoBehaviour
{

    public static List<Trivia> TriviaList { get; private set; }
    public static List<string> ContestantNames { get; private set; }

    public static bool Success = false;
    public static bool Loading = false;
    public static bool LoadingDone = false;


    //object used in order to read data from json

    //holds the data read from the json
    [System.Serializable]
    public class JsonData
    {
        public List<string> CharacterNames; //list of all the names of the contestants
        public List<JsonTrivia> QuizBank; //list of all the questions and its respective data
    }

    [System.Serializable]
    public class JsonTrivia
    {
        public string Question; //the question being asked
        public List<string> Answers;  //strings that will pass the questions
        public List<string> WrongAnswers;  //strings that the cpu will use the when they need to get the question wrong
        public string Category;  //They category of the question

        public JsonTrivia(string Question, List<string> Answers, List<string> WrongAnswers, string Category)
        {
            this.Question = Question;
            this.Answers = Answers;
            this.WrongAnswers = WrongAnswers;
            this.Category = Category;
        }
    }

    //Loads the data from the json
    public IEnumerator LoadData(string url)
    {
        Loading = true;

        //Stores the raw text of the grabbed json.
        string dataString;

        WWW request = new WWW(url);
        //Waits until the WWW request returns the JSON file.
        yield return request;

        //If an error occurs, we need to default to the hardcoded file.
        if (request.error != null)
        {
            Success = false;
            Debug.Log("Failed to get data!");

        }

        else
        {
            Debug.Log("Gotten info!");
            dataString = request.text;
            Success = true;

            JsonData deserial = JsonConvert.DeserializeObject<JsonData>(dataString);

            List<Trivia> tempList = deserial.QuizBank.Select(t => ConvertJsonToTrivia(t)).ToList();

            TriviaList = new List<Trivia>();

            //idk why but quesetions are duplicated if this isn't here
            tempList.ForEach(x => { if (FoundQuestion(x.Question)) { TriviaList.Add(x); } } );

            ContestantNames = deserial.CharacterNames;

            deserial.QuizBank.ForEach(t => TriviaList.Add(ConvertJsonToTrivia(t)));
        }

        Loading = false;
        LoadingDone = true;
    }

    private bool FoundQuestion(string question)
    {
        foreach(Trivia t in TriviaList)
        {
            if (t.Question == question)
            {
                return true;
            }
        }

        return false;
    }


    private Trivia ConvertJsonToTrivia(JsonTrivia j)
    {
        Category category;

        switch (j.Category.ToUpper())
        {
            case "KTANE":
                category = Category.KTANE;
                break;

            case "GEOGRAPHY":
                category = Category.Geography;
                break;

            case "LANGUAGE":
                category = Category.Language;
                break;

            case "WILDLIFE":
                category = Category.Wildlife;
                break;

            case "BIOLOGY":
                category = Category.Biology;
                break;

            case "HISTORY":
                category = Category.History;
                break;

            case "MATHS":
                category = Category.Maths;
                break;

            default:
                category = Category.Other;
                break;
        }

        return new Trivia(j.Question, j.Answers, j.WrongAnswers, category);
    }
}
