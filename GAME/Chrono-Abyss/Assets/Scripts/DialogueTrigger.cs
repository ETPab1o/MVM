using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    public string[] dialogueLines;
    public DialogueSystem dialogueSystem;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            dialogueSystem.StartDialogue(dialogueLines);
        }
    }
}
