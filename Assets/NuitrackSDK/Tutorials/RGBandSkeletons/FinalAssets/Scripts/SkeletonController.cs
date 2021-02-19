using nuitrack;
using UnityEngine;
using System.Collections.Generic;

public class SkeletonController : MonoBehaviour
{
    [Range(0, 6)]
    public int skeletonCount = 6;         //Max number of skeletons tracked by Nuitrack
    [SerializeField] SimpleSkeletonAvatar skeletonAvatar;

    List<SimpleSkeletonAvatar> avatars = new List<SimpleSkeletonAvatar>();

    private GameController gameController;

    void OnEnable()
    {
        this.gameController = GameObject.Find("Game Controller").GetComponent<GameController>();
        NuitrackManager.SkeletonTracker.OnSkeletonUpdateEvent += OnSkeletonUpdate;
    }

    void Start()
    {
        for (int i = 0; i < skeletonCount; i++)
        {
            GameObject newAvatar = Instantiate(skeletonAvatar.gameObject, transform, true);
            SimpleSkeletonAvatar simpleSkeleton = newAvatar.GetComponent<SimpleSkeletonAvatar>();
            simpleSkeleton.autoProcessing = false;
            avatars.Add(simpleSkeleton);
        }

        NuitrackManager.SkeletonTracker.SetNumActiveUsers(skeletonCount);
    }

    void OnSkeletonUpdate(SkeletonData skeletonData)
    {
        for (int i = 0; i < avatars.Count; i++)
        {
            if (i < skeletonData.Skeletons.Length)
            {
                avatars[i].gameObject.SetActive(true);
                avatars[i].ProcessSkeleton(skeletonData.Skeletons[i]);
                this.gameController.players[i].SetupPlayer(i);
            }
            else
            {
                avatars[i].gameObject.SetActive(false);
                this.gameController.players[i].ResetPlayer();
            }
        }
    }
}
