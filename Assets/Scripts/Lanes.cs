using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Melanchall.DryWetMidi.Interaction;
using Melanchall.DryWetMidi.Core; //For reading midifile
using Melanchall.DryWetMidi.Common; //For reading midifile

public class Lanes : MonoBehaviour
{
    [SerializeField] Melanchall.DryWetMidi.MusicTheory.NoteName noteName; //This code fixxed the lane into a single note
    [SerializeField] int noteOctave;

    SevenBitNumber noteRestriction;

    public KeyCode input; //This script for customizing keyboard input for this note

    List<double> timeStamps = new List<double>();


    int noteSpawnIndex;
    int tappingIndex = 0;

    [SerializeField] MidiReader midireader;

    [SerializeField] GameObject notePrefab;
    public Color noteColor;
    [SerializeField] float noteSpeed;
    [SerializeField] int noteScore = 20;
    [SerializeField] float noteLifeSpan = 10f;

    public string myLaneInputText;

    bool enableNoteSpawn = false;
    Vector3 spawnPosition = new Vector3(0,0,0);
    [SerializeField] GameObject target;
    GameObject noteIndicator;

    float noteTravelDistance;
    float noteTravelTime;

    float songEndTimer;

    List<Note> spawnedNotes = new List<Note>();

    // Start is called before the first frame update
    void Start()
    {
        noteIndicator = GameObject.FindWithTag("NoteIndicator");
        midireader = GameObject.FindWithTag("MidiReader").GetComponent<MidiReader>();
        noteTravelTime = midireader.songDelayInSeconds; //MidiReader.Instance.songDelayInSeconds;
        noteTravelDistance = noteSpeed * noteTravelTime;

        spawnPosition = new Vector3(target.transform.position.x, noteIndicator.transform.position.y, target.transform.position.z) 
            + new Vector3(0,noteTravelDistance,0);

        noteLifeSpan = 2*noteTravelTime;
        myLaneInputText = input.ToString();
        Debug.Log("My input is " + myLaneInputText);
    }

    public void SetTimeStamps(Melanchall.DryWetMidi.Core.MidiFile midiFile, Melanchall.DryWetMidi.Interaction.Note[] noteArray) 
    //Set the timestamp that there are a note appear in the song
    {
        noteRestriction = Melanchall.DryWetMidi.MusicTheory.NoteUtilities.GetNoteNumber(noteName,noteOctave);
        Debug.Log("My note Restrction = " + noteRestriction);

        Debug.Log("Start set time stamps method");
        foreach (var note in noteArray)
        {
            if (Melanchall.DryWetMidi.MusicTheory.NoteUtilities.GetNoteNumber(note.NoteName, note.Octave) == noteRestriction)//Restrict the timestamp of this lane to prespecified note
            {
                //convert time from tempomap format into min:sec:millisec format
                var metricTimeSpan = TimeConverter.ConvertTo<MetricTimeSpan>(note.Time, midiFile.GetTempoMap());
                //convert time from min:sec:millisec format into seconds format
                timeStamps.Add((double)metricTimeSpan.Minutes * 60f + metricTimeSpan.Seconds + (double)metricTimeSpan.Milliseconds / 1000f);

                Debug.Log("Success for note " + note.NoteName + note.Octave );
            }
        }
        /*if(timeStamps.Count == 0)
        {
            Debug.Log("No note found");
        }
        foreach(var timeStamp in timeStamps)
        {
            Debug.Log(" Timestamp: " + timeStamp + "For note number: " + noteRestriction);
        }*/

        enableNoteSpawn = true;
    } //In summary, this method set the timestamp for every note in note array (only the note of this lane will be set)

    void Update()
    {
        
        if(noteSpawnIndex < timeStamps.Count && enableNoteSpawn)
        {
            if (MidiReader.GetHiddenAudioSourcePlayBackTime() > timeStamps[noteSpawnIndex] )
            {
                Debug.Log("Spawning note at " + (timeStamps[noteSpawnIndex]));
                var note = Instantiate(notePrefab, spawnPosition, Quaternion.identity);
                spawnedNotes.Add(note.GetComponent<Note>());
                note.GetComponent<Note>().Spawn(noteColor, noteSpeed, noteLifeSpan);
                noteSpawnIndex++;
            }
        }
        //timeStamps variable will be use to determine 1.the time that player need to tap to get a score 2.the time that this lane spawn a note
        //The player tapping time must syncronize with audiosource playback time
        if(tappingIndex < timeStamps.Count) 
        {
        double timeStamp = timeStamps[tappingIndex];
        bool hit = false;
        double playBackTime = MidiReader.GetAudioSourcePlayBackTime();
        double marginOfError = midireader.marginOfError;
        double inputDelay = midireader.inputDelay;

            if(Input.GetKeyDown(input))
            {

                if(Math.Abs(playBackTime - timeStamp) < marginOfError && midireader.audioSourceStart)
                //add && midireader.audioSourceStart to ensure that player can only earn score after music start!
                {
                    Debug.Log("You hit at " + playBackTime);
                    ScoreManager.score+=noteScore;
                    Destroy(spawnedNotes[tappingIndex].gameObject);
                    tappingIndex++;
                    hit = true;
                }
                else
                {   
                    Debug.Log("Not hit anything!");
                    //Not hit anything!
                }

            }

            if(playBackTime - (timeStamp) > marginOfError && !hit)
            {
                //Destroy(spawnedNotes[tappingIndex].gameObject);
                Debug.Log("Missed at " + playBackTime);
                tappingIndex++;
            }
        }

    }


}
