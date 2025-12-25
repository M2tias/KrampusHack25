using UnityEngine;
using UnityEngine.SceneManagement;

public class Destruction : MonoBehaviour
{
    [SerializeField]
    private AudioPlayer player;

    private float destructionStarted;
    private float destructionTime = 2f;
    private bool isTriggered = false;
    private bool isGameOver = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (isTriggered && Time.time - destructionStarted > destructionTime)
        {
            if (isGameOver)
            {
                SceneManager.LoadScene("gameOver", LoadSceneMode.Single);
            }

            Destroy(gameObject);
        }
    }

    public void TriggerDestruction(bool isGameOver = false)
    {
        transform.parent = null;
        player.PlayClip();
        destructionStarted = Time.time;
        isTriggered = true;
        this.isGameOver = isGameOver;
    }
}
