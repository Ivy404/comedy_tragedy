using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class VideoPlayerManager : MonoBehaviour
{

    public VideoPlayer videoPlayerIntro;
    public VideoPlayer videoPlayerLoop;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        videoPlayerIntro.loopPointReached += OnVideoEnd;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnVideoEnd(VideoPlayer vp)
    {
        videoPlayerIntro.loopPointReached -= OnVideoEnd;
        Destroy(videoPlayerIntro.gameObject);
        videoPlayerLoop.Play();
        this.GetComponent<TitleScreen>().videoEnded();
    }

}
