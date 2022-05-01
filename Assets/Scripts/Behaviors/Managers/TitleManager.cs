using System;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleManager : BaseBehavior
{
    public const string Scene = "TitleScene";

    public GameObject playButton;
    public GameObject menuButton;
    public CustomButton editorButton;
    public GameObject toggleSoundButton;
    public GameObject levelPanel;
    public GameObject mainMenuPanel;

    private LevelListManager levelManager;
    private MainMenuManager menuManager;
    
    // private Alert alert;

    // Start is called before the first frame update
    protected override void Start()
    {

        base.Start();
        this.editorButton.onClick.AddListener(() => this.loadEdtior());
        this.audioSource = this.GetComponent<AudioSource>();
        // this.alert = this.iapAlert.GetComponent<Alert>();
        // alert will hide once initialized
        var sound = this.toggleSoundButton.GetComponent<CustomButton>();
        sound.Toggle(this.Context.data.IsSoundEnabled);
        sound.OnValueChangeEvent.AddListener((caller, value) =>
        {
            if (!GameContext.IsNavigationEnabled)
            {
                this.EventSystem.SetSelectedGameObject(null);
            }
            this.Context.data.IsSoundEnabled = value;
        });

        this.levelManager = this.levelPanel.GetComponent<LevelListManager>();
        this.levelManager.OnLevelSelectedEvent.AddListener((data, levelData) =>
        {
//            Debug.Log($"Level selection: {levelData.relativePath}");
            this.Context.data.SetActiveData(this.Context, data, levelData.relativePath);
            SceneManager.LoadScene(GameplayManager.Scene);
        });
        this.menuManager = this.mainMenuPanel.GetComponent<MainMenuManager>();
        this.menuManager.OnPageSelectedEvent.AddListener((caller, page) =>
        {
            this.PlaySfx();
            switch (page)
            {
                case MainMenuManager.MenuPageType.Create:
                    SceneManager.LoadScene(EditorManager.Scene);
                    break;
                case MainMenuManager.MenuPageType.LevelList:
                    this.NavigateLevelList();
                    break;
            }
        });

        
        

        this.NavigateTitle();
    }

    void Update()
    {
        if (!GameContext.HasRuntimeNavigation && Input.GetButtonDown("Submit"))
        {
            var wasNavigationEnabled = GameContext.IsNavigationEnabled;
            GameContext.HasRuntimeNavigation = true;
            if (!wasNavigationEnabled)
            {
                this.EventSystem.SetSelectedGameObject(this.playButton);
                var play = this.playButton.GetComponent<CustomButton>();
                play.onClick.Invoke();
            }
        }
    }

    

    private void loadEdtior()
    {
        SceneManager.LoadScene(EditorManager.Scene);
    }

    public void SelectDefault()
    {
        this.EventSystem.SetSelectedGameObject(this.playButton);
    }

    public void NavigateTitle()
    {
        AnalyticsManager.Event(() => AnalyticsEvent.ScreenVisit("title"));
        this.levelManager.ToggleActive(false);
        this.mainMenuPanel.SetActive(true);
        this.UpdateMenuButton();

        this.EventSystem.SetSelectedGameObject(null);
        if (GameContext.IsNavigationEnabled)
        {
            this.SelectDefault();
        }

        // this.restorePurchaseButton?.gameObject?.SetActive(this.allowRestoration);
    }

  

    private void NavigateLevelList()
    {
        var metaData = this.Context.file.Get<WarehouseManager.MetaData>();
        AnalyticsManager.Event(() => AnalyticsEvent.ScreenVisit("levelList"));
        // this.levelPanel.SetActive(true);
        this.levelManager.ToggleActive(true);
        this.mainMenuPanel.SetActive(false);
        this.UpdateMenuButton();
    }

    private void UpdateMenuButton()
    {
        var button = this.menuButton.GetComponent<CustomButton>();
        var text = button.GetComponentInChildren<Text>();

        // level page defaults
        var isActive = true;
        text.text = this.Locale.Get(LocaleTextType.Menu, "Main Menu");
        UnityAction onClick = () => this.NavigateTitle();

       

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(onClick);
        this.menuButton.SetActive(isActive);
    }
}
