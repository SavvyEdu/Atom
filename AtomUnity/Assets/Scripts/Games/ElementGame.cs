using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Atom;
using UnityEngine.UI;

public class ElementGame : GameBase
{
    /*  Game Rules:
        Display Symbol w/o name/abb
        Initial conditions:
          15 seconds
          first 6 elements available
        Correctly choose element: 
          +3 seconds 
          remove element from available
          next 2 elements added to available (max 118)
        Incorrectly choose element:
          -1 second
          new element from available (no removal)
        Survive for 1:00: WIN
        Choose all 118: BONUS WIN
        Run out out time: LOSE          */

    [SerializeField] private Atom.Atom atom;
    [SerializeField] private AtomicSymbol symbol;
    [SerializeField] private PeriodicTable table;
    [SerializeField] private GlowFX glowFX;

    [SerializeField] private Text timerText;
    [SerializeField] private Text completedText;
    private const int START_TIME = 30;
    private int timer = START_TIME;

    private int targetProtonCount;
    private int targetIndex;
    private List<int> availableElements;
    private List<int> completedElements;
    private int maxUnlocked;

    private bool gameRunning = false;

    protected override void Awake()
    {
        base.Awake();

        atom.Interactable = false;

        availableElements = new List<int>();
        completedElements = new List<int>();

        table.OnElementSelect += CheckElement;
    }

    public override void ResetGame()
    {
        gameRunning = false;

        timer = START_TIME;
        timerText.text = $"0:{START_TIME}";

        availableElements.Clear();
        maxUnlocked = 2;
        availableElements.AddRange(new int[] { 1, 2 });

        //hide numbers on table and clear list
        foreach(int protonCount in completedElements)
            table.HideAtomicNumber(protonCount);
        completedElements.Clear();
        completedText.text = "0";
    }

    public override void StartGame()
    {
        if (gameRunning) return;
        gameRunning = true;

        StartCoroutine("Coutdown");

        //remove atom particles
        atom.ForceToCommon(1);

        SetSymbolFromAvailable();
    }

    public override void ContinueGame()
    {
        timer += START_TIME; //add bonus time
    }

    private void SetSymbolFromAvailable()
    {
        //get a random available element 
        targetIndex = Random.Range(0, availableElements.Count);
        targetProtonCount = availableElements[targetIndex];

        //display target element in the atomic symbol
        Element element= Elements.GetElement(targetProtonCount);
        symbol.SetSymbol(element);
        atom.ForceToCommon(targetProtonCount);
    }

    private void CheckElement(int protonCount)
    {
        if(protonCount == targetProtonCount)
        {
            timer += 4;
            //positive feedback
            table.ShowAtomicNumber(protonCount);
            glowFX.BeginAnimation(true, UIColors.yellow, 4.0f, SetSymbolFromAvailable);
            //remove current
            availableElements.RemoveAt(targetIndex);
            //add the next 2 higheset
            if (maxUnlocked < 118)
            {
                availableElements.Add(++maxUnlocked); 
                availableElements.Add(++maxUnlocked);
            }
            //add to completed
            completedElements.Add(protonCount);
            completedText.text = completedElements.Count.ToString();
            //check for win and bonus win
            if(completedElements.Count == 20)
            {
                tutorialUI.ShowWinMessage();
                CompleteGame();
                tutorialUI.ShowContinueButton(true);
            }
            else if(completedElements.Count == 118)
            {
                StopCoroutine("Coutdown"); //stop the coutdown
                ResetGame();
                tutorialUI.ShowBonusMessage();
                tutorialUI.ShowContinueButton(false);
            }
        }
        else
        {
            timer -= 1; 
            glowFX.BeginAnimation(false, UIColors.orange, 4.0f, SetSymbolFromAvailable);
        }
    }

    private IEnumerator Coutdown()
    {
        while (timer > 0)
        {
            //decrement timer every second
            yield return new WaitForSeconds(1);
            if (!tutorialUI.gameObject.activeSelf) //paused
            {
                timer--;

                //show the timer 
                int second = timer % 60;
                if (second < 10)
                    timerText.text = $"{timer / 60}:0{second}";
                else
                    timerText.text = $"{timer / 60}:{second}";
            }
        }
        ResetGame();

        //display the lose message 
        tutorialUI.ShowLoseMessage();
        tutorialUI.ShowContinueButton(false);
        atom.Interactable = false;
    }


}
