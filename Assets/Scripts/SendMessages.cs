using System.Collections.Generic;
using System.IO;
using System.Linq;
using UI;
using UnityEngine;

public class SendMessages : MonoBehaviour
{
    public string fileName;
    
    private const string MessageSep = "***";

    private int _senderID;
    private List<string> _messages;
    private DisplayMessages _displayMessages;
    private int[] _idAllowed;
    private bool _inReadingZone;
    private bool _dialogOpened;

    void Start()
    {
        _senderID = gameObject.GetInstanceID();
        _idAllowed = new[] { 0, _senderID };
        _messages = ReadFile(Application.dataPath + "/Resources/Dialogs/" + fileName);
        _displayMessages = FindFirstObjectByType<DisplayMessages>();
    }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.gameObject.CompareTag("Player")) return;
        
        _inReadingZone = true;
    }
    
    void OnTriggerExit2D(Collider2D other)
    {
        if (!other.gameObject.CompareTag("Player")) return;

        if (_displayMessages != null && _dialogOpened)
            CloseDialog();
        
        _inReadingZone = false;
    }
    
    private void OnMouseDown()
    {
        // On vérifie qu'il n'y ai pas de dialog ouvert avec une autre entité
        // ou bien que le dialogue se fait avec l'entité courante.
        if (!_idAllowed.Contains(_displayMessages.currentUser))
            return;
        
        if (_inReadingZone && !_displayMessages.inReadingMode)
        {
            if (!_dialogOpened)
            {
                _displayMessages.OpenDialog(_senderID, _messages);
                _dialogOpened = true;
            }

            if (_displayMessages.hasNextMsg)
                _displayMessages.NextDialog();
            else
                CloseDialog();
        }
    }
    
    List<string> ReadFile(string filePath)
    {
        
        var content = File.ReadAllText(filePath);
        var messages = new List<string>();

        foreach (var line in content.Split(MessageSep))
        {
            var trimmedLine = line.Trim();
            if (trimmedLine.Length == 0) continue;
            
            messages.Add(line.TrimStart('\n', '\r'));
        }
        return messages;
    }
    
    void CloseDialog()
    {
        _displayMessages.CloseDialog();
        _dialogOpened = false;
    }
}
