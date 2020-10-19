using System.Collections;
using System.Collections.Generic;
//using System.IO;
using UnityEngine;
using UnityEngine.Playables;
//using UnityEngine.Timeline;

public class SpawnCards : MonoBehaviour
{
    public static SpawnCards instance;
    public static double time = 0d;

    public GameObject goodCard;
    public GameObject badCard;
    public GameObject thirdCard;

    public GameObject judgeCard;

    public Transform goodCardPos;
    public Transform badCardPos;
    public Transform thirdCardPos;

    public GameObject jackInTheBox;
    public GameObject levelChanger;
    public GameObject endingCannon;

    public ParticleSystem chooseEffect;

    Animator j_animator;

    DialogueCard goodCardDialogue;
    DialogueCard badCardDialogue;
    DialogueCard thirdCardDialogue;

    Rigidbody goodCardRb;
    Rigidbody badCardRb;
    Rigidbody thirdCardRb;

    PlayableDirector director;
    Stack<PlayableDirector> branchedDirectors; 

    // public just for looksies
    public float jumpToTime = -1f; // when placing red cards, jumps to this point in time in the timeline 

    public float thirdCardJumpToTime = -1f;

    private void Awake() {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        
        director = GetComponent<PlayableDirector>();
        director.Pause();

        branchedDirectors = new Stack<PlayableDirector>();

        goodCardDialogue = goodCard.transform.GetComponent<DialogueCard>();
        badCardDialogue = badCard.transform.GetComponent<DialogueCard>();
        thirdCardDialogue = thirdCard.transform.GetComponent<DialogueCard>();

        goodCardRb = goodCard.transform.GetComponent<Rigidbody>();
        badCardRb = badCard.transform.GetComponent<Rigidbody>();
        thirdCardRb = thirdCard.transform.GetComponent<Rigidbody>();

        j_animator = jackInTheBox.GetComponent<Animator>();

        director.time = time;

        //BadEnding();
    }

    private void Update() {

    }

    // Scene Switch Script
    public void SwitchScene(int level)
    {
        levelChanger.GetComponent<LevelChange>().FadeToLevel(level);
    }

    // Judge asks Dazzle a questions with only one answer
    public void StartSingleAnswerQuestioning() {
        director.Pause();
        goodCard.transform.position = goodCardPos.position;
        goodCard.transform.rotation = goodCardPos.rotation;
        chooseEffect.transform.position = goodCard.transform.position;
        chooseEffect.transform.rotation = goodCard.transform.rotation;
        chooseEffect.Emit(50);

        goodCard.SetActive(true);
        j_animator.SetTrigger("Open");
    }

    // Judge asks Dazzle a question where there are two possible answers
    public void StartQuestioning(float secondBranchTime)
    {
        director.Pause();

        goodCard.transform.position = goodCardPos.position;
        badCard.transform.position = badCardPos.position;
        goodCard.transform.rotation = goodCardPos.rotation;
        badCard.transform.rotation = badCardPos.rotation;

        ShowSmokeOnCards();

        goodCard.SetActive(true);
        badCard.SetActive(true);
        //judgeCard.SetActive(true);

        j_animator.SetTrigger("Open");
        SetSecondBranchTime(secondBranchTime);
    }


    // Judge asks Dazzle a question where there are three possible answers
    public void StartThreeAnswerQuestioning() {
        director.Pause();

        goodCard.transform.position = goodCardPos.position;
        badCard.transform.position = badCardPos.position;
        goodCard.transform.rotation = goodCardPos.rotation;
        badCard.transform.rotation = badCardPos.rotation;
        thirdCard.transform.position = thirdCardPos.position;
        thirdCard.transform.rotation = thirdCardPos.rotation;

        ShowSmokeOnCards();

        goodCard.SetActive(true);
        badCard.SetActive(true);
        thirdCard.SetActive(true);

        //judgeCard.SetActive(true);

        j_animator.SetTrigger("Open");
    }


    public void RegisterBlackCard() => StartCoroutine(RegisterBlackCardCoroutine());

    public void RegisterRedCard() => StartCoroutine(RegisterRedCardCoroutine());

    public void RegisterThirdCard() => StartCoroutine(RegisterThirdCardCoroutine());

    IEnumerator RegisterBlackCardCoroutine() {
        j_animator.SetTrigger("Processing");
        goodCardDialogue.isActive = false;

        yield return new WaitForSeconds(2f);

        director.Resume(); // simply resume for black
        HideCards();
        goodCardDialogue.isActive = true;
        if (branchedDirectors.Count > 0) {
            //branchedDirectors.Pop();
            PlayableDirector temp = branchedDirectors.Pop();
            //temp.time = director.time;
            temp.Resume();
        } else {
        }

    }

