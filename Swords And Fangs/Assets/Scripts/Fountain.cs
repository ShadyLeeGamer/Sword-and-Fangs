using UnityEngine;
using UnityEngine.Rendering;

public class Fountain : Interactable
{
    GameScreen gameScreen;

    public static Fountain Instance { get; private set; }
    [SerializeField] Volume PostProcessing;
    [SerializeField] VolumeProfile[] PostProcessingStyles;

    public override void Awake()
    {
        Instance = this;
        base.Awake();
    }

    void Start()
    {
        gameScreen = GameScreen.Instance;
    }

    void OpenGUI()
    {
        dracula.CanControl = false;
        PostProcessing.profile = PostProcessingStyles[1];
        gameScreen.OpenFountainGUI();
    }

    public void CloseGUI()
    {
        PostProcessing.profile = PostProcessingStyles[0];
        dracula.CanControl = true;
        gameScreen.OpenGameGUI();
        dracula.ResetShortAttackCombo();
    }

    public override void Interact()
    {
        base.Interact();
        OpenGUI();
    }
}