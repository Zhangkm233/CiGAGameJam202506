using UnityEngine;

public class AudioController : MonoBehaviour
{
    //���ڲ��ű������ֺ���Ч�Ŀ�����
    public AudioClip[] audioClips;
    public AudioClip clickSfx;
    public AudioClip dropPieceSfx;
    public AudioSource sfxSource;
    public AudioSource bgmSource;
    public int clipsIndex = 0;

    void Start() {
        if (bgmSource == null) {
            bgmSource = this.GetComponent<AudioSource>();
        }
        PlayClip();
    }
    void Update() {
        if (!bgmSource.isPlaying && audioClips.Length != 0) {
            PlayClip();
        }
    }
    private void PlayClip() {
        if (audioClips.Length > 0) {
            if (clipsIndex >= audioClips.Length) {
                clipsIndex = 0;
            }
            bgmSource.clip = audioClips[clipsIndex];
            bgmSource.Play();
            clipsIndex++;
        } else {
            Debug.LogWarning("audioClipsû������");
        }
    }

    private void PlaySfx(AudioClip audioclip) {
        sfxSource.PlayOneShot(audioclip);
    }

    public void PlayClick() {
        PlaySfx(clickSfx);
    }

    public void PlayDropPiece() {
        PlaySfx(dropPieceSfx);
    }
}