    IEnumerator RegisterRedCardCoroutine() {
        j_animator.SetTrigger("Processing");
        badCardDialogue.isActive = false;

        yield return new WaitForSeconds(2f);

        HideCards();
        badCardDialogue.isActive = true;

        // jump to branch 2 (skipping branch 1) (not best practice)
        // for some reason, you have to resume the main director first, 
        // then resume the child director
        // NOTE: for the excited_confirm branch, the branched director is actually excited, which is 
        // super weird since every other branched timeline does not have that
        director.time = jumpToTime;
        director.Resume();

        if (branchedDirectors.Count > 0) {
            print("ENABLE BRANCHED DIRECTOR");
            PlayableDirector tempDir = branchedDirectors.Pop();
            tempDir.time = jumpToTime;
            tempDir.Resume();
        }
    }

    IEnumerator RegisterThirdCardCoroutine() {
        j_animator.SetTrigger("Processing");
        thirdCardDialogue.isActive = false;

        yield return new WaitForSeconds(2f);

        HideCards();
        thirdCardDialogue.isActive = true;

        // jump to branch 3 (skipping branch 1 and 2) (not best practice)
        print(director.time);
        director.time = thirdCardJumpToTime;
        print(director.time);
        director.Resume();
        print(director.time);


        if (branchedDirectors.Count > 0) {
            print("ENABLE BRANCHED DIRECTOR");
            PlayableDirector tempDir = branchedDirectors.Pop();
            tempDir.time = thirdCardJumpToTime;
            tempDir.Resume();
        }
    }

    void HideCards() {
        ShowSmokeOnCards();
        goodCard.SetActive(false);
        badCard.SetActive(false);
        thirdCard.SetActive(false);
        //judgeCard.SetActive(false);

        // Destroy their dazzle model gameobjects 
        badCardDialogue.DestroyDazzle();
        goodCardDialogue.DestroyDazzle();
        thirdCardDialogue.DestroyDazzle();

        // Reset their rigidbodies as well
        goodCardRb.velocity = Vector3.zero;
        badCardRb.velocity = Vector3.zero;
        thirdCardRb.velocity = Vector3.zero;
    }


    void ShowSmokeOnCards() {
        chooseEffect.transform.position = goodCard.transform.position;
        chooseEffect.transform.rotation = goodCard.transform.rotation;
        chooseEffect.Emit(50);

        // Check cases where only the black card is being registerd
        if (badCard.activeSelf) {
            chooseEffect.transform.position = badCard.transform.position;
            chooseEffect.transform.rotation = badCard.transform.rotation;
            chooseEffect.Emit(50);
        }
        if (thirdCard.activeSelf) {
            chooseEffect.transform.position = thirdCard.transform.position;
            chooseEffect.transform.rotation = thirdCard.transform.rotation;
            chooseEffect.Emit(50);
        }
    }

    // NOTE: This is used in Branch 1 in the timeline
    public void ResumeFrom(float time) {
        director.time = time;
        director.Resume();
    }

    // NOTE: This is used in main timeline to jump to the 2nd branch if the red card is used
    public void SetSecondBranchTime(float time) {
        jumpToTime = time;
    }

    public void SetThirdCardBranchTime(float time) {
        thirdCardJumpToTime = time;
    }

    public void SetBlackCardDazzle(GameObject dazzlePrefab) {
        goodCardDialogue.SwitchDazzle(dazzlePrefab);
    }

    public void SetRedCardDazzle(GameObject dazzlePrefab) {
        badCardDialogue.SwitchDazzle(dazzlePrefab);
    }

    public void SetThirdCardDazzle(GameObject dazzlePrefab) {
        thirdCardDialogue.SwitchDazzle(dazzlePrefab);
    }

    // Used in the nested timeline to re-enable the timeline 
    public void AddBranchedDirector(PlayableDirector dir) {
        branchedDirectors.Push(dir);
    }

    public void RespawnCard(Transform card) {
        chooseEffect.transform.position = card.position;
        chooseEffect.transform.rotation = card.rotation;
        chooseEffect.Emit(50);

        card.position = goodCardPos.position;
        card.rotation = goodCardPos.rotation;
        chooseEffect.transform.position = goodCard.transform.position;
        chooseEffect.transform.rotation = goodCard.transform.rotation;
        chooseEffect.Emit(50);
    }

    public void BadEnding()
    {
        Animator endingAnim = endingCannon.GetComponent<Animator>();
        endingAnim.SetTrigger("End");
        j_animator.SetTrigger("Processing");
    }
}
