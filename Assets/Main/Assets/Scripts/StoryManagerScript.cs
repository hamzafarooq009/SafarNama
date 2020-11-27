using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Doublsb.Dialog;
using UnityEngine.UI;


public class StoryManagerScript : MonoBehaviour
{
    public DialogManager DialogManager;
    public DialogManager FQuizDialogManager;
    public GameObject FQuizGameObject;
    public GameObject ARCam;
    public GameObject QuizBtn;
    public GameObject ARUI;
    public GameObject TourComplete;
    public GameObject Printer;
    public GameObject[] Images;
    public GameObject[] Trackers;
    private string e = "Exhibit";
    private string makhnu = "Makhnu";
    public bool dialogue_ongoing;
    public Dictionary<int, bool> storyData;
    public Dictionary<int, string> exhibitList;

    public int coins; //Current coins collected in the stage
    public Text coinsText;
    public Text finalCoinsText;


    public bool AtFinalQuiz;

    GameObject dialog = null;

    private void Start()
    {
        AtFinalQuiz = false;
        coins = 0;
        coinsText.text = coins.ToString();
        dialogue_ongoing = false;
        storyData = new Dictionary<int, bool>();
        exhibitList = new Dictionary<int, string>();

        for (int i = 0; i < 4; i++)
        {
            storyData.Add(i, false);
        }

        exhibitList.Add(0, "Queen Victoria");
        exhibitList.Add(1, "Tipu Sultan");
        exhibitList.Add(2, "Bahadur Shah Zafar");
        exhibitList.Add(3, "Rani of Jhansi");

        Makhnu();
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

    private void generateDialog(bool takeQuiz, int index, List<DialogData> dialogTexts, int top, RectTransform rt, string e, string question, string[] answers, int correct_choice_index, List<DialogData> correct_responses, List<DialogData> wrong_responses)
    {
        if (dialogue_ongoing)
            return;

        dialogue_ongoing = true;

        // If exhibit has been finished before, tell user to move to next exhibit
        if (!AtFinalQuiz && storyData[index] && index != 3) //To not check on last exhibit
        {
            var returnText = new List<DialogData>();
            returnText.Add(new DialogData("Hi again! you need to look for " + exhibitList[index + 1] + " now.", makhnu, () =>
            {
                dialogue_ongoing = false;
            }));
            DialogManager.Show(returnText);
            return;
        }


        // If user has failed current exhibit, then storyData[index-1] will be false. Use this to send user back to previous exhibit
        if (!AtFinalQuiz && index != 0 && !storyData[index - 1])
        {
            var returnText = new List<DialogData>();
            returnText.Add(new DialogData("/emote:Sad/You have to meet " + exhibitList[index - 1] + " again before " + exhibitList[index] + " will speak with you.", makhnu, () =>
            {
                dialogue_ongoing = false;
            }));
            DialogManager.Show(returnText);
            return;
        }

        var Quiz = new DialogData(question, e);

        if (takeQuiz)
        {
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
            dialogTexts.Add(Quiz);

            Quiz.Callback = () =>
            {
                rt.offsetMax = new Vector2(rt.offsetMax.x, -top); //revert top

                if (DialogManager.Result == "Correct")
                {
                    coins += 50;
                    coinsText.text = coins.ToString();

                    // Only unlock exhibit if not last exhibit
                    if (index != 3)
                    {
                        Unlock_Exhibit(index + 1);
                    }

                    storyData[index] = true;

                    DialogManager.Show(correct_responses);


                    if (index == 3)
                    {
                        AtFinalQuiz = true;
                        QuizBtn.SetActive(true);
                    }
                }
                else if (DialogManager.Result == "Wrong")
                {
                    // Send to last exhibit but only if user not at first exhibit
                    if (index != 0)
                    {
                        storyData[index - 1] = false;
                    }
                    // var answerDiag = new List<DialogData>();
                    // answerDiag.Add(new DialogData(wrong_response, e));
                    DialogManager.Show(wrong_responses);
                }
                dialogue_ongoing = false;
            };
        }

        if (!AtFinalQuiz && !takeQuiz)
        {
            dialogTexts.Add(Quiz);
        }

        // Dialog free on last dialog
        if (AtFinalQuiz)
        {
            int lastIndex = correct_responses.Count - 1;
            correct_responses[lastIndex].Callback = () =>
            {
                dialogue_ongoing = false;
                Hide_All_Images();
            };
            DialogManager.Show(correct_responses);

        }
        else
        {
            DialogManager.Show(dialogTexts);
        }

    }

    public void Makhnu()
    {
        if (dialogue_ongoing)
            return;

        dialogue_ongoing = true;


        RectTransform rt = DialogManager.Printer.GetComponent<RectTransform>();
        var dialogTexts = new List<DialogData>();


        Show_Image(0);
        dialogTexts.Add(new DialogData("The looming shadow of a tall building lies over you...", e));
        dialogTexts.Add(new DialogData("You immediately recognize the place...", e));
        dialogTexts.Add(new DialogData("It is the famous Lahore Museum.", e, () =>
        {
            Show_Image(1);
        }));

        dialogTexts.Add(new DialogData("Hi! I am Makhnu the Markhor! You know me from before.", makhnu));
        dialogTexts.Add(new DialogData("/emote:Happy/ I will be your tour guide!", makhnu, () =>
        {
            Show_Image(2);
        }));
        dialogTexts.Add(new DialogData("Magical exhibits are scattered over all the place.", makhnu));
        dialogTexts.Add(new DialogData("You must find these markers and scan them with your camera.", makhnu));
        dialogTexts.Add(new DialogData("Just tap on the exhibits to listen to what they have to say!", makhnu, () =>
        {
            dialogue_ongoing = false;
            Hide_All_Images();
        }));

        DialogManager.Show(dialogTexts);
    }


    public void QueenVictoria()
    {
        if (dialogue_ongoing)
            return;

        dialogue_ongoing = true;

        int index = 0;

        string[] answers = { "A", "B", "C", "D" };
        var dialogTexts = new List<DialogData>();



        // If exhibit has been finished before, tell user to move to next exhibit
        if (!AtFinalQuiz && storyData[index]) //To not check on last exhibit
        {
            var returnText = new List<DialogData>();
            returnText.Add(new DialogData("Hi again! you need to look for " + exhibitList[index + 1] + " now.", makhnu, () =>
            {
                dialogue_ongoing = false;
            }));
            DialogManager.Show(returnText);
            return;
        }


        dialogTexts.Add(new DialogData("My Lords, Ladies and Gentlemen! You stand in the presence of Alexandrina Victoria,", e));
        dialogTexts.Add(new DialogData("...monarch of Great Britain and Ireland, and Empress of India (after 1876)", e));
        dialogTexts.Add(new DialogData("Let me tell you a story about the 1857 War of Independence.", e));
        dialogTexts.Add(new DialogData("The First War of Indian Independence was a period of rebellions in northern and central India against British power in 1857–1858.", e));
        dialogTexts.Add(new DialogData("Us, the British usually refer to the rebellion of 1857 as the Indian Mutiny or the Sepoy Mutiny.", e));
        dialogTexts.Add(new DialogData("It is widely acknowledged to be the first-ever united rebellion against colonial rule in India.", e));
        dialogTexts.Add(new DialogData("Mangal Pandey, a Sepoy in the colonial British army, was the spearhead of this revolt.", e));
        dialogTexts.Add(new DialogData("The revolt started when Indian soldiers rebelled against their British officers over violation of their religious sensibilities.", e));
        dialogTexts.Add(new DialogData("The uprising grew into a wider rebellion to which the Mughal Emperor, Bahadur Shah, the nominal ruler of India, lent his nominal support.", e));
        dialogTexts.Add(new DialogData("Other main leaders included Rani Lakshmibai of Jhansi and Tatia Tope.", e));
        dialogTexts.Add(new DialogData("Now, to hear the rest of the story you need to go to Tipu Sultan, the Tiger of Mysore, and let him know that the Empress has sent you.", e, () =>
        {
            Unlock_Exhibit(1); // unlock tipu sultan
            storyData[0] = true;
            Hide_All_Images();
            dialogue_ongoing = false;
        }));


        DialogManager.Show(dialogTexts);
    }



    public void TipuSultan()
    {

        int id = 1;
        string question = "Who was the Indian soldier who started the revolt that led to the War of Independence?";
        string[] answers = { "Tatia Tope", "Bahadur Shah", "Mangal Panday", "Tipu Sultan" };
        int correct_choice_index = 2;

        int top = 130;
        RectTransform rt = DialogManager.Printer.GetComponent<RectTransform>();

        var dialogTexts = new List<DialogData>();
        dialogTexts.Add(new DialogData("Salam my young tigers!", e));
        dialogTexts.Add(new DialogData("Before I continue my story of the war, you need to answer a question...", e));
        dialogTexts.Add(new DialogData("...for I need to make sure that it is Queen Victoria who sent you.", e));
        dialogTexts.Add(new DialogData("I have been betrayed by traitors before so you must answer the questions right...", e));
        dialogTexts.Add(new DialogData("...or else I'll have to send you back!", e, () =>
        {
            rt.offsetMax = new Vector2(rt.offsetMax.x, top);
            Hide_All_Images();
        }));


        var correct_responses = new List<DialogData>();
        correct_responses.Add(new DialogData("Very well. Let’s continue the story now.", e));
        correct_responses.Add(new DialogData("So, the British cruelly put down the uprising, slaughtering civilians indiscriminately.", e));
        correct_responses.Add(new DialogData("The result of the uprising was a feeling among the British that they had conquered India and were entitled to rule.", e));
        correct_responses.Add(new DialogData("The Mughal Emperor was banished and Queen Victoria of the United Kingdom was declared sovereign.", e));
        correct_responses.Add(new DialogData("The British East India Company, which had represented the British Government in India and which acted as agent of the Mughals...", e));
        correct_responses.Add(new DialogData("It was closed down and replaced by direct control from London through a Governor-General.", e, () =>
        {
            Show_Image(3);
        }));
        correct_responses.Add(new DialogData("Now to find out more about the role of the Mughal emperor Bahadur Shah in the war, you will have to speak to the emperor himself!", e));
        correct_responses.Add(new DialogData("Let him know that the Tiger of Mysore sent you.", e, () =>
        {
            Hide_All_Images();
        }));


        var wrong_responses = new List<DialogData>();
        wrong_responses.Add(new DialogData("I see that you are an imposter as I suspected.", e));
        wrong_responses.Add(new DialogData("I shall say no more and order you to go back to the previous part of the story!", e));


        generateDialog(true, id, dialogTexts, top, rt, e, question, answers, correct_choice_index, correct_responses, wrong_responses);

    }


    public void BahadurShahZafar()
    {
        int id = 2;
        string question = "Who was declared the sovereign after the War of Independence?";
        string[] answers = { "Bahadur Shah", "Tipu Sultan", "Queen Victoria", "Rani Lakshmibai" };
        int correct_choice_index = 2;
        int top = 130;

        RectTransform rt = DialogManager.Printer.GetComponent<RectTransform>();

        var dialogTexts = new List<DialogData>();
        dialogTexts.Add(new DialogData("Salaam, I am Bahadur Shah Zafar, the last emperor of the Mughal Empire.", e));
        dialogTexts.Add(new DialogData("Let me tell you more about the part I played in the war, but before I begin let me ask you a question to test your knowledge of the war!", e, () =>
        {
            rt.offsetMax = new Vector2(rt.offsetMax.x, top);
            Hide_All_Images();
        }));


        var correct_responses = new List<DialogData>();
        correct_responses.Add(new DialogData("Afreen!", e));
        correct_responses.Add(new DialogData("Alright, so the most significant event taking place during my reign was the uprising of 1857.", e));
        correct_responses.Add(new DialogData("The people of India made a concerted effort to liberate their country from the foreign occupation.", e));
        correct_responses.Add(new DialogData("The uprising sprung from Meerut where sepoy revolted and marched towards Delhi. They declared me as the emperor of India and I accepted their allegiance.", e));
        correct_responses.Add(new DialogData("I nominated my son Mirza Mughal as the commander in chief of the armed forces.", e));
        correct_responses.Add(new DialogData("The situation was highly chaotic but ultimately the revolt was suppressed by the British. ", e));
        correct_responses.Add(new DialogData("I took refuge in the Mughal Emperor Humayun’s tomb from where I was apprehended by Major William Hudson.", e));
        correct_responses.Add(new DialogData("The very next day my sons Mirza Mughal, Mirza Khizzer Sultan and grandson Mirza Abu Bakar were executed.", e));
        correct_responses.Add(new DialogData("I was exiled to Rangoon (in Burma) where I was to spend the rest of my days. This marked the end of the Mughal rule in India.", e, () =>
        {
            Show_Image(4);
        }));
        correct_responses.Add(new DialogData("Now go to Rani Lakshmibai to hear what happened in Jhansi during the war.", e, () =>
        {
            Hide_All_Images();
        }));

        var wrong_responses = new List<DialogData>();
        wrong_responses.Add(new DialogData("I’m afraid your knowledge isn’t good enough for me to continue the story.", e));
        wrong_responses.Add(new DialogData("I command you to return to the previous part of the story!", e));


        generateDialog(true, id, dialogTexts, top, rt, e, question, answers, correct_choice_index, correct_responses, wrong_responses);
    }

    public void RaniOfJhansi()
    {
        int id = 3;
        string e = "Exhibit";
        string question = "What was the fate of Bahadur Shah after the war ended?";
        string[] answers = { "He was executed", "He was made emperor", "He was exiled", "He escaped" };
        int correct_choice_index = 2;

        int top = 130;
        RectTransform rt = DialogManager.Printer.GetComponent<RectTransform>();

        var dialogTexts = new List<DialogData>();
        dialogTexts.Add(new DialogData("Namastay! I am the Rani Lakshmibai of Jhansi.", e));
        dialogTexts.Add(new DialogData("Before I tell you what happened at Jhansi during the war, let me ask you a question.", e, () =>
        {
            rt.offsetMax = new Vector2(rt.offsetMax.x, top);
            Hide_All_Images();
        }));


        var correct_responses = new List<DialogData>();
        correct_responses.Add(new DialogData("Bohut barhiyya!", e));
        correct_responses.Add(new DialogData("Right, so when the war broke out, Jhansi quickly became a centre of the rebellion.", e));
        correct_responses.Add(new DialogData("A small group of Company officials and their families took refuge in Jhansi Fort, and the Rani negotiated their evacuation.", e));
        correct_responses.Add(new DialogData("The uprising sprung from Meerut where sepoy revolted and marched towards Delhi.", e));
        correct_responses.Add(new DialogData("They declared me as the emperor of India and I accepted their allegiance.", e));
        correct_responses.Add(new DialogData("However, when they left the fort they were massacred by the rebels over whom the Rani had no control...", e));
        correct_responses.Add(new DialogData("The British suspected me of complicity, despite my repeated denials.", e));
        correct_responses.Add(new DialogData("In September and October 1857, I led the successful defence of Jhansi against the invading armies of the neighbouring rajas of Datia and Orchha.", e));
        correct_responses.Add(new DialogData("In March 1858, the Central India Field Force, led by Sir Hugh Rose, advanced on and laid siege to Jhansi.", e));
        correct_responses.Add(new DialogData("The Company forces captured the city, but the Rani had to flee in disguise. ", e));
        correct_responses.Add(new DialogData("After being driven from Jhansi, the Rani, along with a group of Marathans, captured the fortress city of Gwalior.", e));
        correct_responses.Add(new DialogData("However, the Central India Field Force very quickly advanced against the city and our Rani had to meet her end.", e));
        correct_responses.Add(new DialogData("Congratulations! /emote:Happy/ You met all the exhibits! ", makhnu));
        correct_responses.Add(new DialogData("Now let’s see what you have learned from our tour today. /emote:Happy/", makhnu));
        correct_responses.Add(new DialogData("Solve this quiz to test your knowledge and finish the tour!", makhnu));
        correct_responses.Add(new DialogData("Just tap on the Final Quiz button when you are ready.", makhnu));


        var wrong_responses = new List<DialogData>();
        wrong_responses.Add(new DialogData("Bohut afsos!", e));
        wrong_responses.Add(new DialogData("You will have to go back as you don’t deserve to listen to my story with such knowledge.", e));


        generateDialog(true, id, dialogTexts, top, rt, e, question, answers, correct_choice_index, correct_responses, wrong_responses);
    }

    public void StartFinalQuiz()
    {
        string question = "Who won the battle or Gawalior?";
        string[] a1 = { "Rani of Jhansi", "British", "Napolean", "Raja of Datia" };
        int correct_choice_index = 1;
        var dialogTexts = new List<DialogData>();

        var Q1 = new DialogData(question, e);
        for (int i = 0; i < 4; i++)
        {
            if (i == correct_choice_index)
            {
                Q1.SelectList.Add("Correct", a1[i]);
            }
            else
            {
                Q1.SelectList.Add("Wrong", a1[i]);
            }
        }

        Q1.Callback = () =>
        {
            if (FQuizDialogManager.Result == "Correct")
            {
                coins += 50;
            }

        };


        question = "Who led the attack on Jhansi?";
        string[] a2 = { "Sir Hugh Rose", "Major William Hudson", "Lord Dalhousie", "Queen Victoria" };
        correct_choice_index = 0;

        var Q2 = new DialogData(question, e);
        for (int i = 0; i < 4; i++)
        {
            if (i == correct_choice_index)
            {
                Q2.SelectList.Add("Correct", a2[i]);
            }
            else
            {
                Q2.SelectList.Add("Wrong", a2[i]);
            }
        }

        Q2.Callback = () =>
        {
            if (FQuizDialogManager.Result == "Correct")
            {
                coins += 50;
            }

        };

        question = "Where was the Mughal emperor Bahadur Shah exiled to?";
        string[] a3 = { "Humayun's Tomb", "Rangoon", "Orchha", "Mysore" };
        correct_choice_index = 1;

        var Q3 = new DialogData(question, e);
        for (int i = 0; i < 4; i++)
        {
            if (i == correct_choice_index)
            {
                Q3.SelectList.Add("Correct", a3[i]);
            }
            else
            {
                Q3.SelectList.Add("Wrong", a3[i]);
            }
        }

        Q3.Callback = () =>
        {
            if (FQuizDialogManager.Result == "Correct")
            {
                coins += 50;
            }

            Printer.transform.GetChild(0).gameObject.SetActive(false);
            Printer.SetActive(true);
            TourComplete.SetActive(true);
            finalCoinsText.text = coins.ToString();

            int AccCoins = PlayerPrefs.GetInt("PlayerCoins", -1);

            if (AccCoins != -1)
            {
                AccCoins += coins;
                PlayerPrefs.SetInt("PlayerCoins", AccCoins);
                PlayerPrefs.Save();
            }
        };

        dialogTexts.Add(Q1);
        dialogTexts.Add(Q2);
        dialogTexts.Add(Q3);

        FQuizDialogManager.Show(dialogTexts);
    }


    public void LockDialog()
    {
        string e = "Makhnu";
        var dialogTexts = new List<DialogData>();

        dialogTexts.Add(new DialogData("/emote:Confused/Looks like this exhibit is locked. You should find another one first!", e));

        if (dialogue_ongoing)
            return;

        dialogue_ongoing = true;

        DialogManager.Show(dialogTexts);
        dialogue_ongoing = false;
    }

}

