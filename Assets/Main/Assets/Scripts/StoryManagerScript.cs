using UnityEngine;
using System.Collections.Generic;
using Doublsb.Dialog;

public class StoryManagerScript : MonoBehaviour
{
    // #TODO: PlayerPrefs
    public DialogManager DialogManager;
    public GameObject[] Image;
    private string exhibit_ch = "Exhibit";
    public bool dialogue_ongoing;
    public Dictionary<int, bool> storyData;
    public Dictionary<int, string> exhibitList;

    // private void Awake()
    // {
    //     // switch()
    //     // Debug.Log(dialogue_ongoing);
    // }

    //PlayerPrefs coins

    private void Start()
    {
        dialogue_ongoing = false;
        storyData = new Dictionary<int, bool>();
        exhibitList = new Dictionary<int, string>();

        for (int i = 0; i < 4; i++)
        {
            storyData.Add(i, false);
        }

        exhibitList.Add(0, "Makhnu");
        exhibitList.Add(1, "Queen Victoria");
        exhibitList.Add(2, "<ENTER NAME HERE>");
        exhibitList.Add(3, "<ENTER NAME HERE>");
    }

    private void Show_Image(int index)
    {
        Image[index].SetActive(true);
    }

    private void generateDialog(int index, List<DialogData> dialogTexts, int top, RectTransform rt, string ch, string question, string[] answers, int correct_choice_index, string correct_response, string wrong_response)
    {
        if (index != 0)
            Debug.Log("In Generate Dialog: " + storyData[index - 1]);
        if (index != 0 && !storyData[index - 1])
        {
            //Will not let the user proceed until previous exhibit is finished.

            // Get name of previous exhibit from the list of exhibits
            // string exhibit_name = exhibit_list[index-1];

            // Makhnu quote: "Visit the {model} exhibit before you can visit this one"
            // string quote = "Visit the" +  {model} exhibit before you can visit this one"


            return;
        }

        if (dialogue_ongoing)
            return;

        dialogue_ongoing = true;


        // var QuestionOnly = new DialogData(question, exhibit_ch);
        // dialogTexts.Add(QuestionOnly);

        var Quiz = new DialogData(question, exhibit_ch);
        for (int i = 0; i < 4; i++)
        {
            if (i == correct_choice_index)
            {
                Quiz.SelectList.Add("Correct", answers[i]);
            }
            else
            {
                Quiz.SelectList.Add("Wrong", answers[i]);
            }
        }

        Quiz.Callback = () =>
        {
            //TODO: Sleep
            rt.offsetMax = new Vector2(rt.offsetMax.x, -top); //revert top

            if (DialogManager.Result == "Correct")
            {
                storyData[index] = true;
                var answerDiag = new List<DialogData>();
                answerDiag.Add(new DialogData(correct_response, exhibit_ch));
                DialogManager.Show(answerDiag);
            }
            else if (DialogManager.Result == "Wrong")
            {
                var answerDiag = new List<DialogData>();
                answerDiag.Add(new DialogData(wrong_response, exhibit_ch));
                DialogManager.Show(answerDiag);
            }
            dialogue_ongoing = false;
        };

        dialogTexts.Add(Quiz);

        DialogManager.Show(dialogTexts);
    }


    public void Makhnu1()
    {
        string ch = "Makhnu";
        string question = "What is 2 times 5?";
        string[] answers = { "10", "7", "5 * 2", "Why should I care?" };
        int correct_choice_index = 0;
        string correct_response = "You are right.";
        string wrong_response = "You are wrong.";
        int top = 130;
        RectTransform rt = DialogManager.Printer.GetComponent<RectTransform>();
        var dialogTexts = new List<DialogData>();

        dialogTexts.Add(new DialogData("/size:up/Hi, /size:init/my name is Makhnu!", ch));
        dialogTexts.Add(new DialogData("You can easily change text /color:red/color, /color:white/and /size:up//size:up/size/size:init/ like this.", ch, () => Show_Image(0)));
        dialogTexts.Add(new DialogData("Just put the command in the string!", ch, () => Show_Image(1)));
        dialogTexts.Add(new DialogData("You can also change the chacter's sprite /emote:Sad/like this, /click//emote:Happy/Smile.", ch, () => Show_Image(2)));
        dialogTexts.Add(new DialogData("If you need an emphasis effect, /wait:0.5/wait... /click/or click command.", ch, () => Show_Image(3)));
        dialogTexts.Add(new DialogData("Text can be /speed:down/slow... /speed:init//speed:up/or fast.", ch, () => Show_Image(4)));
        dialogTexts.Add(new DialogData("You don't even need to click on the window like this.../speed:0.1/ tada!/close/", ch, () =>
        {
            // TODO: Add timeout for last dialog
            rt.offsetMax = new Vector2(rt.offsetMax.x, top);
        }));

        generateDialog(0, dialogTexts, top, rt, ch, question, answers, correct_choice_index, correct_response, wrong_response);
    }


    public void QueenVictoria()
    {
        string ch = "Exhibit";
        string question = "What is 2 times 5?";
        string[] answers = { "10", "7", "5 * 2", "Why should I care?" };
        int correct_choice_index = 0;
        string correct_response = "You are right.";
        string wrong_response = "You are wrong.";
        int top = 130;
        RectTransform rt = DialogManager.Printer.GetComponent<RectTransform>();
        var dialogTexts = new List<DialogData>();

        dialogTexts.Add(new DialogData("/size:up/Hi, /size:init/my name is Queen Victoria!", ch));
        dialogTexts.Add(new DialogData("You don't even need to click on the window like this.../speed:0.1/ tada!/close/", ch, () =>
        {
            rt.offsetMax = new Vector2(rt.offsetMax.x, top);
        }));

        generateDialog(1, dialogTexts, top, rt, ch, question, answers, correct_choice_index, correct_response, wrong_response);
    }


    public void BahadurShahZafar()
    {
        string ch = "Exhibit";
        string question = "What is 2 times 5?";
        string[] answers = { "10", "7", "5 * 2", "Why should I care?" };
        int correct_choice_index = 0;
        string correct_response = "You are right.";
        string wrong_response = "You are wrong.";
        int top = 130;

        RectTransform rt = DialogManager.Printer.GetComponent<RectTransform>();
        var dialogTexts = new List<DialogData>();

        dialogTexts.Add(new DialogData("/size:up/Hi, /size:init/my name is Bahadur Shah Zafar!", ch));
        dialogTexts.Add(new DialogData("You don't even need to click on the window like this.../speed:0.1/ tada!/close/", ch, () =>
        {
            rt.offsetMax = new Vector2(rt.offsetMax.x, top);
        }));

        generateDialog(2, dialogTexts, top, rt, ch, question, answers, correct_choice_index, correct_response, wrong_response);
    }

}
