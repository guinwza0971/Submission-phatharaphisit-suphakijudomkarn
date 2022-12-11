using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Melanchall.DryWetMidi.Core; //For reading midifile
using Melanchall.DryWetMidi.Interaction; //For GetNotes
using System.IO;
using UnityEngine.SceneManagement;

public class MidiReader : MonoBehaviour
{
    public static MidiReader Instance;

    ///Simple Game manager///
    bool gameStart = false;
    public bool audioSourceStart = false; 

    [SerializeField] TMPro.TextMeshProUGUI startText;
    ////////////////////////

    [SerializeField] string fileLocation;
    
    [SerializeField] Lanes[] lanes;

    //public static MidiFile midiFile;

    [SerializeField] MidiFile midiFile;
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioSource hiddenAudioSource;
    

    public float songDelayInSeconds;
    public float noteSpawnTime;

    public double marginOfError;
    public double inputDelay;

    [SerializeField] float delayAfterSongEnd = 1.5f;
    float songEndDelay = 0;
    //public bool songEnded = false;

    void Start()
    {
        Instance = this;
    }

    void Update()
    {
        if(!gameStart && Input.GetKeyDown("space"))
        {
            if (fileLocation != null)
            ReadMidiFile();
            else
            Debug.LogError("Please add midifile into the MidiReader component of songmanager gameobject.");

            startText.text = "";
            gameStart = true;
        }

        if(audioSourceStart && !audioSource.isPlaying)
        {
            songEndDelay += Time.deltaTime;
            if(songEndDelay > delayAfterSongEnd)
            {
                Restart();
            }
        }
    }

    //Read the midifile into the game
    void ReadMidiFile()
    {
        midiFile = MidiFile.Read(Application.streamingAssetsPath + "/" + fileLocation);

        if (midiFile != null)
        {
        Debug.Log("Read MIDI file successful, The file is: " + midiFile);
        GetDataFromMidi(); //Extract the data from the readed file
        }
        /*else 
        Debug.LogError("There are no midifile found in the specified location.");*/
    }

    void GetDataFromMidi()
    {
        //Create an array to store the notes from midifile
        var notes = midiFile.GetNotes(); 
        Debug.Log("Extracted notes as an array, there are " + notes.Count + "notes in the midifile");

        var noteArray = new Melanchall.DryWetMidi.Interaction.Note[notes.Count];
        //Debug.Log("Array of Notes created : " + array);

        notes.CopyTo(noteArray, 0);
        
        foreach(var note in noteArray)
        {
            Debug.Log("Note : " + Melanchall.DryWetMidi.MusicTheory.NoteUtilities.GetNoteNumber(note.NoteName, note.Octave));
        }

        //Start spawning process
        foreach (var lane in lanes) lane.SetTimeStamps(midiFile, noteArray);

        //After start spawning note, wait for ... seconds then start playing audio
        StartGame();
    }

    void StartGame()
    {
        Invoke(nameof(HiddenSong), noteSpawnTime);
        Invoke(nameof(StartSong), songDelayInSeconds + noteSpawnTime);
    }

    void StartSong()
    {
        audioSource.Play();
        audioSourceStart = true;
    }
    public static double GetAudioSourcePlayBackTime()
    {
        return (double)Instance.audioSource.timeSamples / Instance.audioSource.clip.frequency;
    }

    void HiddenSong()
    {
        hiddenAudioSource.Play();
    }
    public static double GetHiddenAudioSourcePlayBackTime()
    {
        return (double)Instance.hiddenAudioSource.timeSamples / Instance.hiddenAudioSource.clip.frequency;
    }

    public void Restart()
    {
        PlayerPrefs.SetInt("highScore", ScoreManager.score);
        ScoreManager.score = 0;
        SceneManager.LoadScene(0);
        /*songEndTimer = 0;
        tappingIndex = 0;
        noteSpawnIndex = 0;
        MidiReader.Instance.songEnded = true;*/
    }
}
