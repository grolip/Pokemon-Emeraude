using System.Collections;
using System.Collections.Generic;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.UI;

public class DisplayMessages : MonoBehaviour
{
    public GameObject arrow;
    public bool hasNextMsg;
    public bool inReadingMode;
    public int currentUser;
    
    [SerializeField] private GameObject textBox;
    [SerializeField] private GameObject content;
    [SerializeField] private GameObject scrollView;
    
    private const float DisplayTime = 0.03f;
    
    private ScrollRect _scrollRect;
    private Bouncing _arrowBouncing;
    private TextMeshProUGUI _text;
    private List<string> _messages;
    private string _message;
    private int _currentIndexMsg;

    void Start()
    {
        _scrollRect = scrollView.GetComponent<ScrollRect>();
        _text = content.GetComponent<TextMeshProUGUI>();
        _text.margin = new Vector4(12f, 12f, 12f, 12f);
        _arrowBouncing = arrow.GetComponent<Bouncing>();
    }
    
    public void OpenDialog(int userId, List<string> newMessages)
    {
        currentUser = userId;
        _messages = newMessages;
        _currentIndexMsg = 0;
        _text.text = "";
        
        hasNextMsg = _currentIndexMsg < _messages.Count;
        textBox.gameObject.SetActive(true);
    }

    public void NextDialog()
    {
        if (!hasNextMsg) return;
        
        _message = _messages[_currentIndexMsg];
        _currentIndexMsg++;
        hasNextMsg = _currentIndexMsg < _messages.Count;
        
        StartCoroutine(WriteText());
    }
    
    public void CloseDialog()
    {
        StopCoroutine(WriteText());
        textBox.gameObject.SetActive(false);
        currentUser = 0;
    }
    
    private IEnumerator WriteText()
    {
        var currentNLines = 0;
        inReadingMode = true;
        _text.text = "";
        
        if (arrow.activeSelf)
        {
            _arrowBouncing.HideArrow();
            arrow.SetActive(false);
        }
        
        foreach (var c in _message)
        {
            _text.text += c;
            
            // Gestion du scroll à chaque nouvelle ligne
            if (currentNLines < _text.textInfo.lineCount)
            {
                currentNLines = _text.textInfo.lineCount;
                _scrollRect.verticalNormalizedPosition = 0f;
            }
            yield return new WaitForSeconds(DisplayTime);
        }
        
        inReadingMode = false;

        if (hasNextMsg)
        {
            arrow.SetActive(true);
            _arrowBouncing.ShowArrow();
        }
    }
}