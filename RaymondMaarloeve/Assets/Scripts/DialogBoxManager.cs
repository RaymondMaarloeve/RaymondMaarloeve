using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Collections;

/// <summary>
/// Manages the dialog box UI for player interactions with NPCs.
/// Handles input, output, and communication with the LLM server.
/// </summary>
public class DialogBoxManager : MonoBehaviour
{
    /// <summary>
    /// Singleton instance of the DialogBoxManager class.
    /// </summary>
    public static DialogBoxManager Instance { get; private set; }

    /// <summary>
    /// Parent GameObject for the dialog box UI.
    /// </summary>
    [Header("Dialog Box")]
    [SerializeField] private GameObject dialogBoxParent;

    /// <summary>
    /// Input field for player dialog text.
    /// </summary>
    [Header("Dialog Box Text Input Field")]
    [SerializeField] private TMP_InputField dialogInputField;

    /// <summary>
    /// Text field for displaying NPC responses.
    /// </summary>
    [Header("Dialog Box Text Output Field")]
    [SerializeField] private TMP_Text npcResponseText;

    /// <summary>
    /// Text field for displaying the NPC's name.
    /// </summary>
    [Header("Dialog Box Npc Name Field")]
    [SerializeField] private TMP_Text npcNameText;

    /// <summary>
    /// Indicates whether the dialog box is waiting for the player to dismiss the NPC's response.
    /// </summary>
    private bool waitingForNpcDismiss = false;

    /// <summary>
    /// Initializes the singleton instance.
    /// </summary>
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(this);
    }

    /// <summary>
    /// Sets the dialog box to inactive at the start.
    /// </summary>
    private void Start()
    {
        dialogBoxParent.SetActive(false);
    }

    /// <summary>
    /// Adds a listener for dialog input submission when the dialog box is enabled.
    /// </summary>
    private void OnEnable()
    {
        dialogInputField.onSubmit.AddListener(OnDialogInputSubmit);
    }

    /// <summary>
    /// Removes the listener for dialog input submission when the dialog box is disabled.
    /// </summary>
    private void OnDisable()
    {
        dialogInputField.onSubmit.RemoveListener(OnDialogInputSubmit);
    }

    /// <summary>
    /// Handles dialog input submission.
    /// </summary>
    /// <param name="text">The text entered by the player.</param>
    private void OnDialogInputSubmit(string text)
    {
        ProcessDialogInput(text);
    }

    /// <summary>
    /// Processes the player's dialog input and sends it to the LLM server.
    /// </summary>
    /// <param name="input">The text entered by the player.</param>
    private void ProcessDialogInput(string input)
    {
        Debug.Log("Player entered and confirmed: " + input);
        dialogInputField.gameObject.SetActive(false);
        PlayerController.Instance.ChattingWith.Chat(input);
    }
    

    /// <summary>
    /// Updates the NPC response text and waits for player dismissal.
    /// </summary>
    /// <param name="response">The response from the LLM server.</param>
    public void ShowResponse(string response)
    {
        npcResponseText.text = response + "\nPress Enter to continue...";
        npcResponseText.gameObject.SetActive(true);
        StartCoroutine(WaitForDismiss());
    }

    /// <summary>
    /// Waits for a short time before allowing the player to dismiss the dialog box.
    /// </summary>
    /// <returns>An enumerator for coroutine execution.</returns>
    private IEnumerator WaitForDismiss()
    {
        // Wait for a short time before allowing the player to dismiss the dialog box
        // Without this, system dismisses the dialog box immediately after the NPC response is shown
        yield return new WaitForSeconds(0.5f);
        waitingForNpcDismiss = true;
    }
    
    
    /// <summary>
    /// Updates the dialog box state every frame.
    /// Handles player dismissal of the NPC's response.
    /// </summary>
    private void Update()
    {
        if (waitingForNpcDismiss && Input.GetKeyDown(KeyCode.Return))
        {
            Debug.Log("Dismissed the dialog box.");
            npcResponseText.gameObject.SetActive(false);

            dialogInputField.text = "";
            dialogInputField.gameObject.SetActive(true);
            dialogInputField.ActivateInputField();

            waitingForNpcDismiss = false;
        }
    }


    /// <summary>
    /// Activates the dialog box and initializes the conversation.
    /// </summary>
    public void ShowDialogBox()
    {
        dialogBoxParent.SetActive(true);
        npcResponseText.gameObject.SetActive(false);
        npcNameText.text = PlayerController.Instance.ChattingWith.Name;

        dialogInputField.text = "";
        dialogInputField.gameObject.SetActive(true);
        dialogInputField.ActivateInputField();
    }

    /// <summary>
    /// Deactivates the dialog box and resets its state.
    /// </summary>
    public void HideDialogBox()
    {
        dialogBoxParent.SetActive(false);
        waitingForNpcDismiss = false;
    }
}