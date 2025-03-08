using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class CauldronMix : MonoBehaviour
{
    [System.Serializable]
    public class PotionCombination
    {
        public string[] ingredients = new string[2];
        public VideoClip resultVideo; // Video en lugar de sprite
        public ParticleSystem successParticles;
        public AudioClip successSound;
    }

    [Header("Configuración Principal")]
    public List<PotionCombination> combinations;
    public GameObject resultDisplay; // Quad con material y Render Texture
    public RenderTexture videoRenderTexture;

    [Header("Respawn")]
    public List<GameObject> bottles;
    private List<Vector3> initialPositions = new List<Vector3>();

    [Header("Efectos")]
    public ParticleSystem failParticles;
    public AudioClip failSound;

    private List<string> currentIngredients = new List<string>();
    private VideoPlayer videoPlayer;
    private AudioSource audioSource;

    void Start()
    {
        videoPlayer = gameObject.AddComponent<VideoPlayer>();
        audioSource = gameObject.AddComponent<AudioSource>();

        // Configurar VideoPlayer
        videoPlayer.playOnAwake = false;
        videoPlayer.targetTexture = videoRenderTexture;
        videoPlayer.isLooping = false;
        videoPlayer.audioOutputMode = VideoAudioOutputMode.AudioSource;
        videoPlayer.SetTargetAudioSource(0, audioSource);

        // Ocultar resultado al inicio
        resultDisplay.SetActive(false);

        // Guardar posiciones iniciales de botellas
        foreach (GameObject bottle in bottles)
        {
            initialPositions.Add(bottle.transform.position);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Potion") && currentIngredients.Count < 2)
        {
            AddIngredient(other.GetComponent<Potion>().color);
            other.gameObject.SetActive(false);
        }
    }

    void AddIngredient(string color)
    {
        currentIngredients.Add(color);

        if (currentIngredients.Count == 2)
        {
            CheckCombination();
            StartCoroutine(ResetCauldron());
        }
    }

    void CheckCombination()
    {
        currentIngredients.Sort();

        foreach (PotionCombination combo in combinations)
        {
            List<string> required = new List<string>(combo.ingredients);
            required.Sort();

            if (AreEqual(currentIngredients, required))
            {
                ShowResult(combo);
                return;
            }
        }

        PlayFailEffects();
    }

    bool AreEqual(List<string> a, List<string> b)
    {
        if (a.Count != b.Count) return false;
        for (int i = 0; i < a.Count; i++)
        {
            if (a[i] != b[i]) return false;
        }
        return true;
    }

    void ShowResult(PotionCombination combo)
    {
        resultDisplay.SetActive(true);

        // Configurar y reproducir video
        videoPlayer.clip = combo.resultVideo;
        videoPlayer.Play();

        // Efectos adicionales
        if (combo.successParticles != null)
            Instantiate(combo.successParticles, transform.position, Quaternion.identity);

        if (combo.successSound != null)
            audioSource.PlayOneShot(combo.successSound);
    }

    void PlayFailEffects()
    {
        if (failParticles != null)
            failParticles.Play();

        if (failSound != null)
            audioSource.PlayOneShot(failSound);
    }

    IEnumerator ResetCauldron()
    {
        // Esperar duración del video
        yield return new WaitForSeconds((float)videoPlayer.clip.length);

        // Detener y resetear
        videoPlayer.Stop();
        resultDisplay.SetActive(false);
        currentIngredients.Clear();

        // Reactivar botellas
        for (int i = 0; i < bottles.Count; i++)
        {
            bottles[i].SetActive(true);
            bottles[i].transform.position = initialPositions[i];
        }
    }
}