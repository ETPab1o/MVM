using System.Collections;
using UnityEngine;
using TMPro;

public class DialogueSystem : MonoBehaviour


{
    public TextMeshProUGUI dialogueText;
    public GameObject dialoguePanel;
    public float textSpeed = 0.05f;
    public AudioSource audioSource;
    public AudioClip typingSound;
    public PlayerController playerController; // Reference to the player controller

    public Vector3 hiddenPosition;
    public Vector3 visiblePosition;
    public float animationSpeed = 1.0f;

    private string[] dialogueLines;
    private int currentLineIndex;

    private void Start()
    {
        dialoguePanel.SetActive(false);
        dialoguePanel.transform.position = hiddenPosition;
    }

    public void StartDialogue(string[] lines)
    {
        dialogueLines = lines;
        currentLineIndex = 0;
        playerController.EnableMovement(false); // Disable player movement
        StartCoroutine(ShowDialoguePanel());
    }

    private IEnumerator ShowDialoguePanel()
    {
        dialoguePanel.SetActive(true);
        float time = 0;
        while (time < 1)
        {
            time += Time.deltaTime * animationSpeed;
            dialoguePanel.transform.position = Vector3.Lerp(hiddenPosition, visiblePosition, time);
            yield return null;
        }
        StartCoroutine(TypeLine());
    }

    private IEnumerator HideDialoguePanel()
    {
        float time = 0;
        while (time < 1)
        {
            time += Time.deltaTime * animationSpeed;
            dialoguePanel.transform.position = Vector3.Lerp(visiblePosition, hiddenPosition, time);
            yield return null;
        }
        dialoguePanel.SetActive(false);
        playerController.EnableMovement(true); // Re-enable player movement
    }

    private IEnumerator TypeLine()
    {
        dialogueText.text = "";
        foreach (char letter in dialogueLines[currentLineIndex].ToCharArray())
        {
            dialogueText.text += letter;
            if (typingSound != null)
            {
                audioSource.PlayOneShot(typingSound);
            }
            yield return new WaitForSeconds(textSpeed);
        }
    }

    private void Update()
    {
        if (dialoguePanel.activeSelf && (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Mouse0)))
        {
            if (dialogueText.text == dialogueLines[currentLineIndex])
            {
                NextLine();
            }
            else
            {
                StopAllCoroutines();
                dialogueText.text = dialogueLines[currentLineIndex];
            }
        }
    }

    private void NextLine()
    {
        if (currentLineIndex < dialogueLines.Length - 1)
        {
            currentLineIndex++;
            StartCoroutine(TypeLine());
        }
        else
        {
            StartCoroutine(HideDialoguePanel());
        }
    }
}
