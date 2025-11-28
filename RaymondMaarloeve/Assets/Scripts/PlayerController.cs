using Gitmanik.Console;
using UnityEngine;

/// <summary>
/// Manages player movement, interaction with NPCs, and state transitions.
/// Handles gravity, animations, and camera behavior during interactions.
/// </summary>
public class PlayerController : MonoBehaviour, IChattable
{
    /// <summary>
    /// Singleton instance of the PlayerController class.
    /// </summary>
    public static PlayerController Instance;

    /// <summary>
    /// Speed at which the player moves.
    /// </summary>
    public float moveSpeed = 5f;

    /// <summary>
    /// Gravity applied to the player.
    /// </summary>
    public float gravity = 9.81f;

    /// <summary>
    /// Reference to the CharacterController component.
    /// </summary>
    private CharacterController characterController;

    /// <summary>
    /// Direction of player movement.
    /// </summary>
    private Vector3 moveDirection;

    /// <summary>
    /// Transform of the NPC the player is targeting for interaction.
    /// </summary>
    private Transform targetNPC = null;

    /// <summary>
    /// Reference to the player's SkinnedMeshRenderer.
    /// </summary>
    private SkinnedMeshRenderer characterMesh;

    /// <summary>
    /// Reference to the Animator component for player animations.
    /// </summary>
    private Animator animator;

    /// <summary>
    /// Reference to chatee. NULL if talking with noone.
    /// </summary>
    public IChattable ChattingWith { get; private set; }

    /// <summary>
    /// Whether player can move.
    /// </summary>
    private bool ShouldMove => ChattingWith == null && !notesParent.activeSelf;

    /// <summary>
    /// Name of the Player.
    /// </summary>
    public string Name => "Detective Raymond Maarloeve";

    /// <summary>
    /// Look target for chatee.
    /// </summary>
    public Transform LookTarget => CameraFollow.Instance.transform;

    /// <summary>
    /// Whether Player is available to char.
    /// </summary>
    public bool AvailableToChat => ChattingWith == null;

    [SerializeField] private NotesManager notesManager;
    [SerializeField] private GameObject notesParent;


    /// <summary>
    /// Initializes the singleton instance.
    /// </summary>
    void Awake()
    {
        Instance = this;
    }

    /// <summary>
    /// Initializes components and sets the camera target to the player.
    /// </summary>
    void Start()
    {
        characterController = GetComponent<CharacterController>();
        CameraFollow.Instance.SetTarget(transform, false);
        characterMesh = GetComponentInChildren<SkinnedMeshRenderer>();
        animator = GetComponentInChildren<Animator>();

        if (notesParent != null)
            notesParent.SetActive(false);
    }

    /// <summary>
    /// Updates player movement and interaction logic every frame.
    /// </summary>
    void Update()
    {
        if (GitmanikConsole.Visible)
            return;

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            ToggleNotes();
        }

        if (ShouldMove)
        {
            HandleMovement();
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (ShouldMove && targetNPC != null)
            {
                StartInteraction(targetNPC.GetComponent<NPC>());
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape) && ChattingWith != null)
        {
            ChattingWith.FinishChatting();
            FinishChatting();
        }
    }

    /// <summary>
    /// Handles player movement, including gravity and animations.
    /// </summary>
    void HandleMovement()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveZ = Input.GetAxisRaw("Vertical");

        moveDirection = new Vector3(moveX, 0, moveZ).normalized * moveSpeed;

        if (moveDirection.magnitude > 0)
        {
            transform.forward = new Vector3(moveX, 0, moveZ);
        }
        if (!characterController.isGrounded)
        {
            moveDirection.y -= gravity * Time.deltaTime;
        }

        characterController.Move(moveDirection * Time.deltaTime);
        if (animator != null)
        {
            Vector3 horizontalMove = moveDirection;
            horizontalMove.y = 0f;
            animator.SetFloat("Speed", horizontalMove.magnitude);
        }

    }

    /// <summary>
    /// Starts interaction with the specified NPC.
    /// Adjusts camera and player state for interaction.
    /// </summary>
    /// <param name="npc">The NPC to interact with.</param>
    public void StartInteraction(NPC npc)
    {
        npc.StartChatting(this);
        StartChatting(npc);
    }

    /// <summary>
    /// Hides game UI and enables chatting dialog.
    /// </summary>
    public void StartChatting(IChattable chattable)
    {
        ChattingWith = chattable;

        moveDirection = Vector3.zero;
        characterMesh.enabled = false;
        GameManager.Instance.MinimapGameObject.SetActive(false);
        CameraFollow.Instance.SetTarget(chattable.LookTarget, true);
        DialogBoxManager.Instance.ShowDialogBox();

        Debug.Log("Started chatting with: " + ChattingWith.Name);
    }

    /// <summary>
    /// Ends interaction with the currently interacting NPC.
    /// Resets camera and player state.
    /// </summary>
    public void FinishChatting()
    {
        characterMesh.enabled = true;
        GameManager.Instance.MinimapGameObject.SetActive(true);
        CameraFollow.Instance.SetTarget(transform, false);
        DialogBoxManager.Instance.HideDialogBox();

        Debug.Log($"Finished chatting with: {ChattingWith.Name}");
        ChattingWith = null;
    }

    /// <summary>
    /// Redirects response from NPC to DialogBoxManager.
    /// </summary>
    public void Chat(string message)
    {
        DialogBoxManager.Instance.ShowResponse(message);
    }

    /// <summary>
    /// Detects when the player enters the trigger zone of an NPC.
    /// </summary>
    /// <param name="other">The collider of the NPC.</param>
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("NPC"))
        {
            targetNPC = other.transform;
        }
    }

    /// <summary>
    /// Detects when the player exits the trigger zone of an NPC.
    /// </summary>
    /// <param name="other">The collider of the NPC.</param>
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("NPC"))
        {
            targetNPC = null;
        }
    }

    private void ToggleNotes()
    {
        if (notesParent == null || notesManager == null)
        {
            Debug.LogError("Brakuje referencji do notesParent lub notesManager!");
            return;
        }

        bool nowActive = !notesParent.activeSelf;
        notesParent.SetActive(nowActive);

        if (nowActive)
        {
            moveDirection = Vector3.zero;

            characterMesh.enabled = false;
            GameManager.Instance.MinimapGameObject.SetActive(false);

            notesManager.ActivateInputAtEnd();
        }
        else
        {
            characterMesh.enabled = true;
            GameManager.Instance.MinimapGameObject.SetActive(true);
        }
    }
}
