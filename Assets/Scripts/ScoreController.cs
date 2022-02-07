using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreController : MonoBehaviour
{
    private int score { get; set; }
    [SerializeField] private CharacterMovement player;    
    private TMP_Text _scoreText;
    public static ScoreController instance;
    public List<CollectableController> collectables;
    

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
        score = 0;
        _scoreText = gameObject.GetComponent<TMP_Text>();
        _scoreText.SetText(score.ToString());
        collectables = new List<CollectableController>();
    }

    public void AddCollectable(CollectableController cc)
    {
        collectables.Add(cc);
        cc.OnPickup += HandleCollection;
    }

    public void HandleCollection(CollectableController cc)
    {
        score++;
        _scoreText.SetText(score.ToString());
        cc.OnPickup -= HandleCollection;
        collectables.Remove(cc);
        player.RecoverSuperJump();
    }

}
