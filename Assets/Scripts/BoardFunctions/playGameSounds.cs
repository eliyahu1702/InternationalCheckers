using UnityEngine;

public class playGameSounds : MonoBehaviour
{
    public AudioSource source;
    public AudioClip CheckerMovingPlayer;
    public AudioClip CheckerMovingOponent;
    public AudioClip QueenMoving;
    public AudioClip Taking;
    public AudioClip Promotion;
    public AudioClip Losing;
    public AudioClip Winning;



    public void playCheckerPlayer()
    { source.PlayOneShot(CheckerMovingPlayer); }
    public void playCheckerMoveOponent()
    { source.PlayOneShot(CheckerMovingOponent); }
    public void playCheckerMove(Checker c)
    {
        if (c.GetValue() == gameValues.whiteChecker())
            source.PlayOneShot(CheckerMovingPlayer);
        else
            source.PlayOneShot(CheckerMovingOponent);
    }
        public void playQueenMove()
        { source.PlayOneShot(QueenMoving); }
        public void playTaking()
        { source.PlayOneShot(Taking); }
        public void playPromotion()
        { source.PlayOneShot(Promotion); }
        public void playLosing()
        { source.PlayOneShot(Losing); }
        public void playWinning()
        { source.PlayOneShot(Winning); }
    }
