using UnityEngine;
using System.Collections.Generic;
using Doublsb.Dialog;
using UnityEngine.UI;

public class StoryManagerScript : MonoBehaviour
{
    // #TODO: PlayerPrefs
    public DialogManager DialogManager;
    public GameObject[] Images;
    public GameObject[] Trackers;
    private string exhibit_ch = "Exhibit";
    public bool dialogue_ongoing;
    public Dictionary<int, bool> storyData;
    public Dictionary<int, string> exhibitList;

    public int coins; //Current coins collected in the stage
    public Text coinsText;

    private void Start()
    {

        coins = 0;
        coinsText.text = "Coins: " + coins.ToString();
        dialogue_ongoing = false;
        storyData = new Dictionary<int, bool>();
        exhibitList = new Dictionary<int, string>();

        for (int i = 0; i < 4; i++)
        {
            storyData.Add(i, false);
        }

        exhibitList.Add(0, "Makhnu");
        exhibitList.Add(1, "Queen Victoria");
        exhibitList.Add(2, "Bahadur Shah Zafar");
        exhibitList.Add(3, "<ENTER NAME HERE>");
    }

    private void Show_Image(int index)
    {
        for (int i = 0; i < Images.Length; i++)
        {
            if (i == index)
            {
                Images[i].SetActive(true);
            }
            else
            {
                Images[i].SetActive(false);
            }
        }
    }

    private void Hide_All_Images()
    {
        for (int i = 0; i < Images.Length; i++)
        {
            Images[i].SetActive(false);
        }
    }

    private void Unlock_Exhibit(int index)
    {
        Trackers[index].transform.GetChild(0).gameObject.SetActive(false); // lock set false, LOCK MUST BE FIRST CHILD
        Trackers[index].transform.GetChild(1).gameObject.SetActive(true); // model set active
    }

    private void generateDialog(int index, List<DialogData> dialogTexts, int top, RectTransform rt, string ch, string question, string[] answers, int correct_choice_index, string correct_response, string wrong_response)
    {
        if (index != 0)
            Debug.Log("In Generate Dialog: " + storyData[index - 1]);
        // if (index != 0 && !storyData[index - 1]) //TODO: Do not understand Unlock_Exhibit
        // {
        //     //Will not let the user proceed until previous exhibit is finished.

        //     // Get name of previous exhibit from the list of exhibits
        //     // string exhibit_name = exhibit_list[index-1];

        //     // Makhnu quote: "Visit the {model} exhibit before you can visit this one"
        //     // string quote = "Visit the" +  {model} exhibit before you can visit this one"


        //     return;
        // }

        if (dialogue_ongoing)
            return;

        dialogue_ongoing = true;

        // If exhibit has been finished before, tell user to move to next exhibit
        if (storyData[index] && index != 3) //To not check on last exhibit
        {
            var returnText = new List<DialogData>();
            returnText.Add(new DialogData("Hi again! you need to look for " + exhibitList[index + 1] + " now", ch, () =>
            {
                dialogue_ongoing = false;
            }));
            DialogManager.Show(returnText);
            return;
        }

        // If exhibit has been finished before, tell user to move to next exhibit
        if (storyData[index] && index != 3) //To not check on last exhibit
        {
            var returnText = new List<DialogData>();
            returnText.Add(new DialogData("Hi again! you need to look for " + exhibitList[index + 1] + " now", ch, () =>
            {
                dialogue_ongoing = false;
            }));
            DialogManager.Show(returnText);
            return;
        }

        // If user has failed current exhibit, then storyData[index-1] will be false. Use this to send user back to previous exhibit
        if (index != 0 && !storyData[index - 1])
        {
            var returnText = new List<DialogData>();
            returnText.Add(new DialogData("You have to meet " + exhibitList[index - 1] + " again before I will speak with you.", ch, () =>
            {
                dialogue_ongoing = false;
            }));
            DialogManager.Show(returnText);
            return;
        }

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
                coins += 50;
                coinsText.text = "Coins: " + coins.ToString();

                // Only unlock exhibit if not last exhibit
                if (index != 2) // FIXME: change length
                {
                    Unlock_Exhibit(index + 1);
                }

                storyData[index] = true;
                var answerDiag = new List<DialogData>();
                answerDiag.Add(new DialogData(correct_response, exhibit_ch));
                DialogManager.Show(answerDiag);
            }
            else if (DialogManager.Result == "Wrong")
            {
                // Send to last exhibit but only if user not at first exhibit
                if (index != 0)
                {
                    storyData[index - 1] = false;
                }
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
        int id = 0;
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
            rt.offsetMax = new Vector2(rt.offsetMax.x, top);
            Hide_All_Images();
        }));

        generateDialog(id, dialogTexts, top, rt, ch, question, answers, correct_choice_index, correct_response, wrong_response);
    }


    public void QueenVictoria()
    {
        int id = 1;
        string ch = "Exhibit";
        string question = "What is 2 times 5?";
        string[] answers = { "10", "7", "5 * 2", "Why should I care?" };
        int correct_choice_index = 0;
        string correct_response = "You are right.";
        string wrong_response = "You are wrong. You need to revisit " + exhibitList[id - 1] + " now.";
        int top = 130;
        RectTransform rt = DialogManager.Printer.GetComponent<RectTransform>();
        var dialogTexts = new List<DialogData>();

        dialogTexts.Add(new DialogData("/size:up/Hi, /size:init/my name is Queen Victoria!", ch));
        dialogTexts.Add(new DialogData("You don't even need to click on the window like this.../speed:0.1/ tada!/close/", ch, () =>
        {
            rt.offsetMax = new Vector2(rt.offsetMax.x, top);
            Hide_All_Images();
        }));

        generateDialog(id, dialogTexts, top, rt, ch, question, answers, correct_choice_index, correct_response, wrong_response);
    }


    public void BahadurShahZafar()
    {
        int id = 2;
        string ch = "Exhibit";
        string question = "What is 2 times 5?";
        string[] answers = { "10", "7", "5 * 2", "Why should I care?" };
        int correct_choice_index = 0;
        string correct_response = "You are right.";
        string wrong_response = "You are wrong. You need to revisit " + exhibitList[id - 1] + " now.";
        int top = 130;

        RectTransform rt = DialogManager.Printer.GetComponent<RectTransform>();
        var dialogTexts = new List<DialogData>();

        dialogTexts.Add(new DialogData("/size:up/Hi, /size:init/my name is Bahadur Shah Zafar!", ch));
        dialogTexts.Add(new DialogData("You don't even need to click on the window like this.../speed:0.1/ tada!/close/", ch, () =>
        {
            rt.offsetMax = new Vector2(rt.offsetMax.x, top);
            Hide_All_Images();
        }));

        generateDialog(id, dialogTexts, top, rt, ch, question, answers, correct_choice_index, correct_response, wrong_response);
    }

}
