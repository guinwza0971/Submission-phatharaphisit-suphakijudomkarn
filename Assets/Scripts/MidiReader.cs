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
    public static MidiReader Instance; //This script allows access to this class from other class

    bool gameStart = false; //Use this to detect begining of main game loop
    public bool audioSourceStart = false; //Use this bool to detect if the music already start

    [SerializeField] TMPro.TextMeshProUGUI startText; //The "Press space to start" text

    [SerializeField] string fileLocation; //Location of drum.mid and the midi file should stored in "Streaming Assets" Folder
    
    [SerializeField] Lanes[] lanes; //The lanes is use for managing note properties and behavior and player gameplay input

    //public static MidiFile midiFile;

    [SerializeField] MidiFile midiFile;//this variable stores midifile data from midifile reading function
    [SerializeField] AudioSource audioSource; //this is for the music that play
    [SerializeField] AudioSource hiddenAudioSource; // the same audiosource but this audiosource must have volume = 0 and only use as timer for note spawner
    

    public float songDelayInSeconds; //The delay time between spawning first note and begin playing music
    public float noteSpawnTime; //The delay time between initializing the game after pressing start and begin spawning first note

    public double marginOfError; //This would allow player to play the game easier because it is not probable that player could tap at exact milisec that the note plays
    public double inputDelay; //This isn't use in this current project, but it would be neccessary to consider input delay to prevent player from feeling out of sync with the song

    [SerializeField] float delayAfterSongEnd = 1.5f; //After the song end, this is the cooldown time before let player press space to start game again
    float songEndDelay = 0; //This will work in tendem with "delayAfterSongEnd" to measure the time after song end.
    //public bool songEnded = false;

    void Start()
    {
        Instance = this;
    }

    void Update()
    {
        if(!gameStart && Input.GetKeyDown("space"))// Player need to press space to start the game loop
        {
            if (fileLocation != null)
            ReadMidiFile(); //This script start the sequence of the game loop
            else
            Debug.LogError("Please add midifile into the MidiReader component of songmanager gameobject.");
            
            if (startText != null)
            startText.text = ""; // must erase the "Press space to start" Text because it would obstruct player sight.
            
            gameStart = true; //This if start the script that lead to begining the game loop so this should be set to true to indicate game start state
        }

        if(audioSourceStart && !audioSource.isPlaying) //Indicate ending of the song
        {
            songEndDelay += Time.deltaTime; //Run the timer
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
        var prevHighScore = 0;

        if(PlayerPrefs.GetInt("highScore") != 0)
        prevHighScore = PlayerPrefs.GetInt("highScore");

        PlayerPrefs.SetInt("highScore", Mathf.Max(ScoreManager.score,prevHighScore));
        ScoreManager.score = 0;
        SceneManager.LoadScene(0);
        /*songEndTimer = 0;
        tappingIndex = 0;
        noteSpawnIndex = 0;
        MidiReader.Instance.songEnded = true;*/
    }
}
